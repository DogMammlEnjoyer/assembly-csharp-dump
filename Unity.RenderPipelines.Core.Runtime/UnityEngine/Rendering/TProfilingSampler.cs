using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	internal class TProfilingSampler<TEnum> : ProfilingSampler where TEnum : Enum
	{
		static TProfilingSampler()
		{
			string[] names = Enum.GetNames(typeof(TEnum));
			Array values = Enum.GetValues(typeof(TEnum));
			for (int i = 0; i < names.Length; i++)
			{
				TProfilingSampler<TEnum> value = new TProfilingSampler<TEnum>(names[i]);
				TProfilingSampler<TEnum>.samples.Add((TEnum)((object)values.GetValue(i)), value);
			}
		}

		public TProfilingSampler(string name) : base(name)
		{
		}

		internal static Dictionary<TEnum, TProfilingSampler<TEnum>> samples = new Dictionary<TEnum, TProfilingSampler<TEnum>>();
	}
}
