using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PCI.VSP.Management
{
    internal class VendorQuestionModule
    {
        internal void CheckExpiredVendorQuestions()
        {
            Trace.WriteLine("Entering CheckExpiredVendorQuestions");
            DataLogic.VendorQuestionDataLogic vqdl = new DataLogic.VendorQuestionDataLogic(Program.GetDefaultAuthRequest());

            List<Model.VendorQuestion> vqs = vqdl.RetrieveExpiredVendorQuestions();
            if (vqs == null || vqs.Count == 0) 
            {
                Trace.WriteLine("No Expired Vendor Questions Found.");
                Trace.WriteLine("Exiting CheckExpiredVendorQuestions");
                return; 
            }
            else
            {
                Trace.WriteLine("Expired Vendor Question count: " + vqs.Count);
            }

            foreach (Model.VendorQuestion vq in vqs)
                vq.InvalidAnswerReason = 2;
            Trace.WriteLine("Invalid Answer Reason set to: 2");

            vqdl.Update(vqs);
            Trace.WriteLine("Exiting CheckExpiredVendorQuestions");
        }
    }
}
