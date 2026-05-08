using Il2CppTMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UIFramework.UiExtensions
{
	public interface IUiExtension
	{

	}

	/// <summary>
	/// Describes properties for text inputs that define its behavior.
	/// </summary>
	public interface ITextInputBehaviorDescriptor : IUiExtension
	{
		/// <summary>
		/// What type of content is going into the textinput
		/// </summary>
		public TMP_InputField.ContentType ContentType { get; set; }
		public int CharacterLimit { get; set; }
		public bool IsReadOnly { get; set; }
	}

	/// <summary>
	/// Describes properties of text inputs that define how it looks
	/// </summary>
	public interface ITextInputAppearanceDescriptor : IUiExtension
	{
		public int FontSize { get; set; }
		/// <summary>
		/// Set to true to have the text auto size between AutoSizeMin and AutoSizeMax. If false, FontSize will be used as the font size.
		/// </summary>
		public bool IsAutoSizing { get; set; }
		public int AutoSizeMin { get; set; }
		public int AutoSizeMax { get; set; }

		public FontStyles FontStyle { get; set; }
		public bool IsRichText { get; set; }
	}


	///	<summary>
	/// Presents the entry as a dropdown and describes the options within it.
	/// </summary>
	public interface IDynamicDropdownDescriptor : IUiExtension
	{
		//public List<DropdownItem> DropdownItems { get; set; }
		public List <DropdownItem> GetDropdownItems();
		/// <summary>
		/// Sets the items to be displayed in the dropdown. 
		/// 
		/// When doing a custom implementation, make sure to fire OnDropdownItemsUpdated after storing your data
		/// 
		/// </summary>
		/// <param name="items"></param>
		public void SetDropdownItems(List<DropdownItem> items);
		public Action OnDropdownItemsUpdated { get; set; }

	}

	/// <summary>
	/// 
	/// </summary>
	public class DropdownItem
	{
		public string DisplayName { get; set; }
		public object Value { get; set; }
		public DropdownItem(string displayName, object value)
		{
			DisplayName = displayName;
			Value = value;
		}

		public DropdownItem(string value)
		{
			DisplayName = value;
			Value = value;
		}
	}


	/// <summary>
	/// Describes numeric up down controls
	/// </summary>
	public interface INumericUpDownDescriptor : IUiExtension
	{
		/// <summary>
		/// 0 = default (1 for ints, 0.1 for floats). 
		/// </summary>
		public float Increments { get; set; }
	}

	/// <summary>
	/// Implementing this will present the entry as a slider in the UI
	/// </summary>
	public interface ISliderDescriptor : IUiExtension
	{
		public float Min { get; set; }
		public float Max { get; set; }
		public int DecimalPlaces { get; set; }
	}


	/// <summary>
	/// Use this if you wanna be informed of edits made by the user that aren't applied to the Value property yet
	/// </summary>
	public interface IUserEditedNotifier : IUiExtension
	{
		/// <summary>
		/// Subscribe to this action the method you want to run when the edits a value in the UI.
		/// It must take an object parameter for the new value
		/// </summary>
		public abstract Action<object> OnUserEdit { get; set; }
	}


	/// <summary>
	/// Prevent entry from triggering a refresh when a user edits values in the UI or when the entry value changes in code
	/// Useful for making sure the UI doesn't refresh while the user is actively editing an entry 
	/// 
	/// This might be removed in a future version. Use this only if you can't find a way to prevent the UI from updating
	/// when the user uses a control that has continuous triggers (e.g. sliders) and you can't find a way to defer value application
	/// (e.g. using an event trigger for OnPointerUp)
	/// </summary>
	/// <remarks>
	/// These prevent the entry from <em>causing</em> the UI to refresh
	/// This does not mean the entry prevents interruptions from refreshes
	/// 
	/// </remarks>
	public interface IRefreshInhibitor : IUiExtension
	{
		/// <summary>
		/// Prevents the UI from automatically refreshing when the user edits an entry
		/// Use this when the entry involves a control that has continuous input with no easy way to detect when user input has ended
		/// </summary>
		public bool InhibitRefreshOnEdit { get; set; }
		/// <summary>
		/// Prevents the UI from refreshing when the value of the entry changes in the background. 
		/// Use this on entries where your code might change its values while the user is using the UI
		/// <remarks>
		/// This also means that the entry's value won't be reflected in the UI.
		/// </remarks>
		/// </summary>
		public bool InhibitRefreshOnValueChange { get; set; }
	}

	public interface ICustomUIProvider : IUiExtension
	{
		public GameObject WidgetPrefab { get; set; }
	}


	public interface IButtonDescriptor : IUiExtension
	{
		public string ButtonText { get; set; }
		public string DisplayName { get; set; }
		public string Description { get; set; }
		public Action Handler { get; set; }
	}

}
