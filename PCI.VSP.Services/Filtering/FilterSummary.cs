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
        public string Result
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_result))
                {
                    _result = GetSummary();
                    return _result;
                }
                else
                {
                    return _result;
                }
            }
            set
            {
                _result = value;
            }
        }

        private string _result;

        private List<Filter.VendorProductFilterResult> AllResultsFromFilter;
        private List<Filter.VendorProductFilterResult> SelectedProductsFromFilter;

        private StringBuilder StringBuilder;
        private VspService VspService;

        public FilterSummary(List<Filter.VendorProductFilterResult> _allResultsFromFilter, List<Filter.VendorProductFilterResult> _selectedProductsFromFilter)
        {
            AllResultsFromFilter = _allResultsFromFilter;
            SelectedProductsFromFilter = _selectedProductsFromFilter;

            StringBuilder = new StringBuilder();
            VspService = new VspService();
        }
    }
}
