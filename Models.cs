using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UIFramework
{

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
		public interface IModelable
		{
			public List<IModelable> SubModels { get; set; }
			public string Name { get; }
			public GameObject GetNewUIInstance();
			public void SaveAction();

		}
		public abstract class ModelBase : IModelable
		{
			public List<IModelable> SubModels { get; set; } = new();
			public abstract string Name { get; }
			public abstract GameObject GetNewUIInstance();
			public virtual void SaveAction()
			{

			}
			public virtual IModelable GetSubmodel(string name)
			{
				return SubModels.FirstOrDefault(m => m.Name == name);
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
			public override GameObject GetNewUIInstance()
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

			public override GameObject GetNewUIInstance()
			{
				return GameObject.Instantiate(Prefabs.ModTab);
			}

		}

		public class ModelCategory : ModelBase
		{
			public MelonPreferences_Category PrefCat;
			public override string Name => PrefCat.Identifier;


			public ModelCategory(MelonPreferences_Category cat)
			{
				PrefCat = cat;
				foreach (MelonPreferences_Entry entry in PrefCat.Entries)
				{
					SubModels.Add(new ModelEntry(entry));
				}

			}

			public override GameObject GetNewUIInstance()
			{
				return GameObject.Instantiate(Prefabs.CatTab);
			}

			public override void SaveAction()
			{
				PrefCat.SaveToFile();
			}

			public void AddEntry (IEntry model)
			{
				SubModels.Add((IModelable)model);
			}
		}
		public interface IEntry
		{
			public string Name { get; }
			public string Description { get; }
			public string DisplayName { get; }
			public object BoxedValue { get; set; }
		}
		/// <summary>
		/// 
		/// </summary>
		public class ModelEntry : ModelBase, IEntry
		{
			public MelonPreferences_Entry PrefEntry;
			public override string Name => PrefEntry.Identifier;

			public virtual string Description => PrefEntry.Description;
			public virtual string DisplayName => PrefEntry.DisplayName;

			public object BoxedValue 
			{
				get => PrefEntry.BoxedValue;
				set => PrefEntry.BoxedValue = value;
			}
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
			/// Returns an instance of the game object associated with the MelonPreferences_Entry type.
			/// If a custom one is provided, it will return an instance of that instead
			/// </summary>
			/// <returns></returns>
			public override GameObject GetNewUIInstance()
			{
				if (_uiPrefabSource == null)
				{

					switch (PrefEntry.BoxedValue)
					{
						case bool:
							return UIFramework.GetPrefab(InputType.Toggle);
							break;
						case string:
							return UIFramework.GetPrefab(InputType.TextField);
							break;
						case int:
							return UIFramework.GetPrefab(InputType.NumericInt);
						case float:
							return UIFramework.GetPrefab(InputType.NumericFloat);
						case double:
							return UIFramework.GetPrefab(InputType.NumericDouble);
						default:
							Debug.Log("Unsupported type detected with no custom widget prefab provided. Defaulting to text input. Creating custom component recommended", false, 1);
							return UIFramework.GetPrefab(InputType.TextField);


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

		#region customs
		public class ButtonEntry : ModelBase, IEntry
		{
			private string _name;
			public override string Name => _name;

			private string _description;
			public string Description => _description;

			private string _displayName;
			public string DisplayName => _displayName;
			/// <summary>
			/// This is only to satisfy the contract for IEntry. 
			/// </summary>
			public object BoxedValue { get; set; }

			public Action<IEntry> OnClick;
			public ButtonEntry(string name, string description = "", string displayName = "")
			{
				_name = name;
				_description = description;
				_displayName = displayName;
			}

			public override GameObject GetNewUIInstance() => UIFramework.GetPrefab(InputType.Button);
			public virtual void OnClickRelay()
			{
				OnClick?.Invoke(this);
			}


		}
		#endregion
	}
}
