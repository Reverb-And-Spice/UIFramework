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

		public abstract class Entry : MonoBehaviour, IChildable
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

			/// <summary>
			/// Runs when the model property has been set. 
			/// </summary>
			public virtual void ModelSet() 
			{
				EntryModel.OnUICreated?.Invoke(this);
			}

			public UIFModel.IEntry EntryModel;
			public virtual UIFModel.IModelable Model
			{
				get { return (UIFModel.IModelable)EntryModel; }
				set
				{

					EntryModel = (UIFModel.IEntry)value;
					DescriptionText = EntryModel.Description;
					DisplayName = EntryModel.DisplayName;

					ModelSet();
				}
			}
			/// <summary>
			/// This is called when the Save button is pressed. Override to create custom behaviour.
			/// </summary>
			/// <remarks>Generally MelonPreferences are saved from the category, not the indivial entries.</remarks>
			public virtual void SaveAction()
			{
				EntryModel.SaveAction();
			}
		}
		/// <summary>
		/// Inherit this class to create your own custom entry controllers for your own input controls.
		/// </summary>
		public abstract class MelonEntry : Entry
		{
			
			

			/// <inheritdoc/>
			public override void ModelSet() { }

			/// <inheritdoc/>
			public virtual bool ValidationCheck()
			{
				return true;
			}


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
				PlaceHolderText = _prefModel.BoxedValue.ToString();
				base.ModelSet();
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
			public virtual string Value => textField.text;
			/// <inheritdoc/>
			public override void SaveAction()
			{
				try
				{
					if (Value.Trim() != "")
					{
						_prefModel.BoxedValue = Value;
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
			public int Value => int.Parse(textField.text);
			/// <inheritdoc/>
			public override void SaveAction()
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
			public float Value => float.Parse(textField.text);

			public override void SaveAction()
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
			public double Value => double.Parse(textField.text);
			/// <inheritdoc/>
			public override void SaveAction()
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
			protected UIFModel.ModelMelonEntry _prefModel => (UIFModel.ModelMelonEntry)EntryModel;
			public bool Value => this.gameObject.transform.Find("Data/Toggle").gameObject.GetComponent<Toggle>().isOn;
			/// <inheritdoc/>
			public override void ModelSet()
			{
				this.gameObject.transform.Find("Data/Toggle").gameObject.GetComponent<Toggle>().isOn = (bool)_prefModel.BoxedValue;
				base.ModelSet();

			}
			/// <inheritdoc/>
			public override void SaveAction()
			{
				try
				{
					_prefModel.BoxedValue = Value;
				}
				catch (Exception ex)
				{
					Log(ex.Message, false, 2);
				}

			}

		}

		[RegisterTypeInIl2Cpp]
		public class ButtonEntry : Entry
		{

			public GameObject ButtonGo;
			public virtual UIFModel.IModelable Model
			{
				get { return (UIFModel.IModelable)EntryModel; }
				set
				{

					EntryModel = (UIFModel.IEntry)value;
					DescriptionText = EntryModel.Description;
					DisplayName = EntryModel.Identifier;
					ModelSet();
				}
			}
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
		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class PrefDropDown : MelonEntry
		{
			protected UIFModel.ModelMelonEntry _prefModel => (UIFModel.ModelMelonEntry)EntryModel;
			public TMP_Dropdown dropdown;

			public Type prefEnum;
			public override void ModelSet()
			{
				base.ModelSet();
				dropdown = this.gameObject.transform.Find("Data/Dropdown").GetComponent<TMP_Dropdown>();
				prefEnum = _prefModel.PrefEntry.BoxedValue.GetType();

				string[] enumValues = Enum.GetNames(prefEnum);
				
				//Uhhh... Guess I have to do this? someone figure it out for me later
				Il2CppSystem.Collections.Generic.List<string> valueList = new();
				foreach (string value in enumValues)
				{
					valueList.Add(value);
				}


				dropdown.ClearOptions();
				dropdown.AddOptions(valueList);

				dropdown.value = (int)_prefModel.BoxedValue;

				base.ModelSet();
			}

			public override void SaveAction()
			{
				_prefModel.PrefEntry.BoxedValue = Enum.Parse(prefEnum, dropdown.value.ToString());
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
	}
		#endregion
}