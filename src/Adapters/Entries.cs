using Il2CppTMPro;
using MelonLoader;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Tomlet;
using Tomlet.Models;
using UIFramework.UiExtensions;
using UIFramework.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UIFramework.Debug;
using UnityEngine.EventSystems;
namespace UIFramework.Adapters

{
	[RegisterTypeInIl2Cpp]
	public abstract class PrefEntryAdapter : SubModelAdapter, IChildable
	{
		/// <summary>
		/// Sets the description text
		/// </summary>
		public virtual string DescriptionText
		{
			get { return this.gameObject.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text; }
			set { this.gameObject.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = value; }
		}
		/// <summary>
		/// Sets the identifier text
		/// </summary>
		public virtual string DisplayName
		{
			get { return this.gameObject.gameObject.transform.Find("Data/Label").gameObject.GetComponent<TextMeshProUGUI>().text; }
			set { this.gameObject.gameObject.transform.Find("Data/Label").gameObject.GetComponent<TextMeshProUGUI>().text = value; }
		}

		public virtual EntryState EntryStatus { get; set; }

		/// <summary>
		/// Runs when the model property has been set. 
		/// </summary>


		public IEntry EntryModel => (IEntry)_internalModel;
		/// <summary>
		/// This is called when the Save button is pressed. Override to create custom behaviour.
		/// </summary>
		/// <remarks>Generally MelonPreferences are saved from the category, not the indivial entries.</remarks>
		public virtual void SaveAction()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		public override void ModelSet()
		{
			SetData();
			DescriptionText = EntryModel.Description;
			DisplayName = EntryModel.DisplayName;
		}
		public virtual void SetData()
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

			TomlParser parser = new TomlParser();
			TomlDocument inputToml = parser.Parse(wrappedEntry);
			TomlValue inputVal = inputToml.GetValue("temp");

