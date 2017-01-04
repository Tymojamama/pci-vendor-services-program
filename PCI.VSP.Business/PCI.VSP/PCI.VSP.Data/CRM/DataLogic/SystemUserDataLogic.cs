using System;
using System.Collections.Generic;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using PCI.VSP.Data.CRM.Model;
using PCI.VSP.Data.CRM.Model.CustomExceptions;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class SystemUserDataLogic : ServiceObjectBase<SystemUser, Guid>
    {
        private static String[] _columnSet = new String[]{ "systemuserid", "domainname", "firstname", "lastname", "internalemailaddress" };
        private const String _entityName = "systemuser";

        public SystemUserDataLogic(IAuthenticationRequest authRequest)
            : base(authRequest, _entityName, _columnSet)
        {
        }

        public SystemUser Retrieve(string domainName, string userName)
        {
            List<DynamicEntity> des = base.RetrieveMultiple(GetFilterExpression(domainName, userName));
            DynamicEntity de;

            try
            {
                de = base.GetUniqueResult(des);
            }
            catch (TooManyResultsException ex)
            {
                ex.Data.Add("domainName", domainName);
                ex.Data.Add("userName", userName);
                throw;
            }
            
            if (de == null) { return null; }
            return new SystemUser(de);
        }

        private FilterExpression GetFilterExpression(string domainName, string userName)
        {
            FilterExpression fe = new FilterExpression();
            fe.AddCondition("domainname", ConditionOperator.Equal, domainName + @"\" + userName);
            return fe;
        }

    }
}
