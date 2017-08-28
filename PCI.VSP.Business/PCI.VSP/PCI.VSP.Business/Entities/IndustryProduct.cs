using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCI.VSP.Business.Entities
{
    /// <summary>
    /// Represents a VSP industry product that can interact with the database.
    /// </summary>
    public class IndustryProduct
    {
        public Guid CreatedBy { get; private set; }
        public Guid IndustryProductId { get; private set; }
        public Guid ModifiedBy { get; private set; }
        public Guid StatusId { get; set; }
        public Guid StatusReasonId { get; set; }

        public string Name { get; set; }

        public int? EmailThresholdPercent { get; set; }

        public DateTime CreatedOn { get; private set; }
        public DateTime ModifiedOn { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="IndustryProduct"/> that does not exist as a database record.
        /// </summary>
        public IndustryProduct()
        {
            IndustryProductId = Guid.NewGuid();

            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new instance of <see cref="IndustryProduct"/> that has a related database record.
        /// </summary>
        /// <param name="industryProductId">Used to get the IndustryProduct database record.</param>
        public IndustryProduct(Guid industryProductId)
        {
            IndustryProductId = industryProductId;

            throw new NotImplementedException();
        }
    }
}
