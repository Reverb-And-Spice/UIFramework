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
//using static UIFramework.UIFController;
using static Unity.Collections.AllocatorManager;
using static UIFramework.Debug;
using System.Globalization;
namespace UIFramework
{

	public partial class UIFController
	{
		//protected override GameObject UIPrefab { get { return Prefabs.TextPrefab; } }
		public abstract class TabButtonController : MonoBehaviour, IChildable
		{
			protected UIFModel.IHoldSubmodels _model;
			public virtual UIFModel.IModelable Model
			{
				get { return _model; }
				set
				{
					_model = (UIFModel.IHoldSubmodels)value;
					Label = _model.DisplayName;

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
				WindowController ParentWindow = gameObject.transform.parent.parent.parent.parent.parent.parent.gameObject.GetComponent<WindowController>();
				if (_model.SubModels.Count > 0)
				{
					switch (_model.SubModels[0])
					{
						case UIFModel.ModelCategoryItem:
							TargetContainer = ParentWindow.CatRegistryPanel;//Prefabs.CatDisplayList.GetComponent<TopBar>();
							ParentWindow.PrefRegistryPanel.ContainerReset();
							break;
						case UIFModel.IEntry:
							TargetContainer = ParentWindow.PrefRegistryPanel;
							break;
						default:
							TargetContainer = ParentWindow.PrefRegistryPanel;
							break;
					}
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
		public class Mod : TabButtonController, IChildable
		{

			public string ModName { get; set; }


		}
		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		public class Category : TabButtonController, IChildable
		{
		}
	}
}