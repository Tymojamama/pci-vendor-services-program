using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCI.VSP.Web.Security
{
    internal class Utility
    {
        internal PCI.VSP.Services.AuthenticationRequest GetAuthRequest()
        {
            PCI.VSP.Services.Model.IUser user = GetUser();
            PCI.VSP.Services.AuthenticationRequest authRequest = new Services.AuthenticationRequest()
            {
                Username = user.Username
            };
            return authRequest;
        }

        internal PCI.VSP.Services.Model.IUser GetUser()
        {
            PCI.VSP.Services.Model.IUser user = (PCI.VSP.Services.Model.IUser)HttpContext.Current.Session["User"];
            if (user == null)
            {
                PCI.VSP.Services.VspService service = new Services.VspService();
                user = service.GetUser(HttpContext.Current.User.Identity.Name);
                HttpContext.Current.Session["User"] = user;
            }
            return user;
        }

        internal static System.Security.SecureString MakeSecureString(string value)
        {
            if (String.IsNullOrWhiteSpace(value)) { return null; }
            System.Security.SecureString secureString = new System.Security.SecureString();
            foreach (Char c in value.Trim().ToCharArray())
                secureString.AppendChar(c);
            secureString.MakeReadOnly();
            return secureString;
        }
    }
}