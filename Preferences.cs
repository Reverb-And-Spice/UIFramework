using MelonLoader;

namespace UIFramework
{
	/// <summary>
	/// UIFramework's own preferences go here 
	/// </summary>
	internal static class Preferences
	{
		private const string CONFIG_FILE = "config.cfg";
		private const string USER_DATA = "UserData/UIFramework/";

		internal static MelonPreferences_Category CatUIFramework;
		internal static MelonPreferences_Entry<bool> EnableDebugLogs;
		
		internal static MelonPreferences_Category Experimental;
		internal static MelonPreferences_Entry<bool> TestBool;
		internal static MelonPreferences_Entry<string> TestString;
		internal static MelonPreferences_Entry<int> TestInt;
		internal static MelonPreferences_Entry<float> TestFloat;
		internal static MelonPreferences_Entry<double> TestDouble;
		internal static MelonPreferences_Entry<InputType> TestEnum;

		internal static void InitializePrefs()
		{
			if (!Directory.Exists(USER_DATA))
				Directory.CreateDirectory(USER_DATA);


			CatUIFramework = MelonPreferences.CreateCategory("UIFramework", "UIFramework Settings");
			CatUIFramework.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
			EnableDebugLogs = CatUIFramework.CreateEntry("EnableDebugLogs", true, "Enable Debug Logs", "Enables or disables debug logs for UIFramework. Useful for mod developers.");

			Experimental = MelonPreferences.CreateCategory("UIFrameworkExperimental", "Experimental Settings");
			Experimental.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
			TestBool = Experimental.CreateEntry("TestBool", false, "Test Bool", "This is a test bool.");
			TestString = Experimental.CreateEntry("TestString", "Hello, World!", "Test String", "This is a test string.");
			TestInt = Experimental.CreateEntry("TestInt", 42, "Test Int", "This is a test int.");
			TestFloat = Experimental.CreateEntry("TestFloat", 3.14f, "Test Float", "This is a test float.");
			TestDouble = Experimental.CreateEntry("TestDouble", 3.14159, "Test Double", "This is a test double.");
			TestEnum = Experimental.CreateEntry("TestEnum", InputType.TextField, "Test Enum", "This is a test enum.");

			CatUIFramework.SaveToFile();
			Experimental.SaveToFile();

		}

	}
}
