using System;

namespace UnityEngine.Rendering.RadeonRays
{
	internal static class Common
	{
		public static uint CeilDivide(uint val, uint div)
		{
			return (val + div - 1U) / div;
		}

		public static void EnableKeyword(CommandBuffer cmd, ComputeShader shader, string keyword, bool enable)
		{
			LocalKeyword localKeyword;
			if (enable)
			{
				localKeyword = new LocalKeyword(shader, keyword);
				cmd.EnableKeyword(shader, localKeyword);
				return;
			}
			localKeyword = new LocalKeyword(shader, keyword);
			cmd.DisableKeyword(shader, localKeyword);
		}
	}
}
