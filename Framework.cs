using MelonLoader;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UIFramework
{
	/// <summary>
	/// primary public facing class, modders will interact with this to register their preferences and build the UI.
	/// </summary>
	public static class UIFramework
	{

		internal static UIFModel.RootModel ModelInstance = new();
		internal static GameObject MainWindow;

		internal static UIFController.WindowController WindowInstance;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="modInstance"></param>
		/// <param name="categories"></param>
		/// <returns>A reference to the created Mod Model for further customization</returns>
		public static UIFModel.ModelMod Register(MelonMod modInstance, params MelonPreferences_Category[] categories)
		{
			UIFModel.ModelMod NewModModel = new(modInstance, categories.ToList());
			ModelInstance.AddModModel(NewModModel);
			return NewModModel;
		}


		internal static void InitializeUIObjects()
		{
			MainWindow = GameObject.Instantiate(Prefabs.MainCanvasSource, Prefabs.UIFGameObjects.transform);
			MainWindow.name = "MainWindow";
			MainWindow.SetActive(true);
			WindowInstance = MainWindow.GetComponent<UIFController.WindowController>();

		}
		internal static void BuildUI()
		{
			WindowInstance.SetModel(ModelInstance);

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
	/// </summary>
	public class UIFModel
	{
		public abstract class ModelBase
		{
			public List<ModelBase> SubModels = new();
			public abstract string Name { get; }
			public abstract GameObject GetNewEntryWidgetInstance(); 
			public virtual void SaveAction()
			{

			}


		}

		public class RootModel : ModelBase
		{
			private string _name = string.Empty;
			public override string Name => _name;
			public void SetName(string name)
			{
				_name = name;
			}
			public void AddModModel(ModelMod mod)
			{
				SubModels.Add(mod);
			}
			public override GameObject GetNewEntryWidgetInstance()
			{
				throw new NotImplementedException();
			}

			

		}
		public class ModelMod : ModelBase
		{
			public MelonMod Instance { get; set; }
			//public string ModName => Instance.Info.Name;

			public override string Name => Instance.Info.Name;


			//internal List<ModelBase> catModelList = new();


			public ModelMod(MelonMod instance, List<MelonPreferences_Category> catList)
			{
				Instance = instance;

				foreach (MelonPreferences_Category cat in catList)
				{
					SubModels.Add(new ModelCategory(cat));
				}
			}

			public override GameObject GetNewEntryWidgetInstance()
			{
				return GameObject.Instantiate(Prefabs.ModTab);
			}

		}

		public class ModelCategory : ModelBase
		{
			public MelonPreferences_Category PrefCat;
			public override string Name => PrefCat.Identifier;


			public List<ModelEntry> Entries = new();
			public ModelCategory(MelonPreferences_Category cat)
			{
				PrefCat = cat;
				foreach (MelonPreferences_Entry entry in PrefCat.Entries)
				{
					SubModels.Add(new ModelEntry(entry));
				}

			}

			public override GameObject GetNewEntryWidgetInstance()
			{
				return GameObject.Instantiate(Prefabs.CatTab);
			}

			public override void SaveAction()
			{
				PrefCat.SaveToFile();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public class ModelEntry : ModelBase
		{
			public MelonPreferences_Entry PrefEntry;
			public override string Name => PrefEntry.Identifier;

			public string Description => PrefEntry.Description;
			public string Identifier => PrefEntry.Identifier;
			public string DisplayName => PrefEntry.DisplayName;

			public ModelEntry(MelonPreferences_Entry prefEntry)
			{
				PrefEntry = prefEntry;

			}

			private GameObject _uiPrefabSource;
			/// <summary>
			/// Use this function to provide your own prefab for this entry. 
			/// The prefab must have a component that implements IUIFrameworkEntry and properly handles the value changes and saving. 
			/// If no prefab is provided, a default one will be used based on the type of the preference 
			/// (bools will be toggles, strings will be text input fields and so would numerics).
			/// 
			/// </summary>
			/// <param name="prefab"></param>
			public void SetUIPrefabSource(GameObject prefab)
			{
				_uiPrefabSource = prefab;
			}


			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			public override GameObject GetNewEntryWidgetInstance()
			{
				if (_uiPrefabSource == null)
				{

					switch (PrefEntry.BoxedValue)
					{
						case bool:
							return GameObject.Instantiate(Prefabs.BoolPrefab);
							break;
						case string:
							return GameObject.Instantiate(Prefabs.TextPrefab);
							break;
						case int:
							return GameObject.Instantiate(Prefabs.IntPrefab);
						case float:
							return GameObject.Instantiate(Prefabs.FloatPrefab);
						case double:
							return GameObject.Instantiate(Prefabs.DoublePrefab);
						default:
							Debug.Log("Unsupported type detected with no custom widget prefab provided. Defaulting to text input. Creating custom component recommended");
							return GameObject.Instantiate(Prefabs.TextPrefab);


					}
				}
				else
				{
					return GameObject.Instantiate(_uiPrefabSource);
				}
			}

			public override void SaveAction()
			{

			}

			
		}
	}

	public enum InputType
	{
		Default,
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