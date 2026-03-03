using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using UnityEngine;
using Il2CppTMPro;
namespace UIFramework
{
	/// <summary>
	/// Custom components that will serve as views in this MVP/MVC-ish pattern
	/// </summary>
	public class UIFView
	{

		internal abstract class ContainerView : MonoBehaviour
		{
			internal void Reset()
			{
				Infanticide();
			}

			private void Infanticide()
			{
				while(this.gameObject.transform.ChildCount > 0)
				{
					Destroy(this.transform.GetChild(0));
				}
			} 

			internal abstract void BuildFromModelList(List<UIFramework.Imodelable> modelList);

		}

		[RegisterTypeInIl2Cpp]
		internal class ModListView : ContainerView
		{
			internal override void BuildFromModelList(List<UIFramework.Imodelable> modelList)
			{
				foreach(Mod melon in modelList)
				{
					GameObject tab = GameObject.Instantiate(Prefabs.ModTab);
					Mod ViewController = tab.GetComponent<UIFView.Mod>();
					tab.Label = ViewController.ModName;

				}
			}
		}

		[RegisterTypeInIl2Cpp]
		internal class CatListView : ContainerView
		{
			
		}

		[RegisterTypeInIl2Cpp]
		internal class PrefListView : ContainerView
		{

		}



		internal abstract class TabButtonView : MonoBehaviour
		{
			internal string Label { set { this.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }
			internal ColorARGB TabColor {set;}
			
			
		}

		[RegisterTypeInIl2Cpp]
		internal class Mod : TabButtonView
		{
			internal UIFModel.Mod Model {get; set;}
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