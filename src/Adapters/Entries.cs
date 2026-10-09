using Il2CppRUMBLE.Combat.ShiftStones;
using Il2CppTMPro;
using MelonLoader;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Tomlet;
using Tomlet.Models;
using UIFramework.Models;
using UIFramework.UiExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UIFramework.Debug;
namespace UIFramework.Adapters

{
	[RegisterTypeInIl2Cpp]
	public abstract class PrefEntryAdapterBase : SubModelAdapter
	{
		/// <summary>
		/// Sets the description text
		/// </summary>
		protected virtual string DescriptionText
		{
			get { return this.gameObject.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text; }
			set { this.gameObject.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = value; }
		}
		/// <summary>
		/// Sets the identifier text
		/// </summary>
		protected virtual string DisplayName
		{
			get { return this.gameObject.gameObject.transform.Find("Data/Label").gameObject.GetComponent<TextMeshProUGUI>().text; }
			set { this.gameObject.gameObject.transform.Find("Data/Label").gameObject.GetComponent<TextMeshProUGUI>().text = value; }
		}
		
		/// <summary>
		/// The model for the entry
		/// </summary>
		protected private EntryModelBase EntryModel => (EntryModelBase)_internalModel;
		/// <summary>
		/// This is called when the Save button is pressed. Override to create custom behaviour.
		/// </summary>
		/// <remarks>Generally MelonPreferences are saved from the category, not the indivial entries.</remarks>
		public virtual void PreSaveAction()
		{
		}
		/// <summary>
		/// Called when the model has been set. Calls DisplayMetadata and DisplayContents in that order
		/// </summary>
		protected sealed override void ModelSet()
		{
			DisplayMetadata();
			DisplayContents();
		}
		/// <summary>
		/// Displays the entry model's display name and description by invoking DisplayEntryInfo.
		/// </summary>
		protected sealed override void DisplayMetadata()
		{
			DisplayEntryInfo(EntryModel.DisplayName, EntryModel.Description);
		}
		/// <summary>
		/// Displays the name and description on to the View object
		/// </summary>
		/// <param name="displayName"></param>
		/// <param name="description"></param>
		protected virtual void DisplayEntryInfo(string displayName, string description)
		{
			DescriptionText = description;
			DisplayName = displayName;
		}
		/// <summary>
		/// Displays actual data content
		/// </summary>
		protected virtual void DisplayContents()
		{

		}
		/// <summary>
		/// Serializes an object into the toml string representation of that object
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public string ToTomlString(object input)
		{
			return TomletMain.ValueFrom(input).SerializedValue;
		}
		/// <summary>
		/// Parses a toml string into an object of type with its value.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		public object FromTomlString(string input, Type targetType)
		{
			string wrappedEntry = $"temp = {input.Trim()}";

			TomlParser parser = new();
			TomlDocument inputToml = parser.Parse(wrappedEntry);
			TomlValue inputVal = inputToml.GetValue("temp");

			return TomletMain.To(targetType, inputVal);
		}
	}

	/// <summary>
	/// Inherit this class to create your own custom entry controllers for your own input controls.
	/// </summary>
	/// <remarks>Released</remarks>
	[RegisterTypeInIl2Cpp]
	public abstract class DataEntryAdapter : PrefEntryAdapterBase
	{
		//public override void ModelSet() { base.ModelSet(); }
		/// <summary>
		/// Model for the data entry
		/// </summary>
		private protected DataEntryModelBase DataEntry => (DataEntryModelBase)EntryModel;
		/// <summary>
		/// The boxed value loaded from the model when it was first set
		/// </summary>
		protected object CurrentBoxedValue;
		/// <summary>
		/// Reference to the UI Extension
		/// </summary>
		protected IUiExtension UiExtension => DataEntry.UiExtension;

		/// <inheritdoc/>
		public override void PreSaveAction()
		{

		}
		/// <inheritdoc/>
		protected sealed override void DisplayContents()
		{
			CurrentBoxedValue = DataEntry.ModelBoxedValue;
			DisplayData(DataEntry.ModelBoxedValue);
		}
		/// <summary>
		/// Passes the data from the model for adapting into a format displayed to the user
		/// </summary>
		/// <param name="boxedValue"></param>
		protected virtual void DisplayData(object boxedValue)
		{

		}
		/// <summary>
		/// Submits the value to the model
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected bool SubmitValue(object value)
		{
			return DataEntry.TryApply(value);
		}
	}


