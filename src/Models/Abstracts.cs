using System;
using System.Collections.Generic;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using static UIFramework.UIFController;
using MelonLoader.Preferences;
using UIFramework.ValidatorExtensions;

namespace UIFramework
{

	public partial class UIFModel
	{
		public abstract class ModelBase : IModelable
		{

			/// <inheritdoc/>
			public abstract string Identifier { get; }
			/// <inheritdoc/>
			public abstract GameObject GetNewUIInstance();
			/// <inheritdoc/>
			public abstract string DisplayName { get; }
			public bool IsHidden { get; set; } = false;


			/// <inheritdoc/>
			public virtual void SaveAction()
			{

			}
			public virtual void DiscardAction()
			{
			}
		}
		/// <summary>
		/// Models that represent buttons on the sidebar and topbar
		/// </summary>
		public abstract class SelectableModelBase : ModelBase, IHoldSubmodels
		{
			
			/// <summary>
			/// List of submodels for the model
			/// </summary>
			public virtual List<IModelable> SubModels { get; set; } = new();
			
			/// <summary>
			/// General submodel finder.
			/// </summary>
			public IModelable GetSubmodel(string name)
			{
				return SubModels.FirstOrDefault(m => m.Identifier == name);
			}
				
			/// <summary>
			/// Add a new submodel to the list
			/// </summary>
			public virtual void AddSubmodel(params IModelable[] submodel)
			{
				SubModels.AddRange(submodel);
			}

			/*public virtual void AddSubmodel(List<IModelable> submodels)
			{
				SubModels.AddRange(submodels);
			}*/
			
			/// <summary>
			/// Called when the discard button is pressed
			/// </summary>
			public override void DiscardAction()
			{

			}
			/// <inheritdoc/>
			public override void SaveAction() { }

		}

		public abstract class ModelModItem : SelectableModelBase
		{
			
			public List<ModelCategoryItem> Categories => SubModels.Cast<ModelCategoryItem>().ToList();
			public abstract MelonBase Instance { get; set; }
			public override string Identifier => Instance.Info.Name;
			public string _displayName;
			public override string DisplayName => _displayName;

			public virtual string Version => Instance.Info.Version;
			/// <inheritdoc/>
			public override GameObject GetNewUIInstance()
			{
				return GameObject.Instantiate(Prefabs.ModTab);
			}
			/// <summary>
			/// 
			/// </summary>
			public virtual ModelCategoryItem GetModelCategory(string identifier)
			{
				return (ModelCategoryItem) GetSubmodel(identifier);
			}
			/// <summary>
			/// 
			/// </summary>
			public virtual void AddModelCategory(params ModelCategoryItem[] categoryModel)
			{
				AddSubmodel(categoryModel.Cast<IModelable>().ToArray());
			}
			/// <summary>
			/// Calls individual category models' SaveAction method.
			/// </summary>
			public override void SaveAction()
			{
				foreach (IModelable model in SubModels)
				{
					try
					{
						model.SaveAction();
					}
					catch (Exception ex)
					{
						Debug.Log($"Error saving category {model.Identifier} for mod {Instance.Info.Name}: {ex.Message}", false, 2);
					}
				}
				OnModSaved?.Invoke();
			}
			public override void DiscardAction()
			{
				foreach (IModelable model in SubModels)
				{
					try
					{
						model.DiscardAction();
					}
					catch (Exception ex)
					{
						Debug.Log($"Error loading category {model.Identifier} for mod {Instance.Info.Name}: {ex.Message}", false, 2);
					}
				}
			}
			/// <summary>
			/// Subscribe to this event to run code after all the categories for the mod have been saved.
			/// This will only run if your mod is the currently selelcted mod. 
			/// </summary>
			public event Action OnModSaved;

			public event Action<ModelModItem> OnUiUpdateRequest;

			private bool _isUpdateRequestQueued = false;
			public void RequestUpdateUI() => _isUpdateRequestQueued = true;
			void Update()
			{
				if (!_isUpdateRequestQueued)
					return;
				
				OnUiUpdateRequest?.Invoke(this);
				_isUpdateRequestQueued= false;
				
			}

		}
		public abstract class ModelCategoryItem : SelectableModelBase
		{
			//public List<ModelEntryItem> Entries => SubModels.Cast<ModelEntryItem>().ToList();
			public ModelModItem ParentMod { get; set; }
			public abstract bool IsHidden {get; set;}
			protected ModelCategoryItem(ModelModItem parentMod)
			{
				ParentMod = parentMod;
			}
			/// <inheritdoc/>
			public override GameObject GetNewUIInstance()
			{
				return GameObject.Instantiate(Prefabs.CatTab);
			}
			public virtual void AddEntry(params IEntry[] entryModel)
			{
				AddSubmodel(entryModel.Cast<IModelable>().ToArray());
			}
			//public override void DiscardAction() { }
		}

		public abstract class ModelEntryItem : ModelBase, IEntry
		{
			public ModelCategoryItem ParentCategory { get; set; }
			public ModelEntryItem(ModelCategoryItem parentCategory)
			{
				ParentCategory = parentCategory;
			}
			/// <inheritdoc/>
			public abstract string Description { get; }
			public abstract bool IsHidden {get; set;}
			
			/// <summary>
			/// Called when the corresponding UI element is created
			/// </summary>
			public virtual Action<UIFController.Entry> OnUICreated { get; set; }
			
			/// <summary>
			/// 
			/// </summary>
			public virtual EntryState SaveState {get; set;}

			public override void DiscardAction() { }


			#region UI Commands
			
			#endregion
		}

		/// <summary>
		/// A model for interfacing with a piece of data.
		/// </summary>
		public abstract class ModelDataEntryBase: ModelEntryItem
		{
			protected ModelDataEntryBase(ModelCategoryItem parentCategory) : base(parentCategory) { }
			public abstract DefaultValidator Validator { get; }

			public abstract object ModelBoxedValue {get; protected set;}
			public virtual bool TryApply(object value)
			{
				bool result = false;
				try
				{
					ModelBoxedValue = value;
					result = true;
				}
				catch (Exception ex)
				{
					Debug.Log($"ModelDataEntry TryApply: {ex.Message}\n{ex.StackTrace}", false, 2);
					result = false;

				}
				return result;
			}
			//untested AI generated codbe
			public void SetDataValue(object newValue)
			{
				ModelBoxedValue = newValue;
			}
			protected GameObject _uiPrefabSource;
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
					if(Validator is ISliderDescriptor)
						return UI.GetPrefab(InputType.Slider);

					switch (ModelBoxedValue)
					{
						case bool:
							return UI.GetPrefab(InputType.Toggle);
						case string:
							return UI.GetPrefab(InputType.TextField);
						case Enum:
							return UI.GetPrefab(InputType.Dropdown);

						//numerics
						//integer types
						case sbyte:
						case byte:
						case short:
						case ushort:
						case int:
						case uint:
						case long:
						case ulong:
							return UI.GetPrefab(InputType.NumericInt);
						//floating point types
						case float:
						case double:
						case decimal:
							return UI.GetPrefab(InputType.NumericFloat);
						default:
							//Debug.Log("Unsupported type detected with no custom widget prefab provided. Defaulting to text input. Creating custom component recommended", true, 1);
							return UI.GetPrefab(InputType.TextField);
					}
				}
				else
				{
					return GameObject.Instantiate(_uiPrefabSource);
				}
			}

		}
	}
}
