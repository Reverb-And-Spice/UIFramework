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
namespace UIFramework.Adapters
{

	/// <summary>
	/// Controllers for models that can be submodels of other models. 
	/// Preference Entries
	/// ModButtonView Buttons
	/// CategoryTabView Buttons
	/// </summary>
	public interface IChildable
	{
		/// <summary>
		/// Reference to the model the controller works from.  
		/// </summary>
		public UIFModel.IModelable Model { get; set; }
	}
	public abstract class SubModelAdapter : MonoBehaviour
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

		public WindowCoordinator _rootWindow;

		public WindowCoordinator FindRootWindow()
		{
			WindowCoordinator foundRoot = null; ;
			Transform ancestor = this.gameObject.transform.parent;
			while (ancestor != null)
			{
				if (ancestor.name.Contains("MainWindow"))
				{
					return ancestor.GetComponent<WindowCoordinator>();
				}
				ancestor = ancestor.parent;
			}

			return foundRoot;
		}
		void OnTransformParentChanged()
		{
			_rootWindow = FindRootWindow();
		}
		void Start()
		{
			_rootWindow = FindRootWindow();
		}
		public virtual void ModelSet() { }
	}

	/// <summary>
	/// 
	/// </summary>
	[RegisterTypeInIl2Cpp]
	public class WindowCoordinator : MonoBehaviour
	{
		void Awake()
		{
			Log("WindowCoordinator Awake", true, 1);
		}

		public Color defaultTabColor = new Color(0.22f, 0.22f, 0.22f, 1f);
		public Color openTabColor = new Color(0.24f, 0.17f, 0.42f, 1f);

		protected UIFModel.RootModel _model;
		public UIFModel.IModelable Model { get { return _model; } }

		public GameObject MainCanvas;
		public ModListAdapter ModRegistryPanel;
		public CategoryListAdapter CatRegistryPanel;
		public PrefListAdapter PrefRegistryPanel;
		public Button MainActionButton;
		public Button DiscardActionButton;
		public Button MinimizeButton;
		public TextMeshProUGUI WindowTitle;

		public TextMeshProUGUI TitleButtonText;
		public DragHandle DragHandle;

		public UIFModel.ModelModItem SelectedMod => _selectedMod;
		private UIFModel.ModelModItem _selectedMod = null;

		public UIFModel.ModelCategoryItem SelectedCategory => _selectedCategory;
		private UIFModel.ModelCategoryItem _selectedCategory = null;

		public Dictionary<UIFModel.ModelModItem, UIFModel.ModelCategoryItem> LastCategorySelected = new();


		public void SetSelectedMod(UIFModel.ModelModItem mod)
		{
			_selectedMod = mod;
			TitleButtonText.text = $"{mod.DisplayName}\n{mod.Instance.Info.Version}";

			CatRegistryPanel.SetModel(mod);

			UIFModel.ModelCategoryItem lastSelected = null;
			if (LastCategorySelected.ContainsKey(mod as UIFModel.ModelModItem))
				lastSelected = LastCategorySelected[mod as UIFModel.ModelModItem];


			PrefRegistryPanel.SetModel(lastSelected ?? (UIFModel.ModelCategoryItem)mod.SubModels[0]);

		}
		public void SetSelectedCategory(UIFModel.ModelCategoryItem cat)
		{
			_selectedCategory = cat;
			PrefRegistryPanel.SetModel(cat);
			LastCategorySelected[_selectedMod] = cat;

		}


		public virtual void SetModel(UIFModel.RootModel model)
		{
			_model = model;

			MainCanvas = this.gameObject;
			ModRegistryPanel = MainCanvas.transform.Find("Body/ModRegistry/Viewport/ModRegCont").gameObject.GetComponent<ModListAdapter>();
			CatRegistryPanel = MainCanvas.transform.Find("Body/CatRegistry/Viewport/CatRegCont").gameObject.GetComponent<CategoryListAdapter>();
			PrefRegistryPanel = MainCanvas.transform.Find("Body/PrefRegistry/Viewport/PrefRegCont").gameObject.GetComponent<PrefListAdapter>();

			MainActionButton = MainCanvas.transform.Find("Body/SaveActionButton").gameObject.GetComponent<Button>();
			DiscardActionButton = MainCanvas.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "DiscardActionButton")?.gameObject.GetComponent<Button>();
			MinimizeButton = MainCanvas.transform.Find("Ribbon/Minimize").gameObject.GetComponent<Button>();

			WindowTitle = MainCanvas.transform.Find("Ribbon/WindowTitle").gameObject.GetComponent<TextMeshProUGUI>();
			TitleButtonText = MainCanvas.transform.Find("Body/TitleButton/Text").gameObject.GetComponent<TextMeshProUGUI>();

			MainActionButton.onClick.AddListener((UnityAction)SaveButtonClick);

			MinimizeButton.onClick.AddListener((UnityAction)(() => MainCanvas.SetActive(false)));
			DragHandle = MainCanvas.transform.Find("Ribbon/DragHandle").gameObject.GetComponent<DragHandle>();

			DiscardActionButton.onClick.AddListener((UnityAction)DiscardButtonClick);
			DiscardActionButton.gameObject.SetActive(true);


			WindowTitle.text = $"{Core.Instance.Info.Name} v{Core.Instance.Info.Version}";
			TitleButtonText.text = $"{Core.Instance.Info.Name}\nv{Core.Instance.Info.Version}";


			BuildModList();
		}

		/// <summary>
		/// Resets the containers and build the modlist
		/// </summary>
		public virtual void BuildModList()
		{
			ModRegistryPanel.ContainerReset();

			CatRegistryPanel.GetComponent<CategoryListAdapter>().ContainerReset();
			PrefRegistryPanel.GetComponent<PrefListAdapter>().ContainerReset();

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

			/*				for (int i = PrefRegistryPanel.gameObject.transform.childCount - 1; i >= 0; i--)
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
							}*/

			CatRegistryPanel.Model?.SaveAction();
			//PrefRegistryPanel.SaveAction();
			PrefRegistryPanel.Infanticide();
			PrefRegistryPanel.BuildFromModelList();
		}

		public virtual void DiscardButtonClick()
		{
			CatRegistryPanel.Model?.DiscardAction();
			PrefRegistryPanel.DiscardAction();
			PrefRegistryPanel.Infanticide();
			PrefRegistryPanel.BuildFromModelList();
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
