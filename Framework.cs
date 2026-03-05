using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UIFramework
{
	/// <summary>
	/// primary public facing class, modders will interact with this to register their preferences and build the UI.
	/// </summary>
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
			ModelInstance.AddToList(new UIFModel.ModelMod(modInstance, categories.ToList()));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="modInstance"></param>
		/// <param name="categories"></param>
		public static void Register(MelonMod modInstance, List<MelonPreferences_Category> categories)
		{
			ModelInstance.AddToList(new UIFModel.ModelMod(modInstance, categories));
		}

		internal static void BuildModList()
		{

			/*foreach (UIFModel.ModelMod mod in ModelInstance.ModModelsList)
			{
				GameObject tab = GameObject.Instantiate(Prefabs.ModTab,Prefabs.ModDisplayList.transform);
				UIFController.Mod ViewController = tab.GetComponent<UIFController.Mod>();

				ViewController.Model = mod;
				ViewController.TargetContainer = CatRegistryPanel;


			}*/
			Prefabs.ModDisplayList.GetComponent<UIFController.ModList>().BuildFromModelList(ModelInstance.ModModelsList.Cast<UIFModel.BaseListSources>().ToList());
		}

	}

	/// <summary>
	/// Models define how the UI is built. The heirarchy is simple and follows melonpreferences basic structure
	/// ModelMod ->  ModelCategory -> ModelEntry
	/// Modders can use the default model just by calling UIF.Register(modInstance, categories) in their OnLateInitializeMelon. 
	/// The default model will use simple input methods: bools will be toggles, strings will be text input fields and so would numerics.
	/// More options will eventually be available: sliders, dropdowns, multi checkboxes, radio buttons, etc.
	/// 
	/// Those will be developed after the default model is functional
	/// 
	/// 
	/// </summary>
	public class UIFModel
	{

		public List<ModelMod> ModModelsList = new();

		public void AddToList(ModelMod model)
		{
			//ModModelsDict[modInstance] = model;
			ModModelsList.Add(model);

		}

		public abstract class BaseListSources
		{
			internal List<BaseListSources> subModels = new ();
			
		}

		public class ModelMod : BaseListSources
		{
			internal MelonMod Instance { get; set; }
			internal string ModName {get{ return Instance.Info.Name;}}

			//internal List<BaseListSources> catModelList = new();
			

			internal ModelMod(MelonMod instance, List<MelonPreferences_Category> catList)
			{
				Instance = instance;

				foreach (MelonPreferences_Category cat in catList)
				{
					subModels.Add(new ModelCategory(cat));
				}
			}

		}

		public class ModelCategory : BaseListSources
		{
			internal MelonPreferences_Category PrefCat;

			internal List<ModelEntry> Entries = new ();
			internal ModelCategory(MelonPreferences_Category cat)
			{
				PrefCat = cat;
				foreach (MelonPreferences_Entry entry in PrefCat.Entries)
				{
					Entries.Add(new ModelEntry(entry));
				}
			}

		}
		/// <summary>
		/// 
		/// </summary>
		public class ModelEntry
		{
			internal MelonPreferences_Entry PrefEntry;

			public string Description {get{return PrefEntry.Description;}}
			public string Identifier {get{return PrefEntry.Identifier;}}
			public string DisplayName {get{return PrefEntry.DisplayName;}}

			internal ModelEntry(MelonPreferences_Entry prefEntry)
			{
				PrefEntry = prefEntry;
			}
		}
	}
}