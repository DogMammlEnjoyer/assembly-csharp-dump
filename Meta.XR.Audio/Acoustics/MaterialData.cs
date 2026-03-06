using System;
using UnityEngine;

namespace Meta.XR.Acoustics
{
	[Serializable]
	public class MaterialData
	{
		internal void Clone(MaterialData other)
		{
			this.color = other.color;
			this.absorption.Clone(other.absorption);
			this.transmission.Clone(other.transmission);
			this.scattering.Clone(other.scattering);
		}

		internal bool IsEmpty
		{
			get
			{
				return this.absorption.points.Count == 0 && this.transmission.points.Count == 0 && this.scattering.points.Count == 0;
			}
		}

		[SerializeField]
		internal Spectrum absorption = new Spectrum(null);

		[SerializeField]
		internal Spectrum transmission = new Spectrum(null);

		[SerializeField]
		internal Spectrum scattering = new Spectrum(null);

		[SerializeField]
		internal Color color = Color.yellow;
	}
}
