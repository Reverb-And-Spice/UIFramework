using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIFramework
{
	internal static class Helpers
	{
		/// <summary>
		/// Returns a normalized version of the specified string by converting it to lowercase, trimming whitespace, and
		/// removing all spaces.
		/// </summary>
		/// <param name="text">The string to normalize. Cannot be null.</param>
		/// <returns>A normalized string with all spaces removed, trimmed, and converted to lowercase.</returns>
		///<remarks>Returns empty string if input only contains whitespace</remarks>
		public static string ToNormal(this string text) => text.ToLower().Trim().Replace(" ", "");
	}
}
