using MelonLoader;
using MelonLoader.Logging;
using MelonLoader.Preferences;
using MonoMod.ModInterop;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System;
using Tomlet;
using Tomlet.Models;

//using System;
/*using System.Linq;
using System.Text;
using System.Threading.Tasks;*/
using static UIFramework.Debug;
using Il2CppTMPro;
namespace UIFramework.ValidatorExtensions
{
	public partial class DefaultValidator : ValueValidator
	{
		public override bool IsValid(object value) { return true; }
		public override object EnsureValid(object value) { return value; }
	}

	public interface ITextInputBehaviorDescriptor
	{
		public TMP_InputField.ContentType ContentType { get; set; }
		public int CharacterLimit { get; set; }
		public bool IsReadOnly { get; set; }
	}

	public interface ITextInputAppearanceDescriptor
	{
		public int FontSize { get; set; }
		public bool IsAutoSizing { get; set; }
		public int AutoSizeMin { get; set; }
		public int AutoSizeMax { get; set; }

		public FontStyles FontStyle { get; set; }
		public bool IsRichText { get; set; }
	}

	public interface IDynamicDropdownDescriptor
	{
		public List<string> DropdownOptionNames { get; set; }
	}

	public interface INumericSliderDescriptor
	{
		public float Min { get; set; }
		public float Max { get; set; }
		public int DecimalPlaces { get; set; }
	}

	public class UIFSlider : UIFValidator, INumericSliderHints
	{
		public float Min { get; set; }
		public float Max { get; set; }
		public int DecimalPlaces { get; set; } = 5;
	}

	public interface INumericUpDownHints
	{
		public float Increments { get; set; }
	}

	public interface IInteractable
	{
		public event Action<EventArgs> Interaction;
	}

	public interface IUifButton
	{
		public string ButtonText { get; set; }
		public string Identifier { get; set; }
		public string Description { get; set; }
	}

}