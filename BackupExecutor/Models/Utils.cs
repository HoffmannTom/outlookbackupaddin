using BackupAddInCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using static BackupExecutor.BackupTool;
namespace BackupExecutor
{
    static internal class Utils
    {

        //static internal long size { get; set; }
        //static internal int order { get; set; }

        #region SEND API REQUESTS
        // Static function to send a POST request
        public static async Task SendPostRequestAsync(string url, BackupSettings config, Logger log)
        {
            try
            {
                // Retrieve PC name and current date
                string pcName = Environment.MachineName;
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");



                var values = new Dictionary<string, string>
                {
                    { "nome", $"{pcName}" },
                    { "data", $"{currentDate}" },
                    { "tamanho", " (" + Convertbytes(config.Filesizelastbackup) + ")" }
                };


                // Convert the JSON data to a string
                string jsonString = System.Text.Json.JsonSerializer.Serialize(values);


                //sends http post request ,if the api is down it saves the request in a file, and sends it when the api is back up
                await SendSavedRequestsAsync(url, log, jsonString);


            }
            catch (Exception ex)
            {
                // Log any exception that occurs
                log($"Exception caught in SendPostRequestAsync: {ex.Message}");
            }
        }
        #endregion

        #region SaveFile BadResponses & ReadFile
        static bool SaveDATAinfile_BadResponses(string jsonString)
        {
            try
            {
                string path = AppContext.BaseDirectory + "BadRequests.txt";

                if (!File.Exists(path))
                {

                    // Create a new file with the new line
                    File.WriteAllText(path, jsonString);
                    return true;
                }

                // Read existing content
                string existingContent = File.ReadAllText(path);

                // Combine new line with existing content
                string updatedContent = jsonString + "\n" + existingContent;

                // Overwrite file with updated content
                File.WriteAllText(path, updatedContent);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task SendSavedRequestsAsync(string url, Logger log, string jsonString)
        {
            string path = AppContext.BaseDirectory + $"BadResponses.txt";
            List<string> failedRequests = new List<string>();

            // Check if the file exists and has content
            if (File.Exists(path) && new FileInfo(path).Length > 0)
            {
                failedRequests = File.ReadAllLines(path).ToList();
            }
            else
            {
                // If the file doesn't exist or is empty, use the provided jsonString
                failedRequests.Add(jsonString);
            }

            using (HttpClient client = new HttpClient())
            {
                foreach (var requestJson in failedRequests)
                {
                    try
                    {
                        var content = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(url, content);

                        if (response.IsSuccessStatusCode)
                        {
                            log($"Successfully sent request: {requestJson}");
                        }
                        else
                        {
                            log($"Failed to send request: {requestJson}");
                            if (SaveDATAinfile_BadResponses(jsonString))
                            {
                                log("Request saved in file");
                            }
                            else
                            {
                                log("Cannot save request in file");
                            }


                            // Optionally, save the failed request again
                        }
                    }
                    catch (Exception ex)
                    {
                        log($"Exception caught while sending request: {ex.Message}");
                    }
                }
            }

            // Optionally, clear the file after successfully processing all requests
            if (File.Exists(path))
            {
                File.WriteAllText(path, string.Empty);
            }
        }




        #endregion


        #region Send Email
        static bool CheckForInternetConnection()
        {
            try
            {
                Dns.GetHostEntry("www.google.com");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static String Convertbytes(long file)
        {
            //Long path might occur
            //https://docs.microsoft.com/en-us/windows/win32/fileio/naming-a-file?redirectedfrom=MSDN#maxpath
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };



            //algoritmo para converter o tamanho do ficheiro
            //double len = new FileInfo(filename).Length;
            int order = 0;
            while (file >= 1024 && order < sizes.Length - 1)
            {
                order++;
                file /= 1024;
            }


            return String.Format("{0:0.##} {1}", file, sizes[order]);
        }


        static void QueueEmail(MailMessage mail, Logger log)
        {
            BackupSettings.emailQueue.Add(mail);
            log("No internet connection. Email has been queued.");
        }

        static void SendEmail(MailMessage mail, Logger log)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("sendemailitec@gmail.com", "gfvc oovf xotd ecrn "),
                    EnableSsl = true,
                };
                smtpClient.Send(mail);
                //log("Email sent successfully.");
            }
            catch (Exception ex)
            {
                log($"Exception caught while sending email: {ex.ToString()}");
            }
        }

        static internal bool SendSMTPEmail(Logger log, BackupSettings config)
        {
            try
            {
                // Retrieve PC name and current date
                string pcName = Environment.MachineName;
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");



                // Step 2: Create email message
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("portal.denuncia@itec.pt"),
                    Subject = "Email de conclusao de backup",
                    Body = $"Backup feito com sucesso no computador {pcName} ,na data {currentDate},com o tamanho{" (" + Convertbytes(config.Filesizelastbackup) + ")"}",
                    IsBodyHtml = true,
                };
                mail.To.Add("goncalo.gomes@itec.pt");

                // (Optional) Add attachments
                // mail.Attachments.Add(new Attachment("path/to/attachment"));

                // Check for internet connectivity and send or queue email
                if (CheckForInternetConnection())
                {
                    SendEmail(mail, log);
                    // log("Email sent Sucess");

                }
                else
                {
                    QueueEmail(mail, log);
                    //log("Email Queue");
                }

                // Monitor for internet connection to send queued emails
                while (BackupSettings.emailQueue.Count > 0)
                {
                    if (CheckForInternetConnection())
                    {
                        SendQueuedEmails(log);
                        log("Sending QueuedEmails");
                    }
                    Thread.Sleep(500);
                }


                return true;
            }
            catch (Exception ex)
            {
                log($"Exception caught in SendSmtpEmail(): {ex.ToString()}");
                return false;
            }
        }

        static void SendQueuedEmails(Logger log)
        {
            while (BackupSettings.emailQueue.Count > 0)
            {
                MailMessage mail = BackupSettings.emailQueue[0];
                SendEmail(mail, log);
                BackupSettings.emailQueue.RemoveAt(0);
            }
            log("All queued emails have been sent.");
        }
        #endregion
    }
}
