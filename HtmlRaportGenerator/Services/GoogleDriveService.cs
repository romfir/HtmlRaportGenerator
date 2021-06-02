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
    public class GoogleDriveService
    {
        private readonly HttpClient _httpClient;

        private List<GoogleFile>? _googleFiles;

        public GoogleDriveService(IHttpClientFactory httpClientFactory)
            => _httpClient = httpClientFactory.CreateClient(StaticHelpers.HttpClientName);

        /// <typeparam name="T"></typeparam>
        /// <param name="key">filename without .json</param>
        /// <param name="data">Data to save</param>
        /// <returns></returns>
        public async Task<bool> SaveAsync<T>(string key, T data)
        {
            string fileName = key + ".json";

            GoogleFile? existingFile = _googleFiles?.FirstOrDefault(f => f.Name == fileName);

            if (existingFile is null)
            {
                await LoadFilesFromDriveAsync();
                existingFile = _googleFiles?.FirstOrDefault(f => f.Name == fileName);
            }

            MultipartFormDataContent multipartContent = new("----" + Guid.NewGuid())
            {
                { JsonContent.Create(new GoogleFileToSend { Description = $"File used by HtmlRaportGenerator", MimeType = "application/json", Name = key + ".json" }, new MediaTypeHeaderValue("application/json")), "Metadata" },
                { JsonContent.Create(data, new MediaTypeHeaderValue("multipart/related")), "Media" }
            };

            try
            {
                HttpResponseMessage response;

                if (existingFile?.Id is not null)
                {
                    response = await _httpClient.PatchAsync(@$"upload/drive/v3/files/{existingFile.Id}?uploadType=multipart", multipartContent);
                }
                else
                {
                    response = await _httpClient.PostAsync(@"upload/drive/v3/files?uploadType=multipart", multipartContent);
                }

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine(response.Content.ToString());
                    return false;
                }

                GoogleFile? newFile = await response.Content.ReadFromJsonAsync<GoogleFile>().ConfigureAwait(false);

                if (newFile is null)
                {
                    Console.WriteLine("Unable to parse Google Drive response");
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

                return true;
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }

            return false;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            string fileName = key + ".json";

            GoogleFile? matchingFile = _googleFiles?.FirstOrDefault(f => f.Name == fileName);

            if (matchingFile is null)
            {
                await LoadFilesFromDriveAsync().ConfigureAwait(false);

                matchingFile = _googleFiles?.FirstOrDefault(f => f.Name == fileName);
            }

            if (matchingFile?.Id is null)
            {
                return default;
            }

            return await _httpClient.GetFromJsonAsync<T>($@"drive/v3/files/{Uri.EscapeDataString(matchingFile.Id)}?alt=media").ConfigureAwait(false);
        }

        public async Task LoadFilesFromDriveAsync()
        {
            try
            {
                GoogleFilesResponse? response = await _httpClient.GetFromJsonAsync<GoogleFilesResponse>(@"drive/v3/files").ConfigureAwait(false);

                if (response?.Files is null || response.Files.Count == 0)
                {
                    _googleFiles = null;

                    return;
                }

                _googleFiles = response.Files;
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }
    }
}

