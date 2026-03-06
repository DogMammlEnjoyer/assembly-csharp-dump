using System;
using System.Collections.Generic;

namespace g3
{
	public class FaceGroupOptimizer
	{
		public DMesh3 Mesh
		{
			get
			{
				return this.mesh;
			}
		}

		public FaceGroupOptimizer(DMesh3 meshIn)
		{
			this.mesh = meshIn;
			this.GetEnumeratorF = (() => this.mesh.TriangleIndices());
		}

		public int ClipFins(bool bClipLoners)
		{
			this.temp.Clear();
			foreach (int num in this.GetEnumeratorF())
			{
				if (this.is_fin(num, bClipLoners))
				{
					this.temp.Add(new Index2i(num, this.BackgroundGroupID));
				}
			}
			if (this.temp.Count == 0)
			{
				return 0;
			}
			foreach (Index2i index2i in this.temp)
			{
				this.mesh.SetTriangleGroup(index2i.a, index2i.b);
			}
			return this.temp.Count;
		}

		public int FillEars(bool bFillTinyHoles)
		{
			int num = 0;
			foreach (int tid in this.GetEnumeratorF())
			{
				int num2 = this.is_ear(tid, bFillTinyHoles, this.NoEarGroupSwaps);
				if (num2 >= 0)
				{
					this.mesh.SetTriangleGroup(tid, num2);
					num++;
				}
			}
			return num;
		}

		public bool LocalOptimize(bool bClipFins, bool bFillEars, bool bFillTinyHoles = true, bool bClipLoners = true, int max_iters = 100)
		{
			bool result = false;
			bool flag = false;
			int num = 0;
			while (!flag && num++ < max_iters)
			{
				flag = true;
				int num2 = 0;
				int num3 = 0;
				if (bClipFins)
				{
					num2 = this.ClipFins(bClipLoners);
				}
				if (bFillEars)
				{
					num3 = this.FillEars(bFillTinyHoles);
				}
				if (num2 > 0 || num3 > 0)
				{
					flag = false;
					result = true;
				}
			}
			return result;
		}

		public int DilateAllGroups(int nRings)
		{
			int num = 0;
			for (int i = 0; i < nRings; i++)
			{
				this.temp.Clear();
				foreach (int num2 in this.GetEnumeratorF())
				{
					if (this.mesh.GetTriangleGroup(num2) == this.BackgroundGroupID)
					{
						Index3i triNeighbourTris = this.mesh.GetTriNeighbourTris(num2);
						for (int j = 0; j < 3; j++)
						{
							if (triNeighbourTris[j] != -1)
							{
								int triangleGroup = this.mesh.GetTriangleGroup(triNeighbourTris[j]);
								if (triangleGroup != this.BackgroundGroupID)
								{
									this.temp.Add(new Index2i(num2, triangleGroup));
									break;
								}
							}
						}
					}
				}
				if (this.temp.Count == 0)
				{
					return num;
				}
				foreach (Index2i index2i in this.temp)
				{
					if (this.mesh.GetTriangleGroup(index2i.a) == this.BackgroundGroupID)
					{
						this.mesh.SetTriangleGroup(index2i.a, index2i.b);
						num++;
					}
				}
			}
			return num;
		}

		public int ContractAllGroups(int nRings, bool bBackgroundOnly)
		{
			int num = 0;
			for (int i = 0; i < nRings; i++)
			{
				this.temp.Clear();
				foreach (int num2 in this.GetEnumeratorF())
				{
					int triangleGroup = this.mesh.GetTriangleGroup(num2);
					Index3i triNeighbourTris = this.mesh.GetTriNeighbourTris(num2);
					bool flag = false;
					if (bBackgroundOnly)
					{
						for (int j = 0; j < 3; j++)
						{
							if (flag)
							{
								break;
							}
							if (triNeighbourTris[j] != -1 && this.mesh.GetTriangleGroup(triNeighbourTris[j]) == this.BackgroundGroupID)
							{
								flag = true;
							}
						}
					}
					else
					{
						int num3 = 0;
						while (num3 < 3 && !flag)
						{
							if (triNeighbourTris[num3] != -1 && this.mesh.GetTriangleGroup(triNeighbourTris[num3]) != triangleGroup)
							{
								flag = true;
							}
							num3++;
						}
					}
					if (flag)
					{
						this.temp.Add(new Index2i(num2, this.BackgroundGroupID));
					}
				}
				if (this.temp.Count == 0)
				{
					return num;
				}
				foreach (Index2i index2i in this.temp)
				{
					this.mesh.SetTriangleGroup(index2i.a, index2i.b);
					num++;
				}
			}
			return num;
		}

