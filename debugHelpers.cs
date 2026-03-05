using Il2CppTMPro;
using UnityEngine;
using MelonLoader;

namespace UIFramework
{
	internal static class Debug
	{
		private static bool debugMode = true;
		private static string lastDiffLogMessage = string.Empty;

		private static GameObject DebugUi;
		private static TextMeshPro DebugUiText;
		private static GameObject PlayerUi;

		/*/// <summary>
		/// Creates a debug screen in front of the player 
		/// </summary>
		internal static void BuildDebugScreen()
		{
			PlayerUi = PlayerManager.Instance.LocalPlayer.Controller.gameObject.transform.GetChild(6).GetChild(0).gameObject;
			DebugUi = Calls.Create.NewText("Placeholder text. You shouldn't be seeing this without some UE Shenanigans\n or decompiled code. Unless I (probably) told you about this.", 1f, Color.white, new Vector3(0f, 0.1f, 1f), Quaternion.Euler(0, 0, 0));
			DebugUi.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			DebugUi.transform.localPosition = new Vector3(0f, 0.1f, 0.96f);
			DebugUi.transform.SetParent(PlayerUi.transform, false);
			DebugUiText = DebugUi.GetComponent<TextMeshPro>();
			DebugUi.SetActive(debugMode);
		}


		/// <summary>
		/// updates the debug screen text
		/// </summary>
		/// <param name="message"></param>
		internal static void UpdateDebugScreen(string message)
		{
			if (Calls.IsMapInitialized()) { DebugUiText.text = message; }
		}*/


		/// <summary>
		/// Call in OnUpdate to monitor variables per frame but only logs if they change
		/// </summary>
		/// <param name="message"></param>
		/// <param name="debugOnly"></param>
		/// <param name="logLevel"></param>
		internal static void DiffLog(string message, bool debugOnly = true, int logLevel = 0)
		{
			if (message != lastDiffLogMessage)
			{
				lastDiffLogMessage = message;
				Log("DIFFLOG: " + message, debugOnly, logLevel);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="debugOnly"></param>
		/// <param name="logLevel"></param>
		internal static void Log(string message, bool debugOnly = false, int logLevel = 0)
		{
			if (!debugMode && debugOnly)
				return;
			switch (logLevel)
			{
				case 1:
					Melon<Core>.Logger.Warning(message);
					break;
				case 2:
					Melon<Core>.Logger.Error(message);
					break;
				default:
					Melon<Core>.Logger.Msg(message);
					break;
			}
		}

		internal static void Deb(string message)
		{
			Log(message, true, 0);
		}

		internal static void Msg(string message)
		{
			Log(message, false, 0);
		}

		internal static void Warning(string message)
		{
			Log(message, false, 1);
		}

		internal static void Error(string message)
		{
			Log(message, false, 2);
		}

	}
}
