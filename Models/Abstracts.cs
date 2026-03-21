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


			public virtual void SaveAction()
			{

			}
		}
		/// <summary>
		/// Models that become buttons on the sidebar and topbar
		/// </summary>
		public abstract class SelectableModelBase : ModelBase, IHoldSubmodels
		{
			public virtual List<IModelable> SubModels { get; set; } = new();
			public IModelable GetSubmodel(string name)
			{
				return SubModels.FirstOrDefault(m => m.Identifier == name);
			}
			public virtual void AddSubmodel(IModelable submodel)
			{
				SubModels.Add(submodel);
			}
			public virtual void AddSubmodel(params IModelable[] submodel)
			{
				SubModels.AddRange(submodel);
			}

			public virtual void AddSubmodel(List<IModelable> submodels)
			{
				SubModels.AddRange(submodels);
			}

		}

		public abstract class ModelModItem : SelectableModelBase
		{

			public override GameObject GetNewUIInstance()
			{
				return GameObject.Instantiate(Prefabs.ModTab);
			}

		}
		public abstract class ModelCategoryItem : SelectableModelBase
		{
			public override GameObject GetNewUIInstance()
			{
				return GameObject.Instantiate(Prefabs.CatTab);
			}
		}

		public abstract class ModelEntryItem : ModelBase, IEntry
		{
			public abstract string Description { get; }
			/// <inheritdoc/>
			public virtual Action<UIFController.Entry> OnUICreated { get; set; }


		}

		#endregion
	}
}