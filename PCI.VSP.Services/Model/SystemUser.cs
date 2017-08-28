using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Services.Model
{
    public class SystemUser : PCI.VSP.Data.CRM.Model.SystemUser, IUser
    {
        internal SystemUser(PCI.VSP.Data.CRM.Model.SystemUser su)
            : base(su)
        {
        }
        public String CrmTicket { get; set; }

        public string SecurityQuestion
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string SecurityAnswer
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime LastLogin
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool MustChangePassword
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Guid AccountId
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Boolean HasPlaintextPassword
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsLocked
        {
            get
            {
                return false;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

    }
}
