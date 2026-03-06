using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class MB3_ShadersThatShareTiling
	{
		public static MB3_ShadersThatShareTiling GetShadersThatShareTiling()
		{
			if (MB3_ShadersThatShareTiling._singleton == null)
			{
				MB3_ShadersThatShareTiling.Init();
			}
			return MB3_ShadersThatShareTiling._singleton;
		}

		public static void GetScaleAndOffsetForTextureProp(Material m, string texturePropName, out Vector2 offset, out Vector2 scale)
		{
			MB3_ShadersThatShareTiling.ShaderThatSharesTiling shaderThatSharesTiling;
			if (MB3_ShadersThatShareTiling.GetShadersThatShareTiling().shadersThatShareTiling.TryGetValue(m.shader.name, out shaderThatSharesTiling) && shaderThatSharesTiling.allPropsShareTiling && m.HasProperty(shaderThatSharesTiling.tilingTexturePropName))
			{
				scale = m.GetTextureScale(shaderThatSharesTiling.tilingTexturePropName);
				offset = m.GetTextureOffset(shaderThatSharesTiling.tilingTexturePropName);
				return;
			}
			scale = m.GetTextureScale(texturePropName);
			offset = m.GetTextureOffset(texturePropName);
		}

		private static void Init()
		{
			MB3_ShadersThatShareTiling._singleton = new MB3_ShadersThatShareTiling();
			Dictionary<string, MB3_ShadersThatShareTiling.ShaderThatSharesTiling> dictionary = MB3_ShadersThatShareTiling._singleton.shadersThatShareTiling = new Dictionary<string, MB3_ShadersThatShareTiling.ShaderThatSharesTiling>();
			MB3_ShadersThatShareTiling.ShaderThatSharesTiling shaderThatSharesTiling;
			shaderThatSharesTiling.shadername = "Standard";
			shaderThatSharesTiling.allPropsShareTiling = true;
			shaderThatSharesTiling.tilingTexturePropName = "_MainTex";
			MB3_ShadersThatShareTiling.ShaderThatSharesTiling shaderThatSharesTiling2;
			shaderThatSharesTiling2.shadername = "Standard (Specular setup)";
			shaderThatSharesTiling2.allPropsShareTiling = true;
			shaderThatSharesTiling2.tilingTexturePropName = "_MainTex";
			MB3_ShadersThatShareTiling.ShaderThatSharesTiling shaderThatSharesTiling3;
			shaderThatSharesTiling3.shadername = "Universal Render Pipeline/Lit";
			shaderThatSharesTiling3.allPropsShareTiling = true;
			shaderThatSharesTiling3.tilingTexturePropName = "_BaseMap";
			MB3_ShadersThatShareTiling.ShaderThatSharesTiling shaderThatSharesTiling4;
			shaderThatSharesTiling4.shadername = "Universal Render Pipeline/Simple Lit";
			shaderThatSharesTiling4.allPropsShareTiling = true;
			shaderThatSharesTiling4.tilingTexturePropName = "_BaseMap";
			MB3_ShadersThatShareTiling.ShaderThatSharesTiling shaderThatSharesTiling5;
			shaderThatSharesTiling5.shadername = "Universal Render Pipeline/Complex Lit";
			shaderThatSharesTiling5.allPropsShareTiling = true;
			shaderThatSharesTiling5.tilingTexturePropName = "_BaseMap";
			MB3_ShadersThatShareTiling.ShaderThatSharesTiling shaderThatSharesTiling6;
			shaderThatSharesTiling6.shadername = "Universal Render Pipeline/Baked Lit";
			shaderThatSharesTiling6.allPropsShareTiling = true;
			shaderThatSharesTiling6.tilingTexturePropName = "_BaseMap";
			dictionary.Add(shaderThatSharesTiling.shadername, shaderThatSharesTiling);
			dictionary.Add(shaderThatSharesTiling2.shadername, shaderThatSharesTiling2);
			dictionary.Add(shaderThatSharesTiling3.shadername, shaderThatSharesTiling3);
			dictionary.Add(shaderThatSharesTiling4.shadername, shaderThatSharesTiling3);
			dictionary.Add(shaderThatSharesTiling5.shadername, shaderThatSharesTiling3);
			dictionary.Add(shaderThatSharesTiling6.shadername, shaderThatSharesTiling3);
		}

		private static MB3_ShadersThatShareTiling _singleton;

		private Dictionary<string, MB3_ShadersThatShareTiling.ShaderThatSharesTiling> shadersThatShareTiling;

		public struct ShaderThatSharesTiling
		{
			public string shadername;

			public bool allPropsShareTiling;

			public string tilingTexturePropName;
		}
	}
}
