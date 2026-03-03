using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Il2CppTMPro;
using MelonLoader.Logging;
using MonoMod.ModInterop;
namespace UIFramework
{
	/// <summary>
	/// Custom components that will serve as views in this MVP/MVC-ish pattern
	/// </summary>
	public class UIFController
	{

		internal abstract class ContainerView : MonoBehaviour
		{
			internal void Reset()
			{
				Infanticide();
			}

			private void Infanticide()
			{
				while(this.gameObject.transform.childCount > 0)
				{
					Destroy(this.transform.GetChild(0));
				}
			} 

			internal abstract void BuildFromModelList(List<UIFModel.IModelable> modelList);

		}

		[RegisterTypeInIl2Cpp]
		internal class ModList : ContainerView
		{
			internal override void BuildFromModelList(List<UIFModel.IModelable> modelList)
			{
				foreach(Mod melon in modelList)
				{
					GameObject tab = GameObject.Instantiate(Prefabs.ModTab);
					UIFController.Mod ViewController = tab.GetComponent<UIFController.Mod>();
					ViewController.Label = ViewController.ModName;

				}
			}
		}

		[RegisterTypeInIl2Cpp]
		internal class CatList : ContainerView
		{
			internal override void BuildFromModelList(List<UIFModel.IModelable> modelList)
			{

			}
		}

		[RegisterTypeInIl2Cpp]
		internal class PrefList : ContainerView
		{
			internal override void BuildFromModelList(List<UIFModel.IModelable> modelList)
			{

			}
		}



		internal abstract class TabButtonController : MonoBehaviour
		{
			internal string Label { set { this.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }
			internal ColorARGB TabColor { get;  set; }
			
			
		}

		[RegisterTypeInIl2Cpp]
		internal class Mod : TabButtonController
		{
			internal UIFModel.Mod Model {get; set;}
			internal string ModName { get; set;}
			internal UnityAction OnSelect = new System.Action(() => 
			{
				
			});

		}

		[RegisterTypeInIl2Cpp]
		internal class Category : MonoBehaviour
		{
			public UIFModel.Category Model {get; set;}
			internal UnityAction OnSelect = new System.Action(() => 
			{
				
			});
		}
		public abstract class PreferenceEntry : MonoBehaviour
		{
			public string Description { set { this.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }
			public string Label { set { this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }

		}



		[RegisterTypeInIl2Cpp]
		internal class PrefText : PreferenceEntry
		{

		}
		[RegisterTypeInIl2Cpp]
		internal class PrefBool : PreferenceEntry
		{

		}
		[RegisterTypeInIl2Cpp]
		internal class PrefInt : PreferenceEntry
		{

		}
		[RegisterTypeInIl2Cpp]
		internal class PrefFloat : PreferenceEntry
		{

		}
	}
}