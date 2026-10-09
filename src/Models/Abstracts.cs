using MelonLoader;
using UnityEngine;
using UIFramework.UiExtensions;
using UIFramework.Adapters;

namespace UIFramework.Models
{

	public abstract class ModelBase : IModelable
	{

		/// <inheritdoc/>
		public abstract string Identifier { get; }
		/// <inheritdoc/>
		public abstract GameObject GetNewUIInstance();
		/// <inheritdoc/>
		public abstract string DisplayName { get; }
		public virtual bool IsHidden { get; set; } = false;


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

	public abstract class ModModelBase : SelectableModelBase
	{

		public List<CategoryModelBase> Categories => SubModels.Cast<CategoryModelBase>().ToList();
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
		public virtual CategoryModelBase GetModelCategory(string identifier)
		{
			return (CategoryModelBase)GetSubmodel(identifier);
		}
		/// <summary>
		///
		/// </summary>
		public virtual void AddModelCategory(params CategoryModelBase[] categoryModel)
		{
			AddSubmodel(categoryModel.Cast<IModelable>().ToArray());
		}
		/// <summary>
		/// Calls individual category models' PreSaveAction method.
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
		public void RequestUpdateUI() => UI.RequestRefresh(this);


	}
	public abstract class CategoryModelBase : SelectableModelBase
	{
		//public List<EntryModelBase> Entries => SubModels.Cast<EntryModelBase>().ToList();
		public ModModelBase ParentMod { get; set; }
		public override bool IsHidden { get; set; }
		protected CategoryModelBase(ModModelBase parentMod)
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

	public abstract class EntryModelBase : ModelBase, IEntry
	{
		public CategoryModelBase ParentCategory { get; set; }
		public EntryModelBase(CategoryModelBase parentCategory)
		{
			ParentCategory = parentCategory;
		}
		/// <inheritdoc/>
		public abstract string Description { get; }
		public override bool IsHidden { get; set; }

		/// <summary>
		///
		/// </summary>
		public virtual EntryState SaveState { get; set; }

		public override void DiscardAction() { }


		#region UI Commands

		#endregion
	}

	/// <summary>
	/// A model for interfacing with a piece of data.
	/// </summary>
	public abstract class DataEntryModelBase : EntryModelBase
	{
		protected DataEntryModelBase(CategoryModelBase parentCategory) : base(parentCategory) { }


		public abstract object ModelBoxedValue { get; protected set; }
		public virtual bool TryApply(object value)
		{
			bool result = false;
			try
			{
				//ModelBoxedValue = value;
				SetDataValue(value);
				result = true;

				//ParentCategory.ParentMod.RequestUpdateUI();
			}
			catch (Exception ex)
			{
				Debug.Log($"ModelDataEntry TryApply: {ex.Message}\n{ex.StackTrace}", false, 2);
				result = false;

			}
			return result;
		}

		protected virtual void OnDataValueChanged(object newValue)
		{
			if (!(RefreshInhibitor?.InhibitRefreshOnValueChange ?? false))
			{
				UI.RequestRefresh(ParentCategory.ParentMod);
			}
			else
				Debug.Log($"UI Refresh inhibited when entry value changes", true);
		}

		public abstract IUiExtension UiExtension { get; }
		public virtual IUserEditedNotifier EditNotifier => UiExtension as IUserEditedNotifier;
		public virtual IRefreshInhibitor RefreshInhibitor => UiExtension as IRefreshInhibitor;


		protected void SetDataValue(object newValue)
		{

			Debug.Log($"New Value Applied {newValue}", true);
			ModelBoxedValue = newValue;
			EditNotifier?.OnUserEdit?.Invoke(ModelBoxedValue);
			//Block refresh only if RefreshInhibitorExists with the InhibitRefresh property set to true.
			if (!(RefreshInhibitor?.InhibitRefreshOnEdit ?? false))
			{
				ParentCategory.ParentMod.RequestUpdateUI();
			}
			else
				Debug.Log($"UI Refresh inhibited when user edits values", true);

		}
		protected GameObject _uiPrefabSource;
		/// <summary>
		/// Returns an instance of the game object associated with the MelonPreferences_Entry type.
		/// If a custom one is provided, it will return an instance of that instead
		/// </summary>
		/// <returns>TODO: Move this to the UI builders as this is ui logic</returns>
		public override GameObject GetNewUIInstance()
		{
			//Make the custom UI provided by the validator the first priority if it exists.	
			if (UiExtension is ICustomViewProvider uiProvider)
			{
				if (uiProvider?.EntryViewPrefab is not null)
					return GameObject.Instantiate(uiProvider.EntryViewPrefab);
			}

			if (UiExtension is DynamicDropdownDescriptor)
			{
				GameObject dropdown = UI.GetPrefabInstance(PrefabType.Dropdown);
				dropdown.AddComponent<DynamicDopdownAdapter>();
				return dropdown;
			}

			if (UiExtension is ISliderDescriptor)
			{
				GameObject slider = UI.GetPrefabInstance(PrefabType.Slider);
				slider.AddComponent<NumSliderAdapter>();
				return slider;
			}
			if (UiExtension is IButtonDescriptor)
			{
				GameObject button = UI.GetPrefabInstance(PrefabType.Button);
				button.AddComponent<ButtonEntryAdapter>();
				return button;
			}



			switch (ModelBoxedValue)
			{
				case bool:
					GameObject toggle = UI.GetPrefabInstance(PrefabType.Toggle);
					toggle.AddComponent<BoolToggleAdapter>();
					return toggle;
				case string:
					GameObject textField = UI.GetPrefabInstance(PrefabType.TextField);
					textField.AddComponent<TextEntryAdapter>();
					return textField;
				case Enum:
					GameObject dropdown = UI.GetPrefabInstance(PrefabType.Dropdown);
					dropdown.AddComponent<EnumDropdownAdapter>();
					return dropdown;

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
					GameObject intInput = UI.GetPrefabInstance(PrefabType.NumericInt);
					intInput.AddComponent<NumericEntryAdapter>();
					return intInput;
				//floating point types
				case float:
				case double:
				case decimal:
					GameObject floatInput = UI.GetPrefabInstance(PrefabType.NumericFloat);
					floatInput.AddComponent<NumericEntryAdapter>();
					return floatInput;
				default:
					//Debug.Log("Unsupported type detected with no custom widget prefab provided. Defaulting to text input. Creating custom component recommended", true, 1);
					GameObject defaultInput = UI.GetPrefabInstance(PrefabType.TextField);
					defaultInput.AddComponent<TextEntryAdapter>();
					return defaultInput;
			}

		}

	}

}
