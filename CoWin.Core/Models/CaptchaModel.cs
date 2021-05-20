using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Models
{
    public class CaptchaModel
    {
        public const string ENCRYPTED_TRAINED_MODEL = "YOUR_TRAINED_MODEL_HERE";
        public const string SECRET_KEY = "YOUR_MODEL_DECRYPTION_KEY";

        [JsonProperty("captcha")]
        public string Captcha { get; set; }

    }
}
