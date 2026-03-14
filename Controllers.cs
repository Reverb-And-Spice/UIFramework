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
			public UIFModel.ModelBase Model { get; set; }
		}
		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class WindowController : MonoBehaviour
		{
			public UIFModel.RootModel _model;
			public UIFModel.ModelBase Model { get { return _model; } }

			public GameObject MainCanvas;
			public Sidebar ModRegistryPanel;
			public TopBar CatRegistryPanel;
			public PrefList PrefRegistryPanel;




			void Awake()
			{
				Log("WindowController Awake", true, 1);

				//_model = new UIFModel.RootModel();
			}

			public void SetModel(UIFModel.RootModel model)
			{
				MainCanvas = this.gameObject;
				ModRegistryPanel = MainCanvas.transform.Find("Root/ModRegistry/Viewport/ModRegCont").gameObject.GetComponent<Sidebar>();
				CatRegistryPanel = MainCanvas.transform.Find("Root/CatRegistry/Viewport/CatRegCont").gameObject.GetComponent<TopBar>();
				PrefRegistryPanel = MainCanvas.transform.Find("Root/PrefRegistry/Viewport/PrefRegCont").gameObject.GetComponent<PrefList>();

				_model = model;
				BuildModList();
				Deb("Main Window Full Path: " + Helpers.HierarchyUtility.GetGameObjectPath(this.gameObject));
			}
			public void BuildModList()
			{
				ModRegistryPanel.ContainerReset();
				
				CatRegistryPanel.GetComponent<TopBar>().ContainerReset();
				PrefRegistryPanel.GetComponent<PrefList>().ContainerReset();

				ModRegistryPanel.SetModel(_model);


			}

		}

		/// <summary>
		/// Areas where UI elements are shown to the user. 
		/// 1. Mod list Sidebar 
		/// 2. Category tab top bar
		/// 3. Entries Content area
		/// </summary>
		public class ListArea : MonoBehaviour
		{
			protected UIFModel.ModelBase _model;
			public UIFModel.ModelBase Model => _model;
			public void ContainerReset()
			{
				Infanticide();
			}

			public void Infanticide()
			{
				for (int i = this.transform.childCount - 1; i >= 0; i--)
				{
					GameObject.Destroy(this.transform.GetChild(i).gameObject);
				}
			}

			public virtual void SetModel(UIFModel.ModelBase model)
			{
				_model = model;
				BuildFromModelList(model.SubModels);
			}

			///	<summary>
			/// Builds UI from a list of models
			/// </summary>
			public void BuildFromModelList(List<UIFModel.ModelBase> modelList)
			{
				ContainerReset();
				foreach (UIFModel.ModelBase model in modelList)
				{
					GameObject uiElement = model.GetNewEntryWidgetInstance();//GameObject.Instantiate(GetUIPrefabForModel(model), this.gameObject.transform);
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

		}
		//protected override GameObject UIPrefab { get { return Prefabs.TextPrefab; } }
		public class TabButtonController : MonoBehaviour, IModelListable
		{
			protected UIFModel.ModelBase _model;
			public virtual UIFModel.ModelBase Model
			{
				get { return _model; }
				set
				{
					_model = value;
					Label = _model.Name;

				}
			}

			public string Label { set { this.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }
			public ColorARGB TabColor { get; set; }
			public ListArea TargetContainer;
			public void OnSelect()
			{
				PopTarget();
			}
			/// <summary>
			/// Populates the target container with
			/// </summary>
			public virtual void PopTarget()
			{
				
				switch (this)
				{
					case Mod mod:
						TargetContainer = gameObject.transform.parent.parent.parent.parent.parent.gameObject.GetComponent<WindowController>().CatRegistryPanel;//Prefabs.CatDisplayList.GetComponent<TopBar>();
						break;
					case Category cat:
						TargetContainer = gameObject.transform.parent.parent.parent.parent.parent.gameObject.GetComponent<WindowController>().PrefRegistryPanel;
						break;
				}
				TargetContainer.SetModel(_model);
				Deb("TargetContainer Full Path: " + Helpers.HierarchyUtility.GetGameObjectPath(TargetContainer.gameObject));
			}


			void OnEnable()
			{
				Log("TabButtonController OnEnable", true, 1);
				this.gameObject.GetComponent<Button>().onClick.AddListener((UnityAction)OnSelect);
			}


		}

		
		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class Mod : TabButtonController, IModelListable
		{

			public string ModName { get; set; }


			/*public override void PopTarget()
			{
				TargetContainer = Prefabs.CatDisplayList.GetComponent<TopBar>();
				TargetContainer.BuildFromModelList(Model.SubModels);
			}*/



		}
		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class Category : TabButtonController, IModelListable
		{
			/*public override void PopTarget()
			{
				TargetContainer = Prefabs.PrefDisplayList.GetComponent<PrefList>();
				TargetContainer.BuildFromModelList(Model.SubModels);
			}*/

		}
		//[RegisterTypeInIl2Cpp]
		public interface ISettingEntry : IModelListable
		{
			public string DescriptionText { set; }
			public string IdentifierText { set; }
			//override this function to create your own validation check
			public bool ValidationCheck();
			
		}
		/// <summary>
		/// 
		/// </summary>
		public class PreferenceEntry : MonoBehaviour, ISettingEntry
		{
			protected UIFModel.ModelEntry _model;
			public UIFModel.ModelBase Model
			{
				get { return _model; }
				set
				{
					_model = (UIFModel.ModelEntry)value;
					DescriptionText = _model.Description;
					IdentifierText = _model.Identifier;
					ModelSet();
				}
			}
			
			/// <summary>
			/// Runs when the model property has been set. 
			/// </summary>
			public virtual void ModelSet(){}
			
			public InputType InputType { get; set; }
			public string DescriptionText { set { this.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }
			public string IdentifierText { set { this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }

			public virtual bool ValidationCheck()
			{
				return true;
			}


		}

		
		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public abstract class TextInputEntry : PreferenceEntry, ISettingEntry
		{
			public TextMeshProUGUI textField => this.gameObject.transform.Find("Panel/InputField (TMP)/Text Area/Text").gameObject.GetComponent<TextMeshProUGUI>();
			public string PlaceHolderText { set { this.gameObject.transform.Find("Panel/InputField (TMP)/Text Area/Placeholder").gameObject.GetComponent<TextMeshProUGUI>().text = value; } }

			public override void ModelSet()
			{
				PlaceHolderText = _model.PrefEntry.BoxedValue.ToString();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]

		public class PrefText : TextInputEntry
		{
			public virtual string Value => textField.text;
		}
		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class PrefInt : TextInputEntry
		{
			public int Value => int.Parse(textField.text);
		}
		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class PrefFloat : TextInputEntry
		{
			public float Value => float.Parse(textField.text);
		}
		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class PrefBool : PreferenceEntry
		{
			public bool value => this.gameObject.transform.Find("Panel/Toggle").gameObject.GetComponent<Toggle>().isOn;
		}
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
	}
}
