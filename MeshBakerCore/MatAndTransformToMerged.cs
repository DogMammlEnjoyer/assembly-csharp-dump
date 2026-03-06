using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class MatAndTransformToMerged
	{
		public DRect obUVRectIfTilingSame { get; private set; }

		public DRect samplingRectMatAndUVTiling { get; private set; }

		public DRect materialTiling { get; private set; }

		public MatAndTransformToMerged(DRect obUVrect, bool fixOutOfBoundsUVs)
		{
			this._init(obUVrect, fixOutOfBoundsUVs, null);
		}

		public MatAndTransformToMerged(DRect obUVrect, bool fixOutOfBoundsUVs, Material m)
		{
			this._init(obUVrect, fixOutOfBoundsUVs, m);
		}

		private void _init(DRect obUVrect, bool fixOutOfBoundsUVs, Material m)
		{
			if (fixOutOfBoundsUVs)
			{
				this.obUVRectIfTilingSame = obUVrect;
			}
			else
			{
				this.obUVRectIfTilingSame = new DRect(0f, 0f, 1f, 1f);
			}
			this.mat = m;
		}

		public override bool Equals(object obj)
		{
			if (obj is MatAndTransformToMerged)
			{
				MatAndTransformToMerged matAndTransformToMerged = (MatAndTransformToMerged)obj;
				if (matAndTransformToMerged.mat == this.mat && matAndTransformToMerged.obUVRectIfTilingSame == this.obUVRectIfTilingSame)
				{
					return true;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.mat.GetHashCode() ^ this.obUVRectIfTilingSame.GetHashCode() ^ this.samplingRectMatAndUVTiling.GetHashCode();
		}

		public string GetMaterialName()
		{
			if (this.mat != null)
			{
				return this.mat.name;
			}
			if (this.objName != null)
			{
				return string.Format("[matFor: {0}]", this.objName);
			}
			return "Unknown";
		}

		public void AssignInitialValuesForMaterialTilingAndSamplingRectMatAndUVTiling(bool allTexturesUseSameMatTiling, DRect matTiling)
		{
			if (allTexturesUseSameMatTiling)
			{
				this.materialTiling = matTiling;
			}
			else
			{
				this.materialTiling = new DRect(0f, 0f, 1f, 1f);
			}
			DRect materialTiling = this.materialTiling;
			DRect obUVRectIfTilingSame = this.obUVRectIfTilingSame;
			this.samplingRectMatAndUVTiling = MB3_UVTransformUtility.CombineTransforms(ref obUVRectIfTilingSame, ref materialTiling);
		}

		public Material mat;

		public string objName;
	}
}
