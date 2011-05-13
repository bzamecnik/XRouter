﻿namespace DaemonNT
{
    using System;
    using System.Collections.Generic;
    using System.ServiceProcess;

    /// <summary>
    /// Provides a console program to run various DaemonNT commands.
    /// </summary>
    /// <remarks>
    /// The available commands are: <c>run</c>, <c>debug</c>, <c>install</c>,
    /// <c>uninstall</c>.
    /// </remarks>
    internal class Program
    {
        public static void Main(string[] args)
        {
            // TODO: Lepe poresit parametry prikazove radky (az bude definitivne jiste,
            // co vsechno bude v sobe DaemonNT obsahovat - pokud budeme napr. implementovat
            // watchdog ci konfiguracni GUI)

            if (args.Length < 2)
            {
                PrintUsage();
                return;
            }

            List<string> arguments = new List<string>();
            arguments.AddRange(args);

            if ((arguments.Count > 0) && (arguments[0] == "DaemonNT"))
            {
                arguments.RemoveAt(0);
            }

            ServiceCommands commands = new ServiceCommands();

            // first argument is optional
            if (arguments.Count == 3)
            {
                if (arguments[0].StartsWith("--config-file="))
                {
                    commands.ConfigFile = arguments[0].Split(new[] { '=' }, 2)[1];
                    arguments.RemoveAt(0);
                }
            }

            string command = arguments[0];
            string serviceName = arguments[1];

            try
            {
                switch (command)
                {
                    case "run":
                        commands.Run(serviceName);
                        break;
                    case "debug":
                        commands.Debug(serviceName);
                        break;
                    case "install":
                        commands.Install(serviceName);
                        break;
                    case "uninstall":
                        commands.Uninstall(serviceName);
                        break;
                    case "status":
                        CheckStatus(commands, serviceName);
                        break;
                    case "start":
                        commands.Start(serviceName);
                        break;
                    case "stop":
                        commands.Stop(serviceName);
                        break;
                    case "restart":
                        commands.Restart(serviceName);
                        break;
                    default:
                        PrintUsage();
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: {0}", ex);
                Environment.Exit(-1);
            }
        }

        private static void CheckStatus(ServiceCommands commands, string serviceName)
        {
            bool isInstalled = commands.IsInstalled(serviceName);
            if (isInstalled)
            {
                string status = commands.GetStatus(serviceName);
                Console.WriteLine("Status of service {0}: {1}.", serviceName, status);
            }
            else
            {
                Console.WriteLine("Service {0} is not installed.", serviceName);
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: DaemonNT.exe [options] COMMAND SERVICE_NAME");
            Console.WriteLine("Available commands: debug, run, install, uninstall, start, stop, restart, status.");
            Console.WriteLine("The 'run' command is indended only for the OS's service runner.");
            Console.WriteLine("Options:");
            Console.WriteLine("  --config-file=CONFIG_FILE - path to configuration file");
        }
    }
}
