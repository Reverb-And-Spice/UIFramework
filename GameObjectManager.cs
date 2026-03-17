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
		/// Root Game Object for all of UIFramework in DDOL
		/// </summary>
		internal static GameObject UIFGameObjects = new GameObject("UIFramework");

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

		internal static GameObject ButtonPrefab;
		internal static Button MainActionButton;

		internal static void LoadAssetBundle()
		{
			Debug.Log("LoadingUIFramework AssetBundle", true);
			GameObject.DontDestroyOnLoad(UIFGameObjects);
			TempStorage.transform.SetParent(UIFGameObjects.transform,false);
			TempStorage.SetActive(false);

			AssetBundleLoaded = GameObject.Instantiate(LoadAssetFromStream<GameObject>(Core.Instance, "UIFramework.Assets.uiframework", "UIFramework"), UIFGameObjects.transform);
			AssetBundleLoaded.name = "UIFrameworkAssets";
			//AssetBundleLoaded.SetActive(false);
			//Taco generated (text won't work witout this for some reason)
			foreach (var tmpugui in AssetBundleLoaded.GetComponentsInChildren<TextMeshProUGUI>(true))
			{
				tmpugui.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/Arial SDF");


				var font = Resources.Load<Il2CppTMPro.TMP_FontAsset>("Fonts & Materials/Arial SDF");
				if (font != null)
				{
					tmpugui.font = font;
					tmpugui.fontSharedMaterial = font.material;
					tmpugui.SetVerticesDirty();
					tmpugui.SetMaterialDirty();
				}
			}
			ApplyAndRebuild(AssetBundleLoaded);

			
			
			MainCanvasSource = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "UICanvas")?.gameObject;

			ModDisplayList = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "ModRegCont")?.gameObject;
			CatDisplayList = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "CatRegCont")?.gameObject;
			PrefDisplayList = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefRegCont")?.gameObject;

			ModTab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "ModEntry")?.gameObject;
			CatTab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "CategoryTab")?.gameObject;

			TextPrefab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryText")?.gameObject;
			BoolPrefab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryBoolean")?.gameObject;
			IntPrefab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryInt")?.gameObject;
			FloatPrefab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryFloat")?.gameObject;
			DoublePrefab = GameObject.Instantiate(FloatPrefab, AssetBundleLoaded.transform);

			ButtonPrefab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryButton")?.gameObject;

			MainActionButton = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "SaveActionButton")?.gameObject.GetComponent<Button>();


			

			//MainActionButton.onClick.AddListener(MainButtonClick);

			//ModEntry.TestComponent test = ModDisplayList.AddComponent<ModEntry.TestComponent>();

			//Add the appropriate components to each prefab for later use
			MainCanvasSource.AddComponent<UIFController.WindowController>();
			TextPrefab.AddComponent<UIFController.PrefText>();
			BoolPrefab.AddComponent<UIFController.PrefBool>();
			IntPrefab.AddComponent<UIFController.PrefInt>();
			FloatPrefab.AddComponent<UIFController.PrefFloat>();
			DoublePrefab.AddComponent<UIFController.PrefDouble>();
			ButtonPrefab.AddComponent<UIFController.ButtonEntry>();
			UIFController.Mod baseModTabController = ModTab.AddComponent<UIFController.Mod>();
			Button ModButton = ModTab.GetComponent<Button>();
			//ModButton.onClick.AddListener(baseModTabController.OnSelect);

			UIFController.Category baseCatTabController = CatTab.AddComponent<UIFController.Category>();
			Button CatButton = ModTab.GetComponent<Button>();
			//CatButton.onClick.AddListener(baseCatTabController.OnSelect);


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

		static GameObject GetInputPrefab(InputType type)
		{
			return type switch
			{
				InputType.TextField => TextPrefab,
				InputType.Toggle => BoolPrefab,
				InputType.NumericInt => IntPrefab,
				InputType.NumericFloat => FloatPrefab,
				
				_ => null
			};
		}

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

		#region AIGenerated
		/// <summary>
		/// Wrapping gets disabled when loading and this reenables it
		/// </summary>
		/// <param name="root"></param>
		internal static void ApplyAndRebuild(GameObject root)
		{


			if (root == null) return;

			// 1) Configure TMP Components
			var tmps = root.GetComponentsInChildren<TextMeshProUGUI>(true);
			foreach (var t in tmps)
			{
				if (t == null) continue;
				if (t.gameObject.name == "ButtonText")
				{
					t.enableWordWrapping = false; // Ensure wrapping is off for these
					continue;
				}
				t.enableWordWrapping = true;
				// Overflow mode ensures text wraps and fills the space, 
				// rather than cutting off or masking.
				t.overflowMode = TextOverflowModes.Overflow;

				// Ensure this is not set to shrink the text
				t.enableAutoSizing = false;
			}

			// 2) Force Layout Update
			// Instead of forcing individual RectTransforms, we find the 
			// top-most parent that has a layout group or is the root 
			// and force a complete rebuild.
			Canvas.ForceUpdateCanvases();

			/*
			 * this part of the code didn't work but doesn't seem to be needed?
			 * // This finds all LayoutGroups and ContentSizeFitters in the root
			// and forces them to re-evaluate their children's sizes.
			//var layouts = root.GetComponentsInChildren<LayoutGroup>(true);
			foreach (var l in layouts)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(l.transform as RectTransform);
			}*/
			// Final pass to make sure everything is clean
			Canvas.ForceUpdateCanvases();

		}

		#endregion
	}


}
