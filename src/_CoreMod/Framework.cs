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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace UIFramework
{
	/// <summary>
	/// primary public facing class, modders will interact with this to register their preferences and build the UI.
	/// </summary>
	public static class UI
	{

		internal static UIFModel.RootModel ModelInstance = new();
		internal static GameObject MainWindow;
		public static bool IsVisible { get { return MainWindow.activeSelf; }  }
		internal static UIFController.WindowController WindowInstance;
		/// <summary>
		/// Registers a mod and its categories to the UI instance. 
		/// </summary>
		/// <param name="modInstance">Instance of your MelonMod class</param>
		/// <param name="categories">The range of MelonPreferences_Category objects used by your mod.</param>
		/// <remarks>
		/// Unless the dependency was explicitly declared, Don't use before OnLateInitializeMelon.
		/// Currently, registration after the first gym load means your mod won't show up on the mod list
		/// 
		/// Deprecated: this version of the Register function will be removed in a future version in favor of taking in the MelonBase type instead
		/// Please explicitly cast your mod instance as MelonBase to prevent future incompatibility
		/// So UI.Register((MelonBase)this, Category1, Category2,...);
		/// </remarks>
		/// <returns>A reference to the created Mod Model for further customization</returns>
		[Obsolete(".Register() will be a different function in the future to support plugins.\n " +
			"When that happens, no code changes are needed but you will need to rebuild your project so the compiler can find the correct method\n" +
			"To future-proof your mod, Explicitly cast your mod instance to MelonBase when registering")]
		public static UIFModel.ModelMod Register(MelonMod modInstance, params MelonPreferences_Category[] categories)
		{
			return Register((MelonBase)modInstance, categories);
		}
		
		/// <summary>
		/// Registers a mod or a plugin to UIFramework along with its categories.
		/// </summary>
		/// <param name="modInstance"></param>
		/// <param name="categories"></param>
		/// <remarks>
		/// Unless the dependency was explicitly declared, Don't use before OnLateInitializeMelon.
		/// Currently, registration after the first gym load means your mod won't show up on the mod list
		/// </remarks>
		/// <returns></returns>
		public static UIFModel.ModelMod Register(MelonBase modInstance, params MelonPreferences_Category[] categories)
		{
			UIFModel.ModelMod NewModModel = new(modInstance, categories.ToList());
			ModelInstance.AddSubmodel(NewModModel);
			return NewModModel;
		}

		/// <summary>
		/// Registers a mod with no categories to the framework. Categories need to be manually added
		/// </summary>
		/// <remarks>
		/// Deprecated: this version of the Register function will be removed in a future version in favor of taking in the MelonBase type instead
		/// Please explicitly cast your mod instance as MelonBase to prevent future incompatibility
		/// So UI.Register((MelonBase)this);
		/// </remarks>
		/// <param name="modInstance">Instance of your MelonMod class</param>
		[Obsolete(".Register() will be a different function in the future to support plugins.\n " +
			"When that happens, no code changes are needed but you will need to rebuild your project so the compiler can find the correct method\n" +
			"To future-proof your mod, Explicitly cast your mod instance to MelonBase when registering")]
		public static UIFModel.ModelMod Register(MelonMod modInstance)
		{
			return Register(modInstance);
		}

		/// <summary>
		/// Registers a mod or plugin with no categories to the framework. Categories need to be manually added
		/// </summary>
		/// <param name="modInstance">Instance of your MelonMod class</param>
		public static UIFModel.ModelMod Register(MelonBase modInstance)
		{
			UIFModel.ModelMod NewModModel = new(modInstance);
			ModelInstance.AddSubmodel(NewModModel);
			return NewModModel;
		}


		/// <summary>
		/// 
		/// </summary>
		internal static void InitializeUIObjects()
		{
			MainWindow = GameObject.Instantiate(Prefabs.MainCanvasSource, Prefabs.UIFGameObjects.transform);
			MainWindow.name = "MainWindow";
			MainWindow.SetActive(true);
			WindowInstance = MainWindow.GetComponent<UIFController.WindowController>();

		}
		/// <summary>
		/// 
		/// </summary>
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
				case InputType.Button:
					selectedPrefab = GameObject.Instantiate(Prefabs.ButtonPrefab);
					break;
				case InputType.Dropdown:
					selectedPrefab = GameObject.Instantiate(Prefabs.DropDownPrefab);
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
		[Display(Name = "Default", Description = "Defaults to basic string input")]
		Default,
		[Display(Name = "Text Field", Description = "Basic text field input")]
		TextField,
		[Display(Name = "Toggle", Description = "A simple on/off toggle")]
		Toggle,
		[Display(Name = "Int input", Description = "An Input for inputing Numeric Integers")]
		NumericInt,
		[Display(Name = "Float input", Description = "An Input for inputing Floating Point Numbers")]
		NumericFloat,
		[Display(Name = "Button", Description = "A simple button that can be clicked to trigger an action")]
		Button,
		[Display(Name = "Dropdown", Description = "A dropdown menu for selecting from multiple options")]
		Dropdown,
		/*Slider,
		MultiCheckbox,
		RadioButton*/
	}

	public class UIProperties
	{
		//universal
		public bool IsEnabled{get; set;}
		public bool IsHidden{get; set;}
		public bool IsReadOnly{get; set;}
		
		//text fields
		public bool IsPasswordField{get; set;}
		public bool IsRightToLeft{get; set;}

		//appearance
		//universal
		public Color DisplayNameColor {get; set;}
		public Color DescriptionColor {get; set;}
		
		public Color EntryBaseColor{get; set;}
		public Color EntryDataSectionColor{get; set;}

		public int DisplayNameFontSize {get; set;}
		public int DescriptionFontSize {get; set;}




	}

	
}