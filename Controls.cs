using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIFramework
{
	///<summary>
	/// DO NOT IMPLEMENT EVENT HANDLERS
	/// Currently, the inputs will be handled by the save button
	///</summary>
	internal abstract class Control
	{
		internal GameObject Go {get; set;} = new GameObject;

		internal void SetText(string text)
		{

		}
	}
	///<summary>
	/// 
	///</summary>
	internal class VTab : Control
	{
		CategoryBar CatBar {get; set;}


	}

	///<summary>
	/// 
	///</summary>
	internal class HTab : Control
	{
		BodyPanel Body {get; set;}
	}

	///<summary>
	/// 
	///</summary>
	internal class Button : Control
	{

	}

	///<summary>
	/// 
	/// </summary>
	internal class CheckBox : Control
	{

	}

	///<summary>
	/// 
	///</summary>
	internal class TextBox : Control
	{

	}

	///<summary>
	///  
	///</summary>
	internal class TextBlock : Control
	{

	}

	///<summary>
	/// 
	///</summary>
	internal abstract class LayoutPanel : Control
	{
		internal void Add(Control control)
		{

		}
	}

	///<summary>
	/// 
	///</summary>
	internal class ModsBar : LayoutPanel
	{

	}

	///<summary>
	/// 
	///</summary>
	internal class CategoryBar : LayoutPanel
	{

	}

	///<summary>
	/// 
	///</summary>
	internal class BodyPanel : LayoutPanel
	{

	}

	///<summary>
	/// Defines the entry in the main body that represents a melon preference entry
	///</summary>
	internal class Setting
	{
		internal string Description {get; set;}
		internal Control Interactable {get; set;}
		internal string Identifier {get; set;}

	}
}