	/// <summary>
	/// Base controller for text fields
	/// </summary>
	[RegisterTypeInIl2Cpp]
	public class TextEntryAdapter : DataEntryAdapter
	{
		/// <summary>
		/// Returns the textfield
		/// </summary>
		protected TMP_InputField TextField => this.gameObject.transform.Find("Data/TextControl").gameObject.GetComponent<TMP_InputField>();
		/// <summary>
		/// Sets the placeholder text in the TextField
		/// </summary>
		protected string PlaceHolderText { set { this.gameObject.transform.Find("Data/TextControl/Text Area/Placeholder").gameObject.GetComponent<TextMeshProUGUI>().text = value; } }

		/// <summary>
		/// A reference to the UI Extension as a behavior descriptor
		/// </summary>
		protected ITextInputBehaviorDescriptor BehaviorDescriptor => UiExtension as ITextInputBehaviorDescriptor;
		protected ITextInputAppearanceDescriptor AppearanceDescriptor => UiExtension as ITextInputAppearanceDescriptor;

		Type boxedValueType = null;

		/// <summary>
		/// <inheritdoc/>
		/// Assigns the behavior descriptor properties if present. 
		/// </summary>
		protected override void DisplayData(object boxedValue)
		{
			if (BehaviorDescriptor is not null)
			{
				TextField.contentType = BehaviorDescriptor.ContentType;
				TextField.characterLimit = BehaviorDescriptor.CharacterLimit;
				TextField.readOnly = BehaviorDescriptor.IsReadOnly;
				TextField.asteriskChar = BehaviorDescriptor.PasswordChar;
			}

			boxedValueType = boxedValue.GetType();
			//If boxedvaluetype is a string, display directly
			if (boxedValueType == typeof(string))
			{
				TextField.text = (string)boxedValue;
			}
			//Otherwise resort to TOML
			else
			{
				try
				{
					TextField.text = ToTomlString(boxedValue);

				}
				catch (Exception ex)
				{
					Debug.Log($"{ex.Message}\n{ex.StackTrace}");
				}
			}
		}
		/// <summary>
		/// Parse the contents in the text field through toml then submit it.
		/// </summary>
		protected void ParseThenSubmit()
		{
			if (boxedValueType == typeof(string))
			{
				SubmitValue(TextField.text);
			}
			else
			{
				try
				{
					if (TextField.text.Trim() != "")
					{
						SubmitValue(FromTomlString(TextField.text, DataEntry.ModelBoxedValue.GetType()));
					}
				}
				catch (Exception ex)
				{
					Log(ex.Message, false, 2);
				}
			}
		}

		/// <summary>
		/// Sets the text to non-italicized font style when editing. 
		/// Replaces the need for managing the placeholder text
		/// </summary>
		protected virtual void EditStart(string s)
		{
			if (BehaviorDescriptor?.IsReadOnly == true) return;
				TextField.textComponent.fontStyle = FontStyles.Normal;
		}
		/// <summary>
		/// Sets the text to italicized when done editing
		/// </summary>
		protected virtual void EditEnd(string s)
		{
			if (BehaviorDescriptor?.IsReadOnly == true) return;
			TextField.textComponent.fontStyle = FontStyles.Italic;
			ParseThenSubmit();
		}
		/// <summary>
		/// Unity callback. Subscribes EditStart and EditEnd to the appropriate events
		/// </summary>
		protected virtual void Start()
		{

			TextField.onSelect.AddListener((System.Action<string>)EditStart);
			TextField.onDeselect.AddListener((System.Action<string>)EditEnd);
		}
	}
	/// <summary>
	/// Entry adapter for a numeric view. Derives from text inputs
	/// </summary>
	[RegisterTypeInIl2Cpp]
	public class NumericEntryAdapter : TextEntryAdapter
	{
		/// <summary>
		/// Button for incrementing
		/// </summary>
		protected Button AddButton =>
			gameObject.transform.Find("Data/ButtonGroup/Add").gameObject.GetComponent<Button>();
		/// <summary>
		/// Button for decrementing
		/// </summary>
		protected Button SubtractButton =>
			gameObject.transform.Find("Data/ButtonGroup/Sub").gameObject.GetComponent<Button>();

