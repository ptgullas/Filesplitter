using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSplitter {
    public static class StringExtensions {
        public static bool IsNumeric(this string str) {
            return int.TryParse(str, out int i);
        }

        public static int ToInt(this string str) {
            int result = 0;
            if (str.IsNumeric()) {
                bool b = int.TryParse(str, out result);
            }
            return result;
        }

        public static bool ContainsWildcard(this string str) {
            if (str.Contains('*') || str.Contains('?')) {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
