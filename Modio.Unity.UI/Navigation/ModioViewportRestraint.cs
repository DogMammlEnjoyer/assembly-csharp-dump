using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Modio.Unity.UI.Navigation
{
	public class ModioViewportRestraint : MonoBehaviour
	{
		public void ChildSelected(RectTransform ensureFits)
		{
			Vector3 a;
			Vector3 a2;
			ModioViewportRestraint.<ChildSelected>g__GetWorldAABB|11_0(ensureFits, out a, out a2);
			Vector3 vector;
			Vector3 a3;
			ModioViewportRestraint.<ChildSelected>g__GetWorldAABB|11_0(this.Viewport, out vector, out a3);
			Vector3 b = this.DefaultViewportContainer.position - this._targetPosition;
			vector += b;
			a3 += b;
			Vector3 vector2 = a3 - vector;
			Vector3 b2 = new Vector3(vector2.x * this.PercentPaddingHorizontal, vector2.y * this.PercentPaddingVertical);
			Vector3 a4 = Vector3.Max(Vector3.zero, a2 - (a3 - b2));
			Vector3 b3 = Vector3.Min(Vector3.zero, a - (vector + b2));
			Vector3 b4 = a4 + b3 + b;
			b4.z = 0f;
			if (!this.adjustHorizontally)
			{
				b4.x = 0f;
			}
			if (!this.adjustVertically)
			{
				b4.y = 0f;
			}
			if (b4.sqrMagnitude < 1f)
			{
				return;
			}
			this._targetPosition = this.DefaultViewportContainer.position - b4;
			if (this._animCoroutine != null)
			{
				base.StopCoroutine(this._animCoroutine);
			}
			this._animCoroutine = base.StartCoroutine(this.Transition(this.DefaultViewportContainer));
		}

		private IEnumerator Transition(Transform parent)
		{
			Vector2 startPos = parent.position;
			for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / ModioViewportRestraint.transitionTime)
			{
				parent.position = Vector3.Lerp(startPos, this._targetPosition, t);
				yield return null;
				if (!this.adjustHorizontally)
				{
					startPos.x = (this._targetPosition.x = parent.position.x);
				}
			}
			parent.position = this._targetPosition;
			this._animCoroutine = null;
			yield break;
		}

		[CompilerGenerated]
		internal static void <ChildSelected>g__GetWorldAABB|11_0(RectTransform rectTransform, out Vector3 min, out Vector3 max)
		{
			rectTransform.GetWorldCorners(ModioViewportRestraint.CachedFourCornersArray);
			min = Vector3.one * float.MaxValue;
			max = Vector3.one * float.MinValue;
			foreach (Vector3 rhs in ModioViewportRestraint.CachedFourCornersArray)
			{
				min = Vector3.Min(min, rhs);
				max = Vector3.Max(max, rhs);
			}
		}

		public float PercentPaddingHorizontal = 0.05f;

		public float PercentPaddingVertical = 0.25f;

		public bool adjustHorizontally;

		public bool adjustVertically = true;

		private static float transitionTime = 0.1f;

		public RectTransform Viewport;

		public RectTransform DefaultViewportContainer;

		public RectTransform HorizontalViewportContainer;

		private static readonly Vector3[] CachedFourCornersArray = new Vector3[4];

		private Vector3 _targetPosition;

		private Coroutine _animCoroutine;
	}
}
