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
using MelonLoader.Preferences;
using UIFramework.Models;
namespace UIFramework
{
	public class CustomCategoryTab
	{
		internal CategoryModelBase CategoryModel { get; set; }
		public readonly List<CustomUIEntry> Entries = new List<CustomUIEntry>();

		public string Identifier { get; internal set; }
		public string DisplayName { get; set; }
		public bool IsHidden { get; set; }
		public bool IsInlined { get; set; }

		internal CustomCategoryTab(string identifier, string display_name, bool is_hidden = false, bool is_inlined = false)
		{
			Identifier = identifier;
			DisplayName = display_name;
			IsHidden = is_hidden;
			IsInlined = is_inlined;
		}

		public CustomUIEntry CreateEntry<T>(string identifier, T default_value, string display_name, bool is_hidden)
			=> CreateEntry(identifier, default_value, display_name, null, is_hidden, null);
		//skipped one createentry from melonprefs because I don't need oldIdentifier
		public CustomUIEntry<T> CreateEntry<T>(string identifier, T default_value, string display_name = null,
			string description = null, bool is_hidden = false, ValueValidator validator = null)
		{
			if (string.IsNullOrEmpty(identifier))
				throw new Exception("identifier is null or empty when calling CreateEntry");

			if (display_name == null)
				display_name = identifier;

			var entry = GetEntry<T>(identifier);
			if (entry != null)
				throw new Exception($"Calling CreateEntry for {display_name} when it Already Exists");

			if (validator != null && !validator.IsValid(default_value))
				throw new ArgumentException($"Default value '{default_value}' is invalid according to the provided ValueValidator!");


			entry = new CustomUIEntry<T>
			{
				Identifier = identifier,
				DisplayName = display_name,
				Description = description,
				IsHidden = is_hidden,
				Category = this,
				DefaultValue = default_value,
				Value = default_value,
				Validator = validator,
			};

			//skip following code because we don't need to save to file
			// Preferences.IO.File currentFile = File;
			// if (currentFile == null)
			//     currentFile = MelonPreferences.DefaultFile;
			// currentFile.SetupEntryFromRawValue(entry);
			Entries.Add(entry);

			return entry;
		}
		//DeleteEntry, RenameEntry not needed if not dealing with saved files

		public CustomUIEntry GetEntry(string identifier)
		{
			if (string.IsNullOrEmpty(identifier))
				throw new Exception("identifier cannot be null or empty when calling GetEntry");
			if (Entries.Count <= 0)
				return null;
			return Entries.Find(x => x.Identifier.Equals(identifier));
		}

		public CustomUIEntry<T> GetEntry<T>(string identifier) => (CustomUIEntry<T>)GetEntry(identifier);
		public bool HasEntry(string identifier) => GetEntry(identifier) != null;

		//Skip melonprefs file management methods
	}
}