using MelonLoader;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UIFramework
{
	/// <summary>
	/// primary public facing class, modders will interact with this to register their preferences and build the UI.
	/// </summary>
	public static class UIFramework
	{

		internal static UIFModel.RootModel ModelInstance = new();
		internal static GameObject MainWindow;

		internal static UIFController.WindowController WindowInstance;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="modInstance"></param>
		/// <param name="categories"></param>
		/// <returns>A reference to the created Mod Model for further customization</returns>
		public static UIFModel.ModelMod Register(MelonMod modInstance, params MelonPreferences_Category[] categories)
		{
			UIFModel.ModelMod NewModModel = new(modInstance, categories.ToList());
			ModelInstance.AddModModel(NewModModel);
			return NewModModel;
		}


		internal static void InitializeUIObjects()
		{
			MainWindow = GameObject.Instantiate(Prefabs.MainCanvasSource, Prefabs.UIFGameObjects.transform);
			MainWindow.name = "MainWindow";
			MainWindow.SetActive(true);
			WindowInstance = MainWindow.GetComponent<UIFController.WindowController>();

		}
		internal static void BuildUI()
		{
			WindowInstance.SetModel(ModelInstance);

		}

		public static GameObject GetPrefab(InputType input)
		{
			GameObject selectedPrefab;
			switch (input)
			{
				case InputType.TextField:
					selectedPrefab = GameObject.Instantiate(Prefabs.TextPrefab);
					break;
				case InputType.Toggle:
					selectedPrefab = GameObject.Instantiate(Prefabs.BoolPrefab);
					break;
				case InputType.NumericInt:
					selectedPrefab = GameObject.Instantiate(Prefabs.IntPrefab);
					break;
				case InputType.NumericFloat:
					selectedPrefab = GameObject.Instantiate(Prefabs.FloatPrefab);
					break;
				case InputType.NumericDouble:
					selectedPrefab = GameObject.Instantiate(Prefabs.DoublePrefab);
					break;
				case InputType.Button:
					selectedPrefab = GameObject.Instantiate(Prefabs.ButtonPrefab);
					break;
				default:
					selectedPrefab = GameObject.Instantiate(Prefabs.TextPrefab);
					break;
			}

			selectedPrefab.transform.SetParent(Prefabs.TempStorage.transform);
			return selectedPrefab;
		}
	}
	public enum InputType
	{
		Default,
		TextField,
		Toggle,
		NumericInt,
		NumericFloat,
		NumericDouble,
		Button,
		/*Slider,
		Dropdown,
		MultiCheckbox,
		RadioButton*/
	}
}