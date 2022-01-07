using Newtonsoft.Json;
using Project1.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project1
{
    internal class UploadTimeEntries
    {
        public static void Main()
        {
            string apiKey = "Nzc0Y2E2N2UtZTUzMS00OWViLWE3OTYtNTQ5MWYxY2IxOWVi";
            DateTime dateTime = DateTime.Now;
            string workspaceId;
            string projectId;

            string nameFile = dateTime.ToLongDateString();
            string path = String.Format("C:\\Task\\{0}.txt", nameFile);

            // Read each line of the file into a string array. Each element
            // of the array is one line of the file.
            string[] lines = System.IO.File.ReadAllLines(path);

            // Display the file contents by using a foreach loop.
            string content = "";
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0)
                {
                    continue;
                }
                string line = lines[i].Trim(); 
                // Use a tab to indent each line of the file.
                Console.WriteLine("\t" + line);
                content = content + line + "; ";
            }

            var client = new RestClient("https://api.clockify.me/api/v1/workspaces");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-Api-Key", apiKey);
            var response = client.Execute<List<Workspace>>(request);
            Console.WriteLine(response.Data[0].Id);

            workspaceId = response.Data[0].Id;

            client = new RestClient("https://api.clockify.me/api/v1/workspaces/"+ workspaceId + "/projects");
            client.Timeout = -1;
            request = new RestRequest(Method.GET);
            request.AddHeader("X-Api-Key", apiKey);
            var response2 = client.Execute<List<Project>>(request);
            Console.WriteLine(response2.Data.Where(x => x.Name == "CIMP:AdIns").First().Id);

            projectId = response2.Data.Where(x => x.Name == "CIMP:AdIns").First().Id;

            //assign year, month, day, hour, min, seconds
            DateTime startDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 9, 0, 0);
            DateTime endDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 18, 0, 0);

            Entry entry = new Entry()
            {
                Start = startDateTime.ToUniversalTime().ToString("o"),
                End = endDateTime.ToUniversalTime().ToString("o"),
                ProjectId = response2.Data.Where(x => x.Name == "CIMP:AdIns").First().Id,
                TaskId = null,
                Billable = "false",
                Description = content
            };

            if (!string.IsNullOrEmpty(workspaceId) && !string.IsNullOrEmpty(projectId))
            {
                client = new RestClient("https://api.clockify.me/api/v1/workspaces/" + workspaceId + "/time-entries");
                client.Timeout = -1;
                var request3 = new RestRequest(Method.POST);
                request3.AddHeader("X-Api-Key", apiKey);
                request3.AddHeader("Content-Type", "application/json");
                var body = entry;
                request3.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);

                IRestResponse response3 = client.Execute(request3);
                Console.WriteLine(response3.Content);

                //pending feature, move to ftp success
            }
            else
            {
                Console.WriteLine("something wrong");
                //pending feature, move to ftp failed
            }

            Console.ReadLine();
        }
    }
}
