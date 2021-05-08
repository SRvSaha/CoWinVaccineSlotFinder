using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoWin.Models
{
    public class CaptchaModel
    {

        [JsonProperty("captcha")]
        public string Captcha { get; set; }
    }
}
