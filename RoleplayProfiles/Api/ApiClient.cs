using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Dalamud.Utility;

using RestSharp;

using SocketIOClient;
using SocketIOClient.JsonSerializer;

namespace RoleplayProfiles.Api;

public class ApiClient : IDisposable
{
    private const string ApiUrl = "https://chaosarchives.org/api/rpp";
    private const string SocketIOUrl = "wss://chaosarchives.org/updates";

    // Events

    public delegate void OnDisconnectedEventHandler();
    public delegate void OnCharacterUpdatedEventHandler(Player player);

    public event OnDisconnectedEventHandler? OnDisconnected;
    public event OnCharacterUpdatedEventHandler? OnCharacterUpdated;

    // Instance state

    private readonly RestClient restClient = new RestClient(new RestClientOptions(ApiUrl)
    {
        
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
        socketIOClient = new SocketIO(SocketIOUrl);

        if (Util.IsWine()) {
            // Work around infinite loading bug under Wine set to Windows 10
            socketIOClient.Options.Transport = SocketIOClient.Transport.TransportProtocol.Polling;
            socketIOClient.Options.AutoUpgrade = false;
        }

        (socketIOClient.JsonSerializer as SystemTextJsonSerializer)!.OptionsProvider =
            () => new JsonSerializerOptions(JsonSerializerDefaults.Web);

        socketIOClient.OnDisconnected += (sender, args) =>
        {
            // PluginLog.Information("Disconnected");
            sessionToken = null;
            OnDisconnected?.Invoke();
        };

        socketIOClient.On("session", response =>
        {
            var sessionToken = response.GetValue<SessionResult>().SessionToken;
            // PluginLog.Information("Retrieved session token");
            this.sessionToken = sessionToken;
            promise.TrySetResult(sessionToken);
        });

        socketIOClient.On("character.updated", response =>
        {
            var player = response.GetValue<Player>();
            // PluginLog.Information("Character updated: " + player);
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

        try
        {
            var response = await ExecuteAsync<Profile>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            return response.Data;
        }
        catch (ApiException e)
        {
            if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            throw e;
        }
    }

    public async Task<LoginResponse> Login(string email, string password, string? otp)
    {
        var request = new RestRequest("/login", Method.Post);
        request.AddBody(new LoginRequest
        {
            Email = email,
            Password = password,
            Otp = otp,
        });

        var response = await ExecuteAsync<LoginResponse>(request);
        return response.Data!;
    }

    public async Task<ExtendLoginResponse> ExtendLogin(string accessToken)
    {
        var request = new RestRequest("/extend-login", Method.Post);
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        
        var response = await ExecuteAsync<ExtendLoginResponse>(request);
        return response.Data!;
    }

    public async Task UpdateProfile(Player player, Profile profile, string accessToken)
    {
        var request = new RestRequest($"/profile/{player.Server}/{player.Name}", Method.Put);
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        request.AddBody(profile);

        var response = await ExecuteAsync<Profile>(request);
    }

    private async Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request)
    {
        var response = await restClient.ExecuteAsync<T>(request);
        var responseStatusCategory = ((int) response.StatusCode) % 100;

        if (responseStatusCategory == 4 || responseStatusCategory == 5
            || response.ErrorException is HttpRequestException)
        {
            throw new ApiException(response.StatusCode, GetErrorMessage(response.Content!));
        }

        if (response.ErrorException != null)
        {
            throw response.ErrorException;
        }

        return response;
    }

    private string GetErrorMessage(string errorResponse)
    {
        try {
            var responseObject = JsonSerializer.Deserialize<ApiErrorResponse>(errorResponse,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));
            return responseObject!.Message;
        }
        catch (Exception)
        {
            return errorResponse;
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
