using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    class Joinuser
    {
        [JsonProperty("username")]
        public string User { get; set; }
        [JsonProperty("userroom")]
        public string Room { get; set; }
        [JsonProperty("userid")]
        public string Id { get; set; }
    }
}
