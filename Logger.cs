using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeSteam {
    internal class Logger {
        public static bool ShowVerbose { get; set; } = false;

        public static void Error(string message) {
            Console.WriteLine($"[-] {message}");
        }

        public static void Info(string message) {
            Console.WriteLine($"[+] {message}");
        }

        public static void Verbose(string message) {
            if (ShowVerbose) {
                Console.WriteLine($"[verbose] {message}");
            }
        }
    }
}
