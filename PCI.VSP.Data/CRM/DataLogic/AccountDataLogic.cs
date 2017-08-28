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
    public class AccountDataLogic : ServiceObjectBase<Model.Account, Guid>
    {
        public const String _entityName = "account";
        private static String[] _columnSet = new String[] { "accountid", "name", "telephone1", "fax", "websiteurl", "address2_line1", "address2_line2", "address2_city", "address2_stateorprovince", "address2_postalcode", "customertypecode", DataConstants.accountcategorycode };

        public AccountDataLogic(Model.IAuthenticationRequest authRequest)
            : base(authRequest, _entityName, _columnSet)
        {

        }

        public new Model.Account Retrieve(Guid accountId)
        {
            Microsoft.Crm.SdkTypeProxy.account account = (Microsoft.Crm.SdkTypeProxy.account)base.Retrieve(accountId, _columnSet);
            return CastToDerivative(account);
        }

        public List<Model.Account> RetrieveByName(String accountName)
        {
            QueryExpression query = RetrieveByNameExpression(accountName);
            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null) { return null; }
            
            return des.Select<DynamicEntity, Model.Account>(de => CastToDerivative(de)).ToList();
        }

        public void UpdateVendorTimestamps(Guid vendorId)
        {
            Model.Vendor v = new Model.Vendor()
            {
                Id = vendorId,
                LastUpdated = DateTime.UtcNow,
            };
            base.Update(v);
        }

        public List<Model.Account> RetrieveAllAccounts()
        {
            QueryExpression query = new QueryExpression(_entityName);
            query.Criteria.AddCondition("accountcategorycode", ConditionOperator.Equal, 2);
            query.ColumnSet = new AllColumns();
            //query.Criteria.

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null) return null;
            return des.Select<DynamicEntity, Model.Account>(t => new Model.Account(t)).ToList();
        }

        public List<Model.Account> RetrieveAllVendors()
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new ColumnSet(new String[] { "accountid", "name" })
            };
            //query.Criteria.AddCondition("customertypecode", ConditionOperator.Equal, new Picklist(200003));
            query.Criteria.AddCondition(DataConstants.accountcategorycode, ConditionOperator.Equal, new Picklist(DataConstants.AccountCategoryVendorCode));
            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null) { return null; }

            return des.Select<DynamicEntity, Model.Account>(de => CastToDerivative(de)).ToList();
        }

        private QueryExpression RetrieveByNameExpression(String accountName)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new ColumnSet(_columnSet)
            };

            query.Criteria.AddCondition("name", ConditionOperator.Equal, accountName);
            return query;
        }

        private Model.Account CastToDerivative(DynamicEntity de)
        {
            if (de == null) { return null; }
            String accountCategory = null;

            if (de.Properties[DataConstants.accountcategorycode].GetType() == typeof(Microsoft.Crm.Sdk.Picklist))
                accountCategory = ((Microsoft.Crm.Sdk.Picklist)(de.Properties[DataConstants.accountcategorycode])).name;
            else
                accountCategory = de.Properties[DataConstants.accountcategorycode].ToString();

            //if (de.Properties["customertypecode"].GetType() == typeof(Microsoft.Crm.Sdk.Picklist))
            //    accountType = ((Microsoft.Crm.Sdk.Picklist)(de.Properties[DataConstants.accountcategorycode])).name;
            //else
            //    accountType = de.Properties["customertypecode"].ToString();

            // examine to see if this contact is a vendor agent or a client rep
            switch (accountCategory.ToLower())
            {
                case DataConstants.vendor:
                    return new Model.Vendor(de);
                //case "customer":
                case DataConstants.client:
                    return new Model.Client(de);
            }
            return null;
        }

        private Model.Account CastToDerivative(Microsoft.Crm.SdkTypeProxy.account account)
        {
            Model.Account newAccount = null;

            switch (account.accountcategorycode.name.ToLower())
            {
                case DataConstants.vendor:
                    newAccount = new Model.Vendor()
                    {
                        Address = account.address2_line1,
                        Address2 = account.address2_line2,
                        CategoryCode = account.accountcategorycode.name,
                        City = account.address2_city,
                        Fax = account.fax,
                        Id = account.accountid.Value,
                        Name = account.name,
                        Phone = account.telephone1,
                        PostalCode = account.address2_postalcode,
                        State = account.address2_stateorprovince,
                        WebsiteUrl = account.websiteurl
                    };
                    break;
                case DataConstants.client:
                    newAccount = new Model.Client()
                    {
                        Address = account.address2_line1,
                        Address2 = account.address2_line2,
                        CategoryCode = account.accountcategorycode.name,
                        City = account.address2_city,
                        Fax = account.fax,
                        Id = account.accountid.Value,
                        Name = account.name,
                        Phone = account.telephone1,
                        PostalCode = account.address2_postalcode,
                        State = account.address2_stateorprovince,
                        WebsiteUrl = account.websiteurl
                    };
                    break;
            }
            return newAccount;
        }
    }
}
