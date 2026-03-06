using System;
using System.Collections.Generic;

namespace Unity.Cinemachine
{
	public interface IInputAxisOwner
	{
		void GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes);

		public struct AxisDescriptor
		{
			public IInputAxisOwner.AxisDescriptor.AxisGetter DrivenAxis;

			public string Name;

			public IInputAxisOwner.AxisDescriptor.Hints Hint;

			public unsafe delegate InputAxis* AxisGetter();

			public enum Hints
			{
				Default,
				X,
				Y
			}
		}
	}
}
