using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using Umbraco.Web;

namespace UmbracoPreviewSpike
{
    public class ProtectedPreview : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PostAuthenticateRequest += context_AuthenticateRequest;
        }

        void context_AuthenticateRequest(object sender, EventArgs e)
        {
            if (UmbracoContext.Current != null && UmbracoContext.Current.InPreviewMode)
            {
                var user = Membership.GetUser("test2@test.com");
                var ticket = new FormsAuthenticationTicket(1, user.UserName, DateTime.Now, DateTime.Now.AddMinutes(30),
                    true, "", FormsAuthentication.FormsCookiePath);


                var identity = new FormsIdentity(ticket);
                //set the principal object
                var principal = new GenericPrincipal(identity, Roles.GetRolesForUser(user.UserName));
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = principal;
                }
            }
        }

        public void Dispose()
        {
            
        }
    }
}