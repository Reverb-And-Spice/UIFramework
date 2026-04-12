using AssetsTools.NET.Extra;
using Il2CppInterop.Runtime;
//using Il2CppSystem.Collections.Generic;
using Il2CppTMPro;
using MelonLoader;
using MelonLoader.Logging;
using MonoMod.ModInterop;
using System;
//using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//using static UI.UIFController;
using static Unity.Collections.AllocatorManager;
using static UIFramework.Debug;
using System.Globalization;
using Il2CppSystem.Threading.Tasks;
namespace UIFramework
{
	/// <summary>
	/// Custom components that will serve as views in this MVP/MVC-ish pattern
	/// </summary>
	/// <remarks>I may have gone a little crazy with inheritance</remarks>
	public partial class UIFController
	{
		/// <summary>
		/// Areas where UI elements are shown to the user. 
		/// 1. Mod list Sidebar 
		/// 2. Category tab top bar
		/// 3. Entries Content area
		/// </summary>
		public abstract class ListArea : SubModelController
		{
			protected UIFModel.IHoldSubmodels _model => (UIFModel.IHoldSubmodels)_internalModel;
			
			public virtual void ContainerReset()
			{
				Model = null;
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
			public virtual void SetModel(UIFModel.IHoldSubmodels model)
			{
				if (model == null)
					return;
				ContainerReset();
				Model = model;
				BuildFromModelList();
			}

			///	<summary>
			/// Clears the contents and recreates them from the submodels list in Model
			/// </summary>
			public void BuildFromModelList()
			{
				if (Model == null) return;
				Infanticide();
				foreach (UIFModel.IModelable model in _model.SubModels)
				{
					GameObject uiElement = model.GetNewUIInstance();//GameObject.Instantiate(GetUIPrefabForModel(model), this.gameObject.transform);
					uiElement.SetActive(true);
					uiElement.transform.SetParent(this.gameObject.transform,false);



					IChildable ViewController;

					//Retrieve the appropriate game object controller component depending on the model type. 
					//Switch statement could be unnecessary if interface was replaced with an abstract class

					switch (model)
					{
						case UIFModel.IEntry entryModel:
							ViewController = uiElement.GetComponent<UIFController.Entry>();
							_rootWindow.CatRegistryPanel.SelectTab(Model as UIFModel.IHoldSubmodels);
							break;
						case UIFModel.IHoldSubmodels tabModel:
							ViewController = uiElement.GetComponent<UIFController.TabButtonController>();
							try
							{
								_rootWindow.ModRegistryPanel.SelectTab(Model as UIFModel.IHoldSubmodels);
							}catch (Exception ex)
							{
								Debug.Log(ex.Message);
							}
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
			/// 
			/// </summary>
			/// <param name="buttonModel"></param>
			public void SelectTab(UIFModel.IHoldSubmodels buttonModel)
			{
				for(int i = 0; i < transform.childCount; i++)
				{
					TabButtonController tabButton = transform.GetChild(i).GetComponent<TabButtonController>();
					if(tabButton.Model == buttonModel)
					{
						tabButton.GetComponent<Image>().color = _rootWindow.openTabColor;
					}
					else
					{
						tabButton.GetComponent<Image>().color = _rootWindow.defaultTabColor;
					}
				}
			}
			public virtual void DiscardAction() { }

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
		/// Main body of the UI. Lists individual preferences
		/// </summary>
		[RegisterTypeInIl2Cpp]

		public class PrefList : ListArea
		{
			public UIFModel.ModelCategoryItem SelectedCategory => Model as UIFModel.ModelCategoryItem;
			/// <summary>
			/// When the save button is clicked, the selected category save action will be called. The model is now in charge of what that means
			/// </summary>
			public override void SaveAction()
			{
				SelectedCategory?.SaveAction();

				

			}
			/// <inheritdoc/>
			public override void DiscardAction()
			{
				SelectedCategory.DiscardAction();
				BuildFromModelList();
			}
			
		}
	}
}