using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCI.VSP.Data.CRM.Model;

namespace PCI.VSP.Data.CRM.DataLogic
{
    public class DocumentTemplateDataLogic : ServiceObjectBase<DocumentTemplate, Guid>
    {
        private const string _entityName = "vsp_documenttemplate";

        public DocumentTemplateDataLogic(IAuthenticationRequest authRequest) : base(authRequest, _entityName, null) { }
    }
}
