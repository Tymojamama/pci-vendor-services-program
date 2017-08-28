using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Data.CRM.Model
{
    public class DataRequest<TEntityType>
    {
        private Ticket _ticket = null;
        private TEntityType entity;

        public DataRequest(Ticket ticket, TEntityType entity)
        {
            if (ticket == null)
                throw new CustomExceptions.InvalidTicketException();
            _ticket = ticket;

            if (entity == null)
                throw new CustomExceptions.InvalidEntityException();
        }

    }
}