			return TomletMain.To(targetType, inputVal);
		}
	}

	/// <summary>
	/// Inherit this class to create your own custom entry controllers for your own input controls.
	/// TODO: Refactor this to suggest non-melon related settings storage
	/// </summary>
	[RegisterTypeInIl2Cpp]
	public abstract class DataEntryAdapter : PrefEntryAdapter
	{


		/// <inheritdoc/>
		//public override void ModelSet() { base.ModelSet(); }

		virtual protected DataEntryModelBase _prefModel => (DataEntryModelBase)EntryModel;

		/// <inheritdoc/>
		public virtual bool ValidationCheck()
		{
			return true;
		}

		public virtual void EditCheck() { }

		public override void SaveAction()
		{
			ApplyValueToPref();
			base.SaveAction();
		}

		public virtual void ApplyValueToPref() { }
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
		public TMP_InputField textField => this.gameObject.transform.Find("Data/TextControl").gameObject.GetComponent<TMP_InputField>();
		/// <summary>
		/// Sets the placeholder text in the textField
		/// </summary>
		public string PlaceHolderText { set { this.gameObject.transform.Find("Data/TextControl/Text Area/Placeholder").gameObject.GetComponent<TextMeshProUGUI>().text = value; } }
		/// <inheritdoc/>
		public override void SetData()
		{
			//textField.text = _prefModel.ModelBoxedValue.ToString();
			//TomletMain.TomlStringFrom(_prefModel.ModelBoxedValue).Trim();
			if (_prefModel.ModelBoxedValue.GetType() == typeof(string))
			{
				textField.text = (string)_prefModel.ModelBoxedValue;
			}
			else
			{
				try
				{
					textField.text = ToTomlString(_prefModel.ModelBoxedValue);
					//Debug.Log(ToTomlString(_prefModel.ModelBoxedValue), true);
				}
				catch (Exception ex)
				{
					Debug.Log($"{ex.Message}\n{ex.StackTrace}");
				}
			}
		}
		public override void ApplyValueToPref()
		{
			if (_prefModel.ModelBoxedValue.GetType() == typeof(string))
			{
				_prefModel.TryApply(textField.text);
			}
			else
			{
				try
				{
					if (textField.text.Trim() != "")
					{
						_prefModel.TryApply(FromTomlString(textField.text, _prefModel.ModelBoxedValue.GetType()));
						//Debug.Log($"Toml data parsed {FromTomlString(textField.text, _prefModel.ModelBoxedValue.GetType())}");
					}
				}
				catch (Exception ex)
				{
					Log(ex.Message, false, 2);
				}
			}
		}

		public override void EditCheck()
		{
			/*if(textField.text != _prefModel.PrefEntry.ModelBoxedValue.ToString())
			{
				EntryStatus = EntryState.Edited;
			}*/
		}



		public virtual void EditStart(string s)
		{
			textField.textComponent.fontStyle = FontStyles.Normal;
		}
		public virtual void EditEnd(string s)
		{
			textField.textComponent.fontStyle = FontStyles.Italic;
			ApplyValueToPref();
		}

		protected virtual void Start()
		{
			textField.onSelect.AddListener((System.Action<string>)EditStart);
			textField.onDeselect.AddListener((System.Action<string>)EditEnd);
		}
	}


	[RegisterTypeInIl2Cpp]
	public class NumSliderAdapter : TextEntryAdapter
	{
		protected Slider Slider => gameObject.transform.Find("Data/SliderControl").gameObject.GetComponent<UnityEngine.UI.Slider>();
		protected TMP_InputField _textField => gameObject.transform.Find("Data/TextControl").gameObject.GetComponent<TMP_InputField>();
		protected virtual ISliderDescriptor SliderSettings => _prefModel.UiExtension as ISliderDescriptor;

		public override void SetData()
		{
			_textField.onEndEdit.AddListener((UnityAction<string>)EditEnd);
			_textField.onSelect.AddListener((UnityAction<string>)EditStart);

			Slider.minValue = SliderSettings?.Min ?? 0;
			Slider.maxValue = SliderSettings?.Max ?? 100;
			Slider.value = Convert.ToSingle(_prefModel.ModelBoxedValue);
			Slider.onValueChanged.AddListener((UnityAction<float>)OnValueChanged);
			AddPointerUp();
			if (_prefModel.ModelBoxedValue is int or byte or short or long or sbyte or ushort or uint or ulong)
			{
				Slider.wholeNumbers = true;
				_textField.contentType = TMP_InputField.ContentType.IntegerNumber;
				_textField.text = Slider.value.ToString("F0");
			}
			else
			{
				Slider.wholeNumbers = false;
				_textField.contentType = TMP_InputField.ContentType.DecimalNumber;
				_textField.text = Slider.value.ToString("F" + SliderSettings?.DecimalPlaces);
			}
		}

		public void OnValueChanged(float newValue)
		{
			_textField.text = newValue.ToString(_textField.contentType == TMP_InputField.ContentType.IntegerNumber ? "F0" : "F" + SliderSettings?.DecimalPlaces);
			//ApplyValueToPref();
			//Debug.Log($"Slider value changed to {newValue}", true);
		}

		public override void ApplyValueToPref()
		{
			_prefModel.TryApply(Convert.ChangeType(Slider.value, _prefModel.ModelBoxedValue.GetType()));
		}
		public virtual void EditStart(string s)
		{
			_textField.textComponent.fontStyle = FontStyles.Normal;
		}
		public virtual void EditEnd(string s)
		{
			_textField.textComponent.fontStyle = FontStyles.Italic;

			if (float.TryParse(s, out float result))
			{
				if (SliderSettings != null)
				{
					result = Mathf.Clamp(result, SliderSettings.Min, SliderSettings.Max);
				}
				Slider.value = result;
				ApplyValueToPref();
			}
			else
			{
				Debug.Log($"Invalid input for slider: {s}", false, 2);
				_textField.text = Slider.value.ToString(_textField.contentType == TMP_InputField.ContentType.IntegerNumber ? "F0" : "F" + SliderSettings?.DecimalPlaces);
			}
		}

		//onpointerup

		public void AddPointerUp()
		{
			EventTrigger trigger = Slider.gameObject.AddComponent<EventTrigger>();

			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerUp;

			entry.callback.AddListener((UnityAction<BaseEventData>)PointerUP);

			trigger.triggers.Add(entry);

		}

		protected void PointerUP(BaseEventData eventData)
		{
			ApplyValueToPref();
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[RegisterTypeInIl2Cpp]
	public class BoolToggleAdapter : DataEntryAdapter
	{
		protected Toggle toggle => this.gameObject.transform.Find("Data/ToggleControl").gameObject.GetComponent<Toggle>();
		//protected override DataEntryModelBase _prefModel => (DataEntryModelBase)EntryModel;
		public bool EnteredValue => this.gameObject.transform.Find("Data/ToggleControl").gameObject.GetComponent<Toggle>().isOn;
		/// <inheritdoc/>
		public override void SetData()
		{
			toggle.isOn = (bool)_prefModel.ModelBoxedValue;
			toggle.onValueChanged.AddListener((UnityAction<bool>)OnValueChanged);

		}
		/// <inheritdoc/>
		public override void EditCheck()
		{

		}

		public void OnValueChanged(bool newValue)
		{
			ApplyValueToPref();
		}
		/// <inheritdoc/>
		public override void ApplyValueToPref()
		{
			try
			{
				_prefModel.TryApply(EnteredValue);
			}
			catch (Exception ex)
			{
				Log(ex.Message, false, 2);
			}
		}
	}

	[RegisterTypeInIl2Cpp]
	public abstract class DropDownAdapterBase : DataEntryAdapter
	{
		protected DataEntryModelBase _prefModel => (DataEntryModelBase)EntryModel;
		public System.Collections.Generic.List<int> _indexToValueMap = new();
		public TMP_Dropdown dropdown;

		public override void SetData()
		{
			dropdown = this.gameObject.transform.Find("Data/DropdownControl").GetComponent<TMP_Dropdown>();

			GetDropdownData();

			

			dropdown.onValueChanged.AddListener((UnityAction<int>)OnValueChanged);
		}
		public void OnValueChanged(int index)
		{
			ApplyValueToPref();
		}

		public virtual void GetDropdownData()
		{

		}
	}

	/// <summary>
	/// 
	/// </summary>
	[RegisterTypeInIl2Cpp]
	public class EnumDropdownAdapter : DropDownAdapterBase
	{


		public Type prefEnum;

		public override void GetDropdownData()
		{
			prefEnum = _prefModel.ModelBoxedValue.GetType();

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
			dropdown.value = _indexToValueMap.IndexOf((int)_prefModel.ModelBoxedValue);
		}
		/// <inheritdoc/>

		/// <inheritdoc/>
		public override void EditCheck()
		{

		}


		/// <inheritdoc/>
		public override void ApplyValueToPref()
		{
			_prefModel.TryApply(Enum.ToObject(prefEnum, _indexToValueMap[dropdown.value]));
		}
	}
	[RegisterTypeInIl2Cpp]
	public class DynamicDopdownAdapter : DropDownAdapterBase
	{
		public IDynamicDropdownDescriptor DropdownContents => _prefModel.UiExtension as IDynamicDropdownDescriptor;

		public override void GetDropdownData()
		{
			try
			{
				Il2CppSystem.Collections.Generic.List<string> dropdownItems = new();
				foreach (DropdownItem item in DropdownContents.GetDropdownItems())
				{
					dropdownItems.Add(item.DisplayName);
				}
				dropdown.ClearOptions();
				dropdown.AddOptions(dropdownItems);
				int itemToLoad = DropdownContents.GetDropdownItems().FindIndex(x => object.Equals(x.Value, _prefModel.ModelBoxedValue));
				dropdown.value = itemToLoad;

			}
			catch (Exception ex) { Debug.Log($"{ex}"); }

		}

		public override void SetData()
		{
			base.SetData();
			DropdownContents.OnDropdownItemsUpdated = GetDropdownData;

		}
		public override void ApplyValueToPref()
		{
			_prefModel.TryApply((DropdownContents.GetDropdownItems()[dropdown.value]).Value);
		}

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
		IButtonDescriptor _buttonDescrictor => _prefModel?.UiExtension as IButtonDescriptor;
		public override void SetData()
		{
			_buttonGo = this.gameObject.transform.Find("Data/ButtonControl").gameObject;

			_buttonComponent = _buttonGo.GetComponent<Button>();
			_buttonComponent.onClick.AddListener((UnityAction)_buttonDescrictor?.Handler);
			TextMeshProUGUI buttonText = _buttonGo.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
			buttonText.text = _buttonDescrictor?.ButtonText ?? "Button";

		}
	}

	[RegisterTypeInIl2Cpp]
	public class ButtonModelAdapter : PrefEntryAdapter
	{
		ButtonEntry ButtonModel => (ButtonEntry)EntryModel;
		public GameObject ButtonGo;
		/// <inheritdoc/>
		public override void SetData()
		{
			ButtonGo = this.gameObject.transform.Find("Data/ButtonControl").gameObject;
			ButtonGo.GetComponent<Button>().onClick.AddListener((UnityAction)OnClickRelay);
			base.SetData();
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