using MelonLoader;
using UnityEngine;
using System.ComponentModel.DataAnnotations;
using UIFramework.UiExtensions;
using System.Collections;
using Il2CppTMPro;
using UIFramework.Adapters;
namespace UIFramework
{
	/// <summary>
	/// UI's own preferences go here
	/// </summary>
	internal static class Preferences
	{
		private const string CONFIG_FILE = "config.cfg";
		private const string USER_DATA = "UserData/UIFramework/";

		internal static MelonPreferences_Category CatUIFramework;
		internal static MelonPreferences_Entry<bool> EnableDebugMode;
		internal static MelonPreferences_Entry<bool> AutoHideOnSceneLoad;
		internal static MelonPreferences_Entry<bool> AutoHideOnInactivity;
		internal static MelonPreferences_Entry<int> FadeTimer;
		internal static MelonPreferences_Entry<bool> VrInputToggle;
		internal static MelonPreferences_Entry<bool> HijackModUI;

		internal static MelonPreferences_Entry<ToggleOptions> ToggleSettings;
		internal static MelonPreferences_Entry<int> InactivityTimeout;

		internal static MelonPreferences_Category Footprint;
		internal static MelonPreferences_Entry<Vector2> UiPosition;
		internal static MelonPreferences_Entry<Vector3> UiScale;
		internal static MelonPreferences_Entry<float> UiHeight;


		internal static MelonPreferences_Category Experimental;
		internal static MelonPreferences_Entry<Color> ExperimentalColor;
		internal static MelonPreferences_Entry<float> ExperimentalSlider;
		internal static MelonPreferences_Entry<int> ExperimentalIntSlider;
		internal static MelonPreferences_Entry<bool> TestBool;
		internal static MelonPreferences_Entry<string> TestString;
		internal static MelonPreferences_Entry<int> TestInt;
		internal static MelonPreferences_Entry<float> TestFloat;
		internal static MelonPreferences_Entry<double> TestDouble;
		internal static MelonPreferences_Entry<PrefabType> TestEnum;
		internal static MelonPreferences_Entry<List<int>> TestList;
		internal static MelonPreferences_Entry<List<string>> TestListString;
		internal static MelonPreferences_Entry<NonZeroBased> NonZeroEnum;
		internal static MelonPreferences_Entry<NonContiguous> NonContiguousEnum;

		internal static MelonPreferences_Category Demo;
		internal static MelonPreferences_Entry<int> DemoTextBehavior;
		internal static MelonPreferences_Entry<string> PasswordDemo;

		internal static MelonPreferences_Entry<int> DropdownTest;
		internal static MelonPreferences_Entry<int> DropdownSelection;
		internal static MelonPreferences_Entry<bool> ShowReactivity;
		internal static MelonPreferences_Entry<bool> DemoCounting;
		internal static MelonPreferences_Entry<bool> InhibitRefreshForCount;
		internal static MelonPreferences_Entry<int> DemoInt;
		internal static MelonPreferences_Entry<string> DemoString;

		internal static RefreshInhibitor InhibitRefresh = new() { InhibitRefreshOnValueChange = true };

		internal static MelonPreferences_Category TestEmptyDisplayName;
		internal static MelonPreferences_Entry<string> TestEmptyDisplayPref;


		internal static MelonPreferences_Category TestBooleans;
		internal static List<MelonPreferences_Entry<bool>> TestBoolList = new();

		internal static void InitializePrefs()
		{
			if (!Directory.Exists(USER_DATA))
				Directory.CreateDirectory(USER_DATA);


			CatUIFramework = MelonPreferences.CreateCategory("UI", "UI Settings");
			CatUIFramework.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
			AutoHideOnSceneLoad = CatUIFramework.CreateEntry("AutoHideOnSceneLoad", true, "Auto Hide On Scene Load", "Hides the UI Automatically in between scenes.");
			AutoHideOnInactivity = CatUIFramework.CreateEntry("AutohideOnInactivity", true, "(Deprecated)Auto Hide on Inactivity", "Hide the UI if mouse and keyboard are inactive", true);
			FadeTimer = CatUIFramework.CreateEntry("FadeTimer", 10, "Fade Out Time", "Fade UI after this many seconds.\nSet 0 to disable", false, false);
			InactivityTimeout = CatUIFramework.CreateEntry("InactivityTimeout", 30, "Inactivity Time Out", "Hide UI After this many seconds.\nSet 0 to disable", false, false);

			
			VrInputToggle = CatUIFramework.CreateEntry("VrInputToggle", false, "Toggle with VR buttons", "Toggle UI window by pressing both trigger and primary (A/X) on both hands", true);

			CatUIFramework.CreateEntry("ᜃᜑᜒᜊᜎᜓ ᜀᜅ᜕ ᜊᜃᜒ", true, "Consolas", "anuraaviascit", true);




			ToggleSettings = CatUIFramework.CreateEntry("ToggleSettings", ToggleOptions.Keyboard, "Toggle Options", "Select the input method for toggling the UI.\n" +
				"Keyboard: F9 \n" +
				"VR: Press both trigger and primary (A/X) buttons on both hands\n" +
				"<sup>*If you have ModUI, VR Input matches ModUI. UI Framework will be visible when ModUI is visible\n</sup>");

			HijackModUI = CatUIFramework.CreateEntry("HijackModUI", false, "Force Hide ModUI", "If enabled, UI Framework will find the ModUI object and hide it whenever UI Framework is also hidden.\n" +
				"<size=75%>*Might cause unintended effects. Next ModUI Toggle will need to be done twice</size>");


			EnableDebugMode = CatUIFramework.CreateEntry("EnableDebugMode", false, "Enable Debug Logs", "Enables or disables debug logs for UIFramework.", false, false, new UserEditNotifier { OnUserEdit = UpdateCategoryVis });

			Footprint = MelonPreferences.CreateCategory("UIF_Footprint", "Footprint Settings", true);
			Footprint.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));