		/// <summary>
		/// Gets the defined numeric steps from the descriptor. 
		/// If it doesn't exist, or is zero, it goes back to the default depending on the numeric type.
		/// </summary>
		protected float IncrementStep
		{
			get
			{
				float step = (NumericSettings)?.Steps ?? 0;

				if (step == 0)
				{
					step = CurrentBoxedValue switch
					{
						byte or sbyte or short or ushort or int or uint or long or ulong => 1f,
						_ => 0.1f
					};
				}

				return step;
			}
		}

		/// <summary>
		/// UI Extension as NumberboxDescriptor NumericSettings
		/// </summary>
		protected virtual INumberBoxDescriptor NumericSettings => UiExtension as INumberBoxDescriptor;

		/// <summary>
		/// <inheritdoc/>
		/// Subscribes the button click events to increment/decrement specifically.
		/// </summary>
		protected override void Start()
		{
			base.Start();

			AddButton.onClick.AddListener((UnityAction)Increment);
			SubtractButton.onClick.AddListener((UnityAction)Decrement);

		}

		protected override void DisplayData(object boxedValue)
		{
			//Call the original displaydata method
			base.DisplayData(boxedValue);

			//Take the string, convert it to the correct type, then convert it back to a string with the correct number of decimal places
			if (boxedValue.GetType() != typeof(int))
			{
				TextField.text = Convert.ToSingle(boxedValue).ToString($"F{NumericSettings?.DecimalPlaces ?? 1}");
			}
			//If marked as readonly, disable the increment and decrement buttons
			if (BehaviorDescriptor?.IsReadOnly == true)
			{
				AddButton.interactable = false;
				SubtractButton.interactable = false;
			}

		}
		/// <summary>
		/// Increment button event
		/// </summary>
		protected virtual void Increment()
		{
			//Cancel operation if behavior descriptor marks it as readonly
			if (BehaviorDescriptor?.IsReadOnly == true) return;
			SubmitValue(CurrentBoxedValue == null ? IncrementStep : Convert.ChangeType(Convert.ToSingle(CurrentBoxedValue) + IncrementStep, CurrentBoxedValue.GetType()));
		}
		/// <summary>
		/// Decerement button event
		/// </summary>
		protected virtual void Decrement()
		{
			//Cancel operation if behavior descriptor marks it as readonly
			if (BehaviorDescriptor?.IsReadOnly == true) return;
			SubmitValue(CurrentBoxedValue == null ? -IncrementStep : Convert.ChangeType(Convert.ToSingle(CurrentBoxedValue) - IncrementStep, CurrentBoxedValue.GetType()));
		}
	}
	/// <summary>
	/// <inheritdoc/>
	/// Adapter for slider entries. Inherits from Text inputs
	/// </summary>
	[RegisterTypeInIl2Cpp]
	public class NumSliderAdapter : TextEntryAdapter
	{
		/// <summary>
		/// Control component for slider
		/// </summary>
		protected Slider Slider => gameObject.transform.Find("Data/SliderControl").gameObject.GetComponent<UnityEngine.UI.Slider>();
		/// <summary>
		///UI Extension for the slider descriptor
		/// </summary>
		protected virtual ISliderDescriptor SliderSettings => UiExtension as ISliderDescriptor;

