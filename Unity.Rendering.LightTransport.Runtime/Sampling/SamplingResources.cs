using System;

namespace UnityEngine.Rendering.Sampling
{
	internal sealed class SamplingResources : IDisposable
	{
		public static void BindSobolBlueNoiseTextures(CommandBuffer cmd, SamplingResources resources)
		{
			if (resources.m_SobolScramblingTile != null)
			{
				cmd.SetGlobalTexture(Shader.PropertyToID("_SobolScramblingTile"), resources.m_SobolScramblingTile);
				cmd.SetGlobalTexture(Shader.PropertyToID("_SobolRankingTile"), resources.m_SobolRankingTile);
				cmd.SetGlobalTexture(Shader.PropertyToID("_SobolOwenScrambledSequence"), resources.m_SobolOwenScrambled256Samples);
			}
			if (resources.m_SobolBuffer != null)
			{
				cmd.SetGlobalBuffer("_SobolMatricesBuffer", resources.m_SobolBuffer);
			}
		}

		public void Dispose()
		{
			GraphicsBuffer sobolBuffer = this.m_SobolBuffer;
			if (sobolBuffer == null)
			{
				return;
			}
			sobolBuffer.Dispose();
		}

		private Texture2D m_SobolScramblingTile;

		private Texture2D m_SobolRankingTile;

		private Texture2D m_SobolOwenScrambled256Samples;

		private GraphicsBuffer m_SobolBuffer;

		public static readonly uint[] sobolMatrices = SobolData.SobolMatrices;

		internal enum ResourceType
		{
			BlueNoiseTextures = 1,
			SobolMatrices,
			All
		}
	}
}