			UiPosition = Footprint.CreateEntry("UiPosition", new Vector2(970, -128f), "UI Position", null, true);
			UiScale = Footprint.CreateEntry("UiScale", Vector3.one, "UI Scale", null, true);
			UiHeight = Footprint.CreateEntry("UiHeight", 600f, "UI Height", null, true);

			Experimental = MelonPreferences.CreateCategory("UIFrameworkExperimental", "Experimental Settings", true);
			Experimental.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));


			ExperimentalColor = Experimental.CreateEntry("ColorTest", new UnityEngine.Color(50, 238, 165, 255), "Test color", "ColorTest");
			UI.CreateButtonEntry(Experimental, "Test Buttton", "", "This is a test button entry that should not be treated as an actual preference and should not be saved to the config file.", TestButtonAsEntry);

			ExperimentalSlider = Experimental.CreateEntry("SliderTest", 0.5f, "Test Slider", "This is a test slider with a range from 0 to 1", false, false, new UiExtensions.SliderDescriptor { Min = 0, Max = 1, DecimalPlaces = 3 });

			ExperimentalIntSlider = Experimental.CreateEntry("IntSliderTest", 50, "Test Int Slider", "This is a test int slider with a range from 0 to 100", false, false, new UiExtensions.SliderDescriptor { Min = 0, Max = 100 });
			TestBool = Experimental.CreateEntry("TestBool", false, "Test Bool", "This is a test bool.");
			TestString = Experimental.CreateEntry("TestString", "Hello, World!", "Test String", "This is a test string.");
			TestInt = Experimental.CreateEntry("TestInt", 42, "Test Int", "This is a test int.");
			TestFloat = Experimental.CreateEntry("TestFloat", 3.14f, "Test Float", "This is a test float.");
			TestDouble = Experimental.CreateEntry("TestDouble", 3.14159, "Test Double", "This is a test double.", false, false, new NumberBoxDescriptor { DecimalPlaces = 5 });
			TestEnum = Experimental.CreateEntry("TestEnum", PrefabType.TextField, "Test Enum", "This is a test enum.");
			NonZeroEnum = Experimental.CreateEntry("Non-Zero", NonZeroBased.a, "Non-zero-based enum test", "This tests enums that don't start from zero");
			NonContiguousEnum = Experimental.CreateEntry("Non-Cont", NonContiguous.z, "Non-Contiguous enum test", "This tests enums that have gaps in between the explicitlyi named values");
			TestList = Experimental.CreateEntry("TestList", new List<int> { 1, 2, 3 }, "Test List", "This is a test list of integers.", true);
			TestListString = Experimental.CreateEntry("TestStringList", new List<string> { "hello", "world", "hi" }, "this is a test list of strings");

			TestEmptyDisplayName = MelonPreferences.CreateCategory("EmptyDisplayName", "Empty Display Name Test", true);
			TestEmptyDisplayName.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));

			TestEmptyDisplayPref = TestEmptyDisplayName.CreateEntry("NoDisplayName", "Hello, World!", null, "This is a test string.");

			CatUIFramework.SaveToFile();
			Experimental.SaveToFile();

			TestBooleans = MelonPreferences.CreateCategory("TestBooleans", "Test Booleans", true);
			TestBooleans.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
			for (int i = 0; i < 10; i++)
			{
				TestBoolList.Add(TestBooleans.CreateEntry("TestBool" + i, false, "Test Bool " + i));
			}

			Demo = MelonPreferences.CreateCategory("UIF_Demo", "Demos");
			Demo.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));

			DemoTextBehavior = Demo.CreateEntry("TextBehaviorDemo",5, "Custom Behavior Demo",
				"This tests the BehaviorDescriptor Interface", false, false,
				new TextInputBehaviorDescriptor
					{ ContentType = TMP_InputField.ContentType.IntegerNumber, CharacterLimit = 20, IsReadOnly = true });

			PasswordDemo = Demo.CreateEntry("PasswordDemo", "password", "Password Demo", "This is a demo for the password field behavior descriptor", false, false, new TextInputBehaviorDescriptor { ContentType = TMP_InputField.ContentType.Password });

			UI.CreateButtonEntry(Demo, "Add Entry", "Dropdown Test", "this should add another entry to the dropdown demo", AddDropdownDemo);

			DropdownTest = Demo.CreateEntry("DropdownDemo", -1, "Dropdown Demo", "Dynamic dropdown test. Add items by clicking the button", false, false,
				demoDropdownDescriptor.WithDropdownItemList(demoItems));
			DropdownSelection = Demo.CreateEntry("SelectedDropdownItem", DropdownTest.Value, "Selected Item", "The item selected in the dropdown demo");
			ShowReactivity = Demo.CreateEntry("Entry_Reactivity", false, "Show hidden Entry", "This is a demo preference to hide the reactivity demo button in the demo category.", false, false,
				new UserEditNotifier
					{ OnUserEdit = HideReaction });
			DemoCounting = Demo.CreateEntry("EnableCount", false, "Enable counting demo", "This is a demo for how the UI can update by a change of values in the background", false, false,
				new UserEditNotifier
					{ OnUserEdit = (newVal) => { DemoCounting.EditedValue = (bool)newVal; UI.RequestRefresh(Core.Instance); } });
			DemoInt = Demo.CreateEntry("DemoInt", 0, "Demo Int", "This is a demo int Preference", false, false, InhibitRefresh);
			InhibitRefreshForCount = Demo.CreateEntry("InhibitDemoCount", true, "Inhibit Count Refresh", "Inhibit refreshing the UI when the demo int counts up", false, false, new UserEditNotifier { OnUserEdit = UpdateInhibitDemo });
			DemoString = Demo.CreateEntry("DemoString", "Hello, World!", "Demo String", "This is a demo string preference. This should show the name of the current scene", true);

			DemoInt.Value = 0;
			DemoString.IsHidden = !ShowReactivity.Value;
			

			UpdateCategoryVis(EnableDebugMode.Value);
			InhibitRefreshForCount.Value = true;
			DemoCounting.Value = false;
			MelonCoroutines.Start(DemoCount());

			if (!AutoHideOnInactivity.Value)
			{
				Debug.Log("AutoHideOnInactivity is deprecated. Applying setting to InactivityTimeout and deleting", false, 1);
				InactivityTimeout.Value = 0;
			}

			CatUIFramework.DeleteEntry("AutohideOnInactivity");
			DropdownTest.OnEntryValueChanged.Subscribe(ItemSelectionChanged);
		}
		static List<DropdownItem> demoItems = new() { new DropdownItem("zero", 0), new DropdownItem("one", 1) };


		public static DynamicDropdownDescriptor demoDropdownDescriptor = new();
		internal static void AddDropdownDemo()
		{
			int nextItem = demoDropdownDescriptor.GetDropdownItems().Count;
			string nextName = new('+', nextItem);

			demoDropdownDescriptor.AddDropdownItem(new DropdownItem(" - " + nextName, nextItem));

			//demoDropdownDescriptor.SetDropdownItems(demoItems);
		}

		internal static void ItemSelectionChanged(int old, int neew)
		{
			Debug.Log($"Selected Item: {neew}");
			DropdownSelection.Value = neew;
		}
		internal static void TestButtonAsEntry()
		{
			Debug.Log($"This is a test for submitting a button as a preference entry. \n" +
				$"This should not be treated as an actual preference and should not be saved to the config file.", true);
		}
		internal static void HideReaction(object newValue)
		{
			Debug.Log($"HideReact: {newValue}", false);
			DemoInt.IsHidden = !(bool)newValue;
			ShowReactivity.Value = (bool)newValue;
		}

		internal static IEnumerator DemoCount()
		{
			while (true)
			{
				yield return new WaitForSeconds(1.5f);
				if (DemoCounting.EditedValue)
					DemoInt.Value++;
			}
		}


		internal static void UpdateInhibitDemo(object newValue)
		{
			InhibitRefresh.InhibitRefreshOnValueChange = (bool)newValue;
		}

		internal static void UpdateCategoryVis(object newValue)
		{
			Debug.Log($"UpdateCategoryVis newValue = {newValue}", true);

			Debug.Log($"UpdateCategoryVis Demo.IsHidden = {Demo.IsHidden}", true);
			Experimental.IsHidden = !(bool)newValue;
			TestBooleans.IsHidden = !(bool)newValue;
			TestEmptyDisplayName.IsHidden = !(bool)newValue;
		}


	}

	internal enum NonZeroBased
	{
		a = 3,
		b,
		c,
		d,
	}
	internal enum NonContiguous
	{
		z = 3,
		y = 5,
		x = 10,
		w = 13,

	}

	internal enum ToggleOptions
	{
		[Display(Name = "Keyboard Only")]
		Keyboard,
		[Display(Name = "VR Only")]
		VR,
		[Display(Name = "VR And Keyboard")]
		KeyAndVR,
	}
}
