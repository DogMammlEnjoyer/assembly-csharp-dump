using System;
using UnityEngine;

namespace Oculus.Interaction
{
	[Serializable]
	public struct DistantPointDetectorFrustums
	{
		public ConicalFrustum SelectionFrustum
		{
			get
			{
				return this._selectionFrustum;
			}
		}

		public ConicalFrustum DeselectionFrustum
		{
			get
			{
				return this._deselectionFrustum;
			}
		}

		public ConicalFrustum AidFrustum
		{
			get
			{
				return this._aidFrustum;
			}
		}

		public float AidBlending
		{
			get
			{
				return this._aidBlending;
			}
		}

		public DistantPointDetectorFrustums(ConicalFrustum selection, ConicalFrustum deselection, ConicalFrustum aid, float blend)
		{
			this._selectionFrustum = selection;
			this._deselectionFrustum = deselection;
			this._aidFrustum = aid;
			this._aidBlending = blend;
		}

		[SerializeField]
		private ConicalFrustum _selectionFrustum;

		[SerializeField]
		[Optional]
		private ConicalFrustum _deselectionFrustum;

		[SerializeField]
		[Optional]
		private ConicalFrustum _aidFrustum;

		[SerializeField]
		[Range(0f, 1f)]
		private float _aidBlending;
	}
}
