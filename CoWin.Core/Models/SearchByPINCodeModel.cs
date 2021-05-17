using CoWin.Core.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Models
{
    public class SearchByPINCodeModel
    {
        public bool IsSearchToBeDoneByPINCode { get; set; }
        public List<string> PINCodes { get; set; }
    }
}
