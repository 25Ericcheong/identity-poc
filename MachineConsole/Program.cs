using IdentityModel.Client;

Console.WriteLine("Console started");

var client = new HttpClient();

// Retrieve meta data of identity server
var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5002");
if (disco.IsError)
{
    Console.WriteLine(disco.Error);
    Console.WriteLine(disco.Exception);
}

Console.WriteLine("Successful response from discovery document endpoint");
Console.WriteLine();

var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = disco.TokenEndpoint,
    ClientId = "test-client-machine",
    ClientSecret = "test-secret-machine",
    Scope = "machine.core.financial.read",
    GrantType = "client_credentials"
});
if (tokenResponse.IsError)
{
    Console.WriteLine(tokenResponse.Error);
    Console.WriteLine(tokenResponse.ErrorDescription);
    return;
}
Console.WriteLine("Bearer token retrieved from identity server");
Console.WriteLine(tokenResponse.AccessToken);
Console.WriteLine();

// Send to proxy API first which is then proxied to actual API
var apiClient = new HttpClient();
apiClient.SetBearerToken(tokenResponse.AccessToken!);

var fathomProxyUri = "https://localhost:5000/";
var forecastingEndpoint = "forecasting/identity-machine";
var response = await apiClient.GetAsync($"{fathomProxyUri}{forecastingEndpoint}");
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine(response.StatusCode);
    return;
}

Console.WriteLine("Successful response from forecasting endpoint");
var doc = await response.Content.ReadAsStringAsync();
Console.WriteLine(doc);