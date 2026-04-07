using MelonLoader;
using UnityEngine.Bindings;

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
		internal static MelonPreferences_Entry<int> InactivityTimeout;
		
		internal static MelonPreferences_Category Experimental;
		internal static MelonPreferences_Entry<bool> TestBool;
		internal static MelonPreferences_Entry<string> TestString;
		internal static MelonPreferences_Entry<int> TestInt;
		internal static MelonPreferences_Entry<float> TestFloat;
		internal static MelonPreferences_Entry<double> TestDouble;
		internal static MelonPreferences_Entry<InputType> TestEnum;
		internal static MelonPreferences_Entry<List<int> > TestList;
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
			EnableDebugMode = CatUIFramework.CreateEntry("EnableDebugMode", false, "Enable Debug Logs", "Enables or disables debug logs for UIFramework.");

			Experimental = MelonPreferences.CreateCategory("UIFrameworkExperimental", "Experimental Settings");
			Experimental.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
			TestBool = Experimental.CreateEntry("TestBool", false, "Test Bool", "This is a test bool.");
			TestString = Experimental.CreateEntry("TestString", "Hello, World!", "Test String", "This is a test string.");
			TestInt = Experimental.CreateEntry("TestInt", 42, "Test Int", "This is a test int.");
			TestFloat = Experimental.CreateEntry("TestFloat", 3.14f, "Test Float", "This is a test float.");
			TestDouble = Experimental.CreateEntry("TestDouble", 3.14159, "Test Double", "This is a test double.");
			TestEnum = Experimental.CreateEntry("TestEnum", InputType.TextField, "Test Enum", "This is a test enum.");
			NonZeroEnum = Experimental.CreateEntry("Non-Zero", NonZeroBased.a,"Non-zero-based enum test", "This tests enums that don't start from zero");
			NonContiguousEnum = Experimental.CreateEntry("Non-Cont", NonContiguous.z, "Non-Contiguous enum test", "This tests enums that have gaps in between the explicitlyi named values");
			TestList = Experimental.CreateEntry("TestList", new List<int> { 1, 2, 3 }, "Test List", "This is a test list of integers.");

			
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
}
