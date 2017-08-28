using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.Sdk;
using System.Diagnostics;
using PCI.VSP.Data.Classes;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class ContactDataLogic : ServiceObjectBase<Model.Contact, Guid>
    {
        private static String[] _columnSet = new String[] { "contactid", "parentcustomerid", "emailaddress1", "firstname", "lastname", "vsp_username", 
            "vsp_lastlogin", "vsp_mustchangepassword", "vsp_username", "vsp_securityquestion", "vsp_islocked", "vsp_isadmin" };
        public const String _entityName = "contact";

        public ContactDataLogic(Model.IAuthenticationRequest authRequest)
            : base(authRequest, _entityName, _columnSet)
        {
        }

        public Model.Contact Retrieve(string userName, System.Security.SecureString password)
        {
            try
            {
                QueryExpression qe = new QueryExpression(_entityName)
                {
                    ColumnSet = new ColumnSet(_columnSet),
                    Criteria = GetFilterExpression(userName, password)
                };
                qe.LinkEntities.Add(GetAccountLinkEntity());

                List<DynamicEntity> des = base.RetrieveMultiple(qe);
                DynamicEntity de = base.GetUniqueResult(des);
                if (de == null) { return null; }
                
                // Get the account information
                Microsoft.Crm.Sdk.Customer customer = de.Properties["parentcustomerid"] as Microsoft.Crm.Sdk.Customer;
                Model.Account account = new AccountDataLogic(_authRequest).Retrieve(customer.Value);
                Model.Contact contact = null;

                if (account != null)
                {
                    contact = CastToDerivative(de, account.CategoryCode);
                    UpdateLastLoginDate(contact.Id);
                }

                return contact;
            }
            catch (Model.CustomExceptions.TooManyResultsException ex)
            {
                ex.Data.Add("userName", userName);
                throw;
            }
        }

        private void UpdateLastLoginDate(Guid contactId)
        {
            Model.VendorAgent va = new Model.VendorAgent()
            {
                Id = contactId,
                LastLogin = DateTime.Now
            };
            try
            {
                base.Update(va);
            }
            catch
            {
                // I don't care if your login date isn't updated. Log it here if you care but don't throw the exception.
            }
        }

        public Model.Contact RetrieveByContactId(Guid contactId)
        {
            try
            {
                return this.Retrieve(contactId, String.Empty);
            }
            catch (Exception ex)
            {
                ex.Data.Add("contactId", contactId.ToString());
                throw;
            }
        }

        public Model.Contact Retrieve(string userName)
        {
            try
            {
                return this.Retrieve(new Guid?(), userName);
            }
            catch (Exception ex)
            {
                ex.Data.Add("userName", userName);
                throw;
            }
        }

        private Model.Contact Retrieve(Guid? contactId, string userName)
        {
            QueryExpression qe = new QueryExpression(_entityName);
            qe.ColumnSet = new ColumnSet(_columnSet);
            if (contactId.HasValue)
                qe.Criteria.AddCondition("contactid", ConditionOperator.Equal, contactId.Value);
            if (!String.IsNullOrWhiteSpace(userName))
                qe.Criteria.AddCondition("vsp_username", ConditionOperator.Equal, userName);

            qe.LinkEntities.Add(GetAccountLinkEntity());

            List<DynamicEntity> des = base.RetrieveMultiple(qe);
            DynamicEntity de = base.GetUniqueResult(des);

            // Get the account information
            Microsoft.Crm.Sdk.Customer customer = de.Properties["parentcustomerid"] as Microsoft.Crm.Sdk.Customer;
            Model.Account account = new AccountDataLogic(_authRequest).Retrieve(customer.Value);

            return CastToDerivative(de, account.CategoryCode);
        }

        public Model.Contact[] RetrieveByAccount(Guid accountId)
        {
            try
            {
                FilterExpression fe = new FilterExpression();
                fe.AddCondition("parentcustomerid", ConditionOperator.Equal, accountId);

                QueryExpression qe = new QueryExpression(_entityName)
                {
                    ColumnSet = new ColumnSet(_columnSet),
                    Criteria = fe
                };
                qe.LinkEntities.Add(GetAccountLinkEntity());

                List<DynamicEntity> des = base.RetrieveMultiple(qe);
                if (des == null) { return null; }

                // Get the account information
                Model.Account account = new AccountDataLogic(_authRequest).Retrieve(accountId);

                Model.Contact[] contacts = new Model.Contact[des.Count];

                for (int counter = 0; counter < des.Count; counter++)
                    contacts[counter] = CastToDerivative(des[counter], account.CategoryCode);
                return contacts;
            }
            catch (Exception ex)
            {
                ex.Data.Add("accountId", accountId.ToString());
                throw;
            }
        }

        public Model.Contact RetrieveForPasswordReset(string userName, System.Security.SecureString securityAnswer)
        {
            try
            {
                String[] columnSet = new String[] { "contactid", "vsp_username", "parentcustomerid" };
                List<DynamicEntity> des = base.RetrieveMultiple(GetFilterForPasswordReset(userName, securityAnswer), null, columnSet);
                DynamicEntity de = base.GetUniqueResult(des);

                // Get the account information
                Microsoft.Crm.Sdk.Customer customer = de.Properties["parentcustomerid"] as Microsoft.Crm.Sdk.Customer;
                Model.Account account = new AccountDataLogic(_authRequest).Retrieve(customer.Value);

                return CastToDerivative(de, account.CategoryCode);
            }
            catch (Model.CustomExceptions.TooManyResultsException ex)
            {
                ex.Data.Add("userName", userName);
                throw;
            }
        }

        public Boolean UpdatePassword(Guid contactId, System.Security.SecureString password)
        {
            Model.VendorAgent va = new Model.VendorAgent()
            {
                Id = contactId,
                Password = Globals.UnwrapSecureString(password)
            };

            base.Update(va);
            return true;
        }

        public Boolean UpdatePasswordQuestionAndAnswer(Guid contactId, String question, System.Security.SecureString answer)
        {
            Model.VendorAgent va = new Model.VendorAgent()
            {
                Id = contactId,
                SecurityQuestion = question,
                SecurityAnswer = Globals.UnwrapSecureString(answer)
            };

            base.Update(va);
            return true;
        }

        public new Guid Create(Model.Contact contact)
        {
            return base.Create(contact);
        }

        public new void Update(Model.Contact contact)
        {
            base.Update(contact);
        }

        private Model.Contact CastToDerivative(DynamicEntity de, String accountCategoryCode)
        {
            if (de == null || String.IsNullOrEmpty(accountCategoryCode)) { return null; }

            // examine to see if this contact is a vendor agent or a client rep
            switch (accountCategoryCode.ToLower())
            {
                case DataConstants.vendor:
                    return new Model.VendorAgent(de);
                case DataConstants.client:
                    return new Model.ClientRep(de);
            }
            return null;
        }

        //private FilterExpression GetFilterExpression(string userName)
        //{
        //    FilterExpression fe = new FilterExpression();
        //    fe.AddCondition("vsp_username", ConditionOperator.Equal, userName);
        //    return fe;
        //}

        private FilterExpression GetFilterExpression(string userName, System.Security.SecureString password)
        {
            FilterExpression fe = new FilterExpression();
            fe.AddCondition("vsp_username", ConditionOperator.Equal, userName);
            fe.AddCondition("vsp_password", ConditionOperator.Equal, Globals.UnwrapSecureString(password));
            return fe;
        }

        private FilterExpression GetFilterForPasswordReset(string userName, System.Security.SecureString securityAnswer)
        {
            FilterExpression fe = new FilterExpression();
            fe.AddCondition("vsp_username", ConditionOperator.Equal, userName);
            fe.AddCondition("vsp_securityanswer", ConditionOperator.Equal, Globals.UnwrapSecureString(securityAnswer));
            return fe;
        }

        private LinkEntity GetAccountLinkEntity()
        {
            LinkEntity le = new LinkEntity()
            {
                JoinOperator = JoinOperator.Inner,
                LinkFromAttributeName = "parentcustomerid",
                LinkFromEntityName = _entityName,
                LinkToAttributeName = "accountid",
                LinkToEntityName = AccountDataLogic._entityName
            };
            return le;
        }

    }
}
