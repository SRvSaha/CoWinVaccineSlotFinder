using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CoWin.Models
{
    public partial class OtpModel
    {
        [JsonProperty("secret")]
        public string Secret { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("txnId")]
        public string TransactionId { get; set; }

        [JsonProperty("otp")]
        public string Otp { get; set; }

        [JsonProperty("token")]
        public string BearerToken { get; set; }
    }
}