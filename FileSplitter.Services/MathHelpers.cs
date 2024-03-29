﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitter.Services {
    public static class MathHelpers {

        public static int CountDigits(long number) {
            if (number == 0) {
                return 1;
            }
            else if (number < 0) {
                throw new ArgumentOutOfRangeException(nameof(number), "Don't use a negative number.");
            }
            else {
                return (int)Math.Floor(Math.Log10(number) + 1); ;
            }
        }

        public static string GetNumericDecimalFormatString(long numberToCountDigits) {
            int numberOfDigits = CountDigits(numberToCountDigits);
            return $"D{numberOfDigits}";
        }

        public static int CalculateNumberOfFiles(long totalLineCount, int linesPerSplitFile) {
            return (int)Math.Ceiling((double)totalLineCount / linesPerSplitFile);
        }

    }
}
