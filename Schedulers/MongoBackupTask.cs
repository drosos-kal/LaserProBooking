using System.Diagnostics;
using System;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using DotVVM.Framework.Compilation.Javascript;
using Coravel.Invocable;
using System.IO;

namespace BeautySalonBookingSystem.Schedulers
{
    public class MongoBackupTask : IInvocable
    {
        public async Task Invoke()
        {
            try
            {
                // Define the command and its arguments
                /*string command = "C:/Program Files/MongoDB/Tools/100/bin/mongodump.exe";
                string arguments = "--host localhost --db Bookings --out C:\\Users\\MSDEV-M\\Desktop";*/

                // Get the current date and format it as a string
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

                // Define the base directory for the output
                string baseDirectory = "/var/backups/laser-pro";

                // Create the output directory path
                string outputDirectory = Path.Combine(baseDirectory, currentDate);

                // Ensure the directory exists
                Directory.CreateDirectory(outputDirectory);

                // Define the command and its arguments
                string command = "/usr/bin/mongodump";
                string arguments = $"--host localhost --db Bookings --out {outputDirectory}";

                // Create a new process start info
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // Start the process
                using (Process process = new Process())
                {
                    process.StartInfo = processStartInfo;
                    process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
                    process.ErrorDataReceived += (sender, e) => Console.WriteLine("ERROR: " + e.Data);
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
