using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;

namespace Fusion
{
	[Preserve]
	public class FusionGlobalScriptableObjectResourceAttribute : FusionGlobalScriptableObjectSourceAttribute
	{
		public FusionGlobalScriptableObjectResourceAttribute(Type objectType, string resourcePath = "") : base(objectType)
		{
			this.ResourcePath = resourcePath;
		}

		public string ResourcePath { get; }

		public bool InstantiateIfLoadedInEditor { get; set; } = true;

		public override FusionGlobalScriptableObjectLoadResult Load(Type type)
		{
			FusionGlobalScriptableObjectAttribute customAttribute = type.GetCustomAttribute<FusionGlobalScriptableObjectAttribute>();
			string text;
			if (string.IsNullOrEmpty(this.ResourcePath))
			{
				string defaultPath = customAttribute.DefaultPath;
				int num = defaultPath.LastIndexOf("/Resources/", StringComparison.OrdinalIgnoreCase);
				if (num < 0)
				{
					return default(FusionGlobalScriptableObjectLoadResult);
				}
				text = defaultPath.Substring(num + "/Resources/".Length);
				if (Path.HasExtension(text))
				{
					text = text.Substring(0, text.LastIndexOf('.'));
				}
			}
			else
			{
				text = this.ResourcePath;
			}
			Object instance = Resources.Load(text, type);
			if (!instance)
			{
				return default(FusionGlobalScriptableObjectLoadResult);
			}
			if (this.InstantiateIfLoadedInEditor && Application.isEditor)
			{
				Object clone = Object.Instantiate(instance);
				return new FusionGlobalScriptableObjectLoadResult((FusionGlobalScriptableObject)clone, delegate(FusionGlobalScriptableObject x)
				{
					Object.Destroy(clone);
				});
			}
			return new FusionGlobalScriptableObjectLoadResult((FusionGlobalScriptableObject)instance, delegate(FusionGlobalScriptableObject x)
			{
				Resources.UnloadAsset(instance);
			});
		}
	}
}
