using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Services.Model
{
    [Serializable]
    public class VendorQuestion : PCI.VSP.Data.CRM.Model.VendorQuestion, IAccountQuestion
    {
        private readonly Guid _tempId = Guid.NewGuid();
        private String _assetClass = String.Empty;
        private string _noteBase64Data = string.Empty;
        private string _noteFileName = string.Empty;
        private List<PCI.VSP.Data.CRM.Model.Annotation> _notes = new List<Data.CRM.Model.Annotation>();
        private PCI.VSP.Data.CRM.Model.Annotation _documentTemplate = new Data.CRM.Model.Annotation();

        public VendorQuestion(PCI.VSP.Data.CRM.Model.VendorQuestion vq) : base(vq) { }

        public VendorQuestion() : base() { }

        internal VendorQuestion(Guid vendorId, Guid vendorProductId, PCI.VSP.Data.CRM.Model.Question q)
        {
            if (q == null) { return; }

            // convert question to vendorquestion
            this.Name = q.Name;
            this.AnswerType = q.VendorAnswerType;
            this.AccountWording = q.VendorWording;
            this.QuestionDataType = q.QuestionDataType;
            this.QuestionId = q.QuestionId;
            this.VendorId = vendorId;
            this.VendorProductId = vendorProductId;
            this.ChoiceAnswers = q.ChoiceAnswers;
            this.QuestionType = q.QuestionType;
            this.PCICommentToVendor = q.PCICommentToVendor;
            this.PlanAssumptionId = q.PlanAssumptionId;
            this.VendorEntityName = q.VendorEntityName;
            this.AttributeName = q.AttributeName;
        }

        //public new String AssetClass
        //{
        //    get
        //    {
        //        if (!String.IsNullOrEmpty(_assetClass))
        //            return _assetClass;
        //        else
        //            return base.AssetClass;
        //    }
        //    set
        //    {
        //        _assetClass = value;
        //    }
        //}

        public Guid AccountId
        {
            get
            {
                return base.VendorId;
            }
            set
            {
                base.VendorId = value;
            }
        }

        public string AccountWording
        {
            get
            {
                return base.VendorWording;
            }
            set
            {
                base.VendorWording = value;
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
                return base.VendorCommentToPCI;
            }
            set
            {
                base.VendorCommentToPCI = value;
            }
        }

        public string CommentFromPCI
        {
            get
            {
                return base.PCICommentToVendor;
            }
            set
            {
                base.PCICommentToVendor = value;
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


        public Data.CRM.Model.Annotation DocumentTemplate
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
