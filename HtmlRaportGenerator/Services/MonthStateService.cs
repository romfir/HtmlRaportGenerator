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
            if (nextStore != _currentDataStore)
            {
                _currentDataStore = nextStore;

                await _localStorageService.SetItemAsync(StaticHelpers.DataStoreTypeKey, nextStore);

                AuthenticationState state = await _authenticationStateProvider.GetAuthenticationStateAsync().ConfigureAwait(false);

                if (state.User.Identity?.IsAuthenticated is true)
                {
                    await _googleDriveService.SaveAsync(StaticHelpers.DataStoreTypeKey, nextStore);
                }

                return true;
            }

            return false;
        }

        public async Task<bool> SaveAsync(List<Day> days, string yearMonth)
        {
            await LoadConfigurationIfEmptyAsync().ConfigureAwait(false);

            if (_currentDataStore == DataStore.LocalStorage)
            {
                await _localStorageService.SetItemAsync(yearMonth, days).ConfigureAwait(false);

                return true;
            }
            else if (_currentDataStore == DataStore.GoogleDrive)
            {
                return await _googleDriveService.SaveAsync(yearMonth, days).ConfigureAwait(false);
            }

            return false;
        }

        public async Task<List<Day>?> GetAsync(string yearMonth)
        {
            await LoadConfigurationIfEmptyAsync().ConfigureAwait(false);

            return _currentDataStore switch
            {
                DataStore.LocalStorage => await _localStorageService.GetItemAsync<List<Day>>(yearMonth),
                DataStore.GoogleDrive => await _googleDriveService.GetAsync<List<Day>>(yearMonth),
                _ => null
            };
        }

        public Task LoadConfigurationIfEmptyAsync()
        {
            if (_currentDataStore is object)
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
                await LoadConfigurationFromGoogleDriveAsync();
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
    }
}
