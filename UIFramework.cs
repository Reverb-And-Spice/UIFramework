using MelonLoader;

[assembly: MelonInfo(typeof(UIFramework.UIFramework), UIFramework.BuildInfo.Name, UIFramework.BuildInfo.Version, UIFramework.BuildInfo.Author)]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]
[assembly: MelonColor(255, 255, 255, 255)]
[assembly: MelonAuthorColor(255, 255, 255, 255)]


namespace UIFramework
{
	public static class BuildInfo
	{
		public const string Name = "UIFramework";
		public const string Author = "Blankochan, iListen2Sound, TacoSlayer36";
		public const string Version = "1.0.0";
	}

	public partial class UIFramework : MelonMod
	{
		private string CurrentScene = "";
		
		public override void OnInitializeMelon()
		{

			LoggerInstance.Msg("Initialized.");
		}

		public override void OnUpdate()
		{
			Debug.DiffLog($"");

		}




	}
}