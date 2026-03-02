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
		[RegisterTypeInIl2Cpp]
		internal class Mod : MonoBehaviour
		{
			public string Label { set { this.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }
		}
		[RegisterTypeInIl2Cpp]
		internal class Category : MonoBehaviour
		{

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