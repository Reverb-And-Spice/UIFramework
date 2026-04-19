using AssetsTools.NET.Extra;
using Il2CppInterop.Runtime;
using Il2CppSystem.Collections.Generic;
using Il2CppTMPro;
using MelonLoader;
using MelonLoader.Logging;
using MonoMod.ModInterop;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Tomlet;
using Tomlet.Models;

//using System;
/*using System.Linq;
using System.Text;
using System.Threading.Tasks;*/
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UIFramework.Debug;
using static Unity.Collections.AllocatorManager;
namespace UIFramework
{
	public partial class UIFController
	{

		public abstract class Entry : SubModelController, IChildable
		{
			/// <summary>
			/// Sets the description text
			/// </summary>
			public virtual string DescriptionText
			{
				get { return this.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text; }
				set { this.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = value; }
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


			public UIFModel.IEntry EntryModel => (UIFModel.IEntry)_internalModel;
			/// <summary>
			/// This is called when the Save button is pressed. Override to create custom behaviour.
			/// </summary>
			/// <remarks>Generally MelonPreferences are saved from the category, not the indivial entries.</remarks>
			public virtual void SaveAction()
			{
			}

			public override void ModelSet()
			{
				DescriptionText = EntryModel.Description;
				DisplayName = EntryModel.DisplayName;
				EntryModel.OnUICreated?.Invoke(this);
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
		public abstract class MelonEntry : Entry
		{


			/// <inheritdoc/>
			public override void ModelSet() { base.ModelSet(); }

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
		public class TextInputEntry : MelonEntry
		{
			/// <summary>
			/// TODO: This is jank. Deal with this by creating a base class for preference entries that aren't based on melonloader.
			/// </summary>
			protected UIFModel.ModelDataEntryBase _prefModel => (UIFModel.ModelDataEntryBase)EntryModel;
			/// <summary>
			/// Returns the textfield
			/// </summary>
			public TMP_InputField textField => this.gameObject.transform.Find("Data/Control").gameObject.GetComponent<TMP_InputField>();
			/// <summary>
			/// Sets the placeholder text in the textField
			/// </summary>
			public string PlaceHolderText { set { this.gameObject.transform.Find("Data/Control/Text Area/Placeholder").gameObject.GetComponent<TextMeshProUGUI>().text = value; } }
			/// <inheritdoc/>
			public override void ModelSet()
			{
				//textField.text = _prefModel.BoxedValue.ToString();
				//TomletMain.TomlStringFrom(_prefModel.BoxedValue).Trim();
				if (_prefModel.BoxedValue.GetType() == typeof(string))
				{
					textField.text = (string)_prefModel.BoxedValue;
				}
				else
				{
					try
					{
						textField.text = ToTomlString(_prefModel.BoxedValue);
						Debug.Log(ToTomlString(_prefModel.BoxedValue), true);
					}
					catch (Exception ex)
					{
						Debug.Log($"{ex.Message}\n{ex.StackTrace}");
					}
				}
				base.ModelSet();
			}
			public override void ApplyValueToPref()
			{
				if (_prefModel.BoxedValue.GetType() == typeof(string))
				{
					_prefModel.SetDataValue(textField.text);
				}
				else
				{
					try
					{
						if (textField.text.Trim() != "")
						{
							_prefModel.SetDataValue(FromTomlString(textField.text, _prefModel.BoxedValue.GetType()));
							Debug.Log($"Toml data parsed {FromTomlString(textField.text, _prefModel.BoxedValue.GetType())}");
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
				/*if(textField.text != _prefModel.PrefEntry.BoxedValue.ToString())
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

		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class PrefBool : MelonEntry
		{
			protected Toggle toggle => this.gameObject.transform.Find("Data/Toggle").gameObject.GetComponent<Toggle>();
			protected UIFModel.ModelDataEntryBase _prefModel => (UIFModel.ModelDataEntryBase)EntryModel;
			public bool EnteredValue => this.gameObject.transform.Find("Data/Toggle").gameObject.GetComponent<Toggle>().isOn;
			/// <inheritdoc/>
			public override void ModelSet()
			{
				toggle.isOn = (bool)_prefModel.BoxedValue;
				toggle.onValueChanged.AddListener((UnityAction<bool>)OnValueChanged);

				base.ModelSet();

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
					_prefModel.SetDataValue(EnteredValue);
				}
				catch (Exception ex)
				{
					Log(ex.Message, false, 2);
				}
			}
		}


		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class PrefDropDown : MelonEntry
		{
			protected UIFModel.ModelDataEntryBase _prefModel => (UIFModel.ModelDataEntryBase)EntryModel;
			public System.Collections.Generic.List<int> _indexToValueMap = new();
			public TMP_Dropdown dropdown;

			public Type prefEnum;
			/// <inheritdoc/>
			public override void ModelSet()
			{
				dropdown = this.gameObject.transform.Find("Data/Dropdown").GetComponent<TMP_Dropdown>();
				prefEnum = _prefModel.BoxedValue.GetType();

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

				dropdown.value = _indexToValueMap.IndexOf((int)_prefModel.BoxedValue);

				dropdown.onValueChanged.AddListener((UnityAction<int>)OnValueChanged);

				base.ModelSet();
			}
			/// <inheritdoc/>
			public override void EditCheck()
			{

			}

			public void OnValueChanged(int index)
			{
				ApplyValueToPref();
			}
			/// <inheritdoc/>
			public override void ApplyValueToPref()
			{
				_prefModel.SetDataValue(Enum.ToObject(prefEnum, _indexToValueMap[dropdown.value]));
			}
		}

		[RegisterTypeInIl2Cpp]
		public class ButtonEntry : Entry
		{

			public GameObject ButtonGo;
			/// <inheritdoc/>
			public override void ModelSet()
			{
				ButtonGo = this.gameObject.transform.Find("Data/Button").gameObject;
				ButtonGo.GetComponent<Button>().onClick.AddListener((UnityAction)OnClickRelay);
				base.ModelSet();


			}

			public void OnClickRelay()
			{
				((UIFModel.ButtonEntry)EntryModel).OnClick?.Invoke(this);
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
		public class PrefMulti : MelonEntry
		{

		}
		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class PrefSlider : MelonEntry
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

}