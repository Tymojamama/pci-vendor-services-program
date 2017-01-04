using PensionConsultants.Data.Access;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCI.VSP.Business.Components
{
    /// <summary>
    /// Represents the database used in the VSP.
    /// </summary>
    public class Database
    {
        public static DataAccessComponent Vsp;

        public static bool ConnectionSucceeded()
        {
            return Vsp.ConnectionSucceeded();
        }
    }
}
