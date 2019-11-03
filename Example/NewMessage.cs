using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    public class NewMessage
    {
        [JsonProperty("username")]
        public string User { get; set; }

        [JsonProperty("message")]
        public string Text { get; set; }

        [JsonProperty("messagecolor")]
        public string Color { get; set; }
        [JsonProperty("messagespeed")]
        public string Speed { get; set; }
        [JsonProperty("messagearea")]
        public string Area { get; set; }
        [JsonProperty("messagesize")]
        public string Size { get; set; }

    }
}
