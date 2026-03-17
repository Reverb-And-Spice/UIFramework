using AssetsTools.NET.Extra;
using Il2CppInterop.Runtime;
//using Il2CppSystem.Collections.Generic;
using Il2CppTMPro;
using MelonLoader;
using MelonLoader.Logging;
using MonoMod.ModInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//using static UIFramework.UIFController;
using static Unity.Collections.AllocatorManager;
using static UIFramework.Debug;
using System.Globalization;
namespace UIFramework
{
	/// <summary>
	/// Custom components that will serve as views in this MVP/MVC-ish pattern
	/// </summary>
	/// <remarks>I may have gone a little crazy with inheritance</remarks>
	public class UIFController
	{
		//[RegisterTypeInIl2Cpp]
		public interface IModelListable
		{
			/// <summary>
			/// Reference to the model the controller works from.  
			/// </summary>
			public UIFModel.IModelable Model { get; set; }
		}
		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class WindowController : MonoBehaviour
		{
			protected UIFModel.RootModel _model;
			public UIFModel.ModelBase Model { get { return _model; } }

			public GameObject MainCanvas;
			public Sidebar ModRegistryPanel;
			public TopBar CatRegistryPanel;
			public PrefList PrefRegistryPanel;
			public Button MainActionButton;

			

			void Awake()
			{
				Log("WindowController Awake", true, 1);

				//_model = new UIFModel.RootModel();
			}

			public virtual void SetModel(UIFModel.RootModel model)
			{
				MainCanvas = this.gameObject;
				ModRegistryPanel = MainCanvas.transform.Find("Root/ModRegistry/Viewport/ModRegCont").gameObject.GetComponent<Sidebar>();
				CatRegistryPanel = MainCanvas.transform.Find("Root/CatRegistry/Viewport/CatRegCont").gameObject.GetComponent<TopBar>();
				PrefRegistryPanel = MainCanvas.transform.Find("Root/PrefRegistry/Viewport/PrefRegCont").gameObject.GetComponent<PrefList>();
				MainActionButton = MainCanvas.transform.Find("Root/SaveActionButton").gameObject.GetComponent<Button>();

				MainActionButton.onClick.AddListener((UnityAction)SaveButtonClick);
				_model = model;
				BuildModList();
				Deb("Main Window Full Path: " + Helpers.HierarchyUtility.GetGameObjectPath(this.gameObject));
			}
			public virtual void BuildModList()
			{
				ModRegistryPanel.ContainerReset();
				
				CatRegistryPanel.GetComponent<TopBar>().ContainerReset();
				PrefRegistryPanel.GetComponent<PrefList>().ContainerReset();

				ModRegistryPanel.SetModel(_model);


			}
			/// <summary>
			/// Gets called when the save button gets clicked. 
			/// </summary>
			/// <remarks>
			/// Generally Melonpreferences are saved by category. 
			/// But this iterates through the child controllers and call their SaveAction() method.
			/// This allows for custom behavior before the actual category gets saved
			/// </remarks>
			public virtual void SaveButtonClick()
			{
				
				for (int i = PrefRegistryPanel.gameObject.transform.childCount - 1; i >= 0; i--)
				{
					PreferenceEntry entry = PrefRegistryPanel.gameObject.transform.GetChild(i).gameObject.GetComponent<PreferenceEntry>();
					entry.SaveAction();

				}
				PrefRegistryPanel.SaveAction();
				PrefRegistryPanel.Infanticide();
				PrefRegistryPanel.BuildFromModelList();
			}

		}


		#region List Areas
		/// <summary>
		/// Areas where UI elements are shown to the user. 
		/// 1. Mod list Sidebar 
		/// 2. Category tab top bar
		/// 3. Entries Content area
		/// </summary>
		public class ListArea : MonoBehaviour
		{
			protected UIFModel.ModelBase _model;
			public UIFModel.IModelable Model => _model;
			public virtual void ContainerReset()
			{
				_model = null;
				Infanticide();
			}


