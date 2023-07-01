using System;
using System.Text.Json;
using System.Threading.Tasks;

using Dalamud.Logging;
using Dalamud.Utility;

using RestSharp;

using SocketIOClient;
using SocketIOClient.JsonSerializer;

namespace RoleplayProfiles.Api;

public class ApiClient : IDisposable
{
    // Events

    public delegate void OnDisconnectedEventHandler();
    public delegate void OnCharacterUpdatedEventHandler(Player player);

    public event OnDisconnectedEventHandler? OnDisconnected;
    public event OnCharacterUpdatedEventHandler? OnCharacterUpdated;

    // Instance state

    private readonly RestClient restClient = new RestClient(new RestClientOptions("https://chaosarchives.org/api/rpp")
    {
        ThrowOnAnyError = true,
    });

    private SocketIO? socketIOClient = null;
    private string? sessionToken = null;

    private async Task<string> EnsureSessionToken()
    {
        if (sessionToken != null)
        {
            // Already connected and retrieved the session token
            return sessionToken;
        }

        if (socketIOClient != null)
        {
            await socketIOClient.DisconnectAsync();
        }

        var promise = new TaskCompletionSource<string>();
        socketIOClient = new SocketIO("wss://chaosarchives.org/updates");

        if (Util.IsLinux()) {
            // Work around infinite loading bug under Wine set to Windows 10
            socketIOClient.Options.Transport = SocketIOClient.Transport.TransportProtocol.Polling;
            socketIOClient.Options.AutoUpgrade = false;
        }

        (socketIOClient.JsonSerializer as SystemTextJsonSerializer)!.OptionsProvider =
            () => new JsonSerializerOptions(JsonSerializerDefaults.Web);

        socketIOClient.OnDisconnected += (sender, args) =>
        {
            PluginLog.Information("Disconnected");
            sessionToken = null;
            OnDisconnected?.Invoke();
        };

        socketIOClient.On("session", response =>
        {
            var sessionToken = response.GetValue<SessionResult>().SessionToken;
            PluginLog.Information("Retrieved session token");
            this.sessionToken = sessionToken;
            promise.TrySetResult(sessionToken);
        });

        socketIOClient.On("character.updated", response =>
        {
            var player = response.GetValue<Player>();
            PluginLog.Information("Character updated: " + player);
            OnCharacterUpdated?.Invoke(player);
        });

        await socketIOClient.ConnectAsync();
        return await promise.Task; // Wait for the session token to become available
    }

    public async Task<Profile?> GetProfile(Player player)
    {
        var sessionToken = await EnsureSessionToken();
        var request = new RestRequest($"/profile/{player.Server}/{player.Name}");
        request.AddParameter("sessionToken", sessionToken);

        var response = await restClient.ExecuteAsync<Profile>(request);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        if (response.ErrorException != null)
        {
            throw response.ErrorException;
        }

        return response.Data;
    }

    public async Task<LoginResponse> Login(string email, string password)
    {
        var request = new RestRequest("/login", Method.Post);
        request.AddBody(new LoginRequest
        {
            Email = email,
            Password = password,
        });

        var response = await restClient.ExecuteAsync<LoginResponse>(request);

        if (response.ErrorException != null)
        {
            throw response.ErrorException;
        }

        return response.Data!;
    }

    public async Task<ExtendLoginResponse> ExtendLogin(string accessToken)
    {
        var request = new RestRequest("/extend-login", Method.Post);
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        
        var response = await restClient.ExecuteAsync<ExtendLoginResponse>(request);

        if (response.ErrorException != null)
        {
            throw response.ErrorException;
        }

        return response.Data!;
    }

    public async Task UpdateProfile(Player player, Profile profile, string accessToken)
    {
        var request = new RestRequest($"/profile/{player.Server}/{player.Name}", Method.Put);
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        request.AddBody(profile);

        var response = await restClient.ExecuteAsync<Profile>(request);

        if (response.ErrorException != null)
        {
            throw response.ErrorException;
        }
    }

    public void Dispose()
    {
        sessionToken = null;

        if (socketIOClient != null)
        {
            socketIOClient.Dispose();
        }

        restClient.Dispose();
    }
}
