using HtmlRaportGenerator.Models;
using HtmlRaportGenerator.Tools.Enums;
using HtmlRaportGenerator.Tools.GoogleDriveDtos;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HtmlRaportGenerator.Tools.JsonContexts
{
    [JsonSerializable(typeof(GoogleFile))]
    //[JsonSerializable(typeof(GoogleFileToSend))]
    //[JsonSerializable(typeof(GoogleFilesResponse))]
    [JsonSerializable(typeof(DataStore))]
    [JsonSerializable(typeof(List<Day>))]
    internal partial class GoogleDriveContext : JsonSerializerContext
    {
    }
}
