using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using static UIFramework.UIFController;

namespace UIFramework
{
	//TODO: Too much repitition Reinstate base model classq
	/// <summary>
	/// Models define how the UI is built. The heirarchy is simple and follows melonpreferences basic structure
	/// ModelMod ->  ModelMelonCategory -> ModelMelonEntry
	/// Modders can use the default model just by calling UIF.Register(modInstance, categories) in their OnLateInitializeMelon. 
	/// The default model will use simple input methods: bools will be toggles, strings will be text input fields and so would numerics.
	/// More options will eventually be available: sliders, dropdowns, multi checkboxes, radio buttons, etc.
	/// 
	/// Those will be developed after the default model is functional
	/// </summary>
	public partial class UIFModel
	{
		public class RootModel : IHoldSubmodels
		{

			public virtual List<IModelable> SubModels { get; set; } = new();

			private string _name = string.Empty;

			public string Identifier => _name;
			public string DisplayName => _name;

			public void SetName(string name)
			{
				_name = name;
			}

			public void AddSubmodel(params IModelable[] mod)
			{
				SubModels.AddRange(mod);
			}

			public IModelable GetSubmodel(string name)
			{
				return SubModels.FirstOrDefault(m => m.Identifier == name);
			}
			public ModelModItem GetModModel(string identifier)
			{
				return (ModelModItem)GetSubmodel(identifier);
			}
			
			public GameObject GetNewUIInstance() { return null; }

			public void SaveAction() { }

		}

		public class ModelMod : ModelModItem
		{
			//public List<IModelable> SubModels { get; set; } = new();
			public MelonMod Instance { get; set; }
			//public string ModName => Instance.Info.Identifier;

			public override string Identifier => Instance.Info.Name;
			public override string DisplayName => Identifier;

			//internal List<ModelBase> catModelList = new();


			public ModelMod(MelonMod instance, List<MelonPreferences_Category> catList)
			{
				Instance = instance;

				foreach (MelonPreferences_Category cat in catList)
				{
					SubModels.Add(new ModelMelonCategory(cat));
				}
			}
			public ModelMod(MelonMod instance)
			{
				Instance = instance;
			}

			
		}

		public class ModelMelonCategory : ModelCategoryItem
		{
			//public List<IModelable> SubModels { get; set; }
			public MelonPreferences_Category PrefCat;
			public override string Identifier => PrefCat.Identifier;
			public override string DisplayName => PrefCat.DisplayName.Trim() == "" ? PrefCat.Identifier : PrefCat.DisplayName;

			public ModelMelonCategory(MelonPreferences_Category cat)
			{
				PrefCat = cat;
				foreach (MelonPreferences_Entry entry in PrefCat.Entries)
				{
					SubModels.Add(new ModelMelonEntry(entry));
				}

			}

			public override void SaveAction()
			{
				PrefCat.SaveToFile();
			}

			/*public void AddSubModel(IEntry model)
			{
				SubModels.Add((IModelable)model);
			}*/
			

		}

		/// <summary>
		/// 
		/// </summary>
		public class ModelMelonEntry : ModelEntryItem
		{
			
			public virtual MelonPreferences_Entry PrefEntry { get; set; }
			public override string Identifier => PrefEntry.Identifier;
			public override string DisplayName => PrefEntry.DisplayName.Trim() == "" ? PrefEntry.Identifier : PrefEntry.DisplayName;
			public override string Description => PrefEntry.Description;

			public object BoxedValue
			{
				get => PrefEntry.BoxedValue;
				set => PrefEntry.BoxedValue = value;
			}
			public ModelMelonEntry(MelonPreferences_Entry prefEntry)
			{
				PrefEntry = prefEntry;
				SavedValue = prefEntry.BoxedValue;
				PrefEntry.OnEntryValueChangedUntyped.Subscribe(OnValueChanged);
			}

			protected void OnValueChanged(object oldVal, object newVal)
			{
				SavedValue = newVal;
			}
			 
			public virtual object SavedValue { get; set; }

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
			/// Returns an instance of the game object associated with the MelonPreferences_Entry type.
			/// If a custom one is provided, it will return an instance of that instead
			/// </summary>
			/// <returns>TODO: Move this to the UI builders as this is ui logic</returns>
			public override GameObject GetNewUIInstance()
			{
				if (_uiPrefabSource == null)
				{

					switch (PrefEntry.BoxedValue)
					{
						case bool:
							return UI.GetPrefab(InputType.Toggle);
						case string:
							return UI.GetPrefab(InputType.TextField);
						case int:
							return UI.GetPrefab(InputType.NumericInt);
						case float:
							return UI.GetPrefab(InputType.NumericFloat);
						case double:
							return UI.GetPrefab(InputType.NumericDouble);
						case Enum:
							return UI.GetPrefab(InputType.Dropdown);
						default:
							Debug.Log("Unsupported type detected with no custom widget prefab provided. Defaulting to text input. Creating custom component recommended", false, 1);
							return UI.GetPrefab(InputType.TextField);


					}
				}
				else
				{
					return GameObject.Instantiate(_uiPrefabSource);
				}
			}

			/*public void SaveAction()
			{

			}*/

		}
	}

}
