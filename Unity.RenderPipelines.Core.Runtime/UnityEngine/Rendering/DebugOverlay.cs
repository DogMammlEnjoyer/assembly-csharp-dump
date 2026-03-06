using System;

namespace UnityEngine.Rendering
{
	public class DebugOverlay
	{
		public int x { get; private set; }

		public int y { get; private set; }

		public int overlaySize { get; private set; }

		public void StartOverlay(int initialX, int initialY, int overlaySize, int screenWidth)
		{
			this.x = initialX;
			this.y = initialY;
			this.overlaySize = overlaySize;
			this.m_InitialPositionX = initialX;
			this.m_ScreenWidth = screenWidth;
		}

		public Rect Next(float aspect = 1f)
		{
			int num = (int)((float)this.overlaySize * aspect);
			if (this.x + num > this.m_ScreenWidth && this.x > this.m_InitialPositionX)
			{
				this.x = this.m_InitialPositionX;
				this.y -= this.overlaySize;
			}
			Rect result = new Rect((float)this.x, (float)this.y, (float)num, (float)this.overlaySize);
			this.x += num;
			return result;
		}

		public void SetViewport(CommandBuffer cmd)
		{
			cmd.SetViewport(new Rect((float)this.x, (float)this.y, (float)this.overlaySize, (float)this.overlaySize));
		}

		private int m_InitialPositionX;

		private int m_ScreenWidth;
	}
}
