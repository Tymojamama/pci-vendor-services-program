using PCI.VSP.Data;
using PCI.VSP.Data.CRM.DataLogic;
using PCI.VSP.Data.CRM.Model;
using PCI.VSP.Data.Enums;
using PCI.VSP.Services.Filtering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PCI.VSP.Services.Filtering
{
    public partial class FilterSummary
    {
        private void WriteBenchesMatches()
        {
            StringBuilder.AppendLine("-- PRODUCT MATCHES --").AppendLine();

            foreach (Filter.VendorProductFilterResult _selectedVendorProduct in SelectedProductsFromFilter)
            {
                VendorProduct _vendorProduct = VspService.GetVendorProduct(_selectedVendorProduct.VendorProductId);
                Vendor _vendor = null;

                try
                {
                    _vendor = VspService.GetVendor(_vendorProduct.VendorId);
                }
                catch
                {

                }

                if (_vendor != null)
                {
                    StringBuilder.AppendLine("Vendor: " + _vendor.Name);
                }

                if (_vendorProduct != null)
                {
                    StringBuilder.Append("Vendor Product");
                    if (_selectedVendorProduct.IsBenchmark)
                    {
                        StringBuilder.Append(" (Benchmark)");
                    }
                    StringBuilder.AppendLine(": " + _vendorProduct.VendorProductName);
                }

                string _passDescription;
                if (_selectedVendorProduct.Passed)
                {
                    _passDescription = "Passed";
                }
                else
                {
                    _passDescription = "Failed";
                }

                StringBuilder.Append(_passDescription).Append(" At Question Rank ").Append(_selectedVendorProduct.QuestionRank).AppendLine().AppendLine();
            }
        }
    }
}
