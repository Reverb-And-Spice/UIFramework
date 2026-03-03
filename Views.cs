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
		}

		[RegisterTypeInIl2Cpp]
		internal class ModListView : ContainerView
		{
			
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
			public string Label { set { this.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }
			
		}
		[RegisterTypeInIl2Cpp]
		internal class Mod : TabButtonView
		{
			public UIFModel.Mod Model {get; set;}
		}
		[RegisterTypeInIl2Cpp]
		internal class Category : MonoBehaviour
		{
			public UIFModel.Category Model {get; set;}
		}
		public abstract class IPreferenceEntry : MonoBehaviour
		{
			public string Description { set { this.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }
			public string Label { set { this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }

		}
		[RegisterTypeInIl2Cpp]
		internal class PrefText : IPreferenceEntry
		{

		}
		[RegisterTypeInIl2Cpp]
		internal class PrefBool : IPreferenceEntry
		{

		}
		[RegisterTypeInIl2Cpp]
		internal class PrefInt : IPreferenceEntry
		{

		}
		[RegisterTypeInIl2Cpp]
		internal class PrefFloat : IPreferenceEntry
		{

		}
	}
}