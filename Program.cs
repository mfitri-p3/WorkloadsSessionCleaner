using System;
using System.Diagnostics;
using System.Linq;

namespace WorkloadsSessionCleaner
{
    class Program()
    {
        static string[] processFindList = new string[]
        {
            "WorkloadsSessionManager",
            "WorkloadsSessionHost"
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Process Cleaner on WorkloadsSession for Windows. Starting program...");
            #if DEBUG
            #else
            if (!Environment.IsPrivilegedProcess)
            {
                Console.WriteLine("Please run program as Administrator first.\nExiting program now.");
                Console.Write("Press any key to end program.");
                Console.ReadKey();
                return;
            }
            #endif
            List<Process> relevantProcesses = Process.GetProcesses()
            .ToList()
            .FindAll(pr => processFindList.Any(fl => pr.ProcessName == fl));
            if (relevantProcesses.Count > 0)
            {
                Console.WriteLine("There are {0} processes found running:", relevantProcesses.Count);
                Console.WriteLine("Id\tRAM Usage\tName");
                foreach (Process process in relevantProcesses)
                {
                    Console.WriteLine("{0}\t{1:F2}MB\t{2}", process.Id, ConvertBytesToMegabytes(process.WorkingSet64), process.ProcessName);
                }
                Console.WriteLine("\nProceed to clear these processes?");
                Console.WriteLine("Press 'y' to accept. Press any other key to skip.");
                ConsoleKeyInfo nextKey = Console.ReadKey();
                if (nextKey.Key == ConsoleKey.Y)
                {
                    Console.WriteLine("\nClearing the processes now...");
                    foreach (Process process in relevantProcesses)
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Unable to stop process {0}. Error: {1}", process.ProcessName, ex.Message);
                        }
                    }
                    Console.WriteLine("Clearing processes completed.");
                }
            }
            else
            {
                Console.WriteLine("No relevant processes are running now.");
            }
            Console.Write("Press any key to end program.");
            Console.ReadKey();
        }

        static double ConvertBytesToMegabytes(long input)
        {
            return (double)(input / Math.Pow(1024, 2));
        }
    }
}
