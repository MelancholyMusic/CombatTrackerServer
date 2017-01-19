using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using CombatTrackerServer.Models;
using CombatTrackerServer.Models.AccountViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Core;
using OpenIddict.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CombatTrackerServer.Controllers
{
	[Authorize]
	public class AuthorizationController : Controller
	{
		private readonly OpenIddictApplicationManager<OpenIddictApplication> _applicationManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;

		public AuthorizationController(
			OpenIddictApplicationManager<OpenIddictApplication> applicationManager,
			SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager)
		{
			_applicationManager = applicationManager;
			_signInManager = signInManager;
			_userManager = userManager;
		}

		[HttpPost("~/api/register")]
		[AllowAnonymous]
		public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			if(ModelState.IsValid)
			{
				var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
				var result = await _userManager.CreateAsync(user, model.Password);
				if(result.Succeeded)
				{
					// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
					// Send an email with this link
					//var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					//var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
					//await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
					//    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
					return Ok();
				}
				else
				{
					return BadRequest(result.Errors.ElementAt(0).Code);
				}
			}

			// If we got this far, something failed, redisplay form
			return BadRequest(ModelState.Values.ElementAt(0).Errors[0].ErrorMessage);
		}

		[HttpPost("~/connect/token"), Produces("application/json")]
		[AllowAnonymous]
		public async Task<IActionResult> Exchange(OpenIdConnectRequest request)
		{
			Debug.Assert(request.IsTokenRequest(),
				"The OpenIddict binder for ASP.NET Core MVC is not registered. " +
				"Make sure services.AddOpenIddict().AddMvcBinders() is correctly called.");
			if(!request.IsPasswordGrantType())
			{
				return BadRequest(new OpenIdConnectResponse
				{
					Error = OpenIdConnectConstants.Errors.UnsupportedGrantType,
					ErrorDescription = "The specified grant type is not supported."
				});
			}

			ApplicationUser user = await _userManager.FindByNameAsync(request.Username);
			if(user == null)
			{
				return BadRequest(new OpenIdConnectResponse
				{
					Error = OpenIdConnectConstants.Errors.InvalidGrant,
					ErrorDescription = "The username/password couple is invalid."
				});
			}

			// Ensure the user is allowed to sign in.
			if(!await _signInManager.CanSignInAsync(user))
			{
				return BadRequest(new OpenIdConnectResponse
				{
					Error = OpenIdConnectConstants.Errors.InvalidGrant,
					ErrorDescription = "The specified user is not allowed to sign in."
				});
			}

			// Reject the token request if two-factor authentication has been enabled by the user.
			if(_userManager.SupportsUserTwoFactor && await _userManager.GetTwoFactorEnabledAsync(user))
			{
				return BadRequest(new OpenIdConnectResponse
				{
					Error = OpenIdConnectConstants.Errors.InvalidGrant,
					ErrorDescription = "The specified user is not allowed to sign in."
				});
			}

			// Ensure the user is not already locked out.
			if(_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user))
			{
				return BadRequest(new OpenIdConnectResponse
				{
					Error = OpenIdConnectConstants.Errors.InvalidGrant,
					ErrorDescription = "The username/password couple is invalid."
				});
			}

			// Ensure the password is valid.
			if(!await _userManager.CheckPasswordAsync(user, request.Password))
			{
				if(_userManager.SupportsUserLockout)
				{
					await _userManager.AccessFailedAsync(user);
				}

				return BadRequest(new OpenIdConnectResponse
				{
					Error = OpenIdConnectConstants.Errors.InvalidGrant,
					ErrorDescription = "The username/password couple is invalid."
				});
			}

			if(_userManager.SupportsUserLockout)
			{
				await _userManager.ResetAccessFailedCountAsync(user);
			}

			// Create a new authentication ticket.
			var ticket = await CreateTicketAsync(request, user);

			return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
		}

		private async Task<AuthenticationTicket> CreateTicketAsync(OpenIdConnectRequest request, ApplicationUser user)
		{
			// Create a new ClaimsPrincipal containing the claims that
			// will be used to create an id_token, a token or a code.
			var principal = await _signInManager.CreateUserPrincipalAsync(user);

			// Note: by default, claims are NOT automatically included in the access and identity tokens.
			// To allow OpenIddict to serialize them, you must attach them a destination, that specifies
			// whether they should be included in access tokens, in identity tokens or in both.

			foreach(var claim in principal.Claims)
			{
				// In this sample, every claim is serialized in both the access and the identity tokens.
				// In a real world application, you'd probably want to exclude confidential claims
				// or apply a claims policy based on the scopes requested by the client application.
				claim.SetDestinations(OpenIdConnectConstants.Destinations.AccessToken,
									  OpenIdConnectConstants.Destinations.IdentityToken);
			}

			// Create a new authentication ticket holding the user identity.
			var ticket = new AuthenticationTicket(
				principal, new AuthenticationProperties(),
				OpenIdConnectServerDefaults.AuthenticationScheme);

			// Set the list of scopes granted to the client application.
			// Note: the offline_access scope must be granted
			// to allow OpenIddict to return a refresh token.
			ticket.SetScopes(new[] {
				OpenIdConnectConstants.Scopes.OpenId,
				OpenIdConnectConstants.Scopes.Email,
				OpenIdConnectConstants.Scopes.Profile,
				OpenIdConnectConstants.Scopes.OfflineAccess,
				OpenIddictConstants.Scopes.Roles
			}.Intersect(request.GetScopes()));

			return ticket;
		}
	}
}