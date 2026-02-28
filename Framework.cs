using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using UnityEngine;

namespace UIFramework
{
	public class UIFramework
	{

		internal static GameObject ModRegistryPanel;
		internal static GameObject CatRegistryPanel;
		internal static GameObject PrefRegistryPanel;

		static List<ModEntry> RegisteredMods { get; set; } = new();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="modInstance"></param>
		/// <param name="categories"></param>
		public static void Register(MelonMod modInstance, params MelonPreferences_Category[] categories)
		{
			RegisteredMods.Add(new ModEntry(modInstance, new List<MelonPreferences_Category>(categories)));
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class ModEntry
	{
		/// <summary>
		/// 
		/// </summary>
		public MelonMod Mod { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public List<MelonPreferences_Category> Categories { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="modInstance"></param>
		/// <param name="categories"></param>
		public ModEntry(MelonMod modInstance, List<MelonPreferences_Category> categories)
		{
			Mod = modInstance;
			Categories = categories;



		}

		/// <summary>
		/// Test Component lol
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class TestComponent : MonoBehaviour
		{
			public string test { get; set; }

		}
	}
}