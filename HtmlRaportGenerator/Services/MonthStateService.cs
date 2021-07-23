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

        private Dictionary<string, IReadOnlyCollection<Day>> _cachedDays = new();

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
            if (nextStore == _currentDataStore || nextStore == DataStore.None)
            {
                return false;
            }

            _currentDataStore = nextStore;

            await _localStorageService.SetItemAsync(StaticHelpers.DataStoreTypeKey, nextStore)
                .ConfigureAwait(false);

            AuthenticationState state = await _authenticationStateProvider.GetAuthenticationStateAsync().ConfigureAwait(false);

            if (state.User.Identity?.IsAuthenticated is true)
            {
                bool result = await _googleDriveService.SaveAsync(StaticHelpers.DataStoreTypeKey, nextStore)
                    .ConfigureAwait(false);

                if (!result)
                {
                    return false;
                }
            }

            ClearCache();

            return true;
        }

        public async Task<bool> SaveAsync(IReadOnlyCollection<Day> days, string yearMonth)
        {
            days.CheckNotNull(nameof(days));

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

        public async ValueTask<IReadOnlyCollection<Day>?> GetAsync(string yearMonth)
        {
            if (_cachedDays.TryGetValue(yearMonth, out IReadOnlyCollection<Day>? result))
            {
                return result;
            }

            await LoadConfigurationIfEmptyAsync().ConfigureAwait(false);

            IReadOnlyCollection<Day>? days = _currentDataStore switch
            {
                DataStore.LocalStorage => await _localStorageService.GetItemAsync<IReadOnlyCollection<Day>>(yearMonth)
                    .ConfigureAwait(false),
                DataStore.GoogleDrive => await _googleDriveService.GetAsync<IReadOnlyCollection<Day>>(yearMonth)
                    .ConfigureAwait(false),
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
            _currentDataStore = await _localStorageService.GetItemAsync<DataStore?>(StaticHelpers.DataStoreTypeKey)
                .ConfigureAwait(false);

            if (_currentDataStore is null or DataStore.None)
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
            => _cachedDays = new Dictionary<string, IReadOnlyCollection<Day>>();
    }
}
