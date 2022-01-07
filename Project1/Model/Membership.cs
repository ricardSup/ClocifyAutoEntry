using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Model
{
    public class Membership
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("hourlyRate")]
        public object HourlyRate { get; set; }

        [JsonProperty("costRate")]
        public object CostRate { get; set; }

        [JsonProperty("targetId")]
        public string TargetId { get; set; }

        [JsonProperty("membershipType")]
        public string MembershipType { get; set; }

        [JsonProperty("membershipStatus")]
        public string MembershipStatus { get; set; }
    }
}