		/// <summary>
		/// <inheritdoc/>
		/// Gets the data from the model then set the value of the slider to match
		/// </summary>
		protected override void DisplayData(object boxedValue)
		{
			base.DisplayData(boxedValue);
			//Make slider uninteractable if behavior descriptor calls for a readonly entry
			if (BehaviorDescriptor?.IsReadOnly == true)
			{
				Slider.interactable = false;
			}
			//Add events
			TextField.onEndEdit.AddListener((UnityAction<string>)EditEnd);
			TextField.onSelect.AddListener((UnityAction<string>)EditStart);
			//Sets the values according to the slider descriptor
			Slider.minValue = SliderSettings?.Min ?? 0;
			Slider.maxValue = SliderSettings?.Max ?? 100;
			Slider.value = Convert.ToSingle(boxedValue);
			//Set an event for a slider's onValueChanged to update the text input controls
			Slider.onValueChanged.AddListener((UnityAction<float>)OnValueChanged);
			//Add the onPointerUP event handler
			AddPointerUp();
			//sets the sliders to whole numbers only for integral values. 
			if (boxedValue is int or byte or short or long or sbyte or ushort or uint or ulong)
			{
				Slider.wholeNumbers = true;
				TextField.contentType = TMP_InputField.ContentType.IntegerNumber;
				TextField.text = Slider.value.ToString("F0");
			}
			else
			{
				Slider.wholeNumbers = false;
				TextField.contentType = TMP_InputField.ContentType.DecimalNumber;
				TextField.text = Slider.value.ToString("F" + SliderSettings?.DecimalPlaces);
			}
		}
		/// <summary>
		///Updates the text input with the slider's values
		/// </summary>
		protected void OnValueChanged(float newValue)
		{
			TextField.text = newValue.ToString(TextField.contentType == TMP_InputField.ContentType.IntegerNumber ? "F0" : "F" + SliderSettings?.DecimalPlaces);
			SliderSettings?.OnSliderValueChanged?.Invoke((float)newValue);
			//ApplyValueToPref();
			//Debug.Log($"Slider value changed to {newValue}", true);
		}
		/// <summary>
		/// Submits the slider's value to the model
		/// </summary>
		public void SubmitSliderValue()
		{
			SubmitValue(Convert.ChangeType(Slider.value, DataEntry.ModelBoxedValue.GetType()));
		}
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		protected override void EditStart(string s)
		{
			TextField.textComponent.fontStyle = FontStyles.Normal;
		}
		/// <summary>
		/// <inheritdoc/>
		/// After editing the text input, apply the value in it to the slider position
		/// </summary>
		protected override void EditEnd(string s)
		{
			TextField.textComponent.fontStyle = FontStyles.Italic;

			if (float.TryParse(s, out float result))
			{
				if (SliderSettings != null)
				{
					result = Mathf.Clamp(result, SliderSettings.Min, SliderSettings.Max);
				}
				Slider.value = result;
				SubmitSliderValue();
			}
			else
			{
				Debug.Log($"Invalid input for slider: {s}", false, 2);
				TextField.text = Slider.value.ToString(TextField.contentType == TMP_InputField.ContentType.IntegerNumber ? "F0" : "F" + SliderSettings?.DecimalPlaces);
			}
		}
		/// <summary>
		/// Adds a pointer up event toe the slider which does the actual submission
		/// </summary>
		public void AddPointerUp()
		{
			EventTrigger trigger = Slider.gameObject.AddComponent<EventTrigger>();

			EventTrigger.Entry entry = new()
			{
				eventID = EventTriggerType.PointerUp
			};
			entry.callback.AddListener((UnityAction<BaseEventData>)PointerUP);

			trigger.triggers.Add(entry);

		}
		/// <summary>
		/// Only submit the slider value after the user stops dragging it
		/// Otherwise, UI will continously try to refresh making the user only
		/// able to edit one frame.
		/// </summary>
		protected void PointerUP(BaseEventData eventData)
		{
			SubmitSliderValue();
		}
	}

	/// <summary>
	/// <inheritdoc/>
	/// Adapter for booleans shown as toggles
	/// </summary>
	[RegisterTypeInIl2Cpp]
	public class BoolToggleAdapter : DataEntryAdapter
	{
		/// <summary>
		/// The actual toggle control component
		/// </summary>
		protected Toggle Toggle => this.gameObject.transform.Find("Data/ToggleControl").gameObject.GetComponent<Toggle>();
		//protected override DataEntryModelBase DataEntry => (DataEntryModelBase)EntryModel;
		/// <summary>
		/// Gets the value of the toggle
		/// </summary>
		public bool EnteredValue => this.gameObject.transform.Find("Data/ToggleControl").gameObject.GetComponent<Toggle>().isOn;
		/// <inheritdoc/>
		protected override void DisplayData(object boxedValue)
		{
			Toggle.isOn = (bool)boxedValue;
			Toggle.onValueChanged.AddListener((UnityAction<bool>)OnValueChanged);
		}

