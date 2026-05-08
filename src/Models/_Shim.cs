using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIFramework.Models;
using UIFramework.Adapters;
using UnityEngine.Identifiers;
namespace UIFramework
{
	/// <summary>
	/// These are shim classes to keep backwards compatibility with old models. 
	/// They don't do anything and just inherit from the new models so that existing mods
	/// don't break using the old .Register function
	/// </summary>
	public class UIFModel
	{
		[Obsolete("This class is just a shim for backwards compatibility and should not be used directly. Use the new models instead in UIFramework.Models")]
		public interface IModelable : global::UIFramework.Models.IModelable
		{
			// No logic here
		}
		[Obsolete]
		public abstract class SelectableModelBase : global::UIFramework.Models.SelectableModelBase
		{
			[Obsolete("This class is just a shim for backwards compatibility and should not be used directly. Use the new models instead in UIFramework.Models")]
			public void AddSubmodel(params IModelable[] submodel)
			{
				base.AddSubmodel((UIFramework.Models.IModelable[])submodel);
			}
		}
		[Obsolete("This class is just a shim for backwards compatibility and should not be used directly. Use the new models instead in UIFramework.Models")]
		public abstract class ModelModItem : global::UIFramework.Models.ModModelBase
		{
			// No logic here
		}
		public class ModelMod : global::UIFramework.Models.MelonModel
		{
			public ModelMod(MelonBase instance, List<MelonPreferences_Category> categories) : base(instance, categories)
			{
			}
			public ModelMod(MelonBase instance) : base(instance)
			{ }
		}
		[Obsolete("This class is just a shim for backwards compatibility and should not be used directly. Use the new models instead in UIFramework.Models")]
		public abstract class ModelCategoryItem : global::UIFramework.Models.CategoryModelBase
		{
			[Obsolete("This class is just a shim for backwards compatibility and should not be used directly. Use the new models instead in UIFramework.Models")]
			protected ModelCategoryItem(ModelModItem parentMod) : base(parentMod)
			{
			}
			
		}
		[Obsolete("This class is just a shim for backwards compatibility and should not be used directly. Use the new models instead in UIFramework.Models")]
		public class ModelMelonCategory : global::UIFramework.Models.MelonCategoryModel
		{
			[Obsolete("This class is just a shim for backwards compatibility and should not be used directly. Use the new models instead in UIFramework.Models")]
			public ModelMelonCategory(MelonPreferences_Category cat, ModelModItem parentMod) : base(cat, parentMod)
			{
			}

		}
		[Obsolete("This class is just a shim for backwards compatibility and should not be used directly. Use the new models instead in UIFramework.Models")]
		public class EmptyCategory : global::UIFramework.Models.EmptyCategory
		{
			[Obsolete("This class is just a shim for backwards compatibility and should not be used directly. Use the new models instead in UIFramework.Models")]
			/*public EmptyCategory(string identifier, string displayName, ModModelBase parentMod = null)
			: base(identifier, displayName, parentMod)
			{
			}*/
			public EmptyCategory(string identifier, ModelModItem parentMod = null) : base(identifier, parentMod)
			{
			}

		}
		[Obsolete("This class is just a shim for backwards compatibility and should not be used directly. Use the new models instead in UIFramework.Models")]
		public abstract class ModelEntryItem : global::UIFramework.Models.EntryModelBase
		{
			[Obsolete("This class is just a shim for backwards compatibility and should not be used directly. Use the new models instead in UIFramework.Models")]	
			protected ModelEntryItem(ModelCategoryItem parentCategory) : base(parentCategory)
			{
			}
		}
		[Obsolete("This class is just a shim for backwards compatibility and should not be used directly. Use the new models instead in UIFramework.Models")]
		public class ButtonEntry : global::UIFramework.Models.ButtonEntry
		{
			[Obsolete("This class is just a shim for backwards compatibility and should not be used directly. Use the new models instead in UIFramework.Models")]
			public ButtonEntry(Action<ButtonModelAdapter> onClick, string name, string description = "", string displayName = "", ModelCategoryItem parentCategory = null) : base(onClick, name, description, displayName, parentCategory)
			{
			}

		}
	}
}
