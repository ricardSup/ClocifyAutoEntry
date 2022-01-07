using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Entry
    {
        public Entry()
        { 
            this.TagIds = null;
            this.CustomFields = new List<object> { };
        }

        [JsonProperty("start")]
        public string Start { get; set; }

        [JsonProperty("billable")]
        public string Billable { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("projectId")]
        public string ProjectId { get; set; }

        [JsonProperty("taskId")]
        public object TaskId { get; set; }

        [JsonProperty("end")]
        public string End { get; set; }

        [JsonProperty("tagIds")]
        public object TagIds { get; set; }

        [JsonProperty("customFields")]
        public List<object> CustomFields { get; set; }
    }


}
