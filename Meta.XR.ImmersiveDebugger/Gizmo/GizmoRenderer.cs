using System;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.Gizmo
{
	internal class GizmoRenderer : MonoBehaviour
	{
		public void SetUpGizmo(DebugGizmoType gizmoType, Color gizmoColor)
		{
			this._gizmoType = gizmoType;
			this._gizmoColor = gizmoColor;
		}

		public void UpdateDataSource(object dataSource)
		{
			this._dataSource = dataSource;
		}

		private void Start()
		{
			DebugGizmos.LineWidth = 0.01f;
		}

		private void Update()
		{
			using (new DebugGizmos.ColorScope(this._gizmoColor))
			{
				GizmoTypesRegistry.RenderGizmo(this._gizmoType, this._dataSource);
			}
		}

		private DebugGizmoType _gizmoType;

		private Color _gizmoColor;

		private object _dataSource;
	}
}
