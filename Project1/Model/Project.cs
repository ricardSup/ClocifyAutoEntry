using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    

    public class Project
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("hourlyRate")]
        public object HourlyRate { get; set; }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        [JsonProperty("workspaceId")]
        public string WorkspaceId { get; set; }

        [JsonProperty("billable")]
        public bool Billable { get; set; }

        [JsonProperty("memberships")]
        public List<Membership> Memberships { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("estimate")]
        public object Estimate { get; set; }

        [JsonProperty("archived")]
        public bool Archived { get; set; }

        [JsonProperty("duration")]
        public object Duration { get; set; }

        [JsonProperty("clientName")]
        public string ClientName { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("costRate")]
        public object CostRate { get; set; }

        [JsonProperty("timeEstimate")]
        public object TimeEstimate { get; set; }

        [JsonProperty("budgetEstimate")]
        public object BudgetEstimate { get; set; }

        [JsonProperty("public")]
        public bool Public { get; set; }

        [JsonProperty("template")]
        public bool Template { get; set; }
    }


}
