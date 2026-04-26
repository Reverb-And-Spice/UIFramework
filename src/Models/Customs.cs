using UnityEngine;


namespace UIFramework
{

	public partial class UIFModel
	{
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
			public override bool IsHidden { get; set; } = false;
			public EmptyCategory(string identifier, string displayName, ModelModItem parentMod = null)
				: base(parentMod)
			{
				_identifier = identifier;
				_displayName = displayName;
			}
			public EmptyCategory(string identifier, ModelModItem parentMod = null)
				: base(parentMod)
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
			public override bool IsHidden { get; set; }

			/// <summary>
			/// This is only to satisfy the contract for IEntry. 
			/// </summary>
			public object BoxedValue { get; set; }
			
			/// <inheritdoc/>
			public override void SaveAction() { }

			public Action<UIFController.ButtonEntry> OnClick;


			public ButtonEntry(Action<UIFController.ButtonEntry> onClick, string name, string description = "", string displayName = "", ModelCategoryItem parentCategory = null)
				: base(parentCategory)
			{
				_name = name;
				_description = description;
				_displayName = displayName == "" ? name : displayName;
				OnClick += onClick;
			}
			/// <inheritdoc/>
			public override GameObject GetNewUIInstance()
			{
				GameObject button = GameObject.Instantiate(UI.GetPrefab(InputType.Button));
				UIFController.ButtonEntry buttonEntry = button.GetComponent<UIFController.ButtonEntry>();
				return button;
			}
			


		}
	}
}