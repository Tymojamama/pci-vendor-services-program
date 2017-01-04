using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using PCI.VSP.Services;

namespace PCI.VSP.Web.Security
{
    public class VspRoleProvider : System.Web.Security.RoleProvider
    {
        private NameValueCollection _config = null;

        public override void Initialize(String name, NameValueCollection config)
        {
            _config = config;
            base.Initialize(name, config);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                if (_config["applicationName"] != null)
                    return Convert.ToString(_config["applicationName"]);
                return System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            return new String[] { "VendorAgent", "ClientRep", "SystemUser", "VendorAdmin" };
        }

        public override string[] GetRolesForUser(string username)
        {
            if (String.IsNullOrWhiteSpace(username))
                throw new System.Configuration.Provider.ProviderException("Username cannot be null.");

            VspService vspService = new VspService();
            Services.Model.IUser user = vspService.GetUser(username.Trim());
            if (user == null) { return null; }

            System.Type userType = user.GetType();
            List<String> roles = new List<String>();

            if (userType == typeof(PCI.VSP.Services.Model.VendorAgent))
                roles.Add("VendorAgent");
            if (userType == typeof(PCI.VSP.Services.Model.ClientRep))
                roles.Add("ClientRep");
            if (userType == typeof(PCI.VSP.Services.Model.SystemUser))
                roles.Add("SystemUser");
            if (userType == typeof(PCI.VSP.Services.Model.VendorAdmin))
                roles.Add("VendorAdmin");

            return roles.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            if (String.IsNullOrWhiteSpace(username))
                throw new System.Configuration.Provider.ProviderException("Username cannot be null.");

            if (String.IsNullOrWhiteSpace(roleName))
                throw new System.Configuration.Provider.ProviderException("RoleName cannot be null.");

            VspService vspService = new VspService();
            Services.Model.IUser user = vspService.GetUser(username.Trim());
            if (user == null) { return false; }

            System.Type userType = user.GetType();
            List<String> roles = new List<String>();

            if (userType == typeof(PCI.VSP.Services.Model.VendorAgent) && roleName.ToLower() == "vendoragent")
                return true;
            if (userType == typeof(PCI.VSP.Services.Model.ClientRep) && roleName.ToLower() == "clientrep")
                return true;
            if (userType == typeof(PCI.VSP.Services.Model.SystemUser) && roleName.ToLower() == "systemuser")
                return true;
            if (userType == typeof(PCI.VSP.Services.Model.VendorAdmin) && roleName.ToLower() == "vendoradmin")
                return true;
            return false;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            if (String.IsNullOrWhiteSpace(roleName))
                throw new System.Configuration.Provider.ProviderException("RoleName cannot be null.");

            String[] roles = GetAllRoles();
            if (roles == null) { return false; }

            foreach (String role in roles)
                if (role.ToLower() == roleName.Trim().ToLower()) { return true; }

            return false;
        }
    }

}