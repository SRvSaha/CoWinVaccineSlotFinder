using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Models
{
    public class SearchByDistrictModel
    {
        public bool IsSearchToBeDoneByDistrict { get; set; }
        public List<string> Districts { get; set; }
    }
}
