using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppPlayFab.ClientModels;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;


[assembly: MelonInfo(typeof(UIFramework.Core), UIFramework.BuildInfo.Name, UIFramework.BuildInfo.Version, UIFramework.BuildInfo.Author)]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]
[assembly: MelonColor(255, 255, 255, 255)]
[assembly: MelonAuthorColor(255, 255, 255, 255)]


namespace UIFramework
{
	/// <summary></summary>
	public static class BuildInfo
	{
		/// <summary></summary>
		public const string Name = "UIFramework";
		/// <summary></summary>
		public const string Author = "Blankochan, iListen2Sound, TacoSlayer36";
		/// <summary></summary>
		public const string Version = "1.0.0";
	}
	/// <summary></summary>
	public partial class Core : MelonMod
	{
		internal string CurrentScene = "";
		internal bool isFirstLoad = true;
		/// <summary></summary>
		public override void OnInitializeMelon()
		{

			LoggerInstance.Msg("Initialized.");
		}
		/// <summary></summary>
		public override void OnUpdate()
		{
			Debug.DiffLog($"");
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			CurrentScene = sceneName.Normal();
			if (CurrentScene == "loader")
			{
				
			}

			if(CurrentScene == "gym" && isFirstLoad) FirstGymLoad();
			
		}
		
		internal void FirstGymLoad()
		{
			LoadAssetBundle();
			foreach (var tmpugui in UiAssets.GetComponentsInChildren<TextMeshProUGUI>(true))
				tmpugui.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/Arial SDF");
		}

		
		

	}
}