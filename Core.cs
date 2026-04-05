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
		public const string Version = "0.6.0";
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

		internal int inactiveTimeLimit = (InactivityTimeout?.Value * 1000) ?? 30000;


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

		internal void ToggleUI()
		{
			if(UI.MainWindow.activeSelf)
				HideUI();
			else
				ShowUI();
		}

		internal void ShowUI()
		{
			UI.MainWindow.SetActive(true);
		}
		internal void HideUI()
		{
			UI.MainWindow.SetActive(false);
		}

		public override void OnUpdate()
		{
			if (Input.GetKeyDown(KeyCode.F9))
			{
				if (CurrentScene == "loader")
				{
					Debug.Warning("UIFramework does not work in the loader. Please finish calibrating.");
					return;
				}
				ToggleUI();
			}
			if(isFirstLoad)
				return;

			
			if(!AutoHideOnInactivity?.Value ?? false)
			{ 
				if(UI.MainWindow.activeSelf)
				{

					if(userInteracted)
						if(displayTime.isRunning)
							displayTime.Reset();
					else
						if(!displayTime.isRunning)
							displayTime.Start();	

					if (displayTime.ElapsedMilliseconds > inactiveTimeLimit)
					{
						HideUI();
					}
				}
			}

		}

		private bool userInteracted()
		{
			if(Mouse.current != null)
			{
				Vector2 delta = Mouse.current.delta.ReadValue();
				if (delta.sqrMagnitude > 0)
				{
					return true;
				}
			}

			if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
			{
				return true;
			}
			false;
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
#pragma warning restore CS1591
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




		internal void FirstGymLoad()
		{
			BuildUI();


			isFirstLoad = false;
		}

		internal void BuildUI()
		{
			Preferences.InitializePrefs();
			UIFModel.ModelMod ModModel;
			if (Preferences.EnableDebugMode.Value)
			{
				ModModel = UI.Register(this, Preferences.CatUIFramework, Preferences.Experimental, Preferences.TestBooleans, Preferences.TestEmptyDisplayName);
				UIFModel.ModelMelonCategory tester = (UIFModel.ModelMelonCategory)ModModel.GetSubmodel(Preferences.TestBooleans.Identifier);
				UIFModel.ButtonEntry testButton = new UIFModel.ButtonEntry(CustomClick, "CustomButton", "just a test", "Custom Button");
				tester.AddSubmodel(testButton);
			}
			else
			{
				ModModel = UI.Register(this, Preferences.CatUIFramework);
			}



			Prefabs.LoadAssetBundle();

			UI.InitializeUIObjects();
			UI.BuildUI();

			UI.MainWindow.SetActive(false);
		}

		public void MelPrefsSaved(string s)
		{
			//Debug.Deb("MelPrefsSaved called " + s);
		}

		public void CustomClick(UIFController.Entry button)
		{
			Debug.Log($"Clicked: {button.DisplayName} ");
		}
	}
}