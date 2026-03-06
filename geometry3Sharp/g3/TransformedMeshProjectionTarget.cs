using System;

namespace g3
{
	public class TransformedMeshProjectionTarget : MeshProjectionTarget
	{
		public TransformedMeshProjectionTarget()
		{
		}

		public TransformedMeshProjectionTarget(DMesh3 mesh, ISpatial spatial) : base(mesh, spatial)
		{
		}

		public TransformedMeshProjectionTarget(DMesh3 mesh) : base(mesh)
		{
		}

		public void SetTransform(TransformSequence sourceToTargetX)
		{
			this.SourceToTargetXForm = sourceToTargetX;
			this.TargetToSourceXForm = this.SourceToTargetXForm.MakeInverse();
		}

		public override Vector3d Project(Vector3d vPoint, int identifier = -1)
		{
			Vector3d vPoint2 = this.SourceToTargetXForm.TransformP(vPoint);
			Vector3d p = base.Project(vPoint2, identifier);
			return this.TargetToSourceXForm.TransformP(p);
		}

		public override Vector3d Project(Vector3d vPoint, out Vector3d vProjectNormal, int identifier = -1)
		{
			Vector3d vPoint2 = this.SourceToTargetXForm.TransformP(vPoint);
			Vector3d v;
			Vector3d p = base.Project(vPoint2, out v, identifier);
			vProjectNormal = this.TargetToSourceXForm.TransformV(v).Normalized;
			return this.TargetToSourceXForm.TransformP(p);
		}

		public TransformSequence SourceToTargetXForm;

		public TransformSequence TargetToSourceXForm;
	}
}
