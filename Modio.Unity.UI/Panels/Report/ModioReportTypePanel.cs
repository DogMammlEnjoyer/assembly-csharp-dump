using System;
using Modio.Reports;

namespace Modio.Unity.UI.Panels.Report
{
	public class ModioReportTypePanel : ModioPanelBase
	{
		public void OnUserSubmittedReportType(int type)
		{
			this.OnUserSubmittedReportTypeEnum((ReportType)type);
		}

		public void OnUserSubmittedReportTypeEnum(ReportType type)
		{
			base.ClosePanel();
			ModioPanelManager.GetPanelOfType<ModioReportDetailsPanel>().OpenPanel(type);
		}
	}
}
