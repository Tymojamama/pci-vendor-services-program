using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using System.Collections.Generic;
using System.Linq;

// Microsoft Dynamics CRM namespaces
using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Crm.Workflow;
using Microsoft.Crm.Workflow.Activities;

namespace PCI.VSP.Workflows
{
    [CrmWorkflowActivity("VSP Vendor Profile Threshold Workflow", "VSP Workflows")]
    public class VendorProfileThreshold : SequenceActivity
    {

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            try
            {
                // Get access to the Microsoft Dynamics CRM Web service proxy.
                IContextService contextService = (IContextService)executionContext.GetService(typeof(IContextService));
                IWorkflowContext context = contextService.Context;

                ICrmService crmService = context.CreateCrmService();
                ProcessPercentComplete(crmService, context.PrimaryEntityId);
            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                throw new InvalidPluginExecutionException(
                    String.Format("An error occurred in the {0} workflow.",
                       this.GetType().ToString()),
                     ex);
            }

            return base.Execute(executionContext);
        }

        private void ProcessPercentComplete(ICrmService crmService, Guid vendorId)
        {
            if (vendorId == Guid.Empty) { return; }

            // get vendorProfile
            VendorProfile vendorProfile = new VendorProfileDataLogic(crmService).Retrieve(vendorId);
            if (vendorProfile == null || vendorProfile.Id == Guid.Empty)
                throw new InvalidPluginExecutionException(String.Format("An error occurred in the {0} workflow. The vendor profile specified in the workflow execution context could not be found via query.",
                    this.GetType()));

            // get vendorQuestions
            IEnumerable<VendorQuestion> vqs = new VendorQuestionDataLogic(crmService).RetrieveMultipleByVendorProfile(vendorId);

            // get applicable questions
            IEnumerable<Question> qs = new QuestionDataLogic(crmService).RetrieveMultipleByVendorProfile(vendorId);

            // divide the number of vendorQuestions by Questions
            if (qs == null || qs.Count() == 0)
            {
                VendorProfileStatus = ">=";
                VendorProfilePercentComplete = 100.ToString();
            }

            Int32 percentComplete = Convert.ToInt32(Math.Round(Convert.ToDecimal(vqs.Count() / qs.Count()), 0));
            VendorProfilePercentComplete = percentComplete.ToString();

            Int32 emailThreshhold = vendorProfile.EmailThreshold;
            if (emailThreshhold == 0) { return; }

            if (percentComplete >= emailThreshhold)
                VendorProfileStatus = ">=";
            else
                VendorProfileStatus = "<";
        }

        public static DependencyProperty VendorProfileStatusProperty =
            DependencyProperty.Register("VendorProfileStatus", typeof(System.String), typeof(VendorProfileThreshold));

        [CrmOutput("Vendor Profile Status")]
        public String VendorProfileStatus
        {
            get
            {
                return (String)base.GetValue(VendorProfileStatusProperty);
            }
            set
            {
                base.SetValue(VendorProfileStatusProperty, value);
            }
        }

        public static DependencyProperty VendorProfilePercentCompleteProperty =
            DependencyProperty.Register("VendorProfilePercentComplete", typeof(System.String), typeof(VendorProfileThreshold));

        [CrmOutput("Vendor Profile Percent Complete")]
        public String VendorProfilePercentComplete
        {
            get
            {
                return (String)base.GetValue(VendorProfilePercentCompleteProperty);
            }
            set
            {
                base.SetValue(VendorProfilePercentCompleteProperty, value);
            }
        }

    }
}
