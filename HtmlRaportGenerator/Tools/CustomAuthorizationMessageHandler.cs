using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace HtmlRaportGenerator.Tools;

public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public CustomAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigationManager)
        : base(provider, navigationManager)
    {
        ConfigureHandler(new[] { "https://www.googleapis.com/" },
            new[] { @"https://www.googleapis.com/auth/drive.file", "openid", "profile", "email" });
    }
}