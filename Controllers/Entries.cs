using AssetsTools.NET.Extra;
using Il2CppInterop.Runtime;
using Il2CppTMPro;
using MelonLoader;
using MelonLoader.Logging;
using MonoMod.ModInterop;
//using System;
/*using System.Linq;
using System.Text;
using System.Threading.Tasks;*/
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;
using static UIFramework.Debug;
using Il2CppSystem.Collections.Generic;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
namespace UIFramework
{
	public partial class UIFController
	{
		/*/// <summary>
		/// This was going to be the basis for all entries and what advanced users would have to implement
		/// Unfortunately can't be successfully retrieved with GetComponent in the current setup
		/// Will move to making MelonEntry the required base class
		/// </summary>
		public interface ISettingEntry : IChildable
		{
			public string DescriptionText { set; }
			public string DisplayName { set; }
			//override this function to create your own validation check
			public bool ValidationCheck();
			
		}*/

		public abstract class Entry : SubModelController, IChildable
		{
			/// <summary>
			/// Sets the description text
			/// </summary>
			public virtual string DescriptionText { 
				get { return this.gameObject.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text; } 
				set { this.gameObject.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = value; } }
			/// <summary>
			/// Sets the identifier text
			/// </summary>
			public virtual string DisplayName {
				get { return this.gameObject.gameObject.transform.Find("Data/Label").gameObject.GetComponent<TextMeshProUGUI>().text; }
				set { this.gameObject.gameObject.transform.Find("Data/Label").gameObject.GetComponent<TextMeshProUGUI>().text = value; }
			}

			public virtual EntryState EntryStatus {get; set;}

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
				//EntryModel.SaveAction();
			}

			public override void ModelSet()
			{
				DescriptionText = EntryModel.Description;
				DisplayName = EntryModel.DisplayName;
				EntryModel.OnUICreated?.Invoke(this);
			}
		}

		/// <summary>
		/// Inherit this class to create your own custom entry controllers for your own input controls.
		/// TODO: Refactor this to suggest non-melon related settings storage
		/// </summary>
		public abstract class MelonEntry : Entry
		{
			
			//public abstract string EnteredValue {get; set;}

			/// <inheritdoc/>
			public override void ModelSet() { base.ModelSet();}

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

			public virtual void ApplyValueToPref(){ }
		}


		/// <summary>
		/// Base controller for text fields 
		/// </summary>
		public abstract class TextInputEntry : MelonEntry
		{
			/// <summary>
			/// TODO: This is jank. Deal with this by creating a base class for preference entries that aren't based on melonloader.
			/// </summary>
			protected UIFModel.ModelMelonEntry _prefModel => (UIFModel.ModelMelonEntry)EntryModel;
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
				textField.text = _prefModel.BoxedValue.ToString();
				base.ModelSet();
			} 

			public override void EditCheck()
			{
				if(textField.text != _prefModel.PrefEntry.BoxedValue.ToString())
				{
					EntryStatus = EntryState.Edited;
				}
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

		public class PrefText : TextInputEntry
		{
			/// <summary>
			///
			/// </summary>
			public virtual string EnteredValue => textField.text;
			/// <inheritdoc/>

			public override void ApplyValueToPref()
			{
				try
				{
					if (EnteredValue.Trim() != "")
					{
						_prefModel.BoxedValue = EnteredValue;
					}
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
		public class PrefInt : TextInputEntry
		{
			public int EnteredValue => int.Parse(textField.text);
			/// <inheritdoc/>
			public override void ApplyValueToPref()
			{
				try
				{
					if (textField.text.Trim() != "")
					{
						_prefModel.BoxedValue = int.Parse(textField.text.Trim());
					}
				}
				catch (Exception ex)
				{
					Log($"{ex.Message} {textField.text}", false, 2);
				}
				
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class PrefFloat : TextInputEntry
		{
			public float EnteredValue => float.Parse(textField.text);

			public override void ApplyValueToPref()
			{
				try
				{
					if (textField.text.Trim() != "")
					{
						_prefModel.BoxedValue = float.Parse(textField.text.Trim());
					}
				}
				catch (Exception ex)
				{
					Log($"{ex.Message} {textField.text}", false, 2);
				}
				
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class PrefDouble : TextInputEntry
		{
			public double EnteredValue => double.Parse(textField.text);
			/// <inheritdoc/>

			public override void ApplyValueToPref()
			{
				try
				{
					if (textField.text.Trim() != "")
					{
						_prefModel.BoxedValue = double.Parse(textField.text.Trim());
					}
				}
				catch (Exception ex)
				{
					Log($"{ex.Message} {textField.text}", false, 2);
				}
				
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class PrefBool : MelonEntry
		{
			protected Toggle toggle => this.gameObject.transform.Find("Data/Toggle").gameObject.GetComponent<Toggle>();
			protected UIFModel.ModelMelonEntry _prefModel => (UIFModel.ModelMelonEntry)EntryModel;
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

			public void OnValueChanged(bool newValue )
			{
				ApplyValueToPref();
			}
			/// <inheritdoc/>
			public override void ApplyValueToPref()
			{
				try
				{
					_prefModel.BoxedValue = EnteredValue;
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
			protected UIFModel.ModelMelonEntry _prefModel => (UIFModel.ModelMelonEntry)EntryModel;
			public System.Collections.Generic.List<int> _indexToValueMap = new();
			public TMP_Dropdown dropdown;

			public Type prefEnum;
			/// <inheritdoc/>
			public override void ModelSet()
			{
				dropdown = this.gameObject.transform.Find("Data/Dropdown").GetComponent<TMP_Dropdown>();
				prefEnum = _prefModel.PrefEntry.BoxedValue.GetType();

				//Get a list of display name attributes or the enum name if not available
				Il2CppSystem.Collections.Generic.List<string> enumNames = new(); ;// = Helpers.GetDisplayName(prefEnum);
				foreach(var value in Enum.GetValues(prefEnum))
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

			public void OnValueChanged(int index )
			{
				ApplyValueToPref();
			}
			/// <inheritdoc/>
			public override void ApplyValueToPref()
			{
				_prefModel.PrefEntry.BoxedValue = Enum.ToObject(prefEnum, _indexToValueMap[dropdown.value]);
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
				try
				{
					//ButtonGo.GetComponent<Button>().onClick.RemoveAllListeners();
				}
				catch (Exception ex)
				{
					Debug.Warning($"Can't find ButtonGo in OnDestroy {ex.Message}");
				}
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