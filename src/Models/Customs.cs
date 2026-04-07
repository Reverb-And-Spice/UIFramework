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
		#region customs
		/// <summary>
		/// 
		/// </summary>
		public class EmptyCategory : ModelCategoryItem
		{
			private string _displayName;
			/// <inheritdoc/>
			public override string DisplayName => _displayName;
			private string _identifier;
			/// <inheritdoc/>
			public override string Identifier => _identifier;

			public EmptyCategory(string identifier, string displayName)
			{
				_identifier = identifier;
				_displayName = displayName;
			}
			public EmptyCategory(string identifier)
			{
				_identifier = identifier;
				_displayName = identifier;
			}
		}

		public class ButtonEntry : ModelEntryItem
		{

			private string _name;
			/// <inheritdoc/>
			public override string Identifier => _name;

			private string _description;
			/// <inheritdoc/>
			public override string Description => _description;

			private string _displayName;
			/// <inheritdoc/>
			public override string DisplayName => _displayName;

			/// <summary>
			/// This is only to satisfy the contract for IEntry. 
			/// </summary>
			public object BoxedValue { get; set; }
			
			/// <inheritdoc/>
			public override void SaveAction() { }

			public Action<UIFController.ButtonEntry> OnClick;


			public ButtonEntry(Action<UIFController.ButtonEntry> onClick, string name, string description = "", string displayName = "")
			{
				_name = name;
				_description = description;
				_displayName = displayName == "" ? name : displayName;
				OnClick += onClick;
			}
			/// <inheritdoc/>
			public override GameObject GetNewUIInstance() => UI.GetPrefab(InputType.Button);
			


		}
		#endregion
	}
}