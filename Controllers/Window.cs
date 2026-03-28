using AssetsTools.NET.Extra;
using Il2CppInterop.Runtime;
//using Il2CppSystem.Collections.Generic;
using Il2CppTMPro;
using MelonLoader;
using MelonLoader.Logging;
using MonoMod.ModInterop;
using System;
using System.Collections;
using System.Globalization;
//using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UIFramework.Debug;
//using static UI.UIFController;
using static Unity.Collections.AllocatorManager;
namespace UIFramework
{
	/// <summary>
	/// Custom components that will serve as views in this MVP/MVC-ish pattern
	/// </summary>
	/// <remarks>I may have gone a little crazy with inheritance</remarks>
	public partial class UIFController
	{

		/// <summary>
		/// Controllers for models that can be submodels of other models. 
		/// Preference Entries
		/// Mod Buttons
		/// Category Buttons
		/// </summary>
		public interface IChildable
		{
			/// <summary>
			/// Reference to the model the controller works from.  
			/// </summary>
			public UIFModel.IModelable Model { get; set; }
		}
		public abstract class SubModelController : MonoBehaviour
		{
			protected UIFModel.IModelable _internalModel;
			public virtual UIFModel.IModelable Model
			{
				get
				{
					return _internalModel;
				}
				set
				{
					_internalModel = value;
					ModelSet();
				}
			}

			public WindowController _rootWindow;

			public WindowController FindRootWindow()
			{
				WindowController foundRoot = null; ;
				Transform ancestor = this.gameObject.transform.parent;
				while (ancestor != null)
				{
					if (ancestor.name.Contains("MainWindow"))
					{
						return ancestor.GetComponent<WindowController>();
					}
					ancestor = ancestor.parent;
				}

				return foundRoot;
			}
			void OnTransformParentChanged()
			{
				_rootWindow = FindRootWindow();
			}
			public virtual void ModelSet() { }
		}

		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class WindowController : MonoBehaviour
		{
			protected UIFModel.RootModel _model;
			public UIFModel.IModelable Model { get { return _model; } }

			public GameObject MainCanvas;
			public Sidebar ModRegistryPanel;
			public TopBar CatRegistryPanel;
			public PrefList PrefRegistryPanel;
			public Button MainActionButton;
			public Button MinimizeButton;


			void Awake()
			{
				Log("WindowController Awake", true, 1);

				//EntryModel = new UIFModel.RootModel();
			}

			public virtual void SetModel(UIFModel.RootModel model)
			{
				MainCanvas = this.gameObject;
				ModRegistryPanel = MainCanvas.transform.Find("Root/Body/ModRegistry/Viewport/ModRegCont").gameObject.GetComponent<Sidebar>();
				CatRegistryPanel = MainCanvas.transform.Find("Root/Body/CatRegistry/Viewport/CatRegCont").gameObject.GetComponent<TopBar>();
				PrefRegistryPanel = MainCanvas.transform.Find("Root/Body/PrefRegistry/Viewport/PrefRegCont").gameObject.GetComponent<PrefList>();
				MainActionButton = MainCanvas.transform.Find("Root/Body/SaveActionButton").gameObject.GetComponent<Button>();
				MinimizeButton = MainCanvas.transform.Find("Root/Ribbon/Minimize").gameObject.GetComponent<Button>();


				MainActionButton.onClick.AddListener((UnityAction)SaveButtonClick);

				MinimizeButton.onClick.AddListener((UnityAction)(() => MainCanvas.SetActive(false)));

				_model = model;
				BuildModList();
				Deb("Main Window Full Path: " + Helpers.HierarchyUtility.GetGameObjectPath(this.gameObject));
			}
			
			/// <summary>
			/// Resets the containers and build the modlist
			/// </summary>
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
					//Error handling per child to prevent breaking the whole loop.
					try
					{
						Entry entry = PrefRegistryPanel.gameObject.transform.GetChild(i).gameObject.GetComponent<Entry>();
						entry.SaveAction();
						entry.EntryModel.SaveAction();
					}
					catch (Exception ex)
					{
						Debug.Warning($"Error in entry saving loop {PrefRegistryPanel.gameObject.transform.childCount - i}:");
						Debug.Error(ex.Message);
					}
				}
				PrefRegistryPanel.SaveAction();
				PrefRegistryPanel.Infanticide();
				PrefRegistryPanel.BuildFromModelList();
			}

			public virtual void DiscardButtonClick()
			{
				PrefRegistryPanel.DiscardAction();
			}


			void Update()
			{
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					// Deselect the currently selected UI element
					EventSystem.current.SetSelectedGameObject(null);
				}
			}

		}
	}

}
