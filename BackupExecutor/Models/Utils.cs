using BackupAddInCommon;
using System;
using System.Collections.Generic;
using System.Drawing;
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

                // Create an instance of HttpClient
                using (HttpClient client = new HttpClient())
                {
                    // Set the content type to application/json
                    var content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(url, content);

                    var responseString = await response.Content.ReadAsStringAsync();

                    // Check the response status code
                    if (response.IsSuccessStatusCode)
                    {
                        // Read and output the response content
                        string responseContent = await response.Content.ReadAsStringAsync();
                        log("Response: " + responseContent);
                    }
                    else
                    {
                        // Log the error status code and reason
                        string errorContent = await response.Content.ReadAsStringAsync();
                        log($"Error: {response.StatusCode} - {errorContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any exception that occurs
                log($"Exception caught in SendPostRequestAsync: {ex.Message}");
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
                    Credentials = new NetworkCredential("sendemailitec@gmail.com", "fcmy mvrx wtno nyoy"),
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
