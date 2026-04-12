using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppPlayFab.ClientModels;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using static UIFramework.Debug;
using System.Diagnostics;
using UnityEngine.InputSystem;

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
		public const string Version = "0.6.1";
	}


	/// <summary>
	/// 
	/// </summary>
	public partial class Core : MelonMod
	{
		internal static Core Instance;

		internal string CurrentScene = "";
		internal bool isFirstLoad = true;


		internal Stopwatch displayTime = new Stopwatch();

		internal int inactiveTimeLimit => (Preferences.InactivityTimeout?.Value * 1000) ?? 30000;


#pragma warning disable CS1591
		public override void OnInitializeMelon()
		{

			LoggerInstance.Msg("Initialized.");
			Instance = this;
			MelonPreferences.OnPreferencesSaved.Subscribe(MelPrefsSaved);

		}
		public override void OnLateInitializeMelon()
		{ 

		}

		public override void OnUpdate()
		{
			UiToggleInputCheck();

			if (isFirstLoad)
				return;

			AutoHideCheck();

		}
		private void UiToggleInputCheck()
		{
			if (!Input.GetKeyDown(KeyCode.F9))
				return;

			if (CurrentScene == "loader")
			{
				Debug.Warning("UIFramework does not work in the loader. Please finish calibrating.");
				return;
			}
			UI.MainWindow.SetActive(!UI.MainWindow.activeSelf);

		}

		private void AutoHideCheck()
		{
			//Don't proceed if Autohide preference is set to false or null
			//Don't proceed if UI is inactive. Reset stopwatch if running
			if (!UI.MainWindow.activeSelf || Preferences.AutoHideOnInactivity?.Value != true)
			{
				if (displayTime.IsRunning)
					displayTime.Reset();
				return;
			}

			//Stop and reset stopwatch if user has interacted with mouse or keyboard
			if (UserInteracted())
			{
				if (displayTime.IsRunning)
					displayTime.Reset();
			}

			//Start stopwatch if user stopped interacting
			else
			{
				if (!displayTime.IsRunning)
					displayTime.Start();
			}

			//Once user hasn't interacted with mouse or keyboard abev the inactive time limit, hide the UI window
			if (displayTime.ElapsedMilliseconds >= inactiveTimeLimit)
				UI.MainWindow.SetActive(false);
		}

		private bool UserInteracted()
		{
			if (Mouse.current != null)
			{
				Vector2 delta = Mouse.current.delta.ReadValue();
				if (delta.sqrMagnitude > 0)
				{
					//DiffLog("Interaction Detected", true);
					return true;
				}
			}

			if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
			{
				//DiffLog("Interaction Detected", true);
				return true;
			}
			//DiffLog("No Interaction Detected", true);
			return false;
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			CurrentScene = sceneName.ToNormal();
			if (CurrentScene == "loader")
			{

			}

			if (CurrentScene == "gym" && isFirstLoad) FirstGymLoad();

			if (!isFirstLoad)
			{
				if (UI.MainWindow.activeSelf)
				{
					UI.MainWindow.SetActive(!Preferences.AutoHideOnSceneLoad.Value);
				}
			}
		}
#pragma warning restore CS1591




		internal void FirstGymLoad()
		{
			BuildUI();


			isFirstLoad = false;
		}

		internal void BuildUI()
		{
			Preferences.InitializePrefs();
			UIFModel.ModelMod ModModel;

			//Show extra categories if debug mode is enabled
			if (!Preferences.EnableDebugMode.Value)
			{
				ModModel = UI.Register(this, Preferences.CatUIFramework);
			}

			else
			{
				ModModel = UI.Register(this, Preferences.CatUIFramework, Preferences.Experimental, Preferences.TestBooleans, Preferences.TestEmptyDisplayName);
				UIFModel.ModelMelonCategory tester = (UIFModel.ModelMelonCategory)ModModel.GetSubmodel(Preferences.TestBooleans.Identifier);
				UIFModel.ButtonEntry testButton = new UIFModel.ButtonEntry(CustomClick, "CustomButton", "just a test", "Custom Button");
				tester.AddSubmodel(testButton);
			}

			Prefabs.LoadAssetBundle();

			UI.InitializeUIObjects();
			UI.BuildUI();

			UI.MainWindow.SetActive(false);
		}

		public void MelPrefsSaved(string s)
		{

		}

		public void CustomClick(UIFController.Entry button)
		{
			Debug.Log($"Clicked: {button.DisplayName} ");
		}
		private void SinglesaveClick()
		{
			Debug.Log("Clicked Single Save Button");
		}
	}
}