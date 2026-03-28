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
		#region Abstracts
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
			/*public virtual void AddSubmodel(IModelable submodel)
			{
				SubModels.Add(submodel);
			}*/
				
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
			public virtual void DiscardAction()
			{

			}
			/// <inheritdoc/>
			public override void SaveAction() { }

		}

		public abstract class ModelModItem : SelectableModelBase
		{
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
			
			/// <summary>
			/// Description of the modelentry
			/// </summary>
			public abstract string Description { get; }
			
			/// <summary>
			/// Called when the corresponding UI element is created
			/// </summary>
			public virtual Action<UIFController.Entry> OnUICreated { get; set; }
			
			/// <summary>
			/// 
			/// </summary>
			public virtual EntryState SaveState {get; set;}
		}

		#endregion
	}
}
