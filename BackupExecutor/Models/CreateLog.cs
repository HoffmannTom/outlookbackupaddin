using System;
using System.IO;

namespace BackupExecutor.Models
{
    static internal class CreateLog
    {

        #region Global Variables


        //private static string filesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FILES");
        private static string semana = DateTime.Now.Month.ToString();
        private static string path = AppContext.BaseDirectory + $"log{semana}.txt";

        //private static string folderName = "Logs";
        //private static string pastaDestino = Path.Combine(Environment.CurrentDirectory, folderName);
        #endregion


        public static void CriarLog(string contexto, bool append = true)
        {
            try
            {
                string text = $"[{DateTime.Now.TimeOfDay.ToString("hh\\:mm\\:ss")}]{contexto}";
                if (!File.Exists(path))
                {

                    // Create a new file with the new line
                    File.WriteAllText(path, text);
                    return;
                }

                // Read existing content
                string existingContent = File.ReadAllText(path);

                // Combine new line with existing content
                string updatedContent = text + "\n" + existingContent;

                // Overwrite file with updated content
                File.WriteAllText(path, updatedContent);
            }
            catch (Exception ex)
            {
                // Handle potential exceptions (e.g., file access errors)
                Console.WriteLine($"Error adding line: {ex.Message}");
            }
        }


        public static void CriarLogErro(string erro, string linelocation)
        {
            try
            {
                string text = $"[{DateTime.Now} ] #ERRO " + erro + linelocation;
                if (!File.Exists(path))
                {

                    // Create a new file with the new line
                    File.WriteAllText(path, text);
                    return;
                }
                // Read existing content
                string existingContent = File.ReadAllText(path);

                // Combine new line with existing content
                string updatedContent = text + "\n" + existingContent;

                // Overwrite file with updated content
                File.WriteAllText(path, updatedContent);
            }
            catch (Exception ex)
            {
                // Handle potential exceptions (e.g., file access errors)
                Console.WriteLine($"Error adding line: {ex.Message}");
            }
        }
        public static void CriarLogInicioProjeto(bool append = true)
        {

            try
            {
                //if (!Directory.Exists(pastaDestino))
                //{
                //    Directory.CreateDirectory(pastaDestino);
                //}

                if (!File.Exists(path))
                {

                    // Create a new file with the new line
                    File.WriteAllText(path, $"[{DateTime.Now} ] start");
                    return;
                }

                // Read existing content
                string existingContent = File.ReadAllText(path);

                // Combine new line with existing content
                string updatedContent = $"[{DateTime.Now} ] start" + "\n" + existingContent;

                // Overwrite file with updated content
                File.WriteAllText(path, updatedContent);
            }
            catch (Exception ex)
            {
                // Handle potential exceptions (e.g., file access errors)
                Console.WriteLine($"Error adding line: {ex.Message}");
            }
        }
    }
}
