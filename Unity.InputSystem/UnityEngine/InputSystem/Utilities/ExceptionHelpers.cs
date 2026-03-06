using System;

namespace UnityEngine.InputSystem.Utilities
{
	internal static class ExceptionHelpers
	{
		public static bool IsExceptionIndicatingBugInCode(this Exception exception)
		{
			return exception is NullReferenceException || exception is IndexOutOfRangeException || exception is ArgumentException;
		}
	}
}
