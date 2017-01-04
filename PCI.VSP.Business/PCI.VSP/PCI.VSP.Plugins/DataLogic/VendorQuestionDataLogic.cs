using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;

namespace PCI.VSP.Plugins.DataLogic
{
    internal class VendorQuestionDataLogic : ServiceObjectBase<Model.VendorQuestion>
    {
        public const String _entityName = "vsp_vendorquestion";

        public VendorQuestionDataLogic(ICrmService crmService) : base(crmService) { }

        public new Guid Create(Model.VendorQuestion question)
        {
            return base.Create(question);
        }

        public void Save(Model.VendorQuestion vq, Guid contactId)
        {
            
            if (vq == null) { return; }
            try
            {
                if (vq.Id != Guid.Empty)
                {
                    vq.LastUpdated = DateTime.UtcNow;
                    base.Update(vq);
                }
                else
                    vq.Id = base.Create(vq);
            }
            catch (Exception ex)
            {
                ex.Data.Add("QuestionId", vq.QuestionId);
                ex.Data.Add("VendorId", vq.VendorId);
                ex.Data.Add("VendorProductId", vq.VendorProductId);
                ex.Data.Add("ContactId", contactId);
                throw;
            }
        }

        /// <summary>
        /// Returns all of the Vendor Questions that are based on a specific VSP Question
        /// </summary>
        /// <param name="id">ID of a VSP Question</param>
        /// <returns>List of Vendor Questions</returns>
        public List<Model.VendorQuestion> RetrieveByVSPQuestionId(Guid id)
        {
            List<Model.VendorQuestion> result = new List<Model.VendorQuestion>();
            QueryExpression qe = new QueryExpression(_entityName) { ColumnSet = new AllColumns(), Criteria = new FilterExpression() };
            qe.Criteria.Conditions.Add(new ConditionExpression("vsp_questionid", ConditionOperator.Equal, id));
            var queryResults = base.RetrieveMultiple(qe);

            if (queryResults != null && queryResults.Count > 0)
                result = queryResults.Select(e => new Model.VendorQuestion((DynamicEntity)e)).OrderBy(d => d.SortOrder).ToList();

            return result;
        }
    }
}
