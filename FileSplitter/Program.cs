using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitter {
    class Program {
        // consts used in CountLinesSmarter
        private const char CR = '\r';
        private const char LF = '\n';
        private const char NULL = (char)0;

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
            SplitFile(filePath, maxLines);
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


        static void SplitFile(string pathToFile, int maxLinesPerSplit) {
            FileInfo indexFile = new FileInfo(pathToFile);
            long totalLineCount = CountLinesSmarter(indexFile.OpenRead());
            string baseName = Path.GetFileName(pathToFile);

            string targetFolder = Path.GetDirectoryName(pathToFile);

            // if I ever need to have a different type of filenameGenerator pattern that isn't
            // <original>_<filecount>_<startLine>-<endLine>.txt
            // I'll have to set this up to inject this
            // I'll also want to set up SplitFile in its own class
            // and set up an IFilenameGenerator interface
            FilenameGenerator filenameGenerator = new FilenameGenerator(pathToFile, totalLineCount, maxLinesPerSplit, targetFolder);

            StreamWriter writer = null;
            try {
                using (StreamReader inputfile = new StreamReader(pathToFile)) {
                    int currentSplitLineCount = 0;
                    int currentSplitFileCount = 1;
                    int currentOriginalLineCount = 1;
                    string line;

                    while ((line = inputfile.ReadLine()) != null) {
                        if (writer == null || currentSplitLineCount >= maxLinesPerSplit) {
                            string newPath = filenameGenerator.GenerateFilenameWithPath(currentSplitFileCount, currentOriginalLineCount);
                            writer = CreateNewSplitFile(writer, newPath);

                            currentSplitFileCount++;
                            currentSplitLineCount = 0;
                        }
                        writer.WriteLine(line);
                        currentOriginalLineCount++;
                        ++currentSplitLineCount;
                    }
                }
            }
            finally {
                if (writer != null)
                    writer.Close();
            }
        }

        private static StreamWriter CreateNewSplitFile(StreamWriter writer, string newPath) {
            writer = CloseWriterIfAtMaxSplitLines(writer);
            writer = new StreamWriter(newPath, false);
            Console.WriteLine($"Creating file {newPath}");
            return writer;
        }

        private static StreamWriter CloseWriterIfAtMaxSplitLines(StreamWriter writer) {
            if (writer != null) {
                // if we hit the maxLinesPerSplit...
                writer.Close();
                writer = null;
            }

            return writer;
        }


        // from https://www.nimaara.com/counting-lines-of-a-text-file/
        public static long CountLinesSmarter(Stream stream) {
            // Ensure.NotNull(stream, nameof(stream));
            // replace the above with this. Maybe make it nicer later
            if (stream == null) {
                throw new ArgumentNullException();
            }
            

            var lineCount = 0L;

            var byteBuffer = new byte[1024 * 1024];
            var detectedEOL = NULL;
            var currentChar = NULL;

            int bytesRead;
            while ((bytesRead = stream.Read(byteBuffer, 0, byteBuffer.Length)) > 0) {
                for (var i = 0; i < bytesRead; i++) {
                    currentChar = (char)byteBuffer[i];

                    if (detectedEOL != NULL) {
                        if (currentChar == detectedEOL) {
                            lineCount++;
                        }
                    }
                    else if (currentChar == LF || currentChar == CR) {
                        detectedEOL = currentChar;
                        lineCount++;
                    }
                }
            }

            // We had a NON-EOL character at the end without a new line
            if (currentChar != LF && currentChar != CR && currentChar != NULL) {
                lineCount++;
            }
            return lineCount;
        }
    }
}
