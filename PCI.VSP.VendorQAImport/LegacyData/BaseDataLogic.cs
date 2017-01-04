using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PCI.VSP.VendorQAImport.LegacyData
{
    public abstract class BaseDataLogic<TDataObject, TDataKey> : IDisposable
    {
        internal VSMEntities Context { get; private set; }

        public BaseDataLogic()
        {
            // Build the SqlConnection connection string.
            EntityConnectionStringBuilder con = new EntityConnectionStringBuilder();
            con.Provider = "System.Data.SqlClient";
            con.ProviderConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["VSMEntities"].ToString();
            con.Metadata = "res://*/" + "VSPLegacy" + ".csdl|res://*/" + "VSPLegacy" + ".ssdl|res://*/" + "VSPLegacy" + ".msl";

            Context = new VSMEntities();
            
        }
        public abstract void Save(TDataObject dataObject);
        //public void Save()
        //{
        //    Context.SaveChanges();
        //}
        public abstract void Delete(TDataKey id);
        public abstract TDataObject Retrieve(TDataKey id);
        public abstract List<TDataObject> RetrieveAll();

        public void Dispose()
        {
            Context.Dispose();
            Context = null;
        }
    }
    public partial class VSMEntities
    {
        //public VSMEntities(string connectionString)
        //    : base(connectionString)
        //{
        //}
    }
}
