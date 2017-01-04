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
        private void SetClientQuestionDefaultRanksTo99()
        {
            foreach (ClientQuestion _clientQuestion in ClientQuestions)
            {
                if (_clientQuestion.Rank == 0)
                {
                    _clientQuestion.Rank = 99;
                }
            }
        }
    }
}
