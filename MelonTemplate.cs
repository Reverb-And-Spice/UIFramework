using MelonLoader;
using RumbleModdingAPI;

[assembly: MelonInfo(typeof(UIFramework.UIFramework), UIFramework.BuildInfo.Name, UIFramework.BuildInfo.Version, UIFramework.BuildInfo.Author)]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]
[assembly: MelonColor(255, 255, 255, 255)]
[assembly: MelonAuthorColor(255, 255, 255, 255)]


namespace UIFramework
{
	public static class BuildInfo
	{
		public const string Name = "UIFramework";
		public const string Author = "Author";
		public const string Version = "1.0.0";
	}

	public partial class UIFramework : MelonMod
	{
		private string CurrentScene = "";
		private string lastDiffLogMessage = string.Empty;
		public override void OnInitializeMelon()
		{
			Calls.onAMapInitialized += OnMapInitialized;
			LoggerInstance.Msg("Initialized.");
		}

		private void OnMapInitialized(string sceneName)
		{
			CurrentScene = sceneName;
			BuildDebugScreen();

		}
		public override void OnUpdate()
		{
			DiffLog($"");

		}




	}
}