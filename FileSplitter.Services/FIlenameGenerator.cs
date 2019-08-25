using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileSplitter.Services {
    public class FilenameGenerator {

        public static string TargetFolder { get; set; }
        public static string FilenameRoot { get; set; }
        public static string Extension { get; set; }
        public static long TotalLines { get; set; }
        public static int MaxLinesPerSplitFile { get; set; }
        public static int TotalSplitFiles { get; set; }
        public static int DigitsInFileCount { get; set; }
        public static int DigitsInLineCount { get; set; }

        public FilenameGenerator(string path, long totalLines, int linesToSplit, string targetFolder = null) {
            FilenameRoot = Path.GetFileNameWithoutExtension(path);
            TargetFolder = targetFolder;
            Extension = Path.GetExtension(path);
            TotalLines = totalLines;
            MaxLinesPerSplitFile = linesToSplit;
            TotalSplitFiles = MathHelpers.CalculateNumberOfFiles(TotalLines, MaxLinesPerSplitFile);
            DigitsInFileCount = MathHelpers.CountDigits(TotalSplitFiles);
            DigitsInLineCount = MathHelpers.CountDigits(TotalLines);
        }
        
        public string GenerateFilenameWithPath(int currentFileCount = 1, int startingLineNumber = 1) {
            string newFilename = GenerateFilename(currentFileCount, startingLineNumber);
            return Path.Combine(TargetFolder, newFilename);
        }

        public string GenerateFilename(int currentFileCount = 1, int startingLineNumber = 1) {
            string fileCountDecimalFormat = MathHelpers.GetNumericDecimalFormatString(TotalSplitFiles);
            string lineCountDecimalFormat = MathHelpers.GetNumericDecimalFormatString(TotalLines);
            long endingLineNumber = CalculateEndingLineNumber(startingLineNumber);
            return $"{FilenameRoot}_{currentFileCount.ToString(fileCountDecimalFormat)}_{startingLineNumber.ToString(lineCountDecimalFormat)}-{endingLineNumber.ToString(lineCountDecimalFormat)}{Extension}";
        }

        private static long CalculateEndingLineNumber(int startingLineNumber) {
            long endingLineNumber = startingLineNumber + MaxLinesPerSplitFile - 1;
            if (endingLineNumber > TotalLines) {
                endingLineNumber = TotalLines;
            }
            return endingLineNumber;
        }
    }
}
