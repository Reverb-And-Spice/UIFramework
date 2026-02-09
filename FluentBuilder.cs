using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIFramework
{

	public class ControlBuilder<TControl> where TControl : Control, new()
	{
		protected TControl _control;

		public ControlBuilder()
		{
			_control = new TControl();
		}

		public ControlBuilder WithText(string text)
		{
			_control.SetText(text);
			return this;
		}

	}


}
