using System;

namespace Meta.XR.MRUtilityKit
{
	internal struct Mesh2fDisposer : IDisposable
	{
		internal Mesh2fDisposer(MRUKNativeFuncs.MrukMesh2f mesh)
		{
			this.Mesh = mesh;
		}

		public void Dispose()
		{
			MRUKNativeFuncs.FreeMesh(ref this.Mesh);
		}

		public MRUKNativeFuncs.MrukMesh2f Mesh;
	}
}
