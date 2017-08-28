using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class QuestionCategoryDataLogic : ServiceObjectBase<Model.QuestionCategory, Guid>
    {
        private const String _entityName = "vsp_questioncategory";
        private static String[] _columnSet = new String[] { "vsp_name", "vsp_questioncategoryid", "vsp_sortorder" };

        public QuestionCategoryDataLogic(Model.IAuthenticationRequest authRequest)
            : base(authRequest, _entityName, _columnSet)
        {
        }

        public Dictionary<Guid, Model.QuestionCategory> RetrieveAllIntoDictionary()
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new AllColumns()
            };
            query.AddOrder("vsp_sortorder", OrderType.Ascending);

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null) { return null; }

            Dictionary<Guid, Model.QuestionCategory> qcd = new Dictionary<Guid, Model.QuestionCategory>();
            var qcl = des.Select<DynamicEntity, Model.QuestionCategory>(qc => new Model.QuestionCategory(qc)).ToList();
            foreach (var qc in qcl)
                qcd.Add(qc.Id, qc);

            return qcd;
        }

        public List<Model.QuestionCategory> RetrieveMultiple()
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new ColumnSet(_columnSet)
            };
            query.AddOrder("vsp_sortorder", OrderType.Ascending);

            List<DynamicEntity> des = base.RetrieveMultiple(query);
            if (des == null) { return null; }

            return des.Select<DynamicEntity, Model.QuestionCategory>(qc => new Model.QuestionCategory(qc)).ToList();
        }
    }
}
