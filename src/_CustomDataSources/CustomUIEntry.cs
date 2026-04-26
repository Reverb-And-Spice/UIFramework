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
using Tomlet.Models;
using Tomlet;
namespace UIFramework
{
	public abstract class CustomUIEntry
	{
		public string Identifier { get; internal set; }
		public string DisplayName { get; set; }
		public string Description { get; set; }
		public string Comment { get; set; }
		public bool IsHidden { get; set; }
		public bool DontSaveDefault { get; set; }
		public CustomCategoryTab Category { get; internal set; }

		public abstract object BoxedValue { get; set; }
		public abstract object BoxedEditedValue { get; set; }

		public MelonLoader.Preferences.ValueValidator Validator { get; internal set; }

		//Melon:  public Preferences.ValueValidator Validator { get; internal set; }
		//Melon:  public string GetExceptionMessage(string submsg) => $"Attempted to {submsg} {DisplayName} when it is a {GetReflectedType().FullName}!";
		public abstract Type GetReflectedType();

		public abstract void ResetToDefault();

		public abstract string GetEditedValueAsString();
		public abstract string GetDefaultValueAsString();
		public abstract string GetValueAsString();

        public abstract TomlValue Save();

		public Action<object, object> OnEntryValueChangedUntyped;
		protected void FireUntypedValueChanged(object old, object neew)
		{
			OnEntryValueChangedUntyped.Invoke(old, neew);
		}
	}

	public class CustomUIEntry<T> : CustomUIEntry
	{
		private T myValue;
		public T Value
		{
			get => myValue;
			set
			{
				if (Validator != null)
                    value = (T)Validator.EnsureValid(value);

				if ((myValue == null && value == null) || (myValue != null && myValue.Equals(value)))
					return;

				var old = myValue;
				myValue = value;
				EditedValue = myValue;
				OnEntryValueChanged?.Invoke(old, value);
				//OnValueChanged?.Invoke(old, value); //deprecated in melonprefs
				FireUntypedValueChanged(old, value);
			}
		}


		public T EditedValue { get; set; }
		public T DefaultValue { get; set; }

		public override object BoxedValue
		{
			get => myValue;
			set => Value = (T)value;
		}

		public override object BoxedEditedValue
		{
			get => EditedValue;
			set => EditedValue = (T)value;
		}

		public override void ResetToDefault() => Value = DefaultValue;

		public Action<T, T> OnEntryValueChanged;

		public override Type GetReflectedType() => typeof(T);

		public override string GetEditedValueAsString() => EditedValue?.ToString();
		public override string GetDefaultValueAsString() => DefaultValue?.ToString();
		public override string GetValueAsString() => Value?.ToString();

		//public override void Loa

		public InputType InputUIType;
        public override TomlValue Save()
        {
            Value = EditedValue;
            TomlValue returnval = TomletMain.ValueFrom(Value);
            returnval.Comments.PrecedingComment = Description;
            returnval.Comments.InlineComment = Comment;
            if (!string.IsNullOrEmpty(returnval.Comments.InlineComment))
                returnval.Comments.InlineComment.Replace('\n', ' ');
            return returnval;
        }

	}
}