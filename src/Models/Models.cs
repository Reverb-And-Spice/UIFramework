using MelonLoader;
using MelonLoader.Preferences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UIFramework.ValidatorExtensions;
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
			public bool IsHidden { get; set; } = false;

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
			public void DiscardAction() { }

		}

		public class ModelMod : ModelModItem
		{
			
			public override MelonBase Instance { get; set; }
			
			public ModelMod(MelonBase instance, List<MelonPreferences_Category> catList)
			{
				Instance = instance;
				try
				{
					Type type = instance.GetType();
					Assembly ass = type.Assembly;
					_displayName = ass.GetCustomAttribute<UIInfoAttribute>()?.DisplayName ?? Identifier;
				}
				catch (Exception ex) { }

				foreach (MelonPreferences_Category cat in catList)
				{
					SubModels.Add(new ModelMelonCategory(cat, this));

				}
			}
			public ModelMod(MelonBase instance)
			{
				Instance = instance;
			}
			
		}

		public class ModelMelonCategory : ModelCategoryItem
		{
			public List<ModelMelonEntry> Entries => SubModels.Cast<ModelMelonEntry>().ToList();
			/// <summary>
			/// The MelonPreferences_Category object this adapts into the framework
			/// </summary>
			public MelonPreferences_Category PrefCat;
			public override bool IsHidden 
			{
				get => PrefCat.IsHidden; 
				set => PrefCat.IsHidden = value;
			}
			/// <inheritdoc/>
			public override string Identifier => PrefCat.Identifier;
			/// <inheritdoc/>
			public override string DisplayName => PrefCat.DisplayName.Trim() == "" ? PrefCat.Identifier : PrefCat.DisplayName;

			/// <summary>
			/// Creates a new instance of this class based on a MelonPreferences_Category
			/// </summary>
			public ModelMelonCategory(MelonPreferences_Category cat, ModelModItem parentMod)
				: base(parentMod)
			{
				PrefCat = cat;
				foreach (MelonPreferences_Entry entry in PrefCat.Entries)
				{
					SubModels.Add(new ModelMelonEntry(entry, this));
				}

			}
			/// <inheritdoc/>
			public override void SaveAction()
			{
				PrefCat.SaveToFile();
			}

			public override void DiscardAction()
			{
				PrefCat.LoadFromFile();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public class ModelMelonEntry : ModelDataEntryBase
		{

			/// <summary>
			/// MelonPreferences_Entry this model is meant to adapt
			/// </summary>
			public MelonPreferences_Entry PrefEntry { get; set; }
			public override bool IsHidden 
			{
				get => PrefEntry.IsHidden; 
				set => PrefEntry.IsHidden = value;
			}
			/// <inheritdoc/>
			public override string Identifier => PrefEntry.Identifier;
			/// <inheritdoc/>
			public override string DisplayName => PrefEntry.DisplayName.Trim() == "" ? PrefEntry.Identifier : PrefEntry.DisplayName;
			/// <inheritdoc/>
			public override string Description => PrefEntry.Description;

			public ValueValidator MelonValidator => PrefEntry.Validator;
			public override DefaultValidator Validator => MelonValidator as DefaultValidator;

			/// <summary>
			/// Direct access to the PrefEntry boxedvalue property
			/// </summary>
			public override object BoxedValue
			{
				get => PrefEntry.BoxedEditedValue;
				protected set => PrefEntry.BoxedEditedValue = value;
			}
			/// <summary>
			/// Creates a new instance of this object based around a MelonPreferences_Entry object
			/// </summary>
			public ModelMelonEntry(MelonPreferences_Entry prefEntry, ModelCategoryItem parentCategory)
				: base(parentCategory)
			{
				PrefEntry = prefEntry;
				SavedValue = prefEntry.BoxedValue;
				PrefEntry.OnEntryValueChangedUntyped.Subscribe(OnValueChanged);

				

			}
			protected void OnValueChanged(object oldVal, object newVal)
			{
				//SavedValue = newVal;
			}
			/// <summary>
			/// The value actually saved to the file. 
			/// </summary>
			public virtual object SavedValue { get; set; }
			/// <inheritdoc/>
			public override void SaveAction()
			{
				SavedValue = BoxedValue;
			}
			///	<inheritdoc/>
			public override void DiscardAction()
			{
				
			}
		}
	}

}
