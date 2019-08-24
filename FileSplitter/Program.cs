using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitter {
    class Program {
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
            ColorHelpers.WriteColor("Thanks for running ", ConsoleColor.Yellow);
            ColorHelpers.WriteColor("FILE", ConsoleColor.White);
            ColorHelpers.WriteColor("//", ConsoleColor.Magenta);
            ColorHelpers.WriteColor("SPLITTER", ConsoleColor.White);
            ColorHelpers.WriteLineColor("!", ConsoleColor.Yellow);
            ColorHelpers.WriteColor("Copyright \u00a9 2019, ");
            ColorHelpers.WriteLineColor("Paul T. Gullas", ConsoleColor.Cyan);
            ColorHelpers.WriteLineColor("https://github.com/ptgullas/Filesplitter", ConsoleColor.Green);
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
        }

        static void ProcessArgs(string[] args) {
            if (args.Length != 2) {
                ColorHelpers.WriteLineColor("Need 2 arguments!", ConsoleColor.Red);
                DisplayHelpText();
            }
            else {
                string filePath = ValidateFilePath(args[0]);
                int maxLines = ValidateMaxLines(args[1]);
                AnnounceSplit(filePath, maxLines);
                SplitFile(filePath, maxLines);
            }
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
            // string baseName = current_dir + "\\" + DateTime.Now.ToString("HHmmss") + ".";
            FileInfo indexFile = new FileInfo(pathToFile);
            long totalLineCount = CountLinesSmarter(indexFile.OpenRead());
            string baseName = Path.GetFileName(pathToFile);

            string targetFolder = Path.GetDirectoryName(pathToFile);
            FilenameGenerator filenameGenerator = new FilenameGenerator(pathToFile, totalLineCount, maxLinesPerSplit);
            string newPath = Path.Combine(targetFolder, filenameGenerator.GenerateFilename());

            StreamWriter writer = null;
            try {
                using (StreamReader inputfile = new System.IO.StreamReader(pathToFile)) {
                    int count = 0;
                    int currentSplitCount = 1;
                    int currentOriginalLineCount = 1;
                    string line;
                    while ((line = inputfile.ReadLine()) != null) {

                        if (writer == null || count >= maxLinesPerSplit) {
                            if (writer != null) {
                                writer.Close();
                                writer = null;
                            }

                            string newFilename = filenameGenerator.GenerateFilename(currentSplitCount, currentOriginalLineCount);
                            newPath = Path.Combine(targetFolder, newFilename);
                            writer = new StreamWriter(newPath, false);
                            Console.WriteLine($"Creating file {newPath}");
                            currentSplitCount++;

                            count = 0;
                        }

                        writer.WriteLine(line.ToLower());
                        currentOriginalLineCount++;
                        ++count;
                    }
                }
            }
            finally {
                if (writer != null)
                    writer.Close();
            }
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
