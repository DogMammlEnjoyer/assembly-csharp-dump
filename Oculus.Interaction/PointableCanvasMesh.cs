using System;
using Oculus.Interaction.UnityCanvas;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
	public class PointableCanvasMesh : PointableElement
	{
		protected override void Start()
		{
			base.Start();
		}

		public override void ProcessPointerEvent(PointerEvent evt)
		{
			Vector3 position = this._canvasMesh.ImposterToCanvasTransformPoint(evt.Pose.position);
			Pose pose = new Pose(position, evt.Pose.rotation);
			base.ProcessPointerEvent(new PointerEvent(evt.Identifier, evt.Type, pose, evt.Data));
		}

		public void InjectAllCanvasMeshPointable(CanvasMesh canvasMesh)
		{
			this.InjectCanvasMesh(canvasMesh);
		}

		public void InjectCanvasMesh(CanvasMesh canvasMesh)
		{
			this._canvasMesh = canvasMesh;
		}

		[Tooltip("This CanvasMesh determines the Pose of PointerEvents.")]
		[SerializeField]
		[FormerlySerializedAs("_canvasRenderTextureMesh")]
		private CanvasMesh _canvasMesh;
	}
}
