using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.UtilityScripts
{
	public class CullArea : MonoBehaviour
	{
		public int CellCount { get; private set; }

		public CellTree CellTree { get; private set; }

		public Dictionary<int, GameObject> Map { get; private set; }

		private void Awake()
		{
			this.idCounter = this.FIRST_GROUP_ID;
			this.CreateCellHierarchy();
		}

		public void OnDrawGizmos()
		{
			this.idCounter = this.FIRST_GROUP_ID;
			if (this.RecreateCellHierarchy)
			{
				this.CreateCellHierarchy();
			}
			this.DrawCells();
		}

		private void CreateCellHierarchy()
		{
			if (!this.IsCellCountAllowed())
			{
				if (Debug.isDebugBuild)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"There are too many cells created by your subdivision options. Maximum allowed number of cells is ",
						((int)(250 - this.FIRST_GROUP_ID)).ToString(),
						". Current number of cells is ",
						this.CellCount.ToString(),
						"."
					}));
					return;
				}
				Application.Quit();
			}
			byte b = this.idCounter;
			this.idCounter = b + 1;
			CellTreeNode cellTreeNode = new CellTreeNode(b, CellTreeNode.ENodeType.Root, null);
			if (this.YIsUpAxis)
			{
				this.Center = new Vector2(base.transform.position.x, base.transform.position.y);
				this.Size = new Vector2(base.transform.localScale.x, base.transform.localScale.y);
				cellTreeNode.Center = new Vector3(this.Center.x, this.Center.y, 0f);
				cellTreeNode.Size = new Vector3(this.Size.x, this.Size.y, 0f);
				cellTreeNode.TopLeft = new Vector3(this.Center.x - this.Size.x / 2f, this.Center.y - this.Size.y / 2f, 0f);
				cellTreeNode.BottomRight = new Vector3(this.Center.x + this.Size.x / 2f, this.Center.y + this.Size.y / 2f, 0f);
			}
			else
			{
				this.Center = new Vector2(base.transform.position.x, base.transform.position.z);
				this.Size = new Vector2(base.transform.localScale.x, base.transform.localScale.z);
				cellTreeNode.Center = new Vector3(this.Center.x, 0f, this.Center.y);
				cellTreeNode.Size = new Vector3(this.Size.x, 0f, this.Size.y);
				cellTreeNode.TopLeft = new Vector3(this.Center.x - this.Size.x / 2f, 0f, this.Center.y - this.Size.y / 2f);
				cellTreeNode.BottomRight = new Vector3(this.Center.x + this.Size.x / 2f, 0f, this.Center.y + this.Size.y / 2f);
			}
			this.CreateChildCells(cellTreeNode, 1);
			this.CellTree = new CellTree(cellTreeNode);
			this.RecreateCellHierarchy = false;
		}

		private void CreateChildCells(CellTreeNode parent, int cellLevelInHierarchy)
		{
			if (cellLevelInHierarchy > this.NumberOfSubdivisions)
			{
				return;
			}
			int num = (int)this.Subdivisions[cellLevelInHierarchy - 1].x;
			int num2 = (int)this.Subdivisions[cellLevelInHierarchy - 1].y;
			float num3 = parent.Center.x - parent.Size.x / 2f;
			float num4 = parent.Size.x / (float)num;
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					float num5 = num3 + (float)i * num4 + num4 / 2f;
					byte b = this.idCounter;
					this.idCounter = b + 1;
					CellTreeNode cellTreeNode = new CellTreeNode(b, (this.NumberOfSubdivisions == cellLevelInHierarchy) ? CellTreeNode.ENodeType.Leaf : CellTreeNode.ENodeType.Node, parent);
					if (this.YIsUpAxis)
					{
						float num6 = parent.Center.y - parent.Size.y / 2f;
						float num7 = parent.Size.y / (float)num2;
						float num8 = num6 + (float)j * num7 + num7 / 2f;
						cellTreeNode.Center = new Vector3(num5, num8, 0f);
						cellTreeNode.Size = new Vector3(num4, num7, 0f);
						cellTreeNode.TopLeft = new Vector3(num5 - num4 / 2f, num8 - num7 / 2f, 0f);
						cellTreeNode.BottomRight = new Vector3(num5 + num4 / 2f, num8 + num7 / 2f, 0f);
					}
					else
					{
						float num9 = parent.Center.z - parent.Size.z / 2f;
						float num10 = parent.Size.z / (float)num2;
						float num11 = num9 + (float)j * num10 + num10 / 2f;
						cellTreeNode.Center = new Vector3(num5, 0f, num11);
						cellTreeNode.Size = new Vector3(num4, 0f, num10);
						cellTreeNode.TopLeft = new Vector3(num5 - num4 / 2f, 0f, num11 - num10 / 2f);
						cellTreeNode.BottomRight = new Vector3(num5 + num4 / 2f, 0f, num11 + num10 / 2f);
					}
					parent.AddChild(cellTreeNode);
					this.CreateChildCells(cellTreeNode, cellLevelInHierarchy + 1);
				}
			}
		}

		private void DrawCells()
		{
			if (this.CellTree != null && this.CellTree.RootNode != null)
			{
				this.CellTree.RootNode.Draw();
				return;
			}
			this.RecreateCellHierarchy = true;
		}

		private bool IsCellCountAllowed()
		{
			int num = 1;
			int num2 = 1;
			foreach (Vector2 vector in this.Subdivisions)
			{
				num *= (int)vector.x;
				num2 *= (int)vector.y;
			}
			this.CellCount = num * num2;
			return this.CellCount <= (int)(250 - this.FIRST_GROUP_ID);
		}

		public List<byte> GetActiveCells(Vector3 position)
		{
			List<byte> list = new List<byte>(0);
			this.CellTree.RootNode.GetActiveCells(list, this.YIsUpAxis, position);
			int num = this.NumberOfSubdivisions + 1;
			int num2 = list.Count - num;
			if (num2 > 0)
			{
				list.Sort(num, num2, new ByteComparer());
			}
			return list;
		}

		private const int MAX_NUMBER_OF_ALLOWED_CELLS = 250;

		public const int MAX_NUMBER_OF_SUBDIVISIONS = 3;

		public readonly byte FIRST_GROUP_ID = 1;

		public readonly int[] SUBDIVISION_FIRST_LEVEL_ORDER = new int[]
		{
			0,
			1,
			1,
			1
		};

		public readonly int[] SUBDIVISION_SECOND_LEVEL_ORDER = new int[]
		{
			0,
			2,
			1,
			2,
			0,
			2,
			1,
			2
		};

		public readonly int[] SUBDIVISION_THIRD_LEVEL_ORDER = new int[]
		{
			0,
			3,
			2,
			3,
			1,
			3,
			2,
			3,
			1,
			3,
			2,
			3
		};

		public Vector2 Center;

		public Vector2 Size = new Vector2(25f, 25f);

		public Vector2[] Subdivisions = new Vector2[3];

		public int NumberOfSubdivisions;

		public bool YIsUpAxis;

		public bool RecreateCellHierarchy;

		private byte idCounter;
	}
}
