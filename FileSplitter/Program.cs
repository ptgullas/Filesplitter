using FileSplitter.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitter {
    class Program {


        static void Main(string[] args) {
            DisplayIntro();
            try {
                if (args.Length == 0) {
                    DisplayHelpText();
                }
                else {
                    ProcessArgs(args);
                }
            }
            catch (Exception e) {
                ColorHelpers.WriteLineColor("Uh-oh, got some errors!", ConsoleColor.Red);
                ColorHelpers.WriteLineColor($"{e.GetBaseException()}");
            }
        }

        static void DisplayIntro() {
            ColorHelpers.WriteColor("Thanks for running ", ConsoleColor.White);
            ColorHelpers.WriteColor("FILE", ConsoleColor.Yellow);
            ColorHelpers.WriteColor("//", ConsoleColor.Magenta);
            ColorHelpers.WriteColor("SPLITTER", ConsoleColor.Yellow);
            ColorHelpers.WriteLineColor("!", ConsoleColor.White);
            ColorHelpers.WriteColor("Copyright \u00a9 2019, ");
            ColorHelpers.WriteLineColor("Paul T. Gullas", ConsoleColor.Cyan);
            ColorHelpers.WriteLineColor("https://github.com/ptgullas/Filesplitter", ConsoleColor.Magenta);
        }

        static void DisplayHelpText() {
            string applicationName = "FileSplitter";
            Console.WriteLine("Usage:");
            ColorHelpers.WriteLineColor($"{applicationName}: ");
            Console.WriteLine("\tDisplay this help text");
            ColorHelpers.WriteColor($"{applicationName} ");
            ColorHelpers.WriteColor($"<path of file to split> ", ConsoleColor.Yellow);
            ColorHelpers.WriteLineColor($"<# of lines to split by>:", ConsoleColor.Magenta);
            Console.WriteLine("\tSplit file into multiple files containing specified # of lines");
            ColorHelpers.WriteColor($"{applicationName} ");
            ColorHelpers.WriteColor($"<wildcard pattern> ", ConsoleColor.Yellow);
            ColorHelpers.WriteLineColor($"<# of lines to split by>:", ConsoleColor.Magenta);
            Console.WriteLine("\tSupports wildcards (e.g., *index.txt)");
        }

        static void ProcessArgs(string[] args) {
            if (args.Length != 2) {
                ColorHelpers.WriteLineColor("Need 2 arguments!", ConsoleColor.Red);
                DisplayHelpText();
            }
            else {
                int maxLines = ValidateMaxLines(args[1]);
                if (args[0].ContainsWildcard()) {
                    string wildcardPattern = args[0];
                    SplitMultipleFiles(wildcardPattern, maxLines);
                }
                else {
                    SplitSingleFile(args[0], maxLines);
                }
            }
        }

        private static void SplitMultipleFiles(string wildcardPattern, int maxLines) {
            string currentFolder = GetCurrentFolder();
            List<string> files = Directory.GetFiles(currentFolder, wildcardPattern, SearchOption.TopDirectoryOnly).ToList();
            if (files.Count != 0) {
                ListMultipleFiles(files);
                foreach (string file in files) {
                    SplitSingleFile(file, maxLines);
                }
            }
            else {
                ColorHelpers.WriteLineColor("No files found!", ConsoleColor.Red);
            }
        }

        private static void ListMultipleFiles(List<string> files) {
            foreach (string file in files) {
                ColorHelpers.WriteColor("Found ");
                ColorHelpers.WriteLineColor($"{file}", ConsoleColor.Green);
            }
        }

        private static void SplitSingleFile(string filePathToValidate, int maxLines) {
            string filePath = ValidateFilePath(filePathToValidate);
            AnnounceSplit(filePath, maxLines);
            Filesplitter.SplitFile(filePath, maxLines);
        }

        private static void AnnounceSplit(string filePath, int maxLines) {
            ColorHelpers.WriteColor("Splitting file ");
            ColorHelpers.WriteColor($"{filePath} ", ConsoleColor.Green);
            ColorHelpers.WriteColor("by ");
            ColorHelpers.WriteColor($"{maxLines} ", ConsoleColor.Cyan);
            string lineWord = "lines";
            if (maxLines == 1) {
                lineWord = "line";
            }
            ColorHelpers.WriteLineColor($"{lineWord}");
        }

        private static string ValidateFilePath(string firstArg) {
            string filePath = firstArg;
            if (!File.Exists(filePath)) {
                Console.WriteLine($"{filePath} not found, checking current folder");
                string currentFolder = GetCurrentFolder();
                filePath = Path.Combine(currentFolder, filePath);
                if (!File.Exists(filePath)) {
                    throw new FileNotFoundException("Cannot find file", filePath);
                }
            }
            return filePath;
        }
        static string GetCurrentFolder() {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        private static int ValidateMaxLines(string secondArg) {
            if (!secondArg.IsNumeric()) {
                throw new ArgumentException("secondArg should be a number!", secondArg);
            }
            else {
                int maxLines = secondArg.ToInt();
                if (maxLines <= 0) {
                    throw new ArgumentOutOfRangeException(nameof(secondArg), "secondArg should be greater than zero!");
                }
                return maxLines;
            }
        }


    }
}
