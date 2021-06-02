using Blazored.LocalStorage;
using HtmlRaportGenerator.Models;
using HtmlRaportGenerator.Tools;
using HtmlRaportGenerator.Tools.Enums;
using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HtmlRaportGenerator.Services
{
    public class MonthStateService
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly GoogleDriveService _googleDriveService;

        private readonly AuthenticationStateProvider _authenticationStateProvider;

        private Dictionary<string, List<Day>> _cachedDays = new();

        public MonthStateService(GoogleDriveService googleDriveService, ILocalStorageService localStorageService, AuthenticationStateProvider authenticationStateProvider)
        {
            _localStorageService = localStorageService;
            _googleDriveService = googleDriveService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        private DataStore? _currentDataStore;

        public DataStore CurrentDataStore
            => _currentDataStore ?? DataStore.LocalStorage;

        public async Task<bool> ChangeDataStoreAsync(DataStore nextStore)
        {
            if (nextStore == _currentDataStore)
            {
                return false;
            }

            _currentDataStore = nextStore;

            await _localStorageService.SetItemAsync(StaticHelpers.DataStoreTypeKey, nextStore);

            AuthenticationState state = await _authenticationStateProvider.GetAuthenticationStateAsync().ConfigureAwait(false);

            if (state.User.Identity?.IsAuthenticated is true)
            {
                await _googleDriveService.SaveAsync(StaticHelpers.DataStoreTypeKey, nextStore);
            }

            return true;

        }

        public async Task<bool> SaveAsync(List<Day> days, string yearMonth)
        {
            await LoadConfigurationIfEmptyAsync().ConfigureAwait(false);

            bool result;
            switch (_currentDataStore)
            {
                case DataStore.LocalStorage:
                    await _localStorageService.SetItemAsync(yearMonth, days).ConfigureAwait(false);

                    result = true;

                    break;

                case DataStore.GoogleDrive:
                    result = await _googleDriveService.SaveAsync(yearMonth, days).ConfigureAwait(false);

                    break;
                default:
                    result = false;
                    break;
            }

            if (result)
            {
                _cachedDays[yearMonth] = days;
            }

            return result;
        }

        public async ValueTask<List<Day>?> GetAsync(string yearMonth)
        {
            if (_cachedDays.TryGetValue(yearMonth, out List<Day>? result))
            {
                return result;
            }

            await LoadConfigurationIfEmptyAsync().ConfigureAwait(false);

            List<Day>? days = _currentDataStore switch
            {
                DataStore.LocalStorage => await _localStorageService.GetItemAsync<List<Day>>(yearMonth),
                DataStore.GoogleDrive => await _googleDriveService.GetAsync<List<Day>>(yearMonth),
                _ => null
            };

            if (days is not null)
            {
                _cachedDays[yearMonth] = days;
            }

            return days;
        }

        public Task LoadConfigurationIfEmptyAsync()
        {
            if (_currentDataStore is not null)
            {
                return Task.CompletedTask;
            }

            return LoadConfigurationAsync();
        }

        public async Task LoadConfigurationAsync()
        {
            _currentDataStore = await _localStorageService.GetItemAsync<DataStore?>(StaticHelpers.DataStoreTypeKey);

            if (_currentDataStore is null)
            {
                await LoadConfigurationFromGoogleDriveAsync().ConfigureAwait(false);
            }

            _currentDataStore ??= DataStore.LocalStorage;
        }

        public async Task LoadConfigurationFromGoogleDriveAsync()
        {
            AuthenticationState state = await _authenticationStateProvider.GetAuthenticationStateAsync().ConfigureAwait(false);

            if (state.User.Identity?.IsAuthenticated is true)
            {
                _currentDataStore = await _googleDriveService.GetAsync<DataStore?>(StaticHelpers.DataStoreTypeKey).ConfigureAwait(false);
                await _localStorageService.SetItemAsync(StaticHelpers.DataStoreTypeKey, CurrentDataStore).ConfigureAwait(false);
            }
        }

        public void ClearCache()
            => _cachedDays = new Dictionary<string, List<Day>>();
    }
}
