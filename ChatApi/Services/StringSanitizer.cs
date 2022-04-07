using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatApi.Services
{
    public static class StringSanitizer
    {

        public static string CleanMessage(string message)
        {
            if (message == null)
                return null;
            message = Regex.Replace(message, "<.*?>", String.Empty);
            //message = Regex.Replace(message, @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+" +
            //             @"\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?" +
            //             @"([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*$",
            //             "<a href=\"$1\">$1</a>");
            message = Regex.Replace(message, @"(https?://[^\s]+)",
                         "<a href=\"$1\" target=\"_blank\">$1</a>");
            return message;
        }


        public static string CleanName(string name)
        {
            if (name == null)
                return null;
            name = Regex.Replace(name, "<.*?>", String.Empty);
            return name;
        }
        public static string MakeValidFileName(string name)
        {
            string invalidChars = Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(name, invalidRegStr, "_");
        }
    }
}
