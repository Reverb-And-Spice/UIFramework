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
using static MelonLoader.MelonLogger;
using static UIFramework.UIFController;
using static Unity.Collections.AllocatorManager;
namespace UIFramework
{
	/// <summary>
	/// Custom components that will serve as views in this MVP/MVC-ish pattern
	/// </summary>
	/// <remarks>I may have gone a little crazy with inheritance</remarks>
	public class UIFController
	{
		internal interface IModelController
		{
			/// <summary>
			/// Reference to the model the controller works from.  
			/// </summary>
			public UIFModel.BaseModel Model { get; set; }
		}
		/// <summary>
		/// Areas where UI elements are shown to the user. 
		/// 1. Mod list Sidebar 
		/// 2. Category tab top bar
		/// 3. Entries Content area
		/// </summary>
		internal class ListArea : MonoBehaviour
		{
			private UIFModel.BaseModel _model;
			public UIFModel.BaseModel Model => _model;
			protected void ContainerReset()
			{
				Infanticide();
			}

			protected void Infanticide()
			{
				for (int i = this.transform.childCount - 1; i >= 0; i--)
				{
					GameObject.Destroy(this.transform.GetChild(i).gameObject);
				}
			}

			public virtual void SetModel(UIFModel.BaseModel model)
			{
				_model = model;
				BuildFromModelList(model.subModels);
			}
			//protected virtual GameObject UIPrefab { get; }
			protected virtual GameObject GetUIPrefabForModel(UIFModel.BaseModel model)
			{
				return Prefabs.ModTab;
			}

			///	<summary>
			/// Builds UI from a list of models
			/// </summary>
			internal void BuildFromModelList(List<UIFModel.BaseModel> modelList)
			{
				ContainerReset();
				foreach (UIFModel.BaseModel model in modelList)
				{
					GameObject uiElement = GameObject.Instantiate(GetUIPrefabForModel(model), this.gameObject.transform);

					IModelController ViewController;

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
					if(ViewController != null)
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
		internal class Sidebar : ListArea
		{
			//protected override GameObject UIPrefab { get { return Prefabs.ModTab; } }
			protected override GameObject GetUIPrefabForModel(UIFModel.BaseModel model) 
			{
				return Prefabs.ModTab;
			}

		}

		/// <summary>
		///
		/// </summary>
		[RegisterTypeInIl2Cpp]
		internal class TopBar : ListArea
		{
			//protected override GameObject UIPrefab { get { return Prefabs.CatTab; } }
			protected override GameObject GetUIPrefabForModel(UIFModel.BaseModel model) 
			{
				return Prefabs.CatTab;
			}

		}
		/// <summary>
		/// 
		/// </summary>
		[RegisterTypeInIl2Cpp]
		
		internal class PrefList : ListArea
		{
			//protected override GameObject UIPrefab { get { return Prefabs.TextPrefab; } }
			protected override GameObject GetUIPrefabForModel(UIFModel.BaseModel model = null) 
			{
				UIFModel.ModelEntry entry = (UIFModel.ModelEntry) model;
			// 	internal static GameObject TextPrefab;
			// internal static GameObject BoolPrefab;
			// internal static GameObject IntPrefab;
			// internal static GameObject FloatPrefab;
		 		switch(entry.InputType)
				{
					case InputType.TextField:
						return Prefabs.TextPrefab;
					case InputType.Toggle:
						return Prefabs.BoolPrefab;
					case InputType.NumericInt:
						return Prefabs.IntPrefab;
					case InputType.NumericFloat:
						return Prefabs.FloatPrefab;
					default:
						return Prefabs.TextPrefab;

				}
			}

		}




		internal class TabButtonController : MonoBehaviour, IModelController
		{
			internal UIFModel.BaseModel _model;
			public virtual UIFModel.BaseModel Model
			{
				get { return _model; }
				set
				{
					_model = value;
					Label = _model.Name;

				}
			}

			internal string Label { set { this.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }
			internal ColorARGB TabColor { get; set; }
			internal ListArea TargetContainer;
			internal void OnSelect()
			{
				PopTarget();
			}
			/// <summary>
			/// Populates the target container with
			/// </summary>
			internal virtual void PopTarget()
			{
				switch (this)
				{
					case Mod mod:
						TargetContainer = Prefabs.CatDisplayList.GetComponent<TopBar>();
						break;
					case Category cat:
						TargetContainer = Prefabs.PrefDisplayList.GetComponent<PrefList>();
						break;
				}

				TargetContainer.SetModel(_model);
			}


			void Start()
			{
				this.gameObject.GetComponent<Button>().onClick.AddListener((UnityAction)OnSelect);
			}


		}

		[RegisterTypeInIl2Cpp]
		internal class Mod : TabButtonController
		{

			internal string ModName { get; set; }


			internal override void PopTarget()
			{
				TargetContainer = Prefabs.CatDisplayList.GetComponent<TopBar>();
				TargetContainer.BuildFromModelList(Model.subModels);
			}



		}

		[RegisterTypeInIl2Cpp]
		internal class Category : TabButtonController
		{
			internal override void PopTarget()
			{
				TargetContainer = Prefabs.PrefDisplayList.GetComponent<PrefList>();
				TargetContainer.BuildFromModelList(Model.subModels);
			}

		}
		
		public class PreferenceEntry : MonoBehaviour, IModelController
		{
			internal UIFModel.ModelEntry _model;
			public UIFModel.BaseModel Model 
			{ 
				get { return _model; } 
				set 
				{ 
					_model = (UIFModel.ModelEntry) value; 
					DescriptionText = _model.Description;
					IdentifierText = _model.Identifier;
				}
			}
			public InputType InputType { get; set; }
			public string DescriptionText { set { this.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }
			public string IdentifierText { set { this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }
			

		}
		[RegisterTypeInIl2Cpp]
		internal abstract class TextInputEntry : PreferenceEntry
		{
			protected TextMeshProUGUI textField => this.gameObject.transform.Find("Panel/InputField (TMP)/Text Area/Text").gameObject.GetComponent<TextMeshProUGUI>();
			public string PlaceHolderText { set { this.gameObject.transform.Find("Panel/InputField (TMP)/Text Area/Placeholder").gameObject.GetComponent<TextMeshProUGUI>().text = value; } }
		}

		[RegisterTypeInIl2Cpp]
		internal class PrefText : TextInputEntry
		{
			public virtual string Value => textField.text;
		}

		[RegisterTypeInIl2Cpp]
		internal class PrefInt : TextInputEntry
		{
			public int Value => int.Parse(textField.text);
		}

		[RegisterTypeInIl2Cpp]
		internal class PrefFloat : TextInputEntry
		{
			public float Value => float.Parse(textField.text);
		}

		[RegisterTypeInIl2Cpp]
		internal class PrefBool : PreferenceEntry
		{
			public bool value => this.gameObject.transform.Find("Panel/Toggle").gameObject.GetComponent<Toggle>().isOn;
		}

		[RegisterTypeInIl2Cpp]
		internal class PrefMulti : PreferenceEntry
		{

		}
		[RegisterTypeInIl2Cpp]
		internal class PrefSlider : PreferenceEntry
		{

		}
		[RegisterTypeInIl2Cpp]
		internal class PrefDropDown : PreferenceEntry
		{

		}
	}
}