using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Services
{
    class SecurityService
    {
        // Security
        //Model.IUser ValidateUser(AuthenticationRequest ticketRequest);
        //Boolean ResetUserPassword(ResetPasswordRequest rpr);
        //Boolean ChangePassword(ChangePasswordRequest changePasswordRequest);
        //Boolean ChangePasswordQuestionAndAnswer(ChangePasswordQuestionRequest cpqr);
        //Model.IUser GetUser(String username);
        //Model.IUser GetUser(Guid contactId);

        private AuthenticationRequest GetDefaultAuthRequest()
        {
            return new AuthenticationRequest()
            {
                Username = Data.Globals.CrmServiceSettings.Username,
                Password = Data.Globals.CrmServiceSettings.Password
            };
        }

        private Model.IUser ConvertContactToIUser(Data.CRM.Model.Contact c)
        {
            Model.IUser user = null;
            if (c.GetType() == typeof(Data.CRM.Model.VendorAgent))
            {
                Data.CRM.Model.VendorAgent va = c as Data.CRM.Model.VendorAgent;
                if (va.IsAdmin)
                    user = new Model.VendorAdmin(va);
                else
                    user = new Model.VendorAgent(va);
            }
            else if (c.GetType() == typeof(Data.CRM.Model.ClientRep))
                user = new Model.ClientRep((Data.CRM.Model.ClientRep)c);
            return user;
        }

        private Model.IUser ValidateContact(AuthenticationRequest authRequest)
        {
            Data.CRM.DataLogic.ContactDataLogic cdl = new Data.CRM.DataLogic.ContactDataLogic(authRequest);
            Data.CRM.Model.Contact c = cdl.Retrieve(authRequest.Username, authRequest.Password);
            if (c == null) { return null; }
            return ConvertContactToIUser(c);
        }

        public Model.IUser ValidateUser(AuthenticationRequest authRequest)
        {
            // retrieve the user information
            // could be a contact or a systemuser, cast to IUser

            // attempt to retrieve the systemuser
            Data.CRM.DataLogic.SystemUserDataLogic sudl = new Data.CRM.DataLogic.SystemUserDataLogic(authRequest);
            Data.CRM.Model.SystemUser su = sudl.Retrieve(authRequest.DomainName, authRequest.Username);
            Model.IUser c = null;

            // if no systemuser, attempt to retrieve the contact record
            if (su == null)
                c = ValidateContact(authRequest);

            Model.IUser user = null;
            if (su != null)
                user = new Model.SystemUser(su);
            else if (c != null)
                user = c;
            else
                return null;

            return user;
        }

        public Boolean ResetUserPassword(ResetPasswordRequest rpr)
        {
            PCI.VSP.Data.CRM.DataLogic.ContactDataLogic cdl = new Data.CRM.DataLogic.ContactDataLogic(GetDefaultAuthRequest());
            PCI.VSP.Data.CRM.Model.Contact c = cdl.RetrieveForPasswordReset(rpr.Username, rpr.SecurityAnswer);
            if (c == null) { return false; }

            return cdl.UpdatePassword(c.Id, rpr.NewPassword);
        }

        public Boolean ChangePassword(ChangePasswordRequest cpr)
        {
            AuthenticationRequest authRequest = new AuthenticationRequest()
            {
                Username = cpr.Username,
                Password = cpr.OldPassword
            };

            Model.IUser c = ValidateContact(authRequest);
            if (c == null) { return false; }

            Data.CRM.DataLogic.ContactDataLogic cdl = new Data.CRM.DataLogic.ContactDataLogic(authRequest);
            return cdl.UpdatePassword(c.Id, cpr.NewPassword);
        }

        public Boolean ChangePasswordQuestionAndAnswer(ChangePasswordQuestionRequest cpqr)
        {
            AuthenticationRequest authRequest = new AuthenticationRequest()
            {
                Username = cpqr.Username,
                Password = cpqr.Password
            };

            Model.IUser c = ValidateContact(authRequest);
            if (c == null) { return false; }

            Data.CRM.DataLogic.ContactDataLogic cdl = new Data.CRM.DataLogic.ContactDataLogic(authRequest);
            return cdl.UpdatePasswordQuestionAndAnswer(c.Id, cpqr.PasswordQuestion, cpqr.PasswordAnswer);
        }

        public Model.IUser GetUser(String username)
        {
            PCI.VSP.Data.CRM.DataLogic.ContactDataLogic cdl = new Data.CRM.DataLogic.ContactDataLogic(GetDefaultAuthRequest());
            PCI.VSP.Data.CRM.Model.Contact c = cdl.Retrieve(username);
            if (c == null) { return null; }

            Model.IUser user = null;
            if (c.GetType() == typeof(Data.CRM.Model.VendorAgent))
            {
                Data.CRM.Model.VendorAgent va = c as Data.CRM.Model.VendorAgent;
                if (va.IsAdmin)
                    user = new Model.VendorAdmin(va);
                else
                    user = new Model.VendorAgent(va);
            }
            else if (c.GetType() == typeof(Data.CRM.Model.ClientRep))
                user = new Model.ClientRep((Data.CRM.Model.ClientRep)c);
            return user;
        }

        public Model.IUser GetUser(Guid contactId)
        {
            Data.CRM.DataLogic.ContactDataLogic cdl = new Data.CRM.DataLogic.ContactDataLogic(GetDefaultAuthRequest());
            return ConvertContactToIUser(cdl.RetrieveByContactId(contactId));
        }
    }
}
