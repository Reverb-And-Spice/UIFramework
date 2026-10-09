using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using UIFramework.Adapters;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIFramework
{
	internal static partial class Prefabs
	{
		/// <summary>
		/// Root Game Object for all of UI in DDOL
		/// </summary>
		internal static GameObject UIFGameObjects = new("UI");

		/// <summary>
		/// UI Assets from the asset bundle
		/// </summary>
		internal static GameObject AssetBundleLoaded;

		/// <summary>
		/// Temporary game object storage as they're being instantiated
		/// </summary>
		internal static GameObject TempStorage = new("TempStorage");
		internal static GameObject HiddenStorage = new("HiddenStorage");
		internal static GameObject Canvas;

		public static GameObject MainWindowSource;
		public static GameObject MainWindowDragHandle;
		public static GameObject MainWindowScaleHandle;
		public static GameObject MainWindowStretchHandle;
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
			MainWindowScaleHandle = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "ScaleHandle")?.gameObject;
			MainWindowStretchHandle = AssetBundleLoaded.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "StretchHandle")?.gameObject;


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


			MainWindowSource.AddComponent<WindowCoordinator>();


			ModTab.AddComponent<ModButtonView>();

			CatTab.AddComponent<CategoryTabView>();

			//Add the component to the container sections
			ModDisplayList.AddComponent<ModListAdapter>();
			CatDisplayList.AddComponent<CategoryListAdapter>();
			PrefDisplayList.AddComponent<PrefListAdapter>();


			DragHandle dragScript = MainWindowDragHandle.AddComponent<DragHandle>();
			EventTrigger trigger = MainWindowDragHandle.AddComponent<EventTrigger>();

			MainWindowScaleHandle.AddComponent<ScaleHandle>();
			MainWindowScaleHandle.AddComponent<EventTrigger>();

			MainWindowStretchHandle.AddComponent<StretchHandle>();
			MainWindowStretchHandle.AddComponent<EventTrigger>();

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

			Il2CppSystem.IO.MemoryStream Il2CppStream = new();

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
	public class StretchHandle : MonoBehaviour
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

		public void OnDrag(BaseEventData data)
		{
			PointerEventData eventData = data.TryCast<PointerEventData>();
			if (eventData == null || targetPanel == null) return;

			Canvas canvas = targetPanel.GetComponentInParent<Canvas>();
			float scale = (canvas != null) ? canvas.scaleFactor : 1.0f;

			Rect pixelRect = canvas.pixelRect;
			Vector2 screenSize = new Vector2(pixelRect.width, pixelRect.height) / scale;

			float maxHeight = (screenSize.y + targetPanel.anchoredPosition.y) / targetPanel.localScale.y;
			float newHeight = Mathf.Clamp(targetPanel.sizeDelta.y - eventData.delta.y / scale, 600f, maxHeight);
			targetPanel.sizeDelta = new Vector2(targetPanel.sizeDelta.x, newHeight);
			Preferences.UiHeight.Value = targetPanel.sizeDelta.y;
		}

		public void OnEndDrag(BaseEventData data)
		{
			Preferences.Footprint.SaveToFile(Preferences.EnableDebugMode.Value);
		}
		void Start()
		{
			targetPanel = FindRootWindow();

			EventTrigger trigger = GetComponent<EventTrigger>();
			if (trigger == null) trigger = gameObject.AddComponent<EventTrigger>();
			trigger.triggers.Clear();

			EventTrigger.Entry entry = new();
			entry.eventID = EventTriggerType.Drag;
			entry.callback.AddListener(DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction<BaseEventData>>(
				(System.Action<BaseEventData>)OnDrag));
			trigger.triggers.Add(entry);

			EventTrigger.Entry endEntry = new();
			endEntry.eventID = EventTriggerType.EndDrag;
			endEntry.callback.AddListener(DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction<BaseEventData>>(
				(System.Action<BaseEventData>)OnEndDrag));
			trigger.triggers.Add(endEntry);

			targetPanel.sizeDelta = new Vector2(targetPanel.sizeDelta.x, Preferences.UiHeight.Value);

		}
	}


	[RegisterTypeInIl2Cpp]
	public class ScaleHandle : MonoBehaviour
	{
		public RectTransform targetPanel;
		const float maxScale = 2f;
		const float minScale = 1f;

		private float _initialWidth;
		private Vector3 _initialScale;
		private float _scaleAtDragStart;

		public RectTransform FindRootWindow()
		{
			RectTransform foundRoot = null;
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

		public void OnBeginDrag(BaseEventData data)
		{
		}
		public void OnDrag(BaseEventData data)
		{
			PointerEventData eventData = data.TryCast<PointerEventData>();
			if (eventData == null || targetPanel == null) return;

			Canvas canvas = targetPanel.GetComponentInParent<Canvas>();
			float canvasScale = (canvas != null) ? canvas.scaleFactor : 1.0f;

			float deltaScale = eventData.delta.x / canvasScale / targetPanel.sizeDelta.x;
			float newScale = Mathf.Clamp(targetPanel.localScale.x + deltaScale, 1f, 2f);
			targetPanel.localScale = new Vector3(newScale, newScale, 1f);


			Preferences.UiScale.Value = targetPanel.localScale;

		}

		public void OnEndDrag(BaseEventData data)
		{
			Preferences.Footprint.SaveToFile(Preferences.EnableDebugMode.Value);
		}
		void Start()
		{
			targetPanel = FindRootWindow();

			EventTrigger trigger = GetComponent<EventTrigger>();
			if (trigger == null) trigger = gameObject.AddComponent<EventTrigger>();
			trigger.triggers.Clear();

			EventTrigger.Entry dragEntry = new();
			dragEntry.eventID = EventTriggerType.Drag;
			dragEntry.callback.AddListener(DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction<BaseEventData>>(
				(System.Action<BaseEventData>)OnDrag));
			trigger.triggers.Add(dragEntry);

			EventTrigger.Entry beginEntry = new();
			beginEntry.eventID = EventTriggerType.BeginDrag;
			beginEntry.callback.AddListener(DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction<BaseEventData>>(
				(System.Action<BaseEventData>)OnBeginDrag));
			trigger.triggers.Add(beginEntry);

			EventTrigger.Entry endEntry = new();
			endEntry.eventID = EventTriggerType.EndDrag;
			endEntry.callback.AddListener(DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction<BaseEventData>>(
				(System.Action<BaseEventData>)OnEndDrag));
			trigger.triggers.Add(endEntry);

			targetPanel.localScale = Preferences.UiScale.Value;
			if(targetPanel.localScale.x < minScale || targetPanel.localScale.x > maxScale)
			{
				targetPanel.localScale = new Vector3(
					Mathf.Clamp(targetPanel.localScale.x, minScale, maxScale),
					Mathf.Clamp(targetPanel.localScale.y, minScale, maxScale),
					1f);
			}
		}
	}


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
		public void OnEndDrag(BaseEventData data)
		{
			Preferences.Footprint.SaveToFile(Preferences.EnableDebugMode.Value);
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
			const float keepLeft = 80f;

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

			EventTrigger.Entry entry = new();
			entry.eventID = EventTriggerType.Drag;
			entry.callback.AddListener(DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction<BaseEventData>>((System.Action<BaseEventData>)OnDrag));
			trigger.triggers.Add(entry);

			EventTrigger.Entry endEntry = new();
			endEntry.eventID = EventTriggerType.EndDrag;
			endEntry.callback.AddListener(DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction<BaseEventData>>(
				(System.Action<BaseEventData>)OnEndDrag));
			trigger.triggers.Add(endEntry);

			targetPanel.anchoredPosition = Preferences.UiPosition.Value;
			ClampToBounds();
		}
	}
	#endregion
}