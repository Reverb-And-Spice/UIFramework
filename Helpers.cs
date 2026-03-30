//using System;
//using System.Collections.Generic;
using Il2CppSystem.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.ComponentModel.DataAnnotations;
namespace UIFramework
{
	/// <summary>
	/// contains various helper functions
	/// ToNormal: Normalizes a string
	/// GetDisplayNames
	/// </summary>
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
		/// <summary>
		/// Gets the displaynames from enums and if they don't have one, use their regular names
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks>AI Assisted. Was unfamiliar with enum attributes.</remarks>
		public static Il2CppSystem.Collections.Generic.List<string> GetDisplayName(Type enumType)
		{
			if(!enumType.IsEnum)
			{
				throw new ArgumentException("Type must be an enum");
			}

			Il2CppSystem.Collections.Generic.List<string> displayNames = new Il2CppSystem.Collections.Generic.List<string>();
			foreach(var value in Enum.GetValues(enumType))
			{
				FieldInfo info = enumType.GetField(value.ToString());
				DisplayAttribute attr = info?.GetCustomAttribute<DisplayAttribute>();
				displayNames.Add(attr?.GetName() ?? value.ToString());

			}
			return displayNames;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>AI Generated</remarks>
		internal static class HierarchyUtility
		{
			internal static string GetGameObjectPath(GameObject obj)
			{
				// Use StringBuilder for efficient string concatenation
				StringBuilder builder = new StringBuilder();
				Transform current = obj.transform;

				// Traverse up the hierarchy until there are no more parents (i.e., we reach the root)
				while (current != null)
				{
					// Insert the current object's name at the beginning of the path
					builder.Insert(0, current.name);

					// If it's not the root, add a path separator
					if (current.parent != null)
					{
						builder.Insert(0, "/");
					}

					// Move to the next parent up the hierarchy
					current = current.parent;
				}

				return builder.ToString();
			}
		}



	}

}
