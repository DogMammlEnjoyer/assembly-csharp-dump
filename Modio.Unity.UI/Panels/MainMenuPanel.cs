using System;

namespace Modio.Unity.UI.Panels
{
	public class MainMenuPanel : ModioPanelBase
	{
		protected override void Start()
		{
			base.OpenPanel();
			base.Start();
		}

		protected override void CancelPressed()
		{
		}
	}
}
