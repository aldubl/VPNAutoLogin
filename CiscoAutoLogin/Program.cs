using System.Diagnostics;
using System.Text;
using OtpNet;
using System;
using System.Configuration;

namespace CiscoAutoLogin
{
    internal class Program
    {

        private static int lineCount = 0;
        private static StringBuilder output = new StringBuilder();

        static void Main(string[] args)
        {
            // Подтягиваем настройки
            var appSettings = ConfigurationManager.AppSettings;

            
            var file = appSettings["path"];
            var host = appSettings["host"];
            var user = appSettings["user"];
            var pass = appSettings["pass"];
            byte[] secretKey = Base32Encoding.ToBytes(appSettings["secretKey"]);

            // Генерируем второй пароль
            var totp = new Totp(secretKey, step: 30);
            var token = totp.ComputeTotp(DateTime.UtcNow);

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

            var comands = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}"
                , Environment.NewLine
                , string.Format("connect {0}", host)
                , user
                , pass
                , token
                , "state"
                );

            Run(file + "vpncli.exe", comands, string.Format("-s"));

            Run(file + "vpnui.exe", "", "");
            /*
                        var waitTime = 500;
                        var maxWait = 10;

                        var count = 0;

                        var toConsole = output.ToString();

                        while (!toConsole.ToString().Contains("state: Connected"))
                        {
                            toConsole = output.ToString();

                            if (count > maxWait)
                            {
                                Console.WriteLine("NOT GOOD");
                                Console.WriteLine(toConsole);
                            }

                            count++;
                            Thread.Sleep(waitTime);
                        }

                        Console.WriteLine(toConsole);*/
            Console.OutputEncoding = Encoding.UTF8;
            Console.Write("========================================================================================================================");
            Console.Write("||                                                                                                                    ||");
            Console.Write("||                 ███████████████████████████████████████████████████████████████████████▀█████████████              ||");
            Console.Write("||                 █─▄▄▄─█▄─▄█─▄▄▄▄█─▄▄▄─█─▄▄─████▀▄─██▄─██─▄█─▄─▄─█─▄▄─███▄─▄███─▄▄─█─▄▄▄▄█▄─▄█▄─▀█▄─▄█              ||");
            Console.Write("||                 █─███▀██─██▄▄▄▄─█─███▀█─██─████─▀─███─██─████─███─██─████─██▀█─██─█─██▄─██─███─█▄▀─██              ||");
            Console.Write("||                 ▀▄▄▄▄▄▀▄▄▄▀▄▄▄▄▄▀▄▄▄▄▄▀▄▄▄▄▀▀▀▄▄▀▄▄▀▀▄▄▄▄▀▀▀▄▄▄▀▀▄▄▄▄▀▀▀▄▄▄▄▄▀▄▄▄▄▀▄▄▄▄▄▀▄▄▄▀▄▄▄▀▀▄▄▀              ||");
            Console.Write("||                                                                                                                    ||");
            Console.Write("========================================================================================================================");
            Console.Write("||                                                                                                                    ||");
            Console.Write("||                  Приложение запустило Cisco AnyConnect Secure Mobility Client и авторизировало вас                 ||");
            Console.Write("||                  согласно настройкам в файле конфига. Если что либо пошло не так, приложение Cisco                 ||");
            Console.Write("||                                                  не подключит VPN.                                                 ||");
            Console.Write("||                                                                                                                    ||");
            Console.Write("||                 Так может происходить, если вы ввели неверные данные, ваш хостер VPN не поддерживает               ||");
            Console.Write("||                        работу из вашей страны, либо если произошла непредвиденная ошибка.                          ||");
            Console.Write("||                                                                                                                    ||");
            Console.Write("||                     Для отключения VPN нужно штатно произвести отключение в приложении Cisco.                      ||");
            Console.Write("||                                                                                                                    ||");
            Console.Write("||                      Что бы снова подключиться нужно повторно запустить эту программу.                             ||");
            Console.Write("||                                                                                                                    ||");
            Console.Write("||                                                                                                                    ||");
            Console.Write("||                                                                                                                    ||");
            Console.Write("||                                                                                                                    ||");
            Console.Write("||                                           Теперь это окно можно закрыть.                                           ||");
            Console.Write("||                                                                                                                    ||");
            Console.Write("||                                                                                                               v1   ||");
            Console.Write("||                                                                                                                    ||");
            Console.Write("========================================================================================================================");

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




                /*                var waitTime = 500;
                                var maxWait = 10;

                                var count = 0;
                                var output = stdOut.ToString();
                                while (!output.Contains("state: Connected"))
                                {
                                    output = stdOut.ToString();

                                    if (count > maxWait)
                                    {
                                        Console.WriteLine("NOT GOOD");
                                        Console.WriteLine(output);
                                    }

                                    count++;
                                    Thread.Sleep(waitTime);
                                }

                                Console.WriteLine(output);*/


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