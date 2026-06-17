using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private ClaimsPrincipal _user = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthStateProvider(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
{
    try
    {
        var userIdResult = await _sessionStorage.GetAsync<int>("userId");
        var usernameResult = await _sessionStorage.GetAsync<string>("username");
        var nameResult = await _sessionStorage.GetAsync<string>("name");

        if (userIdResult.Success && usernameResult.Success)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userIdResult.Value.ToString()),
                new Claim(ClaimTypes.Name, usernameResult.Value ?? ""),
                new Claim(ClaimTypes.GivenName, nameResult.Success ? nameResult.Value ?? "" : "")
            };
            var identity = new ClaimsIdentity(claims, "CustomAuth");
            _user = new ClaimsPrincipal(identity);
        }
    }
    catch { }

    return new AuthenticationState(_user);
}
    public async Task Login(int userId, string username, string name)
{
    await _sessionStorage.SetAsync("userId", userId);
    await _sessionStorage.SetAsync("username", username);
    await _sessionStorage.SetAsync("name", name);

    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Name, username),
        new Claim(ClaimTypes.GivenName, name)
    };
    var identity = new ClaimsIdentity(claims, "CustomAuth");
    _user = new ClaimsPrincipal(identity);
    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}

   public async Task Logout()
{
    await _sessionStorage.DeleteAsync("userId");
    await _sessionStorage.DeleteAsync("username");
    await _sessionStorage.DeleteAsync("name");
    _user = new ClaimsPrincipal(new ClaimsIdentity());
    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}
}