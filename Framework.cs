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
			ModelInstance.AddToList(new UIFModel.Mod(modInstance, categories.ToList()));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="modInstance"></param>
		/// <param name="categories"></param>
		public static void Register(MelonMod modInstance, List<MelonPreferences_Category> categories)
		{
			ModelInstance.AddToList(new UIFModel.Mod(modInstance, categories));
		}

		internal static void BuildUIInitial()
		{

		}

	}

	/// <summary>
	/// 
	/// </summary>
	internal class UIFModel
	{
		internal List<Mod> ModModelsList = new();
		//internal Dictionary<MelonMod, Mod> ModModelsDict = new();

		internal void AddToList(Mod model)
		{
			//ModModelsDict[modInstance] = model;
			ModModelsList.Add(model);

		}

		internal interface IModelable
		{

		}

		internal class Mod : IModelable
		{
			internal MelonMod Instance { get; set; }
			internal string ModName {get{ return Instance.Info.Name;}}

			internal List<Category> catModelList = new();

			internal Mod(MelonMod instance, List<MelonPreferences_Category> catList)
			{
				foreach (MelonPreferences_Category cat in catList)
				{
					catModelList.Add(new Category(cat));
				}
			}

		}

		internal class Category : IModelable
		{
			internal MelonPreferences_Category MelonCategory;

			internal List<PreferenceEntry> Entries = new ();
			internal Category(MelonPreferences_Category cat)
			{
				MelonCategory = cat;
				foreach (MelonPreferences_Entry entry in MelonCategory.Entries)
				{
					Entries.Add(new PreferenceEntry(entry));
				}
			}

		}
		internal class PreferenceEntry
		{
			internal MelonPreferences_Entry MelonEntry;

			internal string Description {get{return MelonEntry.Description;}}
			internal string Identifier {get{return MelonEntry.Identifier;}}
			internal string DisplayName {get{return MelonEntry.DisplayName;}}

			internal PreferenceEntry(MelonPreferences_Entry prefEntry)
			{
				MelonEntry = prefEntry;
			}
		}
	}
}