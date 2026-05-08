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
using UIFramework.Models;
using Il2CppSystem.Threading.Tasks;
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
		public IModelable Model { get; set; }
	}
	public abstract class SubModelAdapter : MonoBehaviour
	{
		protected IModelable _internalModel;
		public virtual IModelable Model
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

		protected WindowCoordinator _rootWindow;

		protected WindowCoordinator FindRootWindow()
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

		protected RootModel _model;
		public IModelable Model { get { return _model; } }

		protected GameObject MainCanvas;
		protected ModListAdapter ModRegistryPanel;
		protected CategoryListAdapter CatRegistryPanel;
		protected PrefListAdapter PrefRegistryPanel;
		private Button MainActionButton;
		private Button DiscardActionButton;
		private Button MinimizeButton;
		private TextMeshProUGUI WindowTitle;

		protected TextMeshProUGUI TitleButtonText;
		internal DragHandle DragHandle;

		public ModModelBase SelectedMod => _selectedMod;
		private ModModelBase _selectedMod = null;

		public CategoryModelBase SelectedCategory => _selectedCategory;
		private CategoryModelBase _selectedCategory = null;

		public Dictionary<ModModelBase, CategoryModelBase> LastCategorySelected = new();


		internal void SetSelectedMod(ModModelBase mod)
		{
			_selectedMod = mod;
			TitleButtonText.text = $"{mod.DisplayName}\n{mod.Instance.Info.Version}";

			CatRegistryPanel.SetModel(mod);

			CategoryModelBase lastSelected = null;
			if (LastCategorySelected.ContainsKey(mod as ModModelBase))
				lastSelected = LastCategorySelected[mod as ModModelBase];


			PrefRegistryPanel.SetModel(lastSelected ?? (CategoryModelBase)mod.SubModels[0]);
			RequestRefresh();

		}
		public void SetSelectedCategory(CategoryModelBase cat)
		{
			_selectedCategory = cat;
			PrefRegistryPanel.SetModel(cat);
			LastCategorySelected[_selectedMod] = cat;
			RequestRefresh();
		}

		public void SelectInSideBar(IHoldSubmodels model)
		{
			ModRegistryPanel.SelectTab(model as IHoldSubmodels);
		}
		public void SelectInTopBar(IHoldSubmodels model)
		{
			CatRegistryPanel.SelectTab(model as IHoldSubmodels);
		}

		protected void RequestRefresh()
		{
			_refreshPending = true;
		}

		public virtual void RequestRefresh(ModModelBase model)
		{
			ModModelBase callingMod = model;
			Log($"Refresh Requested by {model.DisplayName}", true, 1);


			if (callingMod != _selectedMod)
			{
				Log("Denied. Calling mod is not selected mod", true, 1);
				return;
			}

			if (callingMod is null)
			{
				Log("Denied Refresh Request. Reason: Calling mod is null", true, 1);
				return;
			}

			RequestRefresh();
			Log("Refresh Request approved", true, 1);
		}
		public virtual void SetModel(RootModel model)
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
		protected virtual void BuildModList()
		{
			ModRegistryPanel.ContainerReset();

			CatRegistryPanel.GetComponent<CategoryListAdapter>().ContainerReset();
			PrefRegistryPanel.GetComponent<PrefListAdapter>().ContainerReset();

			ModRegistryPanel.SetModel(_model);

			RequestRefresh();
		}
		/// <summary>
		/// Gets called when the save button gets clicked. 
		/// </summary>
		/// <remarks>
		/// Generally Melonpreferences are saved by category. 
		/// But this iterates through the child controllers and call their SaveAction() method.
		/// This allows for custom behavior before the actual category gets saved
		/// </remarks>
		protected virtual void SaveButtonClick()
		{

			CatRegistryPanel.Model?.SaveAction();
			RequestRefresh();
		}

		protected virtual void DiscardButtonClick()
		{
			CatRegistryPanel.Model?.DiscardAction();
			PrefRegistryPanel.DiscardAction();
			RequestRefresh();
		}
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				// Deselect the currently selected UI element
				EventSystem.current.SetSelectedGameObject(null);
			}
			CheckRefresh();
		}

		private bool _refreshPending = false;

		private void CheckRefresh()
		{
			if (!_refreshPending)
				return;
			

			ModRegistryPanel.BuildFromModelList();
			CatRegistryPanel.BuildFromModelList();
			PrefRegistryPanel.BuildFromModelList();
			_refreshPending = false;


		}

	}


}
