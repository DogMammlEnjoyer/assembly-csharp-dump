using System;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion
{
	public class XRMovableBody
	{
		public XROrigin xrOrigin { get; private set; }

		public Transform originTransform
		{
			get
			{
				return this.xrOrigin.Origin.transform;
			}
		}

		public IXRBodyPositionEvaluator bodyPositionEvaluator { get; private set; }

		public IConstrainedXRBodyManipulator constrainedManipulator { get; private set; }

		public XRMovableBody(XROrigin xrOrigin, IXRBodyPositionEvaluator bodyPositionEvaluator)
		{
			this.xrOrigin = xrOrigin;
			this.bodyPositionEvaluator = bodyPositionEvaluator;
		}

		public Vector3 GetBodyGroundLocalPosition()
		{
			return this.bodyPositionEvaluator.GetBodyGroundLocalPosition(this.xrOrigin);
		}

		public Vector3 GetBodyGroundWorldPosition()
		{
			return this.bodyPositionEvaluator.GetBodyGroundWorldPosition(this.xrOrigin);
		}

		public void LinkConstrainedManipulator(IConstrainedXRBodyManipulator manipulator)
		{
			IConstrainedXRBodyManipulator constrainedManipulator = this.constrainedManipulator;
			if (constrainedManipulator != null)
			{
				constrainedManipulator.OnUnlinkedFromBody();
			}
			XRMovableBody linkedBody = manipulator.linkedBody;
			if (linkedBody != null)
			{
				linkedBody.UnlinkConstrainedManipulator();
			}
			this.constrainedManipulator = manipulator;
			this.constrainedManipulator.OnLinkedToBody(this);
		}

		public void UnlinkConstrainedManipulator()
		{
			IConstrainedXRBodyManipulator constrainedManipulator = this.constrainedManipulator;
			if (constrainedManipulator != null)
			{
				constrainedManipulator.OnUnlinkedFromBody();
			}
			this.constrainedManipulator = null;
		}
	}
}
