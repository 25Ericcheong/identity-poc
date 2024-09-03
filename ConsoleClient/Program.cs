using System.Text.Json;
using IdentityModel.Client;

// discover endpoints from metadata
var client = new HttpClient();
var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001"); // auth server

if (disco.IsError)
{
    Console.WriteLine(disco.Error);
    return;
}

// request token
// note: Will get forbidden response if scope configured on api is different to what is set below
var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
   Address = disco.TokenEndpoint,
   ClientId = "client",
   ClientSecret = "secret",
   Scope = "delivery"
});

// note: if response returned is invalid client; ClientId does not exist on auth server
// note: if response returned is invalid_scope; Scope does not exist
if (tokenResponse.IsError)
{
    Console.WriteLine(tokenResponse.Error);
    Console.WriteLine(tokenResponse.ErrorDescription);
    return;
}


// call api
var apiClient = new HttpClient();

// access token always non-null when IsError is false. Ref: https://docs.duendesoftware.com/identityserver/v7/quickstarts/1_client_credentials/

// note: Will get unauthorized response if not set
apiClient.SetBearerToken(tokenResponse.AccessToken!);

var response = await apiClient.GetAsync("https://localhost:6001/identity");
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine(response.StatusCode);
}
else
{
    // by default access token contains claims about scope, lifetime (nbf and exp), client_id and issuer name (iss)
    var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
    Console.WriteLine(JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true }));
}