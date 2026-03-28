using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.IO;
using Il2CppTMPro;
using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using static Il2CppSystem.DateTimeParse;
using Il2CppExitGames.Client.Photon;

namespace UIFramework
{
	internal static partial class Prefabs
	{
		/// <summary>
		/// Root Game Object for all of UI in DDOL
		/// </summary>
		internal static GameObject UIFGameObjects = new GameObject("UI");

		/// <summary>
		/// UI Assets from the asset bundle
		/// </summary>
		internal static GameObject AssetBundleLoaded;
		//internal static GameObject PrefabSources = new GameObject("Prefabs");

		/// <summary>
		/// Temporary game object storage as they're being instantiated
		/// </summary>
		internal static GameObject TempStorage = new GameObject("TempStorage");
		public static GameObject MainCanvasSource;
		internal static GameObject ModDisplayList;
		internal static GameObject CatDisplayList;
		internal static GameObject PrefDisplayList;

		internal static GameObject ModTab;
		internal static GameObject CatTab;

		internal static GameObject TextPrefab;
		internal static GameObject BoolPrefab;
		internal static GameObject IntPrefab;
		internal static GameObject FloatPrefab;
		internal static GameObject DoublePrefab;
		internal static GameObject DropDownPrefab;
		

		internal static GameObject ButtonPrefab;

		internal static Button MainActionButton;
		internal static Button DiscardButton;
		internal static Button MinimizeButton;

		internal static void LoadAssetBundle()
		{
			Debug.Log("LoadingUIFramework AssetBundle", true);
			GameObject.DontDestroyOnLoad(UIFGameObjects);
			TempStorage.transform.SetParent(UIFGameObjects.transform,false);
			TempStorage.SetActive(false);

			AssetBundleLoaded = GameObject.Instantiate(LoadAssetFromStream<GameObject>(Core.Instance, "UIFramework.Assets.uiframework", "UIframework"), UIFGameObjects.transform);
			AssetBundleLoaded.name = "UIFrameworkAssets";

			
			
			MainCanvasSource = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "UICanvas")?.gameObject;

			ModDisplayList = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "ModRegCont")?.gameObject;
			CatDisplayList = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "CatRegCont")?.gameObject;
			PrefDisplayList = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefRegCont")?.gameObject;

			ModTab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "ModEntry")?.gameObject;
			CatTab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "CategoryTab")?.gameObject;

			TextPrefab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryText")?.gameObject;
			BoolPrefab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryBool")?.gameObject;
			
			IntPrefab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryInt")?.gameObject;
			FloatPrefab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryFloat")?.gameObject;
			DoublePrefab = GameObject.Instantiate(FloatPrefab, AssetBundleLoaded.transform);

			DropDownPrefab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryDropdown")?.gameObject;

			ButtonPrefab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryButton")?.gameObject;


			MainActionButton = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "SaveActionButton")?.gameObject.GetComponent<Button>();
			MinimizeButton = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "Minimize")?.gameObject.GetComponent<Button>();

			DiscardButton = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "DiscardActionButton")?.gameObject.GetComponent<Button>();
			DiscardButton.gameObject.SetActive(false);



			//MainActionButton.onClick.AddListener(MainButtonClick);

			//ModEntry.TestComponent test = ModDisplayList.AddComponent<ModEntry.TestComponent>();

			//Add the appropriate components to each prefab for later use
			MainCanvasSource.AddComponent<UIFController.WindowController>();

			TextPrefab.AddComponent<UIFController.PrefText>();
			BoolPrefab.AddComponent<UIFController.PrefBool>();
			
			IntPrefab.AddComponent<UIFController.PrefInt>();
			FloatPrefab.AddComponent<UIFController.PrefFloat>();
			DoublePrefab.AddComponent<UIFController.PrefDouble>();
			
			DropDownPrefab.AddComponent<UIFController.PrefDropDown>();

			ButtonPrefab.AddComponent<UIFController.ButtonEntry>();



			ModTab.AddComponent<UIFController.Mod>();

			CatTab.AddComponent<UIFController.Category>();

			//Add the component to the container sections
			ModDisplayList.AddComponent<UIFController.Sidebar>();
			CatDisplayList.AddComponent<UIFController.TopBar>();
			PrefDisplayList.AddComponent<UIFController.PrefList>();


			MainCanvasSource.SetActive(false);
		}

		static UnityAction MainButtonClick = new System.Action(() =>
		{
			Debug.Log("Main Action ButtonGo Clicked!", true, 0);
		});

		#region Ulvak Generated
		internal static T LoadAssetFromStream<T>(MelonMod instance, string path, string assetName) where T : UnityEngine.Object
		{
			using (System.IO.Stream bundleStream = instance.MelonAssembly.Assembly.GetManifestResourceStream(path))
			{
				Il2CppSystem.IO.Stream Il2CppStream = ConvertToIl2CppStream(bundleStream);
				AssetBundle bundle = AssetBundle.LoadFromStream(Il2CppStream);
				Il2CppStream.Close();
				T asset = bundle.LoadAsset<T>(assetName);
				bundle.Unload(false);
				return asset;
			}
		}

		internal static Il2CppSystem.IO.Stream ConvertToIl2CppStream(System.IO.Stream stream)
		{

			Il2CppSystem.IO.MemoryStream Il2CppStream = new Il2CppSystem.IO.MemoryStream();

			const int bufferSize = 4096;
			byte[] managedBuffer = new byte[bufferSize];
			Il2CppStructArray<byte> Il2CppBuffer = new(managedBuffer);

			int bytesRead;
			while ((bytesRead = stream.Read(managedBuffer, 0, managedBuffer.Length)) > 0)
			{
				Il2CppBuffer = managedBuffer;
				Il2CppStream.Write(Il2CppBuffer, 0, bytesRead);
			}
			Il2CppStream.Flush();
			return Il2CppStream;
		}
		#endregion
	}


}
