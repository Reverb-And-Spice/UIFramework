using UnityEngine;
using UIFramework.Adapters;


namespace UIFramework.Models
{

	public class EmptyCategory : CategoryModelBase
	{
		private string _displayName;
		/// <inheritdoc/>
		public override string DisplayName => _displayName;
		private string _identifier;
		/// <inheritdoc/>
		public override string Identifier => _identifier;
		public override bool IsHidden { get; set; } = false;
		public EmptyCategory(string identifier, string displayName, ModModelBase parentMod = null)
			: base(parentMod)
		{
			_identifier = identifier;
			_displayName = displayName;
		}
		public EmptyCategory(string identifier, ModModelBase parentMod = null)
			: base(parentMod)
		{
			_identifier = identifier;
			_displayName = identifier;
		}
	}

	public class ButtonEntry : EntryModelBase
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

		public Action<ButtonModelAdapter> OnClick;


		public ButtonEntry(Action<ButtonModelAdapter> onClick, string name, string description = "", string displayName = "", CategoryModelBase parentCategory = null)
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
			ButtonModelAdapter buttonEntry = button.GetComponent<ButtonModelAdapter>();
			return button;
		}
	}

}