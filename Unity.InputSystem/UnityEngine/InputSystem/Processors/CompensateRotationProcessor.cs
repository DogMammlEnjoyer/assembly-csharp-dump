using System;
using System.ComponentModel;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem.Processors
{
	[DesignTimeVisible(false)]
	internal class CompensateRotationProcessor : InputProcessor<Quaternion>
	{
		public override Quaternion Process(Quaternion value, InputControl control)
		{
			if (!InputSystem.settings.compensateForScreenOrientation)
			{
				return value;
			}
			Quaternion identity = Quaternion.identity;
			switch (InputRuntime.s_Instance.screenOrientation)
			{
			case ScreenOrientation.PortraitUpsideDown:
				identity = new Quaternion(0f, 0f, 1f, 0f);
				break;
			case ScreenOrientation.LandscapeLeft:
				identity = new Quaternion(0f, 0f, 0.70710677f, -0.70710677f);
				break;
			case ScreenOrientation.LandscapeRight:
				identity = new Quaternion(0f, 0f, -0.70710677f, -0.70710677f);
				break;
			}
			return value * identity;
		}

		public override string ToString()
		{
			return "CompensateRotation()";
		}

		public override InputProcessor.CachingPolicy cachingPolicy
		{
			get
			{
				return InputProcessor.CachingPolicy.EvaluateOnEveryRead;
			}
		}
	}
}
