using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;
using PCI.VSP.Plugins.DataLogic;
using PCI.VSP.Plugins.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PCI.CRM.Plugins
{
    public class OngoingApprovalMigration : IPlugin
    {
        private const string _entityName = "new_projecttask";

        private ClientEngagementDataLogic clientEngagementLogic;
        private OverageApprovalDataLogic overageApprovalLogic;
        private TaskDataLogic taskLogic;

        private ICrmService CrmService;

        public void Execute(IPluginExecutionContext context)
        {
            DynamicEntity entity = null;
            try
            {
#if DEBUG
                //tracelistener tl = new textwritertracelistener(system.io.file.createtext(@"c:\temp\pci.vsp.plugins.ongoingapprovalmigration.log"));
                //trace.listeners.add(tl);
#endif

                Trace.WriteLine("context.InputParameters.Properties:");
                foreach (PropertyBagEntry pbe in context.InputParameters.Properties)
                    Trace.WriteLine("Name: " + pbe.Name + "; Value: " + pbe.Value.ToString());
                Trace.WriteLine(string.Empty);

                if (!context.InputParameters.Properties.Contains(ParameterName.Target) || !(context.InputParameters.Properties[ParameterName.Target] is DynamicEntity))
                    return;

                entity = (DynamicEntity)context.InputParameters.Properties[ParameterName.Target];

                // Test for an entity type and message supported by the plug-in.
                Trace.WriteLine("Message name: " + context.MessageName);
                Trace.WriteLine("Entity name: " + entity.Name);
                Trace.WriteLine(String.Empty);

                if (entity.Name != _entityName) { return; }
                switch (context.MessageName)
                {
                    case MessageName.Create:
                        break;
                    default:
                        return;
                }

                CrmService = context.CreateCrmService(true);

                clientEngagementLogic = new ClientEngagementDataLogic(CrmService);
                overageApprovalLogic = new OverageApprovalDataLogic(CrmService);
                PlanClientEngagementDataLogic planLogic = new PlanClientEngagementDataLogic(CrmService);

                taskLogic = new TaskDataLogic(CrmService);

                ComponentTask new_task = new ComponentTask(entity);
                Task task_id = taskLogic.Retrieve(new_task.TaskID);
                
                if (new_task.ClientEngagement != Guid.Empty)
                {
                    ClientEngagement new_engagement = clientEngagementLogic.Retrieve(new_task.ClientEngagement);
                    if (new_engagement.RenewedEngagement != Guid.Empty)
                    {
                        ClientEngagement renewed_engagement = clientEngagementLogic.Retrieve(new_engagement.RenewedEngagement);
                        List<OverageApproval> approvals = overageApprovalLogic.RetrieveByCustomerEngagement(renewed_engagement.Id);

                        IEnumerable<OverageApproval> related_approvals = approvals.Where(a => a.IsOngoing && a.TaskInstance == new_task.TaskInstance && a.ComponentTaskID == task_id.Code);

                        if (related_approvals != null)
                        {
                            foreach (OverageApproval related_approval in related_approvals)
                            {
                                OverageApproval new_approval = new OverageApproval()
                                {
                                    ComponentTask = new_task.Id,
                                    ClientEngagement = new_engagement.Id,
                                    ComponentTaskID = related_approval.ComponentTaskID,
                                    TaskInstance = related_approval.TaskInstance,
                                    HoursApproved = related_approval.HoursApproved,
                                    ApprovalDate = related_approval.ApprovalDate,
                                    FromRenewal = true,
                                    IsOngoing = related_approval.IsOngoing,
                                    CreatedBy = related_approval.CreatedBy,
                                    ApprovedBy = related_approval.ApprovedBy
                                };

                                Guid new_approval_id = overageApprovalLogic.Create(new_approval);
                                Trace.WriteLine("New Overage Approval: " + new_approval_id);
                            }
                        }
                        List<PlanClientEngagement> plans = planLogic.RetrieveByClientEngagementId(renewed_engagement.Id);
                        foreach (var p in plans)
                        {
                            planLogic.Create(p.PlanId, new_engagement.Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("EXCEPTION: " + ex.Message);
                Trace.WriteLine("Stack: " + ex.StackTrace);

                if (ex.GetType() == typeof(System.Web.Services.Protocols.SoapException))
                {
                    Trace.WriteLine("Detail: ");
                    Trace.WriteLine(((System.Web.Services.Protocols.SoapException)ex).Detail.InnerText);
                }

                Exception innerException = ex.InnerException;
                while (innerException != null)
                {
                    Trace.WriteLine(" ");
                    Trace.WriteLine("INNER EXCEPTION: " + innerException.Message);
                    Trace.WriteLine("Stack: " + innerException.StackTrace);
                    innerException = innerException.InnerException;
                }

                throw;
            }
            finally
            {
                Trace.Flush();
                Trace.Close();
            }
        }
    }
}
