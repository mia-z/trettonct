using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TrettonCodeTest
{
    /// <summary>
    /// Basic static utility class containing helper functions.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Regular Expression pattern for matching text between anchor (<a></a>) tags of HTML
        /// Is very basic and doesn't cover every situation, but is fit for the current use case - probably shouldnt ever be used in production
        /// </summary>
        public static Regex AnchorPattern = new Regex(@"<a(.[^>]*)?>(\s*.[^<]*)<\/a>");

        /// <summary>
        /// Regular Expression pattern for matching text between the quotations of the string 'href=""' for the purpose of extracting link text contained within anchor tags
        /// Is also very basic and doesnt cover a lot of cases, but is suited for the current case - again, probably shouldnt ever use this in production
        /// </summary>
        public static Regex HrefPattern = new Regex("(?<=href=\\\"/)(.[^\\ ]*?)(?=\\\")");

        /// <summary>
        /// Returns a list of anchor tag matches with the <see cref="AnchorPattern"/> Regex pattern</returns>
        /// </summary>
        /// <param name="html">The HTML string to search through</param>
        /// <returns>List of type System.String</returns>
        public static List<string> GetAnchors(string html) =>
            AnchorPattern.Matches(html).Select(x => x.Value).ToList();

        /// <summary>
        /// Extracts the text contained within the HREF property on anchor tags
        /// </summary>
        /// <param name="href">The HREF property to extract text from</param>
        /// <returns>System.String</returns>
        public static string GetHrefText(string href)
        {
            return HrefPattern.Match(href).Value; 
        }
    }
}
