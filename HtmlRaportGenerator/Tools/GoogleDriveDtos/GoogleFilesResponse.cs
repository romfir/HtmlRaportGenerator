using System.Collections.Generic;

namespace HtmlRaportGenerator.Tools.GoogleDriveDtos;

public class GoogleFilesResponse
{
    public string Kind { get; set; } = null!;

    public bool IncompleteSearch { get; set; }

    public List<GoogleFile> Files { get; set; } = null!;
}