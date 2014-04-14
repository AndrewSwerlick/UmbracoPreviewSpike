using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Profile;
using System.Web.Security;
using HennyPenny.Extranet.Web.Models;
using Umbraco.Core.Logging;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using umbraco.cms.businesslogic.member;

namespace UmbracoPreviewSpike.SurfaceControllers
{
    public class AuthSurfaceController : SurfaceController
    {
        public AuthSurfaceController(UmbracoContext ctx)
            : base(ctx)
        {

        }

        public AuthSurfaceController() : base() { }

        /// <summary>
        /// Renders the Login view
        /// @Html.Action("RenderLogin","AuthSurface");
        /// </summary>
        /// <returns></returns>
        public ActionResult RenderLogin()
        {
            var loginModel = new LoginViewModel();


            if (string.IsNullOrEmpty(HttpContext.Request["ReturnUrl"]))
            {
                //If returnURL is empty then set it to /
                loginModel.ReturnUrl = "/";
            }
            else
            {
                //Lets use the return URL in the querystring or form post
                loginModel.ReturnUrl = HttpContext.Request["ReturnUrl"];
            }

            return PartialView("Login", loginModel);
        }


        /// <summary>
        /// Handles the login form when user posts the form/attempts to login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //return RedirectToCurrentUmbracoPage();
                return PartialView("Login", model);
            }

            //Member already logged in - redirect to home
            MembershipUser currentUser = null;
            try
            {
                currentUser = Membership.GetUser();
            }
            catch{}

            if (currentUser != null)
            {
                LogHelper.Debug(this.GetType(), "User already logged in. Redirecting to homepage");
                return Redirect("/");
            }

            //Lets TRY to log the user in
            try
            {
                //Try and login the user...

                var validLogin = Membership.ValidateUser(model.Name, model.Password);

                if (validLogin)
                {
                    //Valid credentials

                    var checkMember = Membership.GetUser(model.Name);

                    //Check the member exists
                    if (checkMember != null)
                    {
                        var profile = ProfileBase.Create(model.Name);
                        //Let's check they have verified their email address

                        //If they have verified then lets log them in
                        //Set Auth cookie
                        FormsAuthentication.SetAuthCookie(checkMember.UserName, true);

                        //Once logged in - redirect them back to the return URL
                        if (string.IsNullOrEmpty(model.ReturnUrl))
                            model.ReturnUrl = "/";
                        return new RedirectResult(model.ReturnUrl);
                    }
                }
                else
                {
                    ModelState.AddModelError("LoginForm.", "Invalid details");
                    return PartialView("Login", model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("LoginForm.", "Error: " + ex.ToString());
                return PartialView("Login", model);
            }

            return PartialView("Login", model);
        }

        public ActionResult Logout()
        {
            if (Membership.GetUser() != null)
            {
                //Log member out
                FormsAuthentication.SignOut();

                //Redirect home
                return Redirect("/");
            }
            else
            {
                //Redirect home
                return Redirect("/");
            }
        }
    }
}