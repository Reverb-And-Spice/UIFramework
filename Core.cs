using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppPlayFab.ClientModels;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using static UIFramework.Debug;

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
		public const string Author = "Reverb && Spice";
		/// <summary></summary>
		public const string Version = "1.0.0";
	}

	
	/// <summary>
	/// 
	/// </summary>
	public partial class Core : MelonMod
	{
		internal static Core Instance;

		internal string CurrentScene = "";
		internal bool isFirstLoad = true;
		/// <summary></summary>
		public override void OnInitializeMelon()
		{

			LoggerInstance.Msg("Initialized.");
			Instance = this;
			
		}
		public override void OnLateInitializeMelon()
		{
			Preferences.InitializePrefs();
			UIFModel.ModelMod ModModel = UIFramework.Register(this, Preferences.CatUIFramework, Preferences.Experimental, Preferences.TestBooleans);
			UIFModel.IModelable tester = ModModel.GetSubmodel(Preferences.TestBooleans.Identifier);
			UIFModel.ButtonEntry testButton = new UIFModel.ButtonEntry("CustomButton");
			((UIFModel.ModelCategory)tester).AddEntry(testButton);

		}
		/// <summary></summary>
		public override void OnUpdate()
		{
			DiffLog($"");

			if(Input.GetKeyDown(KeyCode.F9))
			{
				UIFramework.MainWindow.SetActive(!UIFramework.MainWindow.activeSelf);
			}
		}

		

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			CurrentScene = sceneName.ToNormal();
			if (CurrentScene == "loader")
			{

			}

			if (CurrentScene == "gym" && isFirstLoad) FirstGymLoad();

			if(!isFirstLoad)
				UIFramework.MainWindow.SetActive(false); 
			
			
		}

		internal void FirstGymLoad()
		{
			Prefabs.LoadAssetBundle();

			UIFramework.InitializeUIObjects();
			UIFramework.BuildUI();
			isFirstLoad = false;

			UIFramework.MainWindow.SetActive(false);
		}


	}
}	