		private int find_max_nbr(int tid, out int nbr_same, out int nbr_diff, out int bdry_e)
		{
			Index3i triNeighbourTris = this.mesh.GetTriNeighbourTris(tid);
			Index3i max = Index3i.Max;
			for (int i = 0; i < 3; i++)
			{
				int num = triNeighbourTris[i];
				max[i] = ((num == -1) ? -1 : this.mesh.GetTriangleGroup(triNeighbourTris[i]));
			}
			int num2 = -1;
			for (int j = 0; j < 3; j++)
			{
				if (max[j] != -1 && (max[j] == max[(j + 1) % 3] || max[j] == max[(j + 2) % 3]))
				{
					num2 = j;
				}
			}
			nbr_same = 1;
			nbr_diff = 0;
			bdry_e = 0;
			if (num2 == -1)
			{
				return -1;
			}
			int num3 = max[num2];
			for (int k = 1; k < 3; k++)
			{
				int key = (num2 + k) % 3;
				if (max[key] == -1)
				{
					bdry_e++;
				}
				else if (max[key] == num3)
				{
					nbr_same++;
				}
				else
				{
					nbr_diff++;
				}
			}
			return num3;
		}

		private int is_ear(int tid, bool include_tiny_holes, bool bBackgroundOnly)
		{
			int triangleGroup = this.mesh.GetTriangleGroup(tid);
			if (bBackgroundOnly && triangleGroup != this.BackgroundGroupID)
			{
				return -1;
			}
			int num2;
			int num3;
			int num4;
			int num = this.find_max_nbr(tid, out num2, out num3, out num4);
			if (num == -1 || num == triangleGroup)
			{
				return -1;
			}
			if (num4 == 2 && num2 == 1)
			{
				return num;
			}
			if (num2 == 2)
			{
				if (num4 == 1 || num3 == 1)
				{
					return num;
				}
			}
			else if (include_tiny_holes && num2 == 3)
			{
				return num;
			}
			return -1;
		}

		private void count_same_nbrs(int tid, out int nbr_same, out int nbr_diff, out int nbr_bg, out int bdry_e)
		{
			int triangleGroup = this.mesh.GetTriangleGroup(tid);
			Index3i triNeighbourTris = this.mesh.GetTriNeighbourTris(tid);
			nbr_same = 0;
			nbr_diff = 0;
			bdry_e = 0;
			nbr_bg = 0;
			for (int i = 0; i < 3; i++)
			{
				int num = triNeighbourTris[i];
				if (num == -1)
				{
					bdry_e++;
				}
				else
				{
					int triangleGroup2 = this.mesh.GetTriangleGroup(num);
					if (triangleGroup2 == this.BackgroundGroupID)
					{
						nbr_bg++;
					}
					if (triangleGroup2 == triangleGroup)
					{
						nbr_same++;
					}
					else
					{
						nbr_diff++;
					}
				}
			}
		}

		private bool is_fin(int tid, bool include_loners)
		{
			if (this.mesh.GetTriangleGroup(tid) == this.BackgroundGroupID)
			{
				return false;
			}
			int num;
			int num2;
			int num3;
			int num4;
			this.count_same_nbrs(tid, out num, out num2, out num3, out num4);
			bool flag = (num == 1 && num2 == 2) || (include_loners && num == 0 && num2 == 3);
			if (this.DontClipEnclosedFins && (flag & num3 == 0))
			{
				flag = false;
			}
			return flag;
		}

		private DMesh3 mesh;

		public Func<IEnumerable<int>> GetEnumeratorF;

		public int BackgroundGroupID;

		public bool DontClipEnclosedFins = true;

		public bool NoEarGroupSwaps;

		private List<Index2i> temp = new List<Index2i>();
	}
}
