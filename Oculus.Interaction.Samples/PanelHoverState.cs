using System;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class PanelHoverState : MonoBehaviour
{
	public bool Hovered
	{
		get
		{
			return this.hovered;
		}
	}

	public event Action<bool> WhenStateChanged = delegate(bool <p0>)
	{
	};

	private void Update()
	{
		bool flag = this.hovered;
		this.hovered = false;
		using (List<Grabbable>.Enumerator enumerator = this.grabbables.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.PointsCount > 0)
				{
					this.hovered = true;
					break;
				}
			}
		}
		if (flag != this.hovered)
		{
			this.WhenStateChanged(this.hovered);
		}
	}

	public List<Grabbable> grabbables = new List<Grabbable>();

	private bool hovered;
}
