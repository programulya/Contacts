using System;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Contacts.Filters;
using Contacts.Models;

namespace Contacts.Controllers
{
	[Authorize]
	[InitializeSimpleMembership]
	public class AccountController : Controller
	{
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult Login(LoginModel model, string returnUrl)
		{
			if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
			{
				return RedirectToLocal(returnUrl);
			}

			ModelState.AddModelError("", "The user name or password provided is incorrect");
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			WebSecurity.Logout();

			return RedirectToAction("Index", "Home");
		}

		[AllowAnonymous]
		public ActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult Register(RegisterModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
					WebSecurity.Login(model.UserName, model.Password);
					return RedirectToAction("Index", "Home");
				}
				catch (MembershipCreateUserException e)
				{
					ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
				}
			}

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Disassociate(string provider, string providerUserId)
		{
			var ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
			ManageMessageId? message = null;

			if (ownerAccount == User.Identity.Name)
			{
				using (
					var scope = new TransactionScope(TransactionScopeOption.Required,
					                                 new TransactionOptions {IsolationLevel = IsolationLevel.Serializable}))
				{
					var hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
					if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
					{
						OAuthWebSecurity.DeleteAccount(provider, providerUserId);
						scope.Complete();
						message = ManageMessageId.RemoveLoginSuccess;
					}
				}
			}

			return RedirectToAction("Manage", new {Message = message});
		}

		public ActionResult Manage(ManageMessageId? message)
		{
			ViewBag.StatusMessage =
				message == ManageMessageId.ChangePasswordSuccess
					? "Your password has been changed"
					: message == ManageMessageId.SetPasswordSuccess
						  ? "Your password has been set"
						  : message == ManageMessageId.RemoveLoginSuccess
							    ? "The external login was removed"
							    : "";
			ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
			ViewBag.ReturnUrl = Url.Action("Manage");

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Manage(LocalPasswordModel model)
		{
			bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
			ViewBag.HasLocalPassword = hasLocalAccount;
			ViewBag.ReturnUrl = Url.Action("Manage");
			if (hasLocalAccount)
			{
				if (ModelState.IsValid)
				{
					bool changePasswordSucceeded;
					try
					{
						changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
					}
					catch (Exception)
					{
						changePasswordSucceeded = false;
					}

					if (changePasswordSucceeded)
					{
						return RedirectToAction("Manage", new {Message = ManageMessageId.ChangePasswordSuccess});
					}

					ModelState.AddModelError("", "The current password is incorrect or the new password is invalid");
				}
			}
			else
			{
				var state = ModelState["OldPassword"];
				if (state != null)
				{
					state.Errors.Clear();
				}

				if (ModelState.IsValid)
				{
					try
					{
						WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
						return RedirectToAction("Manage", new {Message = ManageMessageId.SetPasswordSuccess});
					}
					catch (Exception)
					{
						ModelState.AddModelError("",
						                         String.Format(
							                         "Unable to create local account. An account with the name \"{0}\" may already exist",
							                         User.Identity.Name));
					}
				}
			}

			return View(model);
		}

		

		#region Helpers

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}

			return RedirectToAction("Index", "Home");
		}

		public enum ManageMessageId
		{
			ChangePasswordSuccess,
			SetPasswordSuccess,
			RemoveLoginSuccess,
		}

		internal class ExternalLoginResult : ActionResult
		{
			public ExternalLoginResult(string provider, string returnUrl)
			{
				Provider = provider;
				ReturnUrl = returnUrl;
			}

			public string Provider { get; private set; }
			public string ReturnUrl { get; private set; }

			public override void ExecuteResult(ControllerContext context)
			{
				OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
			}
		}

		private static string ErrorCodeToString(MembershipCreateStatus createStatus)
		{
			switch (createStatus)
			{
				case MembershipCreateStatus.DuplicateUserName:
					return "User name already exists. Please enter a different user name";

				case MembershipCreateStatus.DuplicateEmail:
					return "A user name for that e-mail address already exists. Please enter a different e-mail address";

				case MembershipCreateStatus.InvalidPassword:
					return "The password provided is invalid. Please enter a valid password value";

				case MembershipCreateStatus.InvalidEmail:
					return "The e-mail address provided is invalid. Please check the value and try again";

				case MembershipCreateStatus.InvalidAnswer:
					return "The password retrieval answer provided is invalid. Please check the value and try again";

				case MembershipCreateStatus.InvalidQuestion:
					return "The password retrieval question provided is invalid. Please check the value and try again";

				case MembershipCreateStatus.InvalidUserName:
					return "The user name provided is invalid. Please check the value and try again";

				case MembershipCreateStatus.ProviderError:
					return
						"The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator";

				case MembershipCreateStatus.UserRejected:
					return
						"The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator";

				default:
					return
						"An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator";
			}
		}

		#endregion
	}
}