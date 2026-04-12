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
			public abstract MelonMod Instance { get; set; }
			public override string Identifier => Instance.Info.Name;
			public override string DisplayName => Identifier;

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

		}
		public abstract class ModelCategoryItem : SelectableModelBase
		{
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
			
			/// <inheritdoc/>
			public abstract string Description { get; }
			
			/// <summary>
			/// Called when the corresponding UI element is created
			/// </summary>
			public virtual Action<UIFController.Entry> OnUICreated { get; set; }
			
			/// <summary>
			/// 
			/// </summary>
			public virtual EntryState SaveState {get; set;}

			public override void DiscardAction() { }
		}
	}
}
