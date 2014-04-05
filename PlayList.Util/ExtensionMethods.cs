using System;

namespace PlayList.Util
{
    public static class ExtensionMethods
    {
        public static DateTime ToDateTime(this string dateTime)
        {
            DateTime parsedDateTime;
            DateTime.TryParse(dateTime, out parsedDateTime);
            return parsedDateTime;
        }

        public static TimeSpan ToTimeSpan(this string timeSpan)
        {
            TimeSpan parsedTimeSpan;
            TimeSpan.TryParse(timeSpan, out parsedTimeSpan);
            return parsedTimeSpan;
        }

        /// <summary>
        /// Takes an input string in the form of a number followed by a size unit and
        /// returns the size of the file, in MB, as an int. 
        /// </summary>
        /// <param name="fileSize"></param>
        /// <returns>The size of the file in MB as an integer value</returns>
        public static int ToFileSize(this string fileSize)
        {
            int fileSizeMB;

            var size = fileSize.Split(' ')[0];
            var unit = fileSize.Split(' ')[1].Trim();

            Int32.TryParse(size, out fileSizeMB);

            if (unit.Equals("GB", StringComparison.CurrentCultureIgnoreCase))
                return fileSizeMB * 1000;
            if (unit.Equals("KB", StringComparison.CurrentCultureIgnoreCase))
                return fileSizeMB / 1000;

            return fileSizeMB;
        }

        /// <summary>
        /// Case and culture insensitive comparision to see if second string is contained within the first.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool ContainsCaseInsensitive(this string first, string second)
        {
            return ContainsImpl(first, second) > -1;
        }

        /// <summary>
        /// Case and culture insensitive comparision to see if second string is contained within the first.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool StartsWithCaseInsensitive(this string first, string second)
        {
            return ContainsImpl(first, second) == 0;
        }

        private static int ContainsImpl(string first, string second)
        {
            if (first == string.Empty)
                return -1;
            if (second == string.Empty)
                return -1;

            return first.IndexOf(second, StringComparison.InvariantCultureIgnoreCase);

        }

    }
}
