using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitter {
    class Program {
        private const char CR = '\r';
        private const char LF = '\n';
        private const char NULL = (char)0;

        static void Main(string[] args) {
            string testfile = @"c:\temp\500lines.txt";
            int maxLines = 100;

            Console.WriteLine($"Splitting file {testfile} to {maxLines} lines per file");
            SplitFile(testfile, maxLines);
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
                            writer = new StreamWriter(newFilename, false);
                            Console.WriteLine($"Creating file {newFilename}");
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