		public void OnValueChanged(bool newValue)
		{
			SubmitValue(newValue);
		}
	}
	/// <summary>
	/// The base class for dropdown adapters
	/// </summary>
	[RegisterTypeInIl2Cpp]
	public abstract class DropDownAdapterBase : DataEntryAdapter
	{
		//protected DataEntryModelBase _prefModel => (DataEntryModelBase)EntryModel;
		/// <summary>
		/// List of ints serving to map the selected index to the the actual entry value of that index in the dropdown
		/// </summary>
		protected System.Collections.Generic.List<int> _indexToValueMap = new();
		/// <summary>
		/// The actual dropdown control
		/// </summary>
		protected TMP_Dropdown dropdown;
		/// <summary>
		/// The value of the model entry
		/// </summary>
		protected object EntryValue { get; set; }
		/// <summary>
		/// <inheritdoc/>
		/// Sets the dropdown object and the EntryValue from the model
		/// Also calls the select dropdown item method
		/// Adds the OnValueChanged listener
		/// </summary>
		protected override void DisplayData(object boxedValue)
		{

			dropdown = this.gameObject.transform.Find("Data/DropdownControl").GetComponent<TMP_Dropdown>();
			EntryValue = boxedValue;
			GetDropdownData();
			SelectDropDownItem(boxedValue);


			dropdown.onValueChanged.AddListener((UnityAction<int>)OnValueChanged);
		}
		/// <summary>
		/// Submits the dropdown selection
		/// </summary>
		public void OnValueChanged(int index)
		{
			SubmitDropdownValue();
		}
		/// <summary>
		/// Virtual overridable method for submitting values
		/// </summary>
		public virtual void SubmitDropdownValue()
		{

		}
		/// <summary>
		/// Virtual Overridable Method for gettting dropdown data. Difference between dynamic vs enum dropdowns
		/// </summary>
		protected virtual void GetDropdownData()
		{
			Debug.Log("GetDropdownData not implemented for " + this.GetType().Name, false, 2);
		}
		/// <summary>
		/// Virtual overridable method for selecting dropdown data. Difference between dynamic vs enum dropdowns
		/// </summary>
		protected virtual void SelectDropDownItem(object boxedValue)
		{
			Debug.Log("SelectDropDownItem not implemented for " + this.GetType().Name, false, 2);
		}
		void Start()
		{

		}
	}

	/// <summary>
	/// Dropdown Adapter for enums
	/// </summary>
	[RegisterTypeInIl2Cpp]
	public class EnumDropdownAdapter : DropDownAdapterBase
	{
		/// <summary>
		/// The enum type of the model value
		/// </summary>
		protected Type prefEnum;
		/// <summary>
		/// Gets the data from the enum to populate the dropdown
		/// </summary>
		protected override void GetDropdownData()
		{
			prefEnum = EntryValue.GetType();

			//Get a list of display name attributes or the enum name if not available
			Il2CppSystem.Collections.Generic.List<string> enumNames = new();
			foreach (var value in Enum.GetValues(prefEnum))
			{
				FieldInfo info = prefEnum.GetField(value.ToString());
				DisplayAttribute attr = info?.GetCustomAttribute<DisplayAttribute>();
				enumNames.Add(attr?.GetName() ?? value.ToString());
				_indexToValueMap.Add(Convert.ToInt32(value));
			}

			dropdown.ClearOptions();
			dropdown.AddOptions(enumNames);

		}
		/// <summary>
		/// Takes the boxedValue from the model and sets the dropdown value accordingly
		/// This allows for enum selections that might not be in order 4
		/// or doesn't start at 0
		/// </summary>
		protected override void SelectDropDownItem(object boxedValue)
		{
			//Get the boxed value as int, then select it from the indextovalue map and select that value in the dropdown
			dropdown.value = _indexToValueMap.IndexOf((int)boxedValue);
		}
		/// <summary>
		/// Take the int from _indexToValue map corresponding to the index of the selection of the dropdown.
		/// Convert that to an enum then submit it to the model
		/// </summary>
		public override void SubmitDropdownValue()
		{
			SubmitValue(Enum.ToObject(prefEnum, _indexToValueMap[dropdown.value]));
		}
	}
	/// <summary>
	/// Dropdown adapter for dynamic dropdowns
	/// </summary>
	[RegisterTypeInIl2Cpp]
	public class DynamicDopdownAdapter : DropDownAdapterBase
	{
		/// <summary>
		/// Get the UI Extension as a dynamic dropdown descriptor
		/// </summary>
		public IDynamicDropdownDescriptor DropdownContents => DataEntry.UiExtension as IDynamicDropdownDescriptor;

