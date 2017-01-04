using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class ClientProjectDataLogic : ServiceObjectBase<Model.ClientProject, Guid>
    {
        private const String _entityName = "vsp_clientproject";

        public ClientProjectDataLogic(Model.IAuthenticationRequest authRequest) : base(authRequest, _entityName, null) { }

        public new Model.ClientProject Retrieve(Guid clientProjectId)
        {
            QueryExpression query = new QueryExpression(_entityName);
            query.ColumnSet = new AllColumns();
            query.Criteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null) { return null; }

            return new Model.ClientProject(base.GetUniqueResult(des));
        }

        public void SaveFilterResults(Guid clientProjectId, Enums.FilterCategory filterCategory, string filterResultSummary)
        {
            string clientProjectName = Retrieve(clientProjectId).ClientProjectName;
            String phaseText = String.Empty;
            switch (filterCategory)
            {
                case Enums.FilterCategory.Filter1:
                    phaseText = "1";
                    break;
                case Enums.FilterCategory.Filter2:
                    phaseText = "2";
                    break;
            }

            Microsoft.Crm.SdkTypeProxy.annotation a = new Microsoft.Crm.SdkTypeProxy.annotation()
            {
                objectid = new Lookup("vsp_clientproject", clientProjectId),
                objecttypecode = new EntityNameReference("vsp_clientproject"),
                notetext = clientProjectName + " Filter " + phaseText + " Results Summary - " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(),
                isdocument = new CrmBoolean(true),
                documentbody = Convert.ToBase64String(new UTF8Encoding().GetBytes(filterResultSummary)),
                filename = clientProjectName + " Filter " + phaseText + " Results Summary - " + DateTime.Now.ToString("yyyy_MM_dd__hh_mm_ss_tt") + ".txt"
            };
            ServiceBroker.GetServiceInstance(base._authRequest).Create(a);
        }
    }
}
