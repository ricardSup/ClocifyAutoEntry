using Newtonsoft.Json;
using Npgsql;
using Project1.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Npgsql;

namespace Project1
{
    public class UploadTimeEntries
    {
        //List src:
        //https://csharp.hotexamples.com/examples/Npgsql/NpgsqlCommand/-/php-npgsqlcommand-class-examples.html
        //https://zetcode.com/csharp/postgresql/    
        //https://stackoverflow.com/questions/3876456/find-the-inner-most-exception-without-using-a-while-loop

        //List pending feature:
        //bikin config file terpisah, utk letak success/fail ftp, ambil data tasknya dsbnya
        //bikin feature utk generate data txtnya.

        public static void Main(string[] args)
        {
            
            DateTime dateTime = DateTime.Now;

            //Console.WriteLine("Enter date of entry in format MM/DD/YYYY: ");    
            //string inputDt = Console.ReadLine();
            string inputDt = "";

            if (args.Length > 0)
            {
                inputDt = args[0].Trim();
            }

            //bool isInput = false;
            if (!string.IsNullOrEmpty(inputDt) && DateTime.TryParse(inputDt, out dateTime)) ;
                //isInput = true;

            //Console.WriteLine($"Date to be process : {dateTime.ToLongDateString()}");
            //string descrIsInput = isInput ? "yes" : "no";
            //Console.WriteLine($"and it was from input : {descrIsInput}");

            string apiKey = "Nzc0Y2E2N2UtZTUzMS00OWViLWE3OTYtNTQ5MWYxY2IxOWVi";
            string workspaceId;
            string projectId;

            string nameFile = dateTime.ToLongDateString();
            string path = String.Format("C:\\Task\\{0}.txt", nameFile);

            bool statusEntry = false;
            string message = "";
            try
            {
                bool isDupl = checkDataAlreadyExistOrNot(dateTime);
                if (isDupl)
                {
                    throw new Exception("Data Already Exists");
                }

                // Get the object used to communicate with the server.
                FtpWebRequest request5 = (FtpWebRequest)WebRequest.Create("ftp://localhost/clocify/" + nameFile + ".txt");
                request5.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                request5.Credentials = new NetworkCredential("clocify", "P@ssw0rd.123");

                // Copy the contents of the file to the request stream.
                byte[] fileContents;
                using (StreamReader sourceStream = new StreamReader(path))
                {
                    fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                }

                request5.ContentLength = fileContents.Length;

                using (Stream requestStream = request5.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }

                using (FtpWebResponse response5 = (FtpWebResponse)request5.GetResponse())
                {
                    //Console.WriteLine($"Upload File Complete, status {response5.StatusDescription}");
                }

                // Read each line of the file into a string array. Each element
                // of the array is one line of the file.
                string[] lines = System.IO.File.ReadAllLines(path);

                if (lines.Length == 0)
                {
                    throw new Exception("File Not Found in " + path.ToString());
                }

                // Display the file contents by using a foreach loop.
                string content = "";
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].Trim(); 
                    // Use a tab to indent each line of the file.
                    //Console.WriteLine("\t" + line);
                    content = content + line + "; ";
                }

                var client = new RestClient("https://api.clockify.me/api/v1/workspaces");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-Api-Key", apiKey);
                var response = client.Execute<List<Workspace>>(request);
                //Console.WriteLine(response.Data[0].Id);

                workspaceId = response.Data[0].Id;

                client = new RestClient("https://api.clockify.me/api/v1/workspaces/"+ workspaceId + "/projects");
                client.Timeout = -1;
                request = new RestRequest(Method.GET);
                request.AddHeader("X-Api-Key", apiKey);
                var response2 = client.Execute<List<Project>>(request);
                //Console.WriteLine(response2.Data.Where(x => x.Name == "CIMP:AdIns").First().Id);

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

                    Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                    //pending feature, move to ftp success
                    FtpWebRequest request4 = (FtpWebRequest)WebRequest.Create("ftp://localhost/clocify/" + nameFile + ".txt");
                    request4.Method = WebRequestMethods.Ftp.Rename;
                    request4.Credentials = new NetworkCredential("clocify", "P@ssw0rd.123");
                    request4.RenameTo = "./Success/" + unixTimestamp.ToString() + "_" + nameFile + ".txt";   //Relative path 
                    FtpWebResponse response4 = (FtpWebResponse)request4.GetResponse();
                    statusEntry = true;
                }
                else
                {
                    throw new Exception("workspaceId or projectId has a problem");
                }
            }
            catch (Exception ex)
            {
                statusEntry = false;
                message = GetExceptionMessages(ex);
            }

            string strEntryStatus = statusEntry ? "INS" : "ERR";
            insertHist(dateTime, nameFile, strEntryStatus, message);   
        }

        public static string GetExceptionMessages(Exception ex)
        {
            if (ex.InnerException is null)
                return ex.Message;
            else return $"{ex.Message}\n{GetExceptionMessages(ex.InnerException)}";
        }

        public static bool checkDataAlreadyExistOrNot(DateTime trxDt)
        {
            bool flag = false;
            string strConnString = "Server=localhost;Port=5432;User Id=postgres;Password=P@ssw0rd.123;Database=Clockify";
            try
            {
                NpgsqlConnection objConn = new NpgsqlConnection(strConnString);
                objConn.Open();
                string strSelectCmd = "select * from UploadHist where status != 'ERR' and TrxUploadDate = @TrxUploadDate";
                NpgsqlCommand cmd = new NpgsqlCommand(strSelectCmd, objConn);
                cmd.Parameters.AddWithValue("@TrxUploadDate", trxDt);
                NpgsqlDataReader dRead = cmd.ExecuteReader();

                while (dRead.Read())
                {
                    if (dRead.FieldCount > 0)
                    {
                        flag = true;
                    }
                }

                objConn.Close();
            }
            catch (Exception ex)
            {

            }

            return flag;
        }

        public static void insertHist(DateTime trxDt, string fileName, string status = "INS", string message = "")
        {
            string strConnString = "Server=localhost;Port=5432;User Id=postgres;Password=P@ssw0rd.123;Database=Clockify";
            try
            {
                NpgsqlConnection objConn = new NpgsqlConnection(strConnString);
                objConn.Open();
                string strInsertCmd = "INSERT INTO UploadHist(TrxUploadDate,FileName,DtmCrt,UploadStatus)VALUES(:TrxUploadDate,:FileName,:DtmCrt,:UploadStatus)";
                NpgsqlCommand cmd = new NpgsqlCommand(strInsertCmd, objConn);

                NpgsqlParameter param1 = new NpgsqlParameter(":TrxUploadDate", NpgsqlTypes.NpgsqlDbType.Date);
                param1.Value = trxDt;
                cmd.Parameters.Add(param1);
                NpgsqlParameter param2 = new NpgsqlParameter(":FileName", NpgsqlTypes.NpgsqlDbType.Varchar, 200);
                param2.Value = fileName;
                cmd.Parameters.Add(param2);
                NpgsqlParameter param3 = new NpgsqlParameter(":DtmCrt", NpgsqlTypes.NpgsqlDbType.Date);
                param3.Value = DateTime.Now;
                cmd.Parameters.Add(param3);
                NpgsqlParameter param4 = new NpgsqlParameter(":UploadStatus", NpgsqlTypes.NpgsqlDbType.Varchar, 100);
                param4.Value = status;
                cmd.Parameters.Add(param4);
                cmd.ExecuteNonQuery();
                objConn.Close();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
