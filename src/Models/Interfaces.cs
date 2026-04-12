using System;
using System.Collections.Generic;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using static UIFramework.UIFController;

namespace UIFramework
{

	public partial class UIFModel
	{
		/// <summary>
		/// Implemented by all models
		/// </summary>
		public interface IModelable
		{
			/// <summary>
			/// Name for the model
			/// </summary>
			public string Identifier { get; }
			/// <summary>
			/// User-facing display name. Should return Identifier if not assigned to a value
			/// </summary>
			public string DisplayName { get; }
			/// <summary>
			/// Instantiates a new Game object associated with them model
			/// </summary>
			/// <returns> UI Game Object</returns>
			public GameObject GetNewUIInstance();
			/// <summary>
			/// Should be called when save button is pressed. Runs after all ancestor's save actions have been run
			/// </summary>
			public void SaveAction();
			public void DiscardAction();
		}
		/// <summary>
		/// Models that contain submodels. Generally these are mods and categories representing tabs
		/// </summary>
		public interface IHoldSubmodels : IModelable
		{
			/// <summary>
			/// A list of submodels
			/// </summary>
			public List<IModelable> SubModels { get; set; }
			/// <summary>
			/// Finds submodel by identifier
			/// </summary>
			/// <param name="identifier"></param>
			/// <returns></returns>
			public IModelable GetSubmodel(string identifier);
			/// <summary>
			/// Adds a new submodel to the SubModels list
			/// </summary>
			/// <param name="model"></param>
			public void AddSubmodel(params IModelable[] model);
		}
		/// <summary>
		/// Goes on the main panel. Contains controls for manipulating preferences or just general UI controls
		/// </summary>
		public interface IEntry
		{
			
			/// <summary>
			/// Name/ID of the entry
			/// </summary>
			public string Identifier { get; }
			
			/// <summary>
			/// Description of the entry
			/// </summary>
			public string Description { get; }
			/// <summary>
			/// Ideally called by the controller to define a save action
			/// </summary>
			public void SaveAction();
			public void DiscardAction();
			public string DisplayName { get; }
			//public object BoxedValue { get; set; }

			/// <summary>
			/// Called when the Entry has been created.
			/// Useful when you wanna change specific elements after creation
			/// </summary>
			public Action<UIFController.Entry> OnUICreated { get; set; }
			
			
		}
		/// <summary>
		/// implemented by models that store data
		/// </summary>
		public interface IStorable
		{
			public object BoxedValue { get; set; }
			public bool TryApply(object value)
			{
				bool result = false;
				try
				{
					BoxedValue = value;
					result = true;
				}
				catch (Exception ex)
				{
					Debug.Log($"ModelMelonEntry TryApply: {ex.Message}", false, 2);
					result = false;

				}
				return result;

			}
		}
	}
}