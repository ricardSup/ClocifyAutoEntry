using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class HourlyRate
    {
        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class Round
    {
        [JsonProperty("round")]
        public string RoundX { get; set; }

        [JsonProperty("minutes")]
        public string Minutes { get; set; }
    }

    public class AutomaticLock
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("changeDay")]
        public string ChangeDay { get; set; }

        [JsonProperty("firstDay")]
        public string FirstDay { get; set; }

        [JsonProperty("dayOfMonth")]
        public int DayOfMonth { get; set; }

        [JsonProperty("olderThanPeriod")]
        public string OlderThanPeriod { get; set; }

        [JsonProperty("olderThanValue")]
        public int OlderThanValue { get; set; }
    }

    public class WorkspaceSettings
    {
        [JsonProperty("timeRoundingInReports")]
        public bool TimeRoundingInReports { get; set; }

        [JsonProperty("onlyAdminsSeeBillableRates")]
        public bool OnlyAdminsSeeBillableRates { get; set; }

        [JsonProperty("onlyAdminsCreateProject")]
        public bool OnlyAdminsCreateProject { get; set; }

        [JsonProperty("onlyAdminsSeeDashboard")]
        public bool OnlyAdminsSeeDashboard { get; set; }

        [JsonProperty("defaultBillableProjects")]
        public bool DefaultBillableProjects { get; set; }

        [JsonProperty("lockTimeEntries")]
        public DateTime? LockTimeEntries { get; set; }

        [JsonProperty("round")]
        public Round Round { get; set; }

        [JsonProperty("projectFavorites")]
        public bool ProjectFavorites { get; set; }

        [JsonProperty("canSeeTimeSheet")]
        public bool CanSeeTimeSheet { get; set; }

        [JsonProperty("canSeeTracker")]
        public bool CanSeeTracker { get; set; }

        [JsonProperty("projectPickerSpecialFilter")]
        public bool ProjectPickerSpecialFilter { get; set; }

        [JsonProperty("forceProjects")]
        public bool ForceProjects { get; set; }

        [JsonProperty("forceTasks")]
        public bool ForceTasks { get; set; }

        [JsonProperty("forceTags")]
        public bool ForceTags { get; set; }

        [JsonProperty("forceDescription")]
        public bool ForceDescription { get; set; }

        [JsonProperty("onlyAdminsSeeAllTimeEntries")]
        public bool OnlyAdminsSeeAllTimeEntries { get; set; }

        [JsonProperty("onlyAdminsSeePublicProjectsEntries")]
        public bool OnlyAdminsSeePublicProjectsEntries { get; set; }

        [JsonProperty("trackTimeDownToSecond")]
        public bool TrackTimeDownToSecond { get; set; }

        [JsonProperty("projectGroupingLabel")]
        public string ProjectGroupingLabel { get; set; }

        [JsonProperty("adminOnlyPages")]
        public List<string> AdminOnlyPages { get; set; }

        [JsonProperty("automaticLock")]
        public AutomaticLock AutomaticLock { get; set; }

        [JsonProperty("onlyAdminsCreateTag")]
        public bool OnlyAdminsCreateTag { get; set; }

        [JsonProperty("onlyAdminsCreateTask")]
        public bool OnlyAdminsCreateTask { get; set; }

        [JsonProperty("timeTrackingMode")]
        public string TimeTrackingMode { get; set; }

        [JsonProperty("isProjectPublicByDefault")]
        public bool IsProjectPublicByDefault { get; set; }
    }

    public class Workspace
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("hourlyRate")]
        public HourlyRate HourlyRate { get; set; }

        [JsonProperty("memberships")]
        public List<Membership> Memberships { get; set; }

        [JsonProperty("workspaceSettings")]
        public WorkspaceSettings WorkspaceSettings { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("featureSubscriptionType")]
        public string FeatureSubscriptionType { get; set; }
    }


}
