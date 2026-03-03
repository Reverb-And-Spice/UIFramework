using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.IO;
using Il2CppTMPro;
using MelonLoader;
//using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using static Il2CppSystem.DateTimeParse;

namespace UIFramework
{
	internal static partial class Prefabs
	{


		internal static GameObject UiAssets;
		internal static GameObject ModDisplayList;
		internal static GameObject CatDisplayList;
		internal static GameObject PrefDisplayList;

		internal static GameObject ModTab;
		internal static GameObject CatTab;

		internal static GameObject TextPrefab;
		internal static GameObject BoolPrefab;
		internal static GameObject IntPrefab;
		internal static GameObject FloatPrefab;

		internal static Button MainActionButton;

		internal static void LoadAssetBundle()
		{
			Debug.Log("LoadingUIFramework AssetBundle", true);
			UiAssets = GameObject.Instantiate(LoadAssetFromStream<GameObject>(Core.Instance, "UIFramework.Assets.uiframework", "UIFramework"));
			foreach (var tmpugui in UiAssets.GetComponentsInChildren<TextMeshProUGUI>(true))
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
				//tmpugui.SetLayoutDirty();
			}

			//Refresh(UiAssets, alsoFixCommonIssues: true);

			// 4) If your LayoutGroups weren’t updating, force a proper rebuild
			/*Canvas.ForceUpdateCanvases();
			var rts = UiAssets.GetComponentsInChildren<RectTransform>(true);
			foreach (var rt in rts) UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
			Canvas.ForceUpdateCanvases();*/
			ApplyAndRebuild(UiAssets);

			GameObject.DontDestroyOnLoad(UiAssets);

			ModDisplayList = UiAssets.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "ModRegCont")?.gameObject;
			CatDisplayList = UiAssets.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "CatRegCont")?.gameObject;
			PrefDisplayList = UiAssets.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefRegCont")?.gameObject;

			ModTab = UiAssets.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "ModEntry")?.gameObject;
			CatTab = UiAssets.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "CategoryTab")?.gameObject;

			TextPrefab = UiAssets.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryText")?.gameObject;
			BoolPrefab = UiAssets.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryBoolean")?.gameObject;
			IntPrefab = UiAssets.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryInt")?.gameObject;
			FloatPrefab = UiAssets.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryFloat")?.gameObject;

			MainActionButton = UiAssets.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "MainActionButton")?.gameObject.GetComponent<Button>();


			

			MainActionButton.onClick.AddListener(MainButtonClick);

			//ModEntry.TestComponent test = ModDisplayList.AddComponent<ModEntry.TestComponent>();

			//Add the appropriate components to each prefab for later use
			TextPrefab.AddComponent<UIFView.PrefText>();
			BoolPrefab.AddComponent<UIFView.PrefBool>();
			IntPrefab.AddComponent<UIFView.PrefInt>();
			FloatPrefab.AddComponent<UIFView.PrefFloat>();

			UIFView.Mod baseModTabController = ModTab.AddComponent<UIFView.Mod>();
			Button ModButton = ModTab.GetComponent<Button>();
			ModButton.onClick.AddListener(baseModTabController.OnSelect);

			UIFView.Category baseCatTabController = CatTab.AddComponent<UIFView.Category>();
			Button CatButton = ModTab.GetComponent<Button>();
			CatButton.onClick.AddListener(baseCatTabController.OnSelect);


			//Add the component to the container sections
			ModDisplayList.AddComponent<UIFView.ModListView>();
			CatDisplayList.AddComponent<UIFView.CatListView>();
			PrefDisplayList.AddComponent<UIFView.PrefListView>();



			//UI Test Section
			GameObject testMod = GameObject.Instantiate(ModTab, ModDisplayList.transform);
			GameObject testCat = GameObject.Instantiate(CatTab, CatDisplayList.transform);
			GameObject testPref = GameObject.Instantiate(TextPrefab, PrefDisplayList.transform);
			GameObject.Instantiate(BoolPrefab, PrefDisplayList.transform);
			GameObject.Instantiate(IntPrefab, PrefDisplayList.transform);
			GameObject.Instantiate(FloatPrefab, PrefDisplayList.transform);
		}

		static UnityAction MainButtonClick = new System.Action(() =>
		{
			Debug.Log("Main Action Button Clicked!", true, 0);
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

		#region AIGenerated

		internal static void ApplyAndRebuild(GameObject root)
		{


			if (root == null) return;

			// 1) Configure TMP Components
			var tmps = root.GetComponentsInChildren<TextMeshProUGUI>(true);
			foreach (var t in tmps)
			{
				if (t == null) continue;

				t.enableWordWrapping = true;
				// Overflow mode ensures text wraps and fills the space, 
				// rather than cutting off or masking.
				t.overflowMode = TextOverflowModes.Overflow;

				// Ensure this is not set to shrink the text
				t.enableAutoSizing = false;
			}
/*
			// 2) Force Layout Update
			// Instead of forcing individual RectTransforms, we find the 
			// top-most parent that has a layout group or is the root 
			// and force a complete rebuild.
			Canvas.ForceUpdateCanvases();

			// This finds all LayoutGroups and ContentSizeFitters in the root
			// and forces them to re-evaluate their children's sizes.
			*//*var layouts = root.GetComponentsInChildren<LayoutGroup>(true);
			foreach (var l in layouts)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(l.transform as RectTransform);
			}*//*

			// Final pass to make sure everything is clean
			Canvas.ForceUpdateCanvases();
*/
		}

		#endregion
	}


}
