using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitter {
    public static class StringExtensions {
        public static bool IsNumeric(this string source) {
            return int.TryParse(source, out int i);
        }

        public static int ToInt(this string source) {
            int result = 0;
            if (source.IsNumeric()) {
                bool b = int.TryParse(source, out result);
            }
            return result;
        }
    }
}
