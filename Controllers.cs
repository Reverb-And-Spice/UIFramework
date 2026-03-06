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
	public class UIFController
	{
		/// <summary>
		///
		/// </summary>

		internal class ContainerView : MonoBehaviour
		{
			protected void ContainerReset()
			{
				Infanticide();
			}

			protected void Infanticide()
			{
				while (this.gameObject.transform.childCount > 0)
				{
					Destroy(this.transform.GetChild(0));
				}
			}

			protected virtual GameObject TabPrefab { get; }

			internal void BuildFromModelList(List<UIFModel.BaseListSources> modelList)
			{
				ContainerReset();
				foreach (UIFModel.BaseListSources mod in modelList)
				{
					GameObject tab = GameObject.Instantiate(TabPrefab, this.gameObject.transform);
					UIFController.TabButtonController ViewController = tab.GetComponent<UIFController.TabButtonController>();

					ViewController.Model = mod;
					//ViewController.TargetContainer = Prefabs.CatDisplayList.GetComponent<CatList>();


				}
			}

		}

		/// <summary>
		///
		/// </summary>
		[RegisterTypeInIl2Cpp]
		internal class ModList : ContainerView
		{
			protected override GameObject TabPrefab { get { return Prefabs.ModTab; } }
			/*internal void BuildFromModelList(List<UIFModel.BaseListSources> modelList)
			{
				ContainerReset();
				foreach (UIFModel.ModelMod mod in modelList)
				{
					GameObject tab = GameObject.Instantiate(Prefabs.ModTab, Prefabs.ModDisplayList.transform);
					UIFController.Mod ViewController = tab.GetComponent<UIFController.Mod>();

					ViewController.Model = mod;
					//ViewController.TargetContainer = Prefabs.CatDisplayList.GetComponent<CatList>();


				}
			}*/
		}

		/// <summary>
		///
		/// </summary>
		[RegisterTypeInIl2Cpp]
		internal class CatList : ContainerView
		{
			//[10:13:42.231][Il2CppInterop] Exception in IL2CPP-to-Managed trampoline, not passing it to il2cpp: System.NullReferenceException: Object reference not set to an instance of an object.
			//at UIFramework.UIFController.ContainerView.BuildFromModelList(List`1 modelList)
			//at UIFramework.UIFController.Category.PopTarget()
			//at UIFramework.UIFController.TabButtonController.OnSelect()
			//at(il2cpp delegate trampoline) System.Void_System.Action(IntPtr, Il2CppMethodInfo*)
			protected override GameObject TabPrefab { get { return Prefabs.CatTab; } }

		}

		[RegisterTypeInIl2Cpp]
		internal class PrefList : ContainerView
		{
			protected override GameObject TabPrefab { get { return Prefabs.TextPrefab; } }

		}




		[RegisterTypeInIl2Cpp]
		internal class TabButtonController : MonoBehaviour
		{
			internal UIFModel.BaseListSources _model;
			internal virtual UIFModel.BaseListSources Model
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
			internal ContainerView TargetContainer;
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
						TargetContainer = Prefabs.CatDisplayList.GetComponent<CatList>();
						break;
					case Category cat:
						TargetContainer = Prefabs.PrefDisplayList.GetComponent<PrefList>();
						break;
				}

				TargetContainer.BuildFromModelList(Model.subModels);
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
				TargetContainer = Prefabs.CatDisplayList.GetComponent<CatList>();
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
		[RegisterTypeInIl2Cpp]
		public class PreferenceEntry : MonoBehaviour
		{
			public string Description { set { this.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }
			public string Label { set { this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }

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
			public int Value { get { return int.Parse(textField.text); } }
		}

		[RegisterTypeInIl2Cpp]
		internal class PrefFloat : TextInputEntry
		{
			public float Value { get { return float.Parse(textField.text); } }
		}

		[RegisterTypeInIl2Cpp]
		internal class PrefBool : PreferenceEntry
		{
			public bool value { get { return this.gameObject.transform.Find("Panel/Toggle").gameObject.GetComponent<Toggle>().isOn; } }
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