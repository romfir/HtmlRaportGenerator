using Blazored.LocalStorage;
using HtmlRaportGenerator.Models;
using HtmlRaportGenerator.Tools;
using HtmlRaportGenerator.Tools.GoogleDriveDtos;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HtmlRaportGenerator.Services
{
    public class MonthStateService //: IMonthStateService todo + singleton?
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;

        private List<GoogleFile>? _googleFiles;



        public MonthStateService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
        {
            _httpClient = httpClientFactory.CreateClient(StaticHelpers.HttpClientName);
            _localStorageService = localStorageService;
        }

        //public DataStore CurrentDataStore { get; private set; } = DataStore.LocalStorage;
        public DataStore CurrentDataStore { get; private set; } = DataStore.GoogleDrive;

        public bool ChangeDataStore(DataStore nextStore)
        {
            if (nextStore != CurrentDataStore)
            {
                CurrentDataStore = nextStore;

                return true;
            }

            return false;
        }

        public async Task<bool> SaveAsync(List<Day> days, string yearMonth)
        {
            //todo validaiton yearMOnth

            if (CurrentDataStore == DataStore.LocalStorage)
            {
                await _localStorageService.SetItemAsync(yearMonth, days);

                return true;
            }
            else if (CurrentDataStore == DataStore.GoogleDrive)
            {
                string fileName = yearMonth + ".json";

                bool fileExists = false;
                string? fileId = null;

                if (_googleFiles is object)
                {
                    GoogleFile? existingFile = _googleFiles.FirstOrDefault(f => f.Name == fileName);

                    if (existingFile is object)
                    {
                        fileExists = true;
                        fileId = existingFile.Id;
                    }
                }


                MultipartFormDataContent multipartContent = new MultipartFormDataContent("----" + Guid.NewGuid().ToString())
                {
                    { JsonContent.Create(new GoogleFileToSend { Description = $"File containing HtmlRaportGenerator data from month {yearMonth}", MimeType = "application/json", Name = yearMonth + ".json" }, new MediaTypeHeaderValue("application/json")), "Metadata" },
                    { JsonContent.Create(days, new MediaTypeHeaderValue("multipart/related")), "Media"}
                };

                try
                {
                    HttpResponseMessage response;

                    if (fileExists)
                    {
                        response = await _httpClient.PatchAsync(@$"upload/drive/v3/files/{fileId}?uploadType=multipart", multipartContent);  //cancelationtoken
                    }
                    else
                    {
                        response = await _httpClient.PostAsync(@"upload/drive/v3/files?uploadType=multipart", multipartContent);  //cancelationtoken
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.Content.ToString());
                        return false;
                    }

                    //response to 
                    GoogleFile? newFile = await response.Content.ReadFromJsonAsync<GoogleFile>().ConfigureAwait(false); //cancelationtoken

                    if (newFile is null)
                    {
                        Console.WriteLine("Unable to pars Google Drive response");
                        return false;
                    }

                    if (_googleFiles is null)
                    {
                        _googleFiles = new List<GoogleFile> { newFile };
                    }
                    else
                    {
                        _googleFiles.Add(newFile);
                    }


                    return false;
                }
                catch (AccessTokenNotAvailableException exception)
                {
                    //Console.WriteLine(exception.ToString());
                    exception.Redirect();

                    //after redirect can we try to save again? IS blazor able to keep state after redirection? Check it!
                }
            }

            return false;
        }


        //todo create object containng List<day>? mb DaysCollectionValidationModel -> rename and add timestamp
        //when miltiple ENums are chosem data is saved in each dataStore, and during load timestamps are checked?
        //todo settings page!
        //todo foreach enum
        public async Task<List<Day>?> GetAsync(string yearMonth) //todo CancellationToken?
        {
            //todo yearMonth validation!

            if (CurrentDataStore == DataStore.LocalStorage)
            {
                return await _localStorageService.GetItemAsync<List<Day>>(yearMonth);
            }

            if (CurrentDataStore == DataStore.GoogleDrive)
            {
                string? matchingFileId = null;

                if (_googleFiles is object)
                {
                    GoogleFile? matchingFile = _googleFiles!.FirstOrDefault(f => f.Name == yearMonth + ".json");

                    if (matchingFile is object)
                    {
                        matchingFileId = matchingFile.Id;
                    }
                }

                if (matchingFileId is null)
                {
                    try
                    {
                        GoogleFilesResponse? response = await _httpClient.GetFromJsonAsync<GoogleFilesResponse>(@"drive/v3/files").ConfigureAwait(false);

                        if (response?.Files is null || response.Files.Count == 0)
                        {
                            return null;
                        }

                        _googleFiles = response.Files;
                    }
                    catch (AccessTokenNotAvailableException exception)
                    {
                        //Console.WriteLine(exception.ToString());
                        exception.Redirect();

                        //after redirect can we try to save again?
                    }

                    GoogleFile? matchingFile = _googleFiles!.FirstOrDefault(f => f.Name == yearMonth + ".json");

                    if (matchingFile is null)
                    {
                        return null;
                    }

                    matchingFileId = matchingFile.Id;
                }

                //todo add queryparams using build in methods, and dont use string interpolation
                return await _httpClient.GetFromJsonAsync<List<Day>>($@"drive/v3/files/{Uri.EscapeDataString(matchingFileId!)}?alt=media").ConfigureAwait(false);
            }

            return null;
        }


        //todo loadConfiguration which will load enum first from google drive if user is authorized then from localstorage
    }
}
