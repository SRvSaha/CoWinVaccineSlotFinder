using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace CoWin.Models
{


    public partial class CovidVaccinationCenters
    {
        [JsonProperty("centers")]
        public List<Center> Centers { get; set; }
    }

    public partial class Center
    {
        [JsonProperty("center_id")]
        public long CenterId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("state_name")]
        public string StateName { get; set; }

        [JsonProperty("district_name")]
        public string DistrictName { get; set; }

        [JsonProperty("block_name")]
        public string BlockName { get; set; }

        [JsonProperty("pincode")]
        public long Pincode { get; set; }

        [JsonProperty("lat")]
        public long Lat { get; set; }

        [JsonProperty("long")]
        public long Long { get; set; }

        [JsonProperty("from")]
        public DateTime From { get; set; }

        [JsonProperty("to")]
        public DateTime To { get; set; }

        [JsonProperty("fee_type")]
        public string FeeType { get; set; }

        [JsonProperty("sessions")]
        public List<Session> Sessions { get; set; }
    }

    public partial class Session
    {
        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("available_capacity")]
        public long AvailableCapacity { get; set; }

        [JsonProperty("min_age_limit")]
        public long MinAgeLimit { get; set; }

        [JsonProperty("vaccine")]
        public string Vaccine { get; set; }

        [JsonProperty("slots")]
        public List<string> Slots { get; set; }

        [JsonProperty("available_capacity_dose1")]
        public long AvailableCapacityFirstDose { get; set; }

        [JsonProperty("available_capacity_dose2")]
        public long AvailableCapacitySecondDose { get; set; }
    }
}
