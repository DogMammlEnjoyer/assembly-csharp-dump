using System;

namespace UnityEngine.Rendering
{
	public struct ShaderDebugPrintInput
	{
		public Vector2 pos { readonly get; set; }

		public bool leftDown { readonly get; set; }

		public bool rightDown { readonly get; set; }

		public bool middleDown { readonly get; set; }

		public string String()
		{
			return string.Format("Mouse: {0}x{1}  Btns: Left:{2} Right:{3} Middle:{4} ", new object[]
			{
				this.pos.x,
				this.pos.y,
				this.leftDown,
				this.rightDown,
				this.middleDown
			});
		}
	}
}
