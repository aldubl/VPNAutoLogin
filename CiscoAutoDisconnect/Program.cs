using System.Diagnostics;
using System.Text;
using System;
using System.Configuration;
using System.Collections.Specialized;

using System.Xml;
using System.Xml.XPath;

namespace CiscoAutoDisconnect
{
    internal class Program
    {

        private static int lineCount = 0;
        private static StringBuilder output = new StringBuilder();

        static void Main(string[] args)
        {
            // Подтягиваем настройки
            XmlDocument doc = new XmlDocument();
            doc.Load(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\CiscoAutoLogin.dll.config");
            string file = doc.DocumentElement.SelectSingleNode("/configuration/appSettings/add[@key='path']").Attributes["value"].Value;

            // Проверяем, что процессы циско в данный момент не работают, иначе убиваем их
            var procFilter = new HashSet<string>() { "vpnui", "vpncli" };
            var existingProcs = Process.GetProcesses().Where(p => procFilter.Contains(p.ProcessName));
            if (existingProcs.Any())
            {
                foreach (var p in existingProcs)
                {
                    p.Kill();
                }
            }

            Run(file + "vpncli.exe", "disconnect", string.Format("-s"));
            Environment.Exit(0);
        }



        static public bool Run(string path, string command, string arguments)
        {
            try
            {
                Process process = new Process();

                process.StartInfo.FileName = path;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;


                process.OutputDataReceived += (s, a) => output.AppendLine(a.Data);
                process.ErrorDataReceived += (s, a) => output.AppendLine(a.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.StandardInput.Write(command);
                process.StandardInput.Flush();
                process.Close();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибки: " + ex.Message);
                return false;
            }
        }
    }
}