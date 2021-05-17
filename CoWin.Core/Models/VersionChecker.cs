using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Models
{
    public class VersionChecker
    {
        public bool IsUpdatedVersionAvailable()
        {
            return false;
        }

        public bool IsVersionUpdateMandatory()
        {
            return false;
        }

        public string GetLatestVersionFeatureInfo()
        {
            return "";
        }
    }
}
