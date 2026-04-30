using Il2CppTMPro;
using MelonLoader;
using MelonLoader.Logging;
using UnityEngine.Events;
using UnityEngine.UI;
namespace UIFramework.Adapters
{

	//protected override GameObject UIPrefab { get { return Prefabs.TextPrefab; } }
	public abstract class TabButtonController : SubModelAdapter, IChildable
	{

		protected WindowCoordinator ParentWindow;
		protected UIFModel.IHoldSubmodels _model => (UIFModel.IHoldSubmodels)_internalModel;

		public override void ModelSet()
		{
			Label = _model.DisplayName;
			ParentWindow = _rootWindow;
		}

		public string Label { set { gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value; } }
		public ColorARGB TabColor { get; set; }
		/// <summary>
		/// Runs when the button is clicked. Implement this in inheriting classes. 
		/// </summary>
		/// <exception cref="NotImplementedException"></exception>
		/// <remarks>IL2CPP does not like abstract methods 😭</remarks>
		public virtual void OnSelect()
		{
			throw new NotImplementedException("Implement OnSelect in inheriting class");
			//this.gameObject.GetComponent<Image>().color = ParentWindow.openTabColor;
		}



		void Start()
		{
			gameObject.GetComponent<Button>().onClick.AddListener((UnityAction)OnSelect);
		}
		void OnDestroy()
		{
			gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
		}


	}


	/// <summary>
	/// 
	/// </summary>
	[RegisterTypeInIl2Cpp]
	public class ModButtonView : TabButtonController, IChildable
	{

		protected UIFModel.ModelMod ModModel => (UIFModel.ModelMod)_internalModel;

		public override void OnSelect()
		{

			ParentWindow.SetSelectedMod(ModModel);




		}

	}
	/// <summary>
	/// </summary>
	[RegisterTypeInIl2Cpp]
	public class CategoryTabView : TabButtonController, IChildable
	{
		public override void OnSelect()
		{
			ParentWindow.SetSelectedCategory((UIFModel.ModelCategoryItem)_internalModel);
		}
	}

}