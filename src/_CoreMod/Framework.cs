using MelonLoader;
using UnityEngine;
using System.ComponentModel.DataAnnotations;
using UIFramework.UiExtensions;
using UIFramework.Adapters;
using UIFramework.Models;
using System.Data;
namespace UIFramework
{
	/// <summary>
	/// primary public facing class, modders will interact with this to register their preferences and build the UI.
	/// </summary>
	public static partial class UI
	{
		internal static RootModel ModelInstance = new();
		internal static GameObject MainWindow;
		public static bool IsVisible { get => MainWindow.activeSelf; internal set => MainWindow.SetActive(value); }

		internal static WindowCoordinator WindowInstance;

		private static CanvasGroup CanvasGroup => MainWindow.GetComponent<CanvasGroup>();

		


		/// <summary>
		/// Registers a mod or plugin to UI Framework with its categories.
		/// </summary>
		/// <param name="melonInstance"></param>
		/// <param name="categories"></param>
		/// <returns></returns>
		///<remarks>Released</remarks>
		public static MelonModel RegisterMelon(MelonBase melonInstance, params MelonPreferences_Category[] categories)
		{
			MelonModel newModModel = new(melonInstance, categories.ToList());
			ModelInstance.AddSubmodel(newModModel);
			return newModModel;
		}
		/// <summary>
		/// Registers a mod or a plugin to UI Framework with no categories. Categories need to be manually added.
		/// </summary>
		/// <param name="melonInstance"></param>
		/// <returns></returns>
		///	<remarks>Released</remarks>
		public static MelonModel RegisterMelon(MelonBase melonInstance)
		{
			MelonModel newModModel = new(melonInstance);
			ModelInstance.AddSubmodel(newModModel);
			return newModModel;
		}


		/// <summary>
		///
		/// </summary>
		internal static void InitializeUIObjects()
		{
			MainWindow = GameObject.Instantiate(Prefabs.MainWindowSource, Prefabs.Canvas.transform);
			MainWindow.name = "MainWindow";
			MainWindow.SetActive(true);
			WindowInstance = MainWindow.GetComponent<WindowCoordinator>();

		}
		/// <summary>
		/// Create an entry that becomes a button in the UI
		/// </summary>
		/// <param name="category"></param>
		/// <param name="buttonText"></param>
		/// <param name="displayName"></param>
		/// <param name="description"></param>
		/// <param name="handler"></param>
		/// <remarks>Released</remarks>
		public static void CreateButtonEntry(MelonPreferences_Category category, string buttonText, string displayName, string description, Action handler)

		{
			ButtonAsEntry button = new() { Handler = handler, ButtonText = buttonText, DisplayName = displayName, Description = description };
			category.CreateEntry<ButtonAsEntry>($"PlaceHolder{buttonText + displayName + description}", button, displayName, description, false, true, button);
		}
		/// <summary>
		///
		/// </summary>
		internal static void BuildUI()
		{
			WindowInstance.SetModel(ModelInstance);

		}

		internal static void RequestRefresh(ModModelBase modModel)
		{
			Debug.Log("RefreshRequested in Framework.cs RequestRefresh(ModModelBase modModel)", true, 1);
			WindowInstance?.RequestRefresh(modModel);

		}
		/// <summary>
		/// Request A refresh from UI Framework. Refresh will happen on the next frame if your mod is selected
		/// </summary>
		/// <param name="melonInstance"></param>
		public static void RequestRefresh(MelonBase melonInstance)
		{
			Debug.Log("Refresh requested in Framework.cs RequestRefresh(MelonBase melonInstance)", true, 1);
			ModModelBase model = ModelInstance.GetModModel(melonInstance.Info.Name);
			WindowInstance?.RequestRefresh(model);
		}

		internal static void Fade()
		{
			CanvasGroup.alpha = 0.25f;
		}
		internal static void Unfade()
		{
			CanvasGroup.alpha = 1f;
		}

		/// <summary>
		/// Get a prefab instance for a View Type
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static GameObject GetPrefabInstance(PrefabType input)
		{
			GameObject selectedPrefab = input switch
			{
				PrefabType.TextField => GameObject.Instantiate(Prefabs.TextPrefab),
				PrefabType.Toggle => GameObject.Instantiate(Prefabs.BoolPrefab),
				PrefabType.NumericInt => GameObject.Instantiate(Prefabs.IntPrefab),
				PrefabType.NumericFloat => GameObject.Instantiate(Prefabs.FloatPrefab),
				PrefabType.Button => GameObject.Instantiate(Prefabs.ButtonPrefab),
				PrefabType.Dropdown => GameObject.Instantiate(Prefabs.DropDownPrefab),
				PrefabType.Slider => GameObject.Instantiate(Prefabs.SliderPrefab),
				_ => GameObject.Instantiate(Prefabs.TextPrefab),
			};
			selectedPrefab.transform.SetParent(Prefabs.TempStorage.transform);
			return selectedPrefab;
		}
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
#pragma warning disable 1591
	/// <summary>
	/// Types of prefab
	/// </summary>
	public enum PrefabType
	{
		[Display(Name = "Default", Description = "Defaults to basic string input")]
		Default,
		[Display(Name = "Text Field", Description = "Basic text field input")]
		TextField,
		[Display(Name = "Toggle", Description = "A simple on/off Toggle")]
		Toggle,
		[Display(Name = "Int input", Description = "An Input for inputing Numeric Integers")]
		NumericInt,
		[Display(Name = "Float input", Description = "An Input for inputing Floating Point Numbers")]
		NumericFloat,
		[Display(Name = "Button", Description = "A simple button that can be clicked to trigger an action")]
		Button,
		[Display(Name = "Dropdown", Description = "A dropdown menu for selecting from multiple options")]
		Dropdown,
		[Display(Name = "Slider", Description = "A slider for selecting a value within a range")]
		Slider,
		/*
		MultiCheckbox,
		RadioButton*/

	}

	public partial class UI
	{
		const string ObsoleteRegisterMessage = "Don't Panic! I'm just moving .Register() and all its overloads to .RegisterMelon(). Just use that function instead. UI Framework is still backwards compatible.";
		[Obsolete(ObsoleteRegisterMessage, true)]
		public static UIFModel.ModelMod Register(MelonMod modInstance, params MelonPreferences_Category[] categories)
		{
			return Register((MelonBase)modInstance, categories);
		}

		[Obsolete(ObsoleteRegisterMessage, true)]
		public static UIFModel.ModelMod Register(MelonBase modInstance, params MelonPreferences_Category[] categories)
		{
			UIFModel.ModelMod NewModModel = new(modInstance, categories.ToList());
			ModelInstance.AddSubmodel(NewModModel);
			return NewModModel;
		}

		[Obsolete(ObsoleteRegisterMessage, true)]
		public static UIFModel.ModelMod Register(MelonMod modInstance)
		{
			return Register((MelonBase)modInstance);
		}

		[Obsolete(ObsoleteRegisterMessage, true)]
		public static UIFModel.ModelMod Register(MelonBase modInstance)
		{
			UIFModel.ModelMod NewModModel = new(modInstance);
			ModelInstance.AddSubmodel(NewModModel);
			return NewModModel;
		}
	}

}