		/// <summary>
		/// Gets the data from the dynamic dropdown descriptor to populate the dropdown
		/// </summary>
		protected override void GetDropdownData()
		{
			//Subscribe this method to the OnDropdownItemsUpdated event from the descriptor
			DropdownContents.OnDropdownItemsUpdated = GetDropdownData;
			try
			{
				Il2CppSystem.Collections.Generic.List<string> dropdownItems = new();
				foreach (DropdownItem item in DropdownContents.GetDropdownItems())
				{
					dropdownItems.Add(item.DisplayName);
				}
				dropdown.ClearOptions();
				dropdown.AddOptions(dropdownItems);
			}

			catch (Exception ex) { Debug.Log($"{ex}"); }
			SelectDropDownItem(EntryValue);

		}
		/// <summary>
		/// Select the display name from dropdown contents that corresponds to the model boxed value
		/// </summary>
		protected override void SelectDropDownItem(object boxedValue)
		{
			int itemToLoad = DropdownContents.GetDropdownItems().FindIndex(x => object.Equals(x.Value, boxedValue));
			dropdown.value = itemToLoad;
		}
		/// <summary>
		/// Submit the value represented by the selected item in the dropdown
		/// </summary>
		public override void SubmitDropdownValue()
		{
			SubmitValue((DropdownContents.GetDropdownItems()[dropdown.value]).Value);
		}
		/// <summary>
		/// Unsubscribe from the dropdown descriptor when this view gets destroyed
		/// </summary>
		void OnDestroy()
		{
			DropdownContents.OnDropdownItemsUpdated -= GetDropdownData;
		}
	}
	[RegisterTypeInIl2Cpp]
	public class ButtonEntryAdapter : DataEntryAdapter
	{
		GameObject _buttonGo;
		Button _buttonComponent;
		IButtonDescriptor ButtonDescriptor => DataEntry?.UiExtension as IButtonDescriptor;
		protected override void DisplayData(object boxedValue)
		{
			_buttonGo = this.gameObject.transform.Find("Data/ButtonControl").gameObject;

			_buttonComponent = _buttonGo.GetComponent<Button>();
			_buttonComponent.onClick.AddListener((UnityAction)ButtonDescriptor?.Handler);
			TextMeshProUGUI buttonText = _buttonGo.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
			buttonText.text = ButtonDescriptor?.ButtonText ?? "Button";
		}
	}

	[RegisterTypeInIl2Cpp]
	public class ButtonModelAdapter : PrefEntryAdapterBase
	{
		ButtonEntry ButtonModel => (ButtonEntry)EntryModel;
		public GameObject ButtonGo;
		/// <inheritdoc/>
		protected override void DisplayContents()
		{
			ButtonGo = this.gameObject.transform.Find("Data/ButtonControl").gameObject;
			ButtonGo.GetComponent<Button>().onClick.AddListener((UnityAction)OnClickRelay);
			base.DisplayContents();
		}

		public void OnClickRelay()
		{
			ButtonModel.OnClick?.Invoke(this);
		}

		void OnDestroy()
		{
		}
	}
	#region no support

	/// <summary>
	///
	/// </summary>
	/*[RegisterTypeInIl2Cpp]
	public class PrefMulti : DataEntryAdapter
	{

	}
	/// <summary>
	/// 
	/// </summary>
	[RegisterTypeInIl2Cpp]
	public class NumSliderAdapter : DataEntryAdapter
	{

	}
	*/
	#endregion
	public enum EntryState
	{
		Untouched,
		Edited,
		Saved,
		Errored,

	}

}