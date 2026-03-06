using System;
using UnityEngine;

public sealed class MetaXRAudioSettings : ScriptableObject
{
	public static MetaXRAudioSettings Instance
	{
		get
		{
			if (MetaXRAudioSettings.instance == null)
			{
				MetaXRAudioSettings.instance = Resources.Load<MetaXRAudioSettings>("MetaXRAudioSettings");
				if (MetaXRAudioSettings.instance == null)
				{
					MetaXRAudioSettings.instance = ScriptableObject.CreateInstance<MetaXRAudioSettings>();
				}
			}
			return MetaXRAudioSettings.instance;
		}
	}

	[SerializeField]
	public int voiceLimit = 64;

	private static MetaXRAudioSettings instance;
}
