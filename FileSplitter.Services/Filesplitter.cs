using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitter.Services {
    public class Filesplitter {

        // consts used in CountLinesSmarter
        private const char CR = '\r';
        private const char LF = '\n';
        private const char NULL = (char)0;

        public static void SplitFile(string pathToFile, int maxLinesPerSplit) {
            FileInfo indexFile = new FileInfo(pathToFile);
            long totalLineCount = CountLinesSmarter(indexFile.OpenRead());
            string baseName = Path.GetFileName(pathToFile);

            string targetFolder = Path.GetDirectoryName(pathToFile);

            // if I ever need to have a different type of filenameGenerator pattern that isn't
            // <original>_<filecount>_<startLine>-<endLine>.txt
            // I'll have to set this up to inject this
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
