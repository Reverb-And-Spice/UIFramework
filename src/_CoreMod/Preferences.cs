using MelonLoader;
using UnityEngine;
using System.ComponentModel.DataAnnotations;
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
		internal static MelonPreferences_Entry<bool> VrInputToggle;
		internal static MelonPreferences_Entry<bool> HijackModUI;

		internal static MelonPreferences_Entry<ToggleOptions> ToggleSettings;
		internal static MelonPreferences_Entry<int> InactivityTimeout;
		internal static MelonPreferences_Entry<Vector2> UiPosition;
		
		internal static MelonPreferences_Category Experimental;
		internal static MelonPreferences_Entry<Color> ExperimentalColor;
		internal static MelonPreferences_Entry<float> ExperimentalSlider;
		internal static MelonPreferences_Entry<int> ExperimentalIntSlider;
		internal static MelonPreferences_Entry<bool> TestBool;
		internal static MelonPreferences_Entry<string> TestString;
		internal static MelonPreferences_Entry<int> TestInt;
		internal static MelonPreferences_Entry<float> TestFloat;
		internal static MelonPreferences_Entry<double> TestDouble;
		internal static MelonPreferences_Entry<InputType> TestEnum;
		internal static MelonPreferences_Entry<List<int> > TestList;
		internal static MelonPreferences_Entry<List<string>> TestListString;
		internal static MelonPreferences_Entry<NonZeroBased> NonZeroEnum;
		internal static MelonPreferences_Entry<NonContiguous> NonContiguousEnum;

		internal static MelonPreferences_Category TestEmptyDisplayName;
		internal static MelonPreferences_Entry<string> TestEmptyDisplayPref;


		internal static MelonPreferences_Category TestBooleans;
		internal static List<MelonPreferences_Entry<bool>> TestBoolList = new List<MelonPreferences_Entry<bool>>();

		internal static void InitializePrefs()
		{
			if (!Directory.Exists(USER_DATA))
				Directory.CreateDirectory(USER_DATA);


			CatUIFramework = MelonPreferences.CreateCategory("UI", "UI Settings");
			CatUIFramework.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
			AutoHideOnSceneLoad = CatUIFramework.CreateEntry("AutoHideOnSceneLoad", true, "Auto Hide On Scene Load", "Hides the UI Automatically in between scenes.");
			AutoHideOnInactivity = CatUIFramework.CreateEntry("AutohideOnInactivity", true, "Auto Hide on Inactivity", "Hide the UI if mouse and keyboard are inactive");
			InactivityTimeout = CatUIFramework.CreateEntry("InactivityTimeout", 30, "Inactivity Time Out (Seconds)", "Number of seconds of inactivity for UI to hide automatically");
			VrInputToggle = CatUIFramework.CreateEntry("VrInputToggle", false, "Toggle with VR buttons", "Toggle UI window by pressing both trigger and primary (A/X) on both hands",true);
			
			ToggleSettings = CatUIFramework.CreateEntry("ToggleSettings", ToggleOptions.Keyboard, "Toggle Options", "Select the input method for toggling the UI.\n" +
				"Keyboard: F9 \n" +
				"VR: Press both trigger and primary (A/X) buttons on both hands\n" +
				"<sup>*If you have ModUI, VR Input matches ModUI. UI Framework will be visible when ModUI is visible\n</sup>");
			
			HijackModUI = CatUIFramework.CreateEntry("HijackModUI", false, "Force Hide ModButtonView UI", "If enabled, UI Framework will find the ModUI object and hide it whenever UI Framework is also hidden.\n" +
				"<size=75%>*Might cause unintended effects. Next ModUI toggle will need to be done twice</size>");

			
			EnableDebugMode = CatUIFramework.CreateEntry("EnableDebugMode", false, "Enable Debug Logs", "Enables or disables debug logs for UIFramework.");
			UiPosition = CatUIFramework.CreateEntry("UiPosition", new Vector2(970, -128f), "UI Position", "The position of the UI on the screen represented",true, true);

			Experimental = MelonPreferences.CreateCategory("UIFrameworkExperimental", "Experimental Settings");
			Experimental.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));


			ExperimentalColor = Experimental.CreateEntry("ColorTest", new UnityEngine.Color(50, 238, 165,255),"Test color", "ColorTest");
			UI.CreateButtonEntry(Experimental, "Test Buttton", "", "This is a test button entry that should not be treated as an actual preference and should not be saved to the config file.", TestButtonAsEntry);
			
			ExperimentalSlider = Experimental.CreateEntry("SliderTest", 0.5f, "Test Slider", "This is a test slider with a range from 0 to 1",false, false, new ValidatorExtensions.SliderDescriptor { Min = 0, Max = 1, DecimalPlaces = 3 }); 

			ExperimentalIntSlider = Experimental.CreateEntry("IntSliderTest", 50, "Test Int Slider", "This is a test int slider with a range from 0 to 100", false, false, new ValidatorExtensions.SliderDescriptor { Min = 0, Max = 100 });
			TestBool = Experimental.CreateEntry("TestBool", false, "Test Bool", "This is a test bool.");
			TestString = Experimental.CreateEntry("TestString", "Hello, World!", "Test String", "This is a test string.");
			TestInt = Experimental.CreateEntry("TestInt", 42, "Test Int", "This is a test int.");
			TestFloat = Experimental.CreateEntry("TestFloat", 3.14f, "Test Float", "This is a test float.");
			TestDouble = Experimental.CreateEntry("TestDouble", 3.14159, "Test Double", "This is a test double.");
			TestEnum = Experimental.CreateEntry("TestEnum", InputType.TextField, "Test Enum", "This is a test enum.");
			NonZeroEnum = Experimental.CreateEntry("Non-Zero", NonZeroBased.a,"Non-zero-based enum test", "This tests enums that don't start from zero");
			NonContiguousEnum = Experimental.CreateEntry("Non-Cont", NonContiguous.z, "Non-Contiguous enum test", "This tests enums that have gaps in between the explicitlyi named values");
			TestList = Experimental.CreateEntry("TestList", new List<int> { 1, 2, 3 }, "Test List", "This is a test list of integers.",true);
			TestListString = Experimental.CreateEntry("TestStringList", new List<string> { "hello", "world", "hi" }, "this is a test list of strings"); 
			
			TestEmptyDisplayName = MelonPreferences.CreateCategory("EmptyDisplayName");
			TestEmptyDisplayName.SetFilePath(Path.Combine(USER_DATA,CONFIG_FILE));

			TestEmptyDisplayPref = TestEmptyDisplayName.CreateEntry("NoDisplayName", "Hello, World!", null, "This is a test string."); 

			CatUIFramework.SaveToFile();
			Experimental.SaveToFile();

			TestBooleans = MelonPreferences.CreateCategory("TestBooleans", "Test Booleans");
			TestBooleans.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
			for (int i = 0; i < 10; i++)
			{
				TestBoolList.Add(TestBooleans.CreateEntry("TestBool" + i, false, "Test Bool " + i));
			}
		}
		internal static void TestButtonAsEntry()
		{
			Debug.Log($"This is a test for submitting a button as a preference entry. \n" +
				$"This should not be treated as an actual preference and should not be saved to the config file.", true);
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
