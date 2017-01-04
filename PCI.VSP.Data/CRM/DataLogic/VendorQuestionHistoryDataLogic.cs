using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class VendorQuestionHistoryDataLogic: ServiceObjectBase<Model.VendorQuestionHistory, Guid>
    {
        public const String _entityName = "vsp_vendorquestionhistory";

        public VendorQuestionHistoryDataLogic(Model.IAuthenticationRequest authRequest)
            : base(authRequest, _entityName, null)
        {
        }

        public Guid Save(Model.VendorQuestionHistory vqh)
        {
            try
            {
                if (vqh.Id == Guid.Empty)
                    return base.Create(vqh);
                else
                {
                    base.Update(vqh);
                    return vqh.Id;
                }
            }
            catch (Exception ex)
            {
                ex.Data.Add("VendorQuestionHistoryId", vqh.Id.ToString());
                ex.Data.Add("VendorQuestionId", vqh.VendorQuestionId.ToString());
                throw;
            }
        }
    }
}
