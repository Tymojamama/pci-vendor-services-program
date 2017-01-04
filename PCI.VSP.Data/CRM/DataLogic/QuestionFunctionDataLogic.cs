using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.Sdk;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class QuestionFunctionDataLogic : ServiceObjectBase<Model.QuestionCategory, Guid>
    {
        private const String _entityName = "vsp_questionfunction";
        private static String[] _columnSet = new String[] { "vsp_name", "vsp_questionfunctionid", "vsp_sortorder" };

        public QuestionFunctionDataLogic(Model.IAuthenticationRequest authRequest)
            : base(authRequest, _entityName, _columnSet)
        {
        }

        public Dictionary<Guid, Model.QuestionFunction> RetrieveAllIntoDictionary()
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new AllColumns()
            };
            query.AddOrder("vsp_sortorder", OrderType.Ascending);

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null) { return null; }

            Dictionary<Guid, Model.QuestionFunction> qfd = new Dictionary<Guid, Model.QuestionFunction>();
            var qfl = des.Select<DynamicEntity, Model.QuestionFunction>(qf => new Model.QuestionFunction(qf)).ToList();
            foreach (var qf in qfl)
                qfd.Add(qf.Id, qf);

            return qfd;
        }

        public List<Model.QuestionFunction> RetrieveMultiple()
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new AllColumns()
            };
            query.AddOrder("vsp_sortorder", OrderType.Ascending);

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null) { return null; }

            return des.Select<DynamicEntity, Model.QuestionFunction>(qf => new Model.QuestionFunction(qf)).ToList();
        }
    }
}
