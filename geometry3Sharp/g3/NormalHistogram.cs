using System;
using System.Collections.Generic;

namespace g3
{
	public class NormalHistogram
	{
		public NormalHistogram(int bins, bool bTrackUsed = false)
		{
			this.Bins = bins;
			this.Points = new SphericalFibonacciPointSet(bins);
			this.Counts = new double[bins];
			if (bTrackUsed)
			{
				this.UsedBins = new HashSet<int>();
			}
		}

		public NormalHistogram(DMesh3 mesh, bool bWeightByArea = true, int bins = 1024) : this(bins, false)
		{
			this.CountFaceNormals(mesh, bWeightByArea);
		}

		public void Count(Vector3d pt, double weight = 1.0, bool bIsNormalized = false)
		{
			int num = this.Points.NearestPoint(pt, bIsNormalized);
			this.Counts[num] += weight;
			if (this.UsedBins != null)
			{
				this.UsedBins.Add(num);
			}
		}

		public void CountFaceNormals(DMesh3 mesh, bool bWeightByArea = true)
		{
			foreach (int tID in mesh.TriangleIndices())
			{
				if (bWeightByArea)
				{
					Vector3d pt;
					double weight;
					Vector3d vector3d;
					mesh.GetTriInfo(tID, out pt, out weight, out vector3d);
					this.Count(pt, weight, true);
				}
				else
				{
					this.Count(mesh.GetTriNormal(tID), 1.0, true);
				}
			}
		}

		public Vector3d FindMaxNormal()
		{
			int num = 0;
			for (int i = 1; i < this.Bins; i++)
			{
				if (this.Counts[i] > this.Counts[num])
				{
					num = i;
				}
			}
			return this.Points[num];
		}

		public int Bins = 1024;

		public SphericalFibonacciPointSet Points;

		public double[] Counts;

		public HashSet<int> UsedBins;
	}
}
