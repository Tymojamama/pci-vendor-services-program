using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Services.Filtering
{
    public class FilterResults
    {
        private readonly Dictionary<Guid, List<Filtering.Filter.VendorClientQuestion>> _mismatches;
        private readonly Dictionary<Guid, List<Filtering.Filter.VendorClientQuestion>> _matches;
        private readonly Dictionary<Guid, List<Filtering.Filter.VendorClientQuestion>> _errors;

        internal FilterResults(Dictionary<Guid, List<Filtering.Filter.VendorClientQuestion>> mismatches,
            Dictionary<Guid, List<Filtering.Filter.VendorClientQuestion>> matches,
            Dictionary<Guid, List<Filtering.Filter.VendorClientQuestion>> errors)
        {
            _mismatches = mismatches;
            _matches = matches;
            _errors = errors;
        }

        public Dictionary<Guid, List<Filtering.Filter.VendorClientQuestion>> Mismatches
        {
            get
            {
                return _mismatches;
            }
        }

        public Dictionary<Guid, List<Filtering.Filter.VendorClientQuestion>> Matches
        {
            get
            {
                return _matches;
            }
        }

        public Dictionary<Guid, List<Filtering.Filter.VendorClientQuestion>> Errors
        {
            get
            {
                return _errors;
            }
        }

        public List<Filtering.Filter.VendorProductRankMatch> RankedMatches { get; internal set; }
        public Filtering.Filter.FilterPhases FilterCategory { get; internal set; }
    }

}
