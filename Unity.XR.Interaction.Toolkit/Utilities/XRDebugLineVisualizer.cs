using System;
using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
	[AddComponentMenu("")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Utilities.XRDebugLineVisualizer.html")]
	internal class XRDebugLineVisualizer : MonoBehaviour
	{
		private void Update()
		{
			for (int i = this.m_DebugLines.Count - 1; i >= 0; i--)
			{
				this.m_DebugLines[i].decayTime -= Time.deltaTime;
				if (this.m_DebugLines[i].decayTime <= 0f)
				{
					Object.Destroy(this.m_DebugLines[i].lineRenderer.gameObject);
					this.m_DebugLines.RemoveAt(i);
				}
			}
		}

		private void OnDestroy()
		{
			this.ClearLines();
		}

		public void UpdateOrCreateLine(string lineName, Vector3 start, Vector3 end, Color color, float decayTime = 0.2f)
		{
			XRDebugLineVisualizer.DebugLine debugLine = this.m_DebugLines.Find((XRDebugLineVisualizer.DebugLine l) => l.name == lineName);
			if (debugLine == null)
			{
				GameObject gameObject = new GameObject(lineName + "Line");
				gameObject.transform.SetParent(base.transform, false);
				LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
				lineRenderer.startWidth = 0.01f;
				lineRenderer.endWidth = 0.01f;
				lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
				lineRenderer.startColor = color;
				lineRenderer.endColor = color;
				debugLine = new XRDebugLineVisualizer.DebugLine
				{
					name = lineName,
					color = color,
					lineRenderer = lineRenderer
				};
				this.m_DebugLines.Add(debugLine);
			}
			debugLine.lineRenderer.SetPosition(0, start);
			debugLine.lineRenderer.SetPosition(1, end);
			debugLine.decayTime = decayTime;
		}

		public void ClearLines()
		{
			foreach (XRDebugLineVisualizer.DebugLine debugLine in this.m_DebugLines)
			{
				Object.Destroy(debugLine.lineRenderer.gameObject);
			}
			this.m_DebugLines.Clear();
		}

		private List<XRDebugLineVisualizer.DebugLine> m_DebugLines = new List<XRDebugLineVisualizer.DebugLine>();

		[Serializable]
		private class DebugLine
		{
			public string name;

			public Color color;

			public LineRenderer lineRenderer;

			public float decayTime;
		}
	}
}
