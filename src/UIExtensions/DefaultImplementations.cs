using Il2CppTMPro;
using MelonLoader;
using MelonLoader.Preferences;
using UIFramework.Models;
using UnityEngine;
namespace UIFramework.UiExtensions
{
	
	/// <summary>
	/// Default implementation of the MelonLoader ValueValidator class.
	/// This satisfies the required members but just acts as a passthrough. 
	/// It's the equivalent of not having a validator at all, but it allows for the use of the other descriptor interfaces without needing to implement a custom validator.
	/// </summary>
	public partial class DefaultValidator : ValueValidator, IUiExtension
	{
		public override bool IsValid(object value) { return true; }
		public override object EnsureValid(object value) { return value; }
	}
	
	
	/// <inheritdoc cref="ITextInputBehaviorDescriptor"/>
	public class TextInputBehaviorDescriptor : DefaultValidator, ITextInputBehaviorDescriptor
	{
		public TMP_InputField.ContentType ContentType { get; set; } = TMP_InputField.ContentType.Standard;
		public int CharacterLimit { get; set; } = 0;
		public bool IsReadOnly { get; set; } = false;
	}

	/// <inheritdoc cref="ITextInputAppearanceDescriptor"/>
	public class TextInputAppearanceDescriptor : DefaultValidator, ITextInputAppearanceDescriptor
	{
		public int FontSize { get; set; } = 18;
		public bool IsAutoSizing { get; set; } = false;
		public int AutoSizeMin { get; set; } = 14;
		public int AutoSizeMax { get; set; } = 30;
		public FontStyles FontStyle { get; set; } = FontStyles.Normal;
		public bool IsRichText { get; set; } = true;
	}


	/// <see cref="IDynamicDropdownDescriptor"/>
	public class DynamicDropdownDescriptor : DefaultValidator, IDynamicDropdownDescriptor
	{
		private List<DropdownItem> _dropdownItems = new();
		public List<DropdownItem> GetDropdownItems() { return _dropdownItems; }
		public void SetDropdownItems(List<DropdownItem> items)
		{
			_dropdownItems = items;
			OnDropdownItemsUpdated?.Invoke();
		}
		public void AddDropdownItem(DropdownItem item)
		{
			_dropdownItems.Add(item);
			OnDropdownItemsUpdated?.Invoke();
		}
		public void RemoveDropdownItem(DropdownItem item)
		{
			_dropdownItems?.Remove(item);
			OnDropdownItemsUpdated?.Invoke();
		}

		public DynamicDropdownDescriptor WithDropdownItemList(List<DropdownItem> dropdownItemList)
		{
			_dropdownItems = dropdownItemList;
			return this;
		}

		public DynamicDropdownDescriptor(List<DropdownItem> items)
		{
			_dropdownItems = items;
		}
		public DynamicDropdownDescriptor()
		{
			_dropdownItems = new();
		}
		public Action OnDropdownItemsUpdated { get; set; }
	}


	/// <summary>
	/// Default implementation of ISliderDescriptor. Used for numeric inputs that want to be sliders. DecimalPlaces defaults to 5, Min defaults to 0, Max defaults to 1.
	/// </summary>
	/// <see cref="ISliderDescriptor"/>
	public class SliderDescriptor : DefaultValidator, ISliderDescriptor, IUserEditedNotifier
	{
		/// <summary>
		/// Minimum value. Defaults 0
		/// </summary>
		public float Min { get; set; } = 0;
		/// <summary>
		/// Max value. Defaults 1.
		/// </summary>
		public float Max { get; set; } = 1;
		/// <summary>
		/// Decimal Places. Defaults 5
		/// </summary>
		public int DecimalPlaces { get; set; } = 5;

		///<inheritdoc/>
		public Action<object> OnUserEdit { get; set;  }
	}

	///<inheritdoc cref="INumericUpDownDescriptor"/>
	public class NumericUpDownDescriptor : DefaultValidator, INumericUpDownDescriptor
	{
		/// <inheritdoc/>
		public float Increments { get; set; }
	}

	/// <summary>
	/// Default implementation of IUserEditedNotifier
	/// Use this if you wanna be informed of edits made by the user that aren't applied to the Value property yet
	/// </summary>
	public class UserEditNotifier : DefaultValidator, IUserEditedNotifier
	{ 
		///<inheritdoc/>
		public Action<object> OnUserEdit { get; set; }
	}

	/// <summary>
	/// Default implementation of IRefreshInhibitor
	/// Prevent entry from triggering a refresh when a user edits values in the UI or when the entry value changes in code
	/// Useful for making sure the UI doesn't refresh while the user is actively editing an entry 
	/// <br/>
	/// This might be removed in a future version. Use this only if you can't find a way to prevent the UI from updating
	/// when the user uses a control that has continuous triggers (e.g. sliders) and you can't find a way to defer value application
	/// (e.g. using an event trigger for OnPointerUp)
	/// </summary>
	/// <remarks>
	/// These prevent the entry from <em>causing</em> the UI to refresh
	/// This does not mean the entry prevents interruptions from refreshes
	/// </remarks>
	public class DefaultRefreshInhibitor : DefaultValidator, IRefreshInhibitor
	{
		///<inheritdoc/>	
		public bool InhibitRefreshOnEdit {get; set;} = false;
		///<inheritdoc/>
		public bool InhibitRefreshOnValueChange {get; set;} = false;
	}


	internal class ButtonAsEntry : DefaultValidator, IButtonDescriptor
	{
		public override bool IsValid(object value) { return true; }
		public override object EnsureValid(object value) { return false; }
		public string ButtonText { get; set; } = "";
		public string DisplayName { get; set; } = "";
		public string Description { get; set; } = "";
		public Action Handler { get; set; }
	}

}