			/// <summary>
			/// 
			/// </summary>
			public void Infanticide()
			{
				for (int i = this.transform.childCount - 1; i >= 0; i--)
				{
					GameObject.Destroy(this.transform.GetChild(i).gameObject);
				}
			}
			/// <summary>
			/// Sets the underlying data model for the current instance.
			/// </summary>
			/// <remarks>
			/// Calling this method updates the internal state to reflect the provided model. Subsequent
			/// operations may depend on the newly set model.
			/// </remarks>
			/// <param name="model">The model to associate with this instance. Cannot be null.</param>
			public virtual void SetModel(UIFModel.ModelBase model)
			{
				_model = model;
				BuildFromModelList();
			}

			///	<summary>
			/// Clears the contents and recreates them from the submodels list in Model
			/// </summary>
			public void BuildFromModelList()
			{
				Infanticide();
				foreach (UIFModel.ModelBase model in Model.SubModels)
				{
					GameObject uiElement = model.GetNewUIInstance();//GameObject.Instantiate(GetUIPrefabForModel(model), this.gameObject.transform);
					uiElement.SetActive(true);
					uiElement.transform.SetParent(this.gameObject.transform, false);



					IModelListable ViewController; //ut= uiElement.GetComponent<UIFController.IModelListable>();

					//Retrieve the appropriate game object controller component depending on the model type
					
					switch (model)
					{
						case UIFModel.ModelMod modModel:
						case UIFModel.ModelCategory catModel:
							ViewController = uiElement.GetComponent<UIFController.TabButtonController>();
							break;
						case UIFModel.ModelEntry entryModel:
						case UIFModel.ButtonEntry buttonEntry:
							ViewController = uiElement.GetComponent<UIFController.PreferenceEntry>();
							break;
						default:
							Warning($"No view found for model type {model.GetType()}");
							continue;
					}
					
					if (ViewController != null)
					{
						ViewController.Model = model;
					}


				}
			}

			/// <summary>
			/// Is called when Save ButtonGo is clicked. Override to create custom behaviour 
			/// </summary>
			public virtual void SaveAction() { }

		}


		/// <summary>
		///
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class Sidebar : ListArea
		{

		}

		/// <summary>
		///
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class TopBar : ListArea
		{


		}
		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]

		public class PrefList : ListArea
		{
			public UIFModel.ModelCategory SelectedCategory => Model as UIFModel.ModelCategory;
			/// <summary>
			/// When the save button is clicked, the selected category save action will be called. The model is now in charge of what that means
			/// </summary>
			public override void SaveAction()
			{
				SelectedCategory.SaveAction();
			}


		}
		#endregion

		#region TabButtons
		//protected override GameObject UIPrefab { get { return Prefabs.TextPrefab; } }
		public class TabButtonController : MonoBehaviour, IModelListable
		{
			protected UIFModel.ModelBase _model;
			public virtual UIFModel.IModelable Model
			{
				get { return _model; }
				set
				{
					_model = (UIFModel.ModelBase) value;
					Label = _model.Name;

				}
			}

			public string Label { set { this.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }
			public ColorARGB TabColor { get; set; }
			public ListArea TargetContainer;
			public void OnSelect()
			{
				SelectTargetPanel();
			}
			/// <summary>
			/// Selects the panel that should be assigned a model next. The target then parents the models' game objects to itself from the list
			/// </summary>
			public virtual void SelectTargetPanel()
			{
				WindowController ParentWindow = gameObject.transform.parent.parent.parent.parent.parent.gameObject.GetComponent<WindowController>();
				switch (this)
				{
					case Mod mod:
						TargetContainer = ParentWindow.CatRegistryPanel;//Prefabs.CatDisplayList.GetComponent<TopBar>();
						ParentWindow.PrefRegistryPanel.ContainerReset();
						break;
					case Category cat:
						TargetContainer = ParentWindow.PrefRegistryPanel;
						break;
				}
				TargetContainer.SetModel(_model);
			}


			void Start()
			{
				this.gameObject.GetComponent<Button>().onClick.AddListener((UnityAction)OnSelect);
			}
			void OnDestroy()
			{
				this.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
			}


		}

		
		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class Mod : TabButtonController, IModelListable
		{

			public string ModName { get; set; }


		}
		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class Category : TabButtonController, IModelListable
		{
		}
		#endregion

		#region Entries
		/*/// <summary>
		/// This was going to be the basis for all entries and what advanced users would have to implement
		/// Unfortunately can't be successfully retrieved with GetComponent in the current setup
		/// Will move to making PreferenceEntry the required base class
		/// </summary>
		public interface ISettingEntry : IModelListable
		{
			public string DescriptionText { set; }
			public string IdentifierText { set; }
			//override this function to create your own validation check
			public bool ValidationCheck();
			
		}*/
		/// <summary>
		/// Inherit this class to create your own custom entry controllers for your own input controls.
		/// </summary>
		public class PreferenceEntry : MonoBehaviour, IModelListable
		{
			protected UIFModel.IEntry _model;
			public virtual UIFModel.IModelable Model
			{
				get { return (UIFModel.IModelable) _model; }
				set
				{

					_model = (UIFModel.IEntry)value;
					DescriptionText = _model.Description;
					IdentifierText = _model.Name;
					ModelSet();
				}
			}
			
			/// <summary>
			/// Runs when the model property has been set. 
			/// </summary>
			public virtual void ModelSet(){}
			/// <summary>
			/// Sets the description text
			/// </summary>
			public virtual string DescriptionText { set { this.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }
			/// <summary>
			/// Sets the identifier text
			/// </summary>
			public virtual string IdentifierText { set { this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }

			public virtual bool ValidationCheck()
			{
				return true;
			}
			/// <summary>
			/// This is called when the Save button is pressed. Override to create custom behaviour.
			/// </summary>
			/// <remarks>Generally MelonPreferences are saved from the category, not the indivial entries.</remarks>
			public virtual void SaveAction()
			{

			}

			
		}

		
		/// <summary>
		/// Base controller for text fields 
		/// </summary>
		public abstract class TextInputEntry : PreferenceEntry
		{
			/// <summary>
			/// Returns the textfield
			/// </summary>
			public TMP_InputField textField => this.gameObject.transform.Find("Panel/InputField (TMP)").gameObject.GetComponent<TMP_InputField>();
			/// <summary>
			/// Sets the placeholder text in the textField
			/// </summary>
			public string PlaceHolderText { set { this.gameObject.transform.Find("Panel/InputField (TMP)/Text Area/Placeholder").gameObject.GetComponent<TextMeshProUGUI>().text = value; } }
			/// <inheritdoc/>
			public override void ModelSet()
			{
				PlaceHolderText = _model.BoxedValue.ToString();
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
						_model.BoxedValue = Value;
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
						_model.BoxedValue = int.Parse(textField.text.Trim());
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
						_model.BoxedValue = float.Parse(textField.text.Trim());
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
						_model.BoxedValue = double.Parse(textField.text.Trim());
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
		public class PrefBool : PreferenceEntry
		{
			public bool Value => this.gameObject.transform.Find("Panel/Toggle").gameObject.GetComponent<Toggle>().isOn;
			/// <inheritdoc/>
			public override void ModelSet()
			{
				base.ModelSet();
				this.gameObject.transform.Find("Panel/Toggle").gameObject.GetComponent<Toggle>().isOn = (bool)_model.BoxedValue;

			}
			/// <inheritdoc/>
			public override void SaveAction()
			{
				try
				{
					_model.BoxedValue = Value;
				}
				catch (Exception ex)
				{
					Log(ex.Message, false, 2);
				}

			}

		}
		#endregion
		[RegisterTypeInIl2Cpp]
		public class ButtonEntry : PreferenceEntry
		{
			public GameObject ButtonGo;
			public override void ModelSet()
			{
				base.ModelSet();
				ButtonGo = this.gameObject.transform.Find("Panel/Panel/Button").gameObject;
				ButtonGo.GetComponent<Button>().onClick.AddListener((UnityAction)((UIFModel.ButtonEntry)_model).OnClickRelay);


			}

			void OnDestroy()
			{
				ButtonGo.GetComponent<Button>().onClick.RemoveAllListeners();
			}
		}
		#region no support

		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class PrefMulti : PreferenceEntry
		{

		}
		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class PrefSlider : PreferenceEntry
		{

		}
		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class PrefDropDown : PreferenceEntry
		{

		}
		#endregion
	}
}
