using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(UIFramework.Core), UIFramework.BuildInfo.Name, UIFramework.BuildInfo.Version, UIFramework.BuildInfo.Author)]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]
[assembly: MelonColor(255, 255, 255, 255)]
[assembly: MelonAuthorColor(255, 255, 255, 255)]


namespace UIFramework
{
	public static class BuildInfo
	{
		public const string Name = "Core";
		public const string Author = "Blankochan, iListen2Sound, TacoSlayer36";
		public const string Version = "1.0.0";
	}
	/// <summary>
	/// 
	/// </summary>
	public partial class Core : MelonMod
	{
		private string CurrentScene = "";
		/// <summary>
		/// 
		/// </summary>
		public override void OnInitializeMelon()
		{

			LoggerInstance.Msg("Initialized.");
		}
		/// <summary>
		/// 
		/// </summary>
		public override void OnUpdate()
		{
			Debug.DiffLog($"");
		}

		public static T LoadAssetFromStream<T>(MelonMod instance, string path, string assetName) where T : UnityEngine.Object
		{
			using (System.IO.Stream bundleStream = instance.MelonAssembly.Assembly.GetManifestResourceStream(path))
			{
				Il2CppSystem.IO.Stream Il2CppStream = ConvertToIl2CppStream(bundleStream);
				AssetBundle bundle = AssetBundle.LoadFromStream(Il2CppStream);
				Il2CppStream.Close();
				T asset = bundle.LoadAsset<T>(assetName);
				bundle.Unload(false);
				return asset;
			}
		}

		private static Il2CppSystem.IO.Stream ConvertToIl2CppStream(System.IO.Stream stream)
		{
			Il2CppSystem.IO.MemoryStream Il2CppStream = new Il2CppSystem.IO.MemoryStream();

			const int bufferSize = 4096;
			byte[] managedBuffer = new byte[bufferSize];
			Il2CppStructArray<byte> Il2CppBuffer = new(managedBuffer);

			int bytesRead;
			while ((bytesRead = stream.Read(managedBuffer, 0, managedBuffer.Length)) > 0)
			{
				Il2CppBuffer = managedBuffer;
				Il2CppStream.Write(Il2CppBuffer, 0, bytesRead);
			}
			Il2CppStream.Flush();
			return Il2CppStream;
		}

	}
}