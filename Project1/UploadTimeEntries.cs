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
using System.Configuration;
using System.Collections.Specialized;

namespace Project1
{
    public class UploadTimeEntries
    {
        //List src:
        //https://csharp.hotexamples.com/examples/Npgsql/NpgsqlCommand/-/php-npgsqlcommand-class-examples.html
        //https://zetcode.com/csharp/postgresql/    
        //https://stackoverflow.com/questions/3876456/find-the-inner-most-exception-without-using-a-while-loop

        //List pending feature:

        public static void Main(string[] args)
        {
            var clocifyApiSettings = ConfigurationManager.GetSection("clocifyApiSettings") as NameValueCollection;
            string apiKey = ConfigurationManager.AppSettings["apiKey"].ToString();
            string pathSourceTaskFile = ConfigurationManager.AppSettings["pathSourceTaskFile"].ToString();
            var ftpSettings = ConfigurationManager.GetSection("ftpSettings") as NameValueCollection;
            string ftpPath = ftpSettings["path"];
            string ftpUsername = ftpSettings["username"];
            string ftpPassword = ftpSettings["password"];

            DateTime dateTime = DateTime.Now;

            string inputDt = "";

            if (args.Length > 0)
            {
                inputDt = args[0].Trim(); //MM/DD/YYYY
            }

            if (!string.IsNullOrEmpty(inputDt) && DateTime.TryParse(inputDt, out dateTime)) ;

            string workspaceId;
            string projectId;

            string nameFile = dateTime.ToLongDateString() + ".txt";
            string path = String.Format(pathSourceTaskFile, nameFile);

            bool statusEntry = false;
            string message = "";
            try
            {
                bool isDupl = checkDataAlreadyExistOrNot(dateTime);
                if (isDupl)
                {
                    throw new Exception("Data Already Exists");
                }

                FtpWebRequest request5 = (FtpWebRequest)WebRequest.Create(String.Format(ftpPath, nameFile));
                request5.Method = WebRequestMethods.Ftp.UploadFile;

                request5.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

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

                string[] lines = System.IO.File.ReadAllLines(path);

                if (lines.Length == 0)
                {
                    throw new Exception("File Not Found in " + path.ToString());
                }

                string content = "";
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    content = content + line + "; ";
                }

                workspaceId = GetWorkspaceId(apiKey, clocifyApiSettings);
                projectId = GetProjectId(apiKey, clocifyApiSettings, workspaceId);

                DateTime startDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 9, 0, 0);
                DateTime endDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 18, 0, 0);

                Entry entry = new Entry()
                {
                    Start = startDateTime.ToUniversalTime().ToString("o"),
                    End = endDateTime.ToUniversalTime().ToString("o"),
                    ProjectId = projectId,
                    TaskId = null,
                    Billable = "false",
                    Description = content
                };

                if (!string.IsNullOrEmpty(workspaceId) && !string.IsNullOrEmpty(projectId))
                {
                    InsertNewEntry(apiKey, clocifyApiSettings, workspaceId, entry);

                    Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                    FtpWebRequest request4 = (FtpWebRequest)WebRequest.Create(String.Format(ftpPath, nameFile));
                    request4.Method = WebRequestMethods.Ftp.Rename;
                    request4.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                    request4.RenameTo = "./Success/" + unixTimestamp.ToString() + "_" + nameFile;   //Relative path 
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
            InsertHist(dateTime, nameFile, strEntryStatus, message);

            GenerateFile(dateTime);
        }

        public static void GenerateFile(DateTime trxDt)
        {
            DateTime nextDay = trxDt.AddDays(1);
            string path = ConfigurationManager.AppSettings["pathSourceTaskFile"].ToString();
            string nameFile = nextDay.ToLongDateString();
            string fileName = String.Format(path, nameFile);

            try
            {
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Create a new file     
                using (StreamWriter sw = File.CreateText(fileName))
                {
                    sw.WriteLine("1.");
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

        public static void InsertNewEntry(string apiKey, NameValueCollection nameValueCollection, string workspaceId, Entry entry)
        {
            var client = new RestClient(String.Format(nameValueCollection["APIInsertEntry"], workspaceId));
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-Api-Key", apiKey);
            request.AddHeader("Content-Type", "application/json");
            var body = entry;
            request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
        }

        public static string GetProjectId(string apiKey, NameValueCollection nameValueCollection, string workspaceId)
        {

            var client = new RestClient(String.Format(nameValueCollection["APIGetProject"], workspaceId));
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-Api-Key", apiKey);
            var response = client.Execute<List<Project>>(request);
            return response.Data.Where(x => x.Name == "CIMP:AdIns").First().Id;
        }

        public static string GetWorkspaceId(string apiKey, NameValueCollection nameValueCollection)
        {
            var client = new RestClient(nameValueCollection["APIGetWorkspace"]);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-Api-Key", apiKey);
            var response = client.Execute<List<Workspace>>(request);
            return response.Data[0].Id;
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
            string strConnString = ConfigurationManager.AppSettings["connString"].ToString();
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
            return flag;
        }

        public static void InsertHist(DateTime trxDt, string fileName, string status = "INS", string message = "")
        {
            string strConnString = ConfigurationManager.AppSettings["connString"].ToString();
            NpgsqlConnection objConn = new NpgsqlConnection(strConnString);
            objConn.Open();
            string strInsertCmd = "INSERT INTO UploadHist(TrxUploadDate,FileName,DtmCrt,UploadStatus)VALUES(:TrxUploadDate,:FileName,:DtmCrt,:UploadStatus, :Message)";
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
            NpgsqlParameter param5 = new NpgsqlParameter(":Message", NpgsqlTypes.NpgsqlDbType.Varchar);
            param5.Value = message;
            cmd.Parameters.Add(param5);
            cmd.ExecuteNonQuery();
            objConn.Close();
        }
    }
}
