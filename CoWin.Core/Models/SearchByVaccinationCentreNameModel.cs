using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Models
{
    public class SearchByVaccinationCentreNameModel
    {
        public bool IsSearchToBeDoneByVaccinationCentreName { get; set; }
        public List<string> VaccinationCentreNames { get; set; }
    }
}
