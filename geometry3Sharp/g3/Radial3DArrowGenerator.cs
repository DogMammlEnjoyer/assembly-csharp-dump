using System;

namespace g3
{
	public class Radial3DArrowGenerator : VerticalGeneralizedCylinderGenerator
	{
		public override MeshGenerator Generate()
		{
			this.Sections = new MeshGenerator.CircularSection[4];
			this.Sections[0] = new MeshGenerator.CircularSection(this.StickRadius, 0f);
			this.Sections[1] = new MeshGenerator.CircularSection(this.StickRadius, this.StickLength);
			this.Sections[2] = new MeshGenerator.CircularSection(this.HeadBaseRadius, this.StickLength);
			this.Sections[3] = new MeshGenerator.CircularSection(this.TipRadius, this.StickLength + this.HeadLength);
			this.Capped = true;
			this.NoSharedVertices = true;
			base.Generate();
			return this;
		}

		public float StickRadius = 0.5f;

		public float StickLength = 1f;

		public float HeadBaseRadius = 1f;

		public float TipRadius;

		public float HeadLength = 0.5f;
	}
}
