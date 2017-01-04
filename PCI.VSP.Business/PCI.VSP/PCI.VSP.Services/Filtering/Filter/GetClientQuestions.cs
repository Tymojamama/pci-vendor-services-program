using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using PCI.VSP.Data.CRM.DataLogic;
using PCI.VSP.Data.CRM.Model;
using System.Diagnostics;

namespace PCI.VSP.Services.Filtering
{
    public partial class Filter
    {
        private void GetClientQuestions()
        {
            ClientQuestions = new List<ClientQuestion>();

            if (FilterPhase == Filter.FilterPhases.Phase1)
            {
                ClientQuestions = _clientQuestionDataLogic.RetrieveForPhase1Filter(ClientProject.Id);
            }
            else if (FilterPhase == Filter.FilterPhases.Phase2)
            {
                ClientQuestions = _clientQuestionDataLogic.RetrieveForPhase2Filter(ClientProject.Id);
            }
        }
    }
}
