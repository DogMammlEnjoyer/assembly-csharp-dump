using System;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public abstract class Style : ScriptableObject
	{
		public bool Instantiated
		{
			get
			{
				return this._instantiated;
			}
		}

		private static string Path<T>() where T : Style
		{
			return "Styles/" + typeof(T).Name + "s/";
		}

		public static T Default<T>() where T : Style
		{
			return Resources.Load<T>(Style.Path<T>() + "Default");
		}

		public static T Load<T>(string name) where T : Style
		{
			T result;
			if ((result = Resources.Load<T>(Style.Path<T>() + name)) == null)
			{
				result = Style.Default<T>();
			}
			return result;
		}

		public static T Instantiate<T>(string name) where T : Style
		{
			T t = Object.Instantiate<T>(Style.Load<T>(name));
			t._instantiated = true;
			return t;
		}

		protected bool _instantiated;
	}
}
