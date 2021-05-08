using System.Collections.Generic;
using Newtonsoft.Json;


namespace CoWin.Models
{
    public partial class BookingModel
    {
        [JsonProperty("dose")]
        public long Dose { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("slot")]
        public string Slot { get; set; }

        [JsonProperty("beneficiaries")]
        public List<string> Beneficiaries { get; set; }

        [JsonProperty("captcha")]
        public string Captcha { get; set; }
    }

}