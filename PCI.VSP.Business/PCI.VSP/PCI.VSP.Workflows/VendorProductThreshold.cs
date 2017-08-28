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
    [CrmWorkflowActivity("VSP Vendor Product Threshold Workflow", "VSP Workflows")]
    public class VendorProductThreshold : SequenceActivity
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

        private void ProcessPercentComplete(ICrmService crmService, Guid vendorProductId)
        {
            if (vendorProductId == Guid.Empty) { return; }

            // get vendorProduct
            VendorProduct vendorProduct = new VendorProductDataLogic(crmService).Retrieve(vendorProductId);
            if (vendorProduct == null || vendorProduct.Id == Guid.Empty)
                throw new InvalidPluginExecutionException(String.Format("An error occurred in the {0} workflow. The vendor product specified in the workflow execution context could not be found via query.",
                    this.GetType()));

            // get vendorQuestions
            IEnumerable<VendorQuestion> vqs = new VendorQuestionDataLogic(crmService).RetrieveMultipleByVendorProduct(vendorProductId);

            // get applicable questions
            IEnumerable<Question> qs = new QuestionDataLogic(crmService).RetrieveMultipleByVendorProduct(vendorProductId);

            // divide the number of vendorQuestions by Questions
            if (qs == null || qs.Count() == 0)
            {
                VendorProductStatus = ">=";
                VendorProductPercentComplete = 100.ToString();
            }

            Int32 percentComplete = Convert.ToInt32(Math.Round(Convert.ToDecimal(vqs.Count() / qs.Count()), 0));
            VendorProductPercentComplete = percentComplete.ToString();

            Int32 emailThreshhold = vendorProduct.EmailThreshold;
            if (emailThreshhold == 0) { return; }

            if (percentComplete >= emailThreshhold)
                VendorProductStatus = ">=";
            else
                VendorProductStatus = "<";
        }

        public static DependencyProperty VendorProductStatusProperty =
            DependencyProperty.Register("VendorProductStatus", typeof(System.String), typeof(VendorProductThreshold));

        [CrmOutput("Vendor Product Status")]
        public String VendorProductStatus
        {
            get
            {
                return (String)base.GetValue(VendorProductStatusProperty);
            }
            set
            {
                base.SetValue(VendorProductStatusProperty, value);
            }
        }

        public static DependencyProperty VendorProductPercentCompleteProperty =
            DependencyProperty.Register("VendorProductPercentComplete", typeof(System.String), typeof(VendorProductThreshold));

        [CrmOutput("Vendor Product Percent Complete")]
        public String VendorProductPercentComplete
        {
            get
            {
                return (String)base.GetValue(VendorProductPercentCompleteProperty);
            }
            set
            {
                base.SetValue(VendorProductPercentCompleteProperty, value);
            }
        }

    }
}
