using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UIFramework.Adapters;

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

		/// <summary>
		/// Temporary game object storage as they're being instantiated
		/// </summary>
		internal static GameObject TempStorage = new GameObject("TempStorage");
		internal static GameObject HiddenStorage = new GameObject("HiddenStorage");
		internal static GameObject Canvas;

		public static GameObject MainWindowSource;
		public static GameObject MainWindowDragHandle;
		internal static GameObject ModDisplayList;
		internal static GameObject CatDisplayList;
		internal static GameObject PrefDisplayList;

		internal static GameObject ModTab;
		internal static GameObject CatTab;

		internal static GameObject TextPrefab;
		internal static GameObject BoolPrefab;
		internal static GameObject IntPrefab;
		internal static GameObject FloatPrefab;
		//internal static GameObject DoublePrefab;
		internal static GameObject DropDownPrefab;
		internal static GameObject SliderPrefab;

		internal static GameObject ButtonPrefab;

		internal static Button MainActionButton;
		internal static Button DiscardButton;
		internal static Button MinimizeButton;

		internal static void LoadAssetBundle()
		{
			Debug.Log("LoadingUIFramework AssetBundle", true);
			GameObject.DontDestroyOnLoad(UIFGameObjects);
			TempStorage.transform.SetParent(UIFGameObjects.transform, false);
			TempStorage.SetActive(false);

			HiddenStorage.transform.SetParent(UIFGameObjects.transform, false);
			HiddenStorage.SetActive(false);
			AssetBundleLoaded = GameObject.Instantiate(LoadAssetFromStream<GameObject>(Core.Instance, "UIFramework.Assets.uiframework", "UIframework"), UIFGameObjects.transform);
			AssetBundleLoaded.name = "UIFrameworkAssets";

			Canvas = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "UICanvas")?.gameObject;
			MainWindowSource = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "Root")?.gameObject;
			MainWindowDragHandle = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "DragHandle")?.gameObject;


			ModDisplayList = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "ModRegCont")?.gameObject;
			CatDisplayList = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "CatRegCont")?.gameObject;
			PrefDisplayList = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefRegCont")?.gameObject;

			ModTab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "ModEntry")?.gameObject;
			CatTab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "CategoryTab")?.gameObject;

			TextPrefab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryText")?.gameObject;
			BoolPrefab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryBool")?.gameObject;

			IntPrefab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryInt")?.gameObject;
			FloatPrefab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryFloat")?.gameObject;
			//DoublePrefab = GameObject.Instantiate(FloatPrefab, AssetBundleLoaded.transform);

			DropDownPrefab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryDropdown")?.gameObject;
			SliderPrefab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntrySlider")?.gameObject;

			ButtonPrefab = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "PrefEntryButton")?.gameObject;


			MainActionButton = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "SaveActionButton")?.gameObject.GetComponent<Button>();
			MinimizeButton = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "Minimize")?.gameObject.GetComponent<Button>();

			DiscardButton = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "DiscardActionButton")?.gameObject.GetComponent<Button>();
			DiscardButton.gameObject.SetActive(false);



			Canvas.transform.SetParent(UIFGameObjects.transform, false);
			MainWindowSource.transform.SetParent(HiddenStorage.transform, false);

			/*//skip direct children of main window
			 * ModDisplayList.transform.SetParent(HiddenStorage.transform, false);
			 * CatDisplayList.transform.SetParent(HiddenStorage.transform, false);
			PrefDisplayList.transform.SetParent(HiddenStorage.transform,false);*/

			ModTab.transform.SetParent(HiddenStorage.transform, false);
			CatTab.transform.SetParent(HiddenStorage.transform, false);

			TextPrefab.transform.SetParent(HiddenStorage.transform, false);
			BoolPrefab.transform.SetParent(HiddenStorage.transform, false);

			IntPrefab.transform.SetParent(HiddenStorage.transform, false);
			FloatPrefab.transform.SetParent(HiddenStorage.transform, false);
			//DoublePrefab.transform.SetParent(HiddenStorage.transform ,false);

			DropDownPrefab.transform.SetParent(HiddenStorage.transform, false);
			SliderPrefab.transform.SetParent(HiddenStorage.transform, false);

			ButtonPrefab.transform.SetParent(HiddenStorage.transform, false);





			//MainActionButton.onClick.AddListener(MainButtonClick);

			//ModEntry.TestComponent test = ModDisplayList.AddComponent<ModEntry.TestComponent>();

			//Add the appropriate components to each prefab for later use
			MainWindowSource.AddComponent<WindowCoordinator>();


			TextPrefab.AddComponent<TextEntryAdapter>();
			BoolPrefab.AddComponent<BoolToggleAdapter>();

			IntPrefab.AddComponent<TextEntryAdapter>();
			FloatPrefab.AddComponent<TextEntryAdapter>();
			//DoublePrefab.AddComponent<   TextEntryAdapter>();

			//DropDownPrefab.AddComponent<EnumDropdownAdapter>();
			SliderPrefab.AddComponent<NumSliderAdapter>();

			//ButtonPrefab.AddComponent<   ButtonModelAdapter>();



			ModTab.AddComponent<ModButtonView>();

			CatTab.AddComponent<CategoryTabView>();

			//Add the component to the container sections
			ModDisplayList.AddComponent<ModListAdapter>();
			CatDisplayList.AddComponent<CategoryListAdapter>();
			PrefDisplayList.AddComponent<PrefListAdapter>();


			DragHandle dragScript = MainWindowDragHandle.AddComponent<DragHandle>();
			EventTrigger trigger = MainWindowDragHandle.AddComponent<EventTrigger>();


			MainWindowSource.SetActive(false);
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
	}


	#region AI Generated
	[RegisterTypeInIl2Cpp]
	public class DragHandle : MonoBehaviour
	{
		public RectTransform FindRootWindow()
		{
			RectTransform foundRoot = null; ;
			Transform ancestor = this.gameObject.transform.parent;
			while (ancestor != null)
			{
				if (ancestor.name.Contains("MainWindow"))
				{
					return ancestor.GetComponent<RectTransform>();
				}
				ancestor = ancestor.parent;
			}

			return foundRoot;
		}

		public RectTransform targetPanel;

		// This method will be called by the EventTrigger
		public void OnDrag(BaseEventData data)
		{
			PointerEventData eventData = data.TryCast<PointerEventData>();
			if (eventData == null || targetPanel == null) return;

			Canvas canvas = targetPanel.GetComponentInParent<Canvas>();
			float scale = (canvas != null) ? canvas.scaleFactor : 1.0f;

			targetPanel.anchoredPosition += eventData.delta / scale;

			ClampToBounds();

			Preferences.UiPosition.Value = targetPanel.anchoredPosition;

		}
		public void ClampToBounds()
		{
			if (targetPanel == null) return;

			Canvas canvas = targetPanel.GetComponentInParent<Canvas>();
			float scale = (canvas != null) ? canvas.scaleFactor : 1.0f;

			Rect pixelRect = canvas.pixelRect;
			Vector2 screenSize = new Vector2(pixelRect.width, pixelRect.height) / scale;
			Vector2 size = targetPanel.rect.size;

			const float keepRight = 30f;
			const float keepBottom = 30f;
			const float keepLeft = 50f;

			float minX = -(size.x - keepLeft);
			float maxX = screenSize.x - keepRight;
			float minY = -(screenSize.y - keepBottom);
			float maxY = 0f;

			targetPanel.anchoredPosition = new Vector2(
				Mathf.Clamp(targetPanel.anchoredPosition.x, minX, maxX),
				Mathf.Clamp(targetPanel.anchoredPosition.y, minY, maxY)
			);

			Preferences.UiPosition.Value = targetPanel.anchoredPosition;
		}
		void Start()
		{
			targetPanel = FindRootWindow();

			EventTrigger trigger = GetComponent<EventTrigger>();
			if (trigger == null) trigger = gameObject.AddComponent<EventTrigger>();

			// Clear old entries if any (prevent double-firing if script is copied)
			trigger.triggers.Clear();

			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.Drag;
			entry.callback.AddListener((UnityEngine.Events.UnityAction<BaseEventData>)OnDrag);
			trigger.triggers.Add(entry);

			targetPanel.anchoredPosition = Preferences.UiPosition.Value;
			ClampToBounds();
		}
	}
	#endregion
}