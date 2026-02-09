using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIFramework
{
	///<summary>
	/// 
	///</summary>
	internal abstract class Control
	{
		internal GameObject Go {get; set;}

		internal void SetText()
		{

		}
	}
	///<summary>
	/// 
	///</summary>
	internal class VTab : Control
	{

	}

	///<summary>
	/// 
	///</summary>
	internal class HTab : Control
	{
		
	}

	///<summary>
	/// 
	///</summary>
	internal class Section : Control
	{

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
	internal abstract class LayoutPanel
	{
		internal void Add(Control control)
		{

		}
	}

	///<summary>
	/// 
	///</summary>
	internal class ModList : LayoutPanel
	{

	}

	///<summary>
	/// 
	///</summary>
	internal class CategoryList : LayoutPanel
	{

	}

	///<summary>
	/// 
	///</summary>
	internal class BodyPanel : LayoutPanel
	{

	}

	///<summary>
	/// 
	///</summary>
	internal class Setting
	{
		internal string Description {get; set;}
		internal Inputs InputType {get; set;}
		internal string Identifier {get; set;}

	}
}
