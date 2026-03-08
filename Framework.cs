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
			Prefabs.ModDisplayList.GetComponent<UIFController.Sidebar>().BuildFromModelList(ModelInstance.ModModelsList.Cast<UIFModel.BaseModel>().ToList());
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

		public abstract class BaseModel
		{
			internal List<BaseModel> subModels = new ();
			internal abstract string Name { get; }
		}

		public class ModelMod : BaseModel
		{
			internal MelonMod Instance { get; set; }
			internal string ModName => Instance.Info.Name;

			internal override string Name => ModName;

			//internal List<BaseModel> catModelList = new();


			internal ModelMod(MelonMod instance, List<MelonPreferences_Category> catList)
			{
				Instance = instance;

				foreach (MelonPreferences_Category cat in catList)
				{
					subModels.Add(new ModelCategory(cat));
				}
			}

		}

		public class ModelCategory : BaseModel
		{
			internal MelonPreferences_Category PrefCat;
			internal override string Name => PrefCat.Identifier;
			

			internal List<ModelEntry> Entries = new ();
			internal ModelCategory(MelonPreferences_Category cat)
			{
				PrefCat = cat;
				foreach (MelonPreferences_Entry entry in PrefCat.Entries)
				{
					subModels.Add(new ModelEntry(entry));
				}
				
			}

		}
		/// <summary>
		/// 
		/// </summary>
		public class ModelEntry : BaseModel
		{
			internal MelonPreferences_Entry PrefEntry;
			internal override string Name => PrefEntry.Identifier; 

			public string Description => PrefEntry.Description;
			public string Identifier => PrefEntry.Identifier;
			public string DisplayName => PrefEntry.DisplayName;

			public InputType InputType { get; set; } = InputType.TextField; 

			internal ModelEntry(MelonPreferences_Entry prefEntry)
			{
				PrefEntry = prefEntry;
				switch(prefEntry.BoxedValue)
				{
					case bool:
						InputType = InputType.Toggle;
						break;
					case string:
						InputType = InputType.TextField;
						break;
					case int:
						InputType = InputType.NumericInt;
						break;
					case float:
					case double:
						InputType = InputType.NumericFloat;
						break;
					default:
						InputType = InputType.TextField;
						break;


				}

			}
		}
	}

	
	public enum InputType
	{
		TextField,
		Toggle,
		NumericInt,
		NumericFloat,
		Slider,
		Dropdown,
		MultiCheckbox,
		RadioButton
	}
}