using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCI.VSP.Data.CRM.Model;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class ProjectVendorDataLogic : ServiceObjectBase<ProjectVendor, Guid>
    {
        private const String _entityName = "vsp_projectvendor";
        private static String[] _columnSet = new String[] { "vsp_vendorproductid", "vsp_projectvendorid", "vsp_clientprojectid", "statuscode", "vsp_name", "vsp_excluded" }; // , "statecode"
        
        public ProjectVendorDataLogic(IAuthenticationRequest authRequest) : base(authRequest, _entityName, _columnSet) { }

        public new Model.ProjectVendor Retrieve(Guid id)
        {
            try
            {
                FilterExpression fe = new FilterExpression();
                fe.AddCondition("vsp_projectvendorid", ConditionOperator.Equal, id);
                List<DynamicEntity> results = base.RetrieveMultiple(fe);
                if (base.GetUniqueResult(results) != null)
                    return new Model.ProjectVendor(base.GetUniqueResult(results));
                else
                    return null;
            }
            catch (Exception e)
            {
                e.Data.Add("ClientProjectVendorId", id.ToString());
                throw;
            }
        }

        public List<Model.ProjectVendor> RetrieveMultiple(Guid accountId, Guid contactId)
        {
            try
            {
                QueryExpression query = GetProjectVendorExpression(accountId, contactId);
                List<DynamicEntity> results = base.RetrieveMultiple(query);
                if (results == null) { return null; }
                return results.Select<DynamicEntity, Model.ProjectVendor>(pv => new Model.ProjectVendor(pv)).ToList();
            }
            catch (Exception e)
            {
                e.Data.Add("AccountId", accountId.ToString());
                e.Data.Add("ContactId", contactId.ToString());
                throw;
            }
        }

        public List<Model.ProjectVendor> RetrieveMultipleByClientProject(Guid clientProjectId)
        {
            try
            {
                QueryExpression query = new QueryExpression()
                {
                    EntityName = _entityName,
                    ColumnSet = new AllColumns()
                };
                query.Criteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);
                List<DynamicEntity> results = base.RetrieveMultiple(query);
                if (results == null) { return null; }
                return results.Select<DynamicEntity, Model.ProjectVendor>(pv => new Model.ProjectVendor(pv)).ToList();
            }
            catch (Exception e)
            {
                e.Data.Add("ClientProjectId", clientProjectId.ToString());
                throw;
            }
        }

        public List<Model.ProjectVendor> RetrieveProjectVendors(Guid clientProjectId, bool isExcluded)
        {
            try
            {
                QueryExpression query = new QueryExpression() { EntityName = _entityName, ColumnSet = new AllColumns() };
                query.Criteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);
                query.Criteria.AddCondition("vsp_excluded", ConditionOperator.Equal, isExcluded);
                List<DynamicEntity> results = base.RetrieveMultiple(query);
                if (results == null) { return null; }
                return results.Select<DynamicEntity, Model.ProjectVendor>(pv => new Model.ProjectVendor(pv)).ToList();
            }
            catch (Exception e)
            {
                e.Data.Add("ClientProjectId", clientProjectId.ToString());
                e.Data.Add("isExcluded", isExcluded.ToString());
                throw;
            }
        }

        public Model.ProjectVendor RetrieveByClientProject(Guid clientProjectId, Guid vendorProductId)
        {
            try
            {
                QueryExpression query = new QueryExpression()
                {
                    EntityName = _entityName,
                    ColumnSet = new AllColumns()
                };
                query.Criteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);
                query.Criteria.AddCondition("vsp_vendorproductid", ConditionOperator.Equal, vendorProductId);
                List<DynamicEntity> results = base.RetrieveMultiple(query);
                if (results == null || results.Count == 0) { return null; }
                return new Model.ProjectVendor(base.GetUniqueResult(results));
            }
            catch (Exception e)
            {
                e.Data.Add("ClientProjectId", clientProjectId.ToString());
                e.Data.Add("VendorProductId", vendorProductId.ToString());
                throw;
            }
        }

        public List<ProjectVendor> RetrieveFilter1Benchmarks(Guid clientProjectId)
        {
            try
            {
                QueryExpression query = new QueryExpression() { EntityName = _entityName, ColumnSet = new AllColumns() };
                query.Criteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);
                query.Criteria.AddCondition("vsp_phase1benchmark", ConditionOperator.Equal, true);
                List<DynamicEntity> results = base.RetrieveMultiple(query);
                if (results == null) { return null; }
                return results.Select<DynamicEntity, Model.ProjectVendor>(pv => new Model.ProjectVendor(pv)).ToList();
            }
            catch (Exception e)
            {
                e.Data.Add("ClientProjectId", clientProjectId.ToString());
                throw;
            }
        }

        public List<ProjectVendor> RetrieveFilter2Benchmarks(Guid clientProjectId)
        {
            try
            {
                QueryExpression query = new QueryExpression() { EntityName = _entityName, ColumnSet = new AllColumns() };
                query.Criteria.AddCondition("vsp_clientprojectid", ConditionOperator.Equal, clientProjectId);
                query.Criteria.AddCondition("vsp_phase2benchmark", ConditionOperator.Equal, true);
                List<DynamicEntity> results = base.RetrieveMultiple(query);
                if (results == null) { return null; }
                return results.Select<DynamicEntity, Model.ProjectVendor>(pv => new Model.ProjectVendor(pv)).ToList();
            }
            catch (Exception e)
            {
                e.Data.Add("ClientProjectId", clientProjectId.ToString());
                throw;
            }
        }

        /// <summary>
        /// Sets the Project Vendor record to Vendor Approved if all criteria is met
        /// </summary>
        /// <param name="clientProjectId">Client Project ID</param>
        /// <param name="vendorProductId">Vendor Product ID</param>
        /// <returns>If the Project Vendor record was set to Vendor Approved</returns>
        public bool SetProjectVendorStatusToVendorApproved(Guid clientProjectId, Guid vendorProductId)
        {
            var pv = RetrieveByClientProject(clientProjectId, vendorProductId);
            var vql = new VendorQuestionDataLogic(_authRequest).RetrieveProjectInquiryQuestions(new VendorProductDataLogic(_authRequest).Retrieve(vendorProductId).VendorId, pv.Id, false);
            bool allConfirmed = true;

            foreach (VendorQuestion vq in vql)
                if (vq.Status != Enums.AccountQuestionStatuses.AccountConfirmed && vq.Status != Enums.AccountQuestionStatuses.PCI_Confirmed)
                {
                    allConfirmed = false;
                    break;
                }

            if (allConfirmed)
                if (pv.Status == Enums.ProjectVendorStatuses.Pending)
                {
                    pv.Status = Enums.ProjectVendorStatuses.VendorApproved;
                    Save(pv);
                }

            return allConfirmed;
        }

        public void Save(Model.ProjectVendor pv)
        {
            if (pv.Id == Guid.Empty)
                base.Create(pv);
            else
                base.Update(pv);
        }

        public List<Model.ProjectVendor> RetrieveMultipleByVendorQuestions(Guid[] vendorQuestionIds)
        {
            try
            {
                QueryExpression query = GetVendorQuestionExpression(vendorQuestionIds);
                List<DynamicEntity> results = base.RetrieveMultiple(query);
                if (results == null) { return null; }
                return results.Select<DynamicEntity, Model.ProjectVendor>(pv => new Model.ProjectVendor(pv)).ToList();
            }
            catch (Exception e)
            {
                e.Data.Add("VendorQuestionIds", vendorQuestionIds);
                throw;
            }
        }

        private QueryExpression GetVendorQuestionExpression(Guid[] vendorQuestionIds)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new ColumnSet(new String[] { "vsp_projectvendorid" })
            };
            // vendorQuestion => vendorProduct => projectVendor

            LinkEntity vendorQuestionLink = new LinkEntity(_entityName, "vsp_vendorquestion", "vsp_vendorproductid", "vsp_vendorproductid", JoinOperator.Inner);
            vendorQuestionLink.LinkCriteria.AddCondition("vsp_vendorquestionid", ConditionOperator.In, vendorQuestionIds);
            query.LinkEntities.Add(vendorQuestionLink);

            return query;
        }

        private QueryExpression GetProjectVendorExpression(Guid accountId, Guid contactId)
        {
            QueryExpression query = new QueryExpression()
            {
                EntityName = _entityName,
                ColumnSet = new ColumnSet(_columnSet)
            };

            // link to vendorProduct
            LinkEntity vendorProductLink = new LinkEntity()
            {
                JoinOperator = JoinOperator.Inner,
                 LinkFromEntityName = "vsp_projectvendor",
                 LinkFromAttributeName = "vsp_vendorproductid",
                 LinkToEntityName = "vsp_vendorproduct",
                 LinkToAttributeName = "vsp_vendorproductid"
            };
            query.LinkEntities.Add(vendorProductLink);

            // filter by vendorId
            vendorProductLink.LinkCriteria.AddCondition("vsp_accountid", ConditionOperator.Equal, accountId);

            return query;
        }
    }
}
