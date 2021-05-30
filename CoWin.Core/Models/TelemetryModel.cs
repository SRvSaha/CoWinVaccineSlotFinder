using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Core.Models
{
    public class TelemetryModel
    {
        public Guid UniqueId { get; set; }
        public string AppVersion { get; set; }
        public string Source { get; set; }
        public DateTime BookedOn { get; set; }
        public double TimeTakenToBookInSeconds { get; set; }
        public string CaptchaMode { get; set; }
        public int Latitude { get; set; }
        public int Longitude { get; set; }
        public int PINCode { get; set; }
        public string District { get; set; }
        public string State { get; set; }
        public int BeneficiaryCount { get; set; }
        public int MinimumAge { get; set; }
        public int MaximumAge { get; set; }
    }
}
