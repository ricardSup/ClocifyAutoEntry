using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class SummaryReportSettings
    {
        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("subgroup")]
        public string Subgroup { get; set; }
    }

    public class Settings
    {
        [JsonProperty("weekStart")]
        public string WeekStart { get; set; }

        [JsonProperty("timeZone")]
        public string TimeZone { get; set; }

        [JsonProperty("timeFormat")]
        public string TimeFormat { get; set; }

        [JsonProperty("dateFormat")]
        public string DateFormat { get; set; }

        [JsonProperty("sendNewsletter")]
        public bool SendNewsletter { get; set; }

        [JsonProperty("weeklyUpdates")]
        public bool WeeklyUpdates { get; set; }

        [JsonProperty("longRunning")]
        public bool LongRunning { get; set; }

        [JsonProperty("scheduledReports")]
        public bool ScheduledReports { get; set; }

        [JsonProperty("approval")]
        public bool Approval { get; set; }

        [JsonProperty("pto")]
        public bool Pto { get; set; }

        [JsonProperty("alerts")]
        public bool Alerts { get; set; }

        [JsonProperty("reminders")]
        public bool Reminders { get; set; }

        [JsonProperty("timeTrackingManual")]
        public bool TimeTrackingManual { get; set; }

        [JsonProperty("summaryReportSettings")]
        public SummaryReportSettings SummaryReportSettings { get; set; }

        [JsonProperty("isCompactViewOn")]
        public bool IsCompactViewOn { get; set; }

        [JsonProperty("dashboardSelection")]
        public string DashboardSelection { get; set; }

        [JsonProperty("dashboardViewType")]
        public string DashboardViewType { get; set; }

        [JsonProperty("dashboardPinToTop")]
        public bool DashboardPinToTop { get; set; }

        [JsonProperty("projectListCollapse")]
        public int ProjectListCollapse { get; set; }

        [JsonProperty("collapseAllProjectLists")]
        public bool CollapseAllProjectLists { get; set; }

        [JsonProperty("groupSimilarEntriesDisabled")]
        public bool GroupSimilarEntriesDisabled { get; set; }

        [JsonProperty("myStartOfDay")]
        public string MyStartOfDay { get; set; }

        [JsonProperty("projectPickerTaskFilter")]
        public bool ProjectPickerTaskFilter { get; set; }

        [JsonProperty("lang")]
        public object Lang { get; set; }

        [JsonProperty("theme")]
        public string Theme { get; set; }
    }

    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("memberships")]
        public List<Membership> Memberships { get; set; }

        [JsonProperty("profilePicture")]
        public string ProfilePicture { get; set; }

        [JsonProperty("activeWorkspace")]
        public string ActiveWorkspace { get; set; }

        [JsonProperty("defaultWorkspace")]
        public string DefaultWorkspace { get; set; }

        [JsonProperty("settings")]
        public Settings Settings { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }


}
