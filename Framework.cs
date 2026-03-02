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


		internal static UIFModel ModelInstance = new();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="modInstance"></param>
		/// <param name="categories"></param>
		public static void Register(MelonMod modInstance, params MelonPreferences_Category[] categories)
		{
			ModelInstance.AddToDict(modInstance, new UIFModel.Mod(modInstance, categories.ToList()));


		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="modInstance"></param>
		/// <param name="categories"></param>
		public static void Register(MelonMod modInstance, List<MelonPreferences_Category> categories)
		{
			ModelInstance.AddToDict(modInstance, new UIFModel.Mod(modInstance, categories));
		}






	}






	/// <summary>
	/// 
	/// </summary>
	internal class UIFModel
	{
		internal List<Mod> ModModelsList = new();
		internal Dictionary<MelonMod, Mod> ModModelsDict = new();

		internal void AddToDict(MelonMod modInstance, Mod model)
		{
			ModModelsDict[modInstance] = model;
		}
		internal class Mod
		{
			internal MelonMod Instance { get; set; }
			internal List<MelonPreferences_Category> CatList = new();


			internal Mod(MelonMod instance, List<MelonPreferences_Category> catList)
			{
				Instance = instance;
				CatList = catList;
			}

		}

		internal class Categories
		{
			internal MelonPreferences_Category category;


		}
		internal class PreferenceEntry
		{

		}
	}
}