using System;
using System.Collections.Generic;
using Technie.PhysicsCreator.Rigid;
using UnityEngine;

namespace Technie.PhysicsCreator
{
	public class PaintingData : ScriptableObject, IEditorData
	{
		public int TotalOutputColliders
		{
			get
			{
				int num = 0;
				foreach (Hull hull in this.hulls)
				{
					if (hull.type == HullType.Auto)
					{
						num += ((hull.autoMeshes != null) ? hull.autoMeshes.Length : 0);
					}
					else
					{
						num++;
					}
				}
				return num;
			}
		}

		public Hash160 CachedHash
		{
			get
			{
				return this.sourceMeshHash;
			}
			set
			{
				this.sourceMeshHash = value;
			}
		}

		public bool HasCachedData
		{
			get
			{
				return this.sourceMeshHash != null && this.sourceMeshHash.IsValid();
			}
		}

		public Mesh SourceMesh
		{
			get
			{
				return this.sourceMesh;
			}
		}

		public IHull[] Hulls
		{
			get
			{
				return this.hulls.ToArray();
			}
		}

		public bool HasSuppressMeshModificationWarning
		{
			get
			{
				return this.suppressMeshModificationWarning;
			}
		}

		public void AddHull(HullType type, PhysicsMaterial material, bool isChild, bool isTrigger)
		{
			this.hulls.Add(new Hull());
			this.hulls[this.hulls.Count - 1].name = "Hull " + this.hulls.Count.ToString();
			this.activeHull = this.hulls.Count - 1;
			this.hulls[this.hulls.Count - 1].colour = GizmoUtils.GetHullColour(this.activeHull);
			this.hulls[this.hulls.Count - 1].type = type;
			this.hulls[this.hulls.Count - 1].material = material;
			this.hulls[this.hulls.Count - 1].isTrigger = isTrigger;
			this.hulls[this.hulls.Count - 1].isChildCollider = isChild;
		}

		public void RemoveHull(int index)
		{
			if (index < 0 || index >= this.hulls.Count)
			{
				return;
			}
			this.hulls[index].Destroy();
			this.hulls.RemoveAt(index);
		}

		public void RemoveAllHulls()
		{
			for (int i = 0; i < this.hulls.Count; i++)
			{
				this.hulls[i].Destroy();
			}
			this.hulls.Clear();
		}

		public bool HasActiveHull()
		{
			return this.activeHull >= 0 && this.activeHull < this.hulls.Count;
		}

		public Hull GetActiveHull()
		{
			if (this.activeHull < 0 || this.activeHull >= this.hulls.Count)
			{
				return null;
			}
			return this.hulls[this.activeHull];
		}

		public bool ContainsMesh(Mesh m)
		{
			foreach (Hull hull in this.hulls)
			{
				if (hull.collisionMesh == m)
				{
					return true;
				}
				if (hull.faceCollisionMesh == m)
				{
					return true;
				}
				if (hull.autoMeshes != null)
				{
					Mesh[] autoMeshes = hull.autoMeshes;
					for (int i = 0; i < autoMeshes.Length; i++)
					{
						if (autoMeshes[i] == m)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool HasAutoHulls()
		{
			using (List<Hull>.Enumerator enumerator = this.hulls.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.type == HullType.Auto)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void SetAssetDirty()
		{
		}

		public HullData hullData;

		public Mesh sourceMesh;

		public Hash160 sourceMeshHash;

		public int activeHull = -1;

		public float faceThickness = 0.1f;

		public List<Hull> hulls = new List<Hull>();

		public AutoHullPreset autoHullPreset = AutoHullPreset.Medium;

		public VhacdParameters vhacdParams = new VhacdParameters();

		public bool hasLastVhacdTimings;

		public AutoHullPreset lastVhacdPreset = AutoHullPreset.Medium;

		public float lastVhacdDurationSecs;

		public bool suppressMeshModificationWarning;
	}
}
