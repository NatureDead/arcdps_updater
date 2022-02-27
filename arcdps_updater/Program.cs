using arcdps_updater.Exceptions;
using System;
using System.Diagnostics;
using System.Linq;

namespace arcdps_updater
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var arcdpsUpdater = new ArcdpsUpdater(new SettingsProvider(), new EnvironmentProvider());
            var isDebug = args.Contains("debug", StringComparer.OrdinalIgnoreCase);
            var isClear = args.Contains("clear", StringComparer.OrdinalIgnoreCase);
            var isVersionCheck = args.Contains("version", StringComparer.OrdinalIgnoreCase);
            var hasNoInput = args.Contains("noinput", StringComparer.OrdinalIgnoreCase);
            var isExecute = !isClear && !isVersionCheck;

            while (true)
            {
                if (!hasNoInput)
                {
                    Console.Write("> ");
                    var readLine = Console.ReadLine();

                    if (IsIn(readLine, "gg", "quit", "exit"))
                        break;

                    isClear = IsIn(readLine, "clear");
                    isVersionCheck = IsIn(readLine, "version");
                    isExecute = IsIn(readLine, "execute", "update", "go", "download");
                    if (IsIn(readLine, "debug"))
                    {
                        isDebug = !isDebug;
                        var debugMode = isDebug ? "enabled" : "disabled";
                        Console.WriteLine($"Debug mode is {debugMode}.");
                    }
                }

                try
                {
                    if (isClear)
                        arcdpsUpdater.Clear();
                    else if (isVersionCheck)
                        arcdpsUpdater.CheckVersion();
                    else if (isExecute)
                        arcdpsUpdater.Execute();
                }
                catch (ElevationRequiredException)
                {
                    Console.WriteLine("The updater needs to be started as admin for the given path.");
                    Console.WriteLine("Do you want to restart?");
                    Console.WriteLine("Press enter for restart or anything else for exit.");

                    var consoleKeyInfo = Console.ReadKey();
                    if (consoleKeyInfo.Key == ConsoleKey.Y)
                        RestartAsAdmin();

                    Environment.Exit(0);
                }
                catch (Exception exception)
                {
                    var exceptionMessage = isDebug ? exception.ToString() : exception.Message;
                    Console.WriteLine($"Error: {exceptionMessage}");
                }          
                
                if (hasNoInput)
                {
                    Console.ReadKey();
                    break;
                }
            }

            Console.WriteLine("\"DPS ist Opium des Volkes\" - Karl Marx");
        }

        private static void RestartAsAdmin()
        {
            var currentProcess = Process.GetCurrentProcess();

            var processStartInfo = currentProcess.StartInfo;
            processStartInfo.UseShellExecute = true;
            processStartInfo.Verb = "runas";

            Process.Start(processStartInfo);
        }

        private static bool IsIn(string value, params string[] values)
        {
            return values.Contains(value, StringComparer.OrdinalIgnoreCase);
        }
    }
}
