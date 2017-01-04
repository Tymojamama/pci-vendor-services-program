using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Services.Model
{
    [Serializable]
    public class ClientQuestion : Data.CRM.Model.ClientQuestion, IAccountQuestion
    {
        private readonly Guid _tempId = Guid.NewGuid();
        private String _assetClass = String.Empty;
        private string _noteBase64Data = string.Empty;
        private string _noteFileName = string.Empty;
        private List<PCI.VSP.Data.CRM.Model.Annotation> _notes = new List<Data.CRM.Model.Annotation>();
        private PCI.VSP.Data.CRM.Model.Annotation _documentTemplate = new Data.CRM.Model.Annotation();

        public ClientQuestion(PCI.VSP.Data.CRM.Model.ClientQuestion vq)
            : base(vq)
        {
        }

        public Guid AccountId
        {
            get
            {
                return base.ClientId;
            }
            set
            {
                base.ClientId = value;
            }
        }

        public string AccountWording
        {
            get
            {
                return base.ClientWording;
            }
            set
            {
                base.ClientWording = value;
            }
        }

        public Guid TempId
        {
            get
            {
                return _tempId;
            }
        }

        public string NoteBase64Data
        {
            get
            {
                return _noteBase64Data;
            }
            set
            {
                _noteBase64Data = value;
            }
        }

        public string NoteFileName
        {
            get
            {
                return _noteFileName;
            }
            set
            {
                _noteFileName = value;
            }
        }


        public string CommentToPCI
        {
            get
            {
                return base.ClientCommentToPCI;
            }
            set
            {
                base.ClientCommentToPCI = value;
            }
        }

        public string CommentFromPCI
        {
            get
            {
                return base.PCICommentToClient;
            }
            set
            {
                base.PCICommentToClient = value;
            }
        }

        public List<Data.CRM.Model.Annotation> Notes
        {
            get
            {
                return _notes;
            }
            set
            {
                _notes = value;
            }
        }


        public PCI.VSP.Data.CRM.Model.Annotation DocumentTemplate
        {
            get
            {
                return _documentTemplate;
            }
            set
            {
                _documentTemplate = value;
            }
        }
    }
}
