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
namespace UIFramework.ValidationControls
{
	public partial class UIFValidator : ValueValidator
	{
		public override bool IsValid(object value) { return true; }
		public override object EnsureValid(object value) { return value; }
	}

	public interface ITextInputOptionsProvider
	{
		public TMP_InputField.ContentType ContentType { get; set; }
		public int FontSize { get; set; }
		public bool IsAutoSizing { get; set; }
		public int AutoSizeMin { get; set; }
		public int AutoSizeMax { get; set; }
		public int CharacterLimit { get; set; }
		public bool IsReadOnly { get; set; }

		public bool IsRichText { get; set; }
		public FontStyles FontStyle { get; set; }
	}

	public interface IDynamicDropdownProvider
	{
		public List<string> DropdownOptionNames { get; set; }
	}

	public interface INumericSliderProvider
	{
		public float Min { get; set; }
		public float Max { get; set; }
		public int DecimalPlaces { get; set; }
	}

	public class UIFSlider : UIFValidator, INumericSliderProvider
	{
		public float Min { get; set; }
		public float Max { get; set; }
		public int DecimalPlaces { get; set; } = 5;
	}

	public interface INumericUpDownProvider
	{
		public float Increments { get; set; }
	}

	public interface InteractionEventProvider
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