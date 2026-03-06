using System;
using System.Linq;

namespace UnityEngine.Rendering.Universal
{
	public static class ShaderUtils
	{
		public static string GetShaderPath(ShaderPathID id)
		{
			int num = (int)id;
			int num2 = ShaderUtils.s_ShaderPaths.Length;
			if (num2 > 0 && num >= 0 && num < num2)
			{
				return ShaderUtils.s_ShaderPaths[num];
			}
			Debug.LogError(string.Concat(new string[]
			{
				"Trying to access universal shader path out of bounds: (",
				id.ToString(),
				": ",
				num.ToString(),
				")"
			}));
			return "";
		}

		public static ShaderPathID GetEnumFromPath(string path)
		{
			return (ShaderPathID)Array.FindIndex<string>(ShaderUtils.s_ShaderPaths, (string m) => m == path);
		}

		public static bool IsLWShader(Shader shader)
		{
			return ShaderUtils.s_ShaderPaths.Contains(shader.name);
		}

		internal static float PersistentDeltaTime
		{
			get
			{
				return Time.deltaTime;
			}
		}

		private static readonly string[] s_ShaderPaths = new string[]
		{
			"Universal Render Pipeline/Lit",
			"Universal Render Pipeline/Simple Lit",
			"Universal Render Pipeline/Unlit",
			"Universal Render Pipeline/Terrain/Lit",
			"Universal Render Pipeline/Particles/Lit",
			"Universal Render Pipeline/Particles/Simple Lit",
			"Universal Render Pipeline/Particles/Unlit",
			"Universal Render Pipeline/Baked Lit",
			"Universal Render Pipeline/Nature/SpeedTree7",
			"Universal Render Pipeline/Nature/SpeedTree7 Billboard",
			"Universal Render Pipeline/Nature/SpeedTree8_PBRLit",
			"SpeedTree9_Dummy_Path",
			"Universal Render Pipeline/Complex Lit"
		};
	}
}
