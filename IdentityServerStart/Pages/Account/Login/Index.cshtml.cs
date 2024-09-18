using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServerStart.Pages.Account.Login;

[AllowAnonymous]
public class Index(
    IIdentityServerInteractionService interaction, 
    IAuthenticationSchemeProvider schemeProvider, 
    IIdentityProviderStore identityProviderStore, 
    TestUserStore? users = null
    ) : PageModel
{
    // For testing purposes - this will be replaced with database moving forward
    private readonly TestUserStore _users = users ?? throw new InvalidOperationException("Please call 'AddTestUsers(TestUsers.Users)' on the IIdentityServerBuilder in Startup or remove the TestUserStore from the AccountController.");

    public ViewModel View { get; set; } = default!;
    
    [BindProperty] public InputModel Input { get; set; } = default!;

    public async Task<IActionResult> OnGet(string? returnUrl = null)
    {
        await BuildModelAsync(returnUrl);
        
        return Page();
    }

    private async Task BuildModelAsync(string? returnUrl)
    {
        Input = new InputModel
        {
            ReturnUrl = returnUrl,
        };
        
        var context = await interaction.GetAuthorizationContextAsync(returnUrl);

        if (context?.IdP != null && await schemeProvider.GetSchemeAsync(context.IdP) != null)
        {
            var isLocal = context.IdP == IdentityServerConstants.LocalIdentityProvider;
            
            //TODO: To remember to find out where and how does it short circuit UI
            // this is meant to short circuit the UI and only trigger the one external IdP
            View = new ViewModel
            {
                EnableLocalLogin = isLocal,
            };

            //TODO: Purpose?
            Input.Username = context.LoginHint;

            if (!isLocal)
            {
                View.ExternalProviders =
                [
                    // no display name?
                    new ViewModel.ExternalProvider(authenticationScheme: context.IdP)
                ];
            }

            return;
        }
        
        //TODO: The differences between the schemes
        // current understanding is one is linked to an external provider like AAD or other external providers
        // another is for cookie scheme set for applications logging in "locally" via this identity provider
        var providers = (await schemeProvider.GetAllSchemesAsync())
            .Where(x => x.DisplayName != null)
            .Select(x => new ViewModel.ExternalProvider 
                (
                    authenticationScheme: x.Name, 
                    displayName: x.DisplayName ?? x.Name
                ))
            .ToList();

        var dynamicSchemes = (await identityProviderStore.GetAllSchemeNamesAsync())
            .Where(x => x.Enabled)
            .Select(x => new ViewModel.ExternalProvider
                (
                    authenticationScheme: x.Scheme, 
                    displayName: x.DisplayName ?? x.Scheme
                ));
        providers.AddRange(dynamicSchemes);

        var allowLocal = true;
        var client = context?.Client;
        if (client != null)
        {
            allowLocal = client.EnableLocalLogin;
            
            //TODO: Significance of restricting external provider schemes for client
            if (client.IdentityProviderRestrictions.Any())
            {
                providers = providers
                    .Where(provider => client.IdentityProviderRestrictions
                        .Contains(provider.AuthenticationScheme))
                    .ToList();
            }
        }
        
        View = new ViewModel
        {
            AllowRememberLogin = LoginOptions.AllowRememberLogin,
            EnableLocalLogin = allowLocal && LoginOptions.AllowLocalLogin,
            ExternalProviders = providers.ToArray()
        };
    }
}