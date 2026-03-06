using System;
using System.ComponentModel;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.U2D;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	[DisplayName("Sprites from Atlases Provider")]
	public class AtlasSpriteProvider : ResourceProviderBase
	{
		public override void Provide(ProvideHandle providerInterface)
		{
			SpriteAtlas dependency = providerInterface.GetDependency<SpriteAtlas>(0);
			if (dependency == null)
			{
				providerInterface.Complete<Sprite>(null, false, new Exception("Sprite atlas failed to load for location " + providerInterface.Location.PrimaryKey + "."));
				return;
			}
			string text;
			string text2;
			ResourceManagerConfig.ExtractKeyAndSubKey(providerInterface.ResourceManager.TransformInternalId(providerInterface.Location), out text, out text2);
			string name = string.IsNullOrEmpty(text2) ? text : text2;
			Sprite sprite = dependency.GetSprite(name);
			providerInterface.Complete<Sprite>(sprite, sprite != null, (sprite != null) ? null : new Exception("Sprite failed to load for location " + providerInterface.Location.PrimaryKey + "."));
		}

		public override void Release(IResourceLocation location, object obj)
		{
			Sprite sprite = obj as Sprite;
			if (sprite != null)
			{
				Object.Destroy(sprite);
			}
		}
	}
}
