using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppPlayFab.ClientModels;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using static UIFramework.Debug;
using System.Diagnostics;
using UnityEngine.InputSystem;
using UnityEngine.Jobs;
using System.Collections;

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
		public const string Version = "0.7.1";
	}


	/// <summary>
	/// 
	/// </summary>
	public partial class Core : MelonMod
	{
		internal static Core Instance;

		internal string CurrentScene = "";
		internal bool isFirstLoad = true;
		internal GameObject ModUIWindow;

		internal Stopwatch displayTime = new Stopwatch();

		internal int inactiveTimeLimit => (Preferences.InactivityTimeout?.Value * 1000) ?? 30000;


#pragma warning disable CS1591
		public override void OnInitializeMelon()
		{
			rightGrip.AddBinding("<XRController>{RightHand}/grip");
			rightPrimary.AddBinding("<XRController>{RightHand}/primaryButton");
			leftGrip.AddBinding("<XRController>{LeftHand}/grip");
			leftPrimary.AddBinding("<XRController>{LeftHand}/primaryButton");
			map.Enable();
			Instance = this;
			MelonPreferences.OnPreferencesSaved.Subscribe(MelPrefsSaved);
			LoggerInstance.Msg("Initialized.");

		}
		public override void OnLateInitializeMelon()
		{ 

		}

		public override void OnUpdate()
		{
			UiToggleInputCheck();

			if (isFirstLoad)
				return;
			//Debug.DiffLog($"UI is Visible {UI.IsVisible}",true);
			AutoHideCheck();

		}
		private void UiToggleInputCheck()
		{
			if (!(Input.GetKeyDown(KeyCode.F9) || VRActivationAction()))
				return;

			if (CurrentScene == "loader")
			{
				Debug.Warning("UIFramework does not work in the loader. Please finish calibrating.");
				return;
			}
			UI.MainWindow.SetActive(!UI.MainWindow.activeSelf);

		}

		#region Baumritter-generated
		//VR input variables
		private static InputActionMap map = new InputActionMap("Tha Map");
		private static InputAction rightGrip = map.AddAction("Right Trigger");
		private static InputAction rightPrimary = map.AddAction("Right Primary");
		private static InputAction leftGrip = map.AddAction("Left Trigger");
		private static InputAction leftPrimary = map.AddAction("Left Primary");
		private bool VRButtonsPressed = false;
		/// <summary>
		/// Checks if activation by VR input has been pressed
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// Code yoinked from Baumritter's ModUI
		/// </remarks>
		private bool VRActivationAction()
        {
			if (isFirstLoad)
				return false;
			if (!Preferences.VrInputToggle.Value)
				return false;
            float High = 0.9f;
            float Low = 0.1f;
			float tRightDepress = rightGrip.ReadValue<float>();
			float pRightPress = rightPrimary.ReadValue<float>();
			float tLeftDepress = leftGrip.ReadValue<float>();
			float pLeftPress = rightPrimary.ReadValue<float>();

/*			Debug.DiffLog($"TR: {tRightDepress}\n" +
				$"PR: {pRightPress}\n" +
				$"TL: {tLeftDepress}\n" +
				$"PL: {pLeftPress}", true);*/



			if (tRightDepress >= High && pRightPress >= High && tLeftDepress >= High && pLeftPress >= High && !VRButtonsPressed)
            {
                VRButtonsPressed = true;
                return true;
            }
            if (tRightDepress <= Low && pRightPress <= Low && tLeftDepress <= Low && pLeftPress <= Low && VRButtonsPressed)
            {
                VRButtonsPressed = false;
            }
            return false;

        }
		#endregion
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
			{
				UI.MainWindow.SetActive(false);
				if(Preferences.HijackModUI.Value)
				{
					ModUIWindow?.SetActive(false);
				}
			}
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
			Debug.Log($"Current buildIndex = {buildIndex} was loaded. sceneName = {sceneName}");
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
					ModUIWindow?.SetActive(!(Preferences.HijackModUI.Value && Preferences.AutoHideOnSceneLoad.Value));
				}
			}
		}
#pragma warning restore CS1591




		internal void FirstGymLoad()
		{
			BuildUI();
			MelonCoroutines.Start(FindModUI());

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
			UI.MainWindow.SetActive(false);
			UI.BuildUI();

		}

		public IEnumerator FindModUI()
		{
			yield return null;
			GameObject uiObject = GameObject.Find("Game Instance/UI");
			GameObject modUiWindow = uiObject.transform.Find("Mod_Setting_UI").gameObject;
			ModUIWindow = modUiWindow;
		
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
	[AttributeUsage(AttributeTargets.Assembly)]
	public class UIInfoAttribute : Attribute
	{
		public string Name { get; }

		public UIInfoAttribute(string name)
		{
			Name = name;
		}
	}
}