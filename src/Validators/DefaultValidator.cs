using MelonLoader;
using MelonLoader.Logging;
using MelonLoader.Preferences;
using MonoMod.ModInterop;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System;
using Tomlet;
using Tomlet.Models;
using UnityEngine;

//using System;
/*using System.Linq;
using System.Text;
using System.Threading.Tasks;*/
using static UIFramework.Debug;
using Il2CppTMPro;
namespace UIFramework.ValidatorExtensions
{
	/// <summary>
	/// Default implementation of the MelonLoader ValueValidator class.
	/// This satisfies the required members but just acts as a passthrough. 
	/// It's the equivalent of not having a validator at all, but it allows for the use of the other descriptor interfaces without needing to implement a custom validator.
	/// </summary>
	public partial class DefaultValidator : ValueValidator
	{
		public override bool IsValid(object value) { return true; }
		public override object EnsureValid(object value) { return value; }
	}
	/// <summary>
	/// Describes properties for text inputs that define its behavior.
	/// </summary>
	public interface ITextInputBehaviorDescriptor
	{
		/// <summary>
		/// What type of content is going into the textinput
		/// </summary>
		public TMP_InputField.ContentType ContentType { get; set; }
		public int CharacterLimit { get; set; }
		public bool IsReadOnly { get; set; }
	}
	/// <inheritdoc cref="ITextInputBehaviorDescriptor"/>
	public class TextInputBehaviorDescriptor : DefaultValidator, ITextInputBehaviorDescriptor
	{
		public TMP_InputField.ContentType ContentType { get; set; } = TMP_InputField.ContentType.Standard;
		public int CharacterLimit { get; set; } = 0;
		public bool IsReadOnly { get; set; } = false;
	}


	/// <summary>
	/// Describes properties of text inputs that define how it looks
	/// </summary>
	public interface ITextInputAppearanceDescriptor
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


	///	<summary>
	/// Presents the entry as a dropdown and describes the options within it.
	/// </summary>
	public interface IDynamicDropdownDescriptor
	{
		public List<string> DropdownOptionNames { get; set; }
	}
	/// <see cref="IDynamicDropdownDescriptor"/>
	public class DynamicDropdownDescriptor : DefaultValidator, IDynamicDropdownDescriptor
	{
		public List<string> DropdownOptionNames { get; set; } = new List<string>();
	}


	/// <summary>
	/// Implementing this will present the entry as a slider in the UI
	/// </summary>
	public interface ISliderDescriptor
	{
		public float Min { get; set; }
		public float Max { get; set; }
		public int DecimalPlaces { get; set; }
	}
	/// <summary>
	/// Default implementation of ISliderDescriptor. Used for numeric inputs that want to be sliders. DecimalPlaces defaults to 5, Min defaults to 0, Max defaults to 1.
	/// </summary>
	/// <see cref="ISliderDescriptor"/>
	public class SliderDescriptor : DefaultValidator, ISliderDescriptor
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
	}

	/// <summary>
	/// Describes numeric up down controls
	/// </summary>
	public interface INumericUpDownDescriptor
	{
		/// <summary>
		/// 0 = default (1 for ints, 0.1 for floats). 
		/// </summary>
		public float Increments { get; set; }
	}
	///<inheritdoc cref="INumericUpDownDescriptor"/>
	public class NumericUpDownDescriptor : DefaultValidator, INumericUpDownDescriptor
	{
		/// <inheritdoc>
		public float Increments { get; set; }
	}

	public interface IInteractable
	{
		public event Action<EventArgs> Interaction;
	}

	public interface ICustomUIProvider
	{
		public GameObject WidgetPrefab { get; set; }
	}


	public interface IButtonDescriptor
	{
		public string ButtonText { get; set; }
		public string DisplayName { get; set; }
		public string Description { get; set; }
		public Action Handler { get; set; }
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