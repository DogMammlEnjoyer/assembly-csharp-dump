using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace Liv.Lck.GorillaTag
{
	[RequireComponent(typeof(BoxCollider))]
	public class GtColliderTriggerProcessor : MonoBehaviour
	{
		public Vector3 LastTapPosition { get; private set; }

		private void Start()
		{
			this._boxCollider = base.GetComponent<BoxCollider>();
		}

		private GtTag GetGTag(Collider other)
		{
			GtTag component = other.GetComponent<GtTag>();
			if (component == null)
			{
				return null;
			}
			if (component.gtTagType == GtTagType.LeftHand || component.gtTagType == GtTagType.RightHand)
			{
				return component;
			}
			return null;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (this._gtTag != null)
			{
				return;
			}
			this._gtTag = this.GetGTag(other);
			if (this._gtTag == null)
			{
				return;
			}
			if (this.IsColliderGrabbingTablet(this._gtTag))
			{
				return;
			}
			if (this._group != null)
			{
				if (this._group.GetCurrentTriggerProcessor() != null)
				{
					return;
				}
				this._group.SetCurrentTriggerProcessor(this);
			}
			this.LastTapPosition = other.ClosestPoint(base.transform.position);
			if (!this.IsTapValid(this.LastTapPosition))
			{
				return;
			}
			if (!this._canTap)
			{
				return;
			}
			this._isTapped = true;
			this._onTriggeredStarted.Invoke();
		}

		private bool IsColliderGrabbingTablet(GtTag tag)
		{
			return GtColliderTriggerProcessor.IsGrabbingTablet && ((tag.gtTagType == GtTagType.LeftHand && GtColliderTriggerProcessor.CurrentGrabbedHand == XRNode.LeftHand) || (tag.gtTagType == GtTagType.RightHand && GtColliderTriggerProcessor.CurrentGrabbedHand == XRNode.RightHand));
		}

		private void OnTriggerExit(Collider other)
		{
			if (this.GetGTag(other) != this._gtTag)
			{
				return;
			}
			this._gtTag = null;
			if (this._group != null)
			{
				if (this._group.GetCurrentTriggerProcessor() != this)
				{
					return;
				}
				this._group.SetCurrentTriggerProcessor(null);
			}
			if (!this._canTap)
			{
				return;
			}
			this._canTap = false;
			base.StartCoroutine(this.AllowTap());
			if (this._isTapped)
			{
				this._isTapped = false;
				this._onTriggeredEnded.Invoke();
			}
		}

		public void ResetToDefaultAfterTap()
		{
			this._canTap = false;
			this._isTapped = false;
			this._gtTag = null;
			base.StartCoroutine(this.AllowTap());
			base.Invoke("SetTriggerNull", this._tapCooldownTime);
		}

		public void BlockTapping()
		{
			this._canTap = false;
			this._isTapped = false;
			this._gtTag = null;
		}

		public void ResetToDefault()
		{
			this._canTap = true;
			this._isTapped = false;
			this._gtTag = null;
			this.SetTriggerNull();
		}

		public void ResetToDefaultAndTriggerButton()
		{
			this._canTap = true;
			this._isTapped = false;
			this._gtTag = null;
			this.SetTriggerNull();
			this._onTriggeredEnded.Invoke();
		}

		private void SetTriggerNull()
		{
			this._group.SetCurrentTriggerProcessor(null);
		}

		private void OnEnable()
		{
			this._canTap = true;
			this._isTapped = false;
		}

		private bool IsTapValid(Vector3 tapPosition)
		{
			Vector3 vector = tapPosition - base.transform.position;
			Vector3 vector2 = Vector3.Scale(this._boxCollider.size, base.transform.lossyScale);
			if (this._checkTriggerFromAbove)
			{
				Vector3 rhs = vector;
				rhs.Normalize();
				float num = Vector3.Dot(base.transform.up, rhs);
				float num2 = Vector3.Dot(base.transform.forward, vector);
				return num > 0.1f && num2 > 0f;
			}
			float num3 = vector2.z * 0.5f;
			return Vector3.Dot(base.transform.forward, vector) > num3;
		}

		private IEnumerator AllowTap()
		{
			yield return new WaitForSeconds(this._tapCooldownTime);
			this._canTap = true;
			yield break;
		}

		[Header("Global Settings")]
		[SerializeField]
		private GtUiSettings _settings;

		[Header("Parameters")]
		[SerializeField]
		private GtColliderTriggerProcessorsGroup _group;

		[SerializeField]
		private float _tapCooldownTime = 0.25f;

		[SerializeField]
		private bool _checkTriggerFromAbove;

		[Header("Events")]
		[SerializeField]
		private UnityEvent _onTriggeredStarted;

		[SerializeField]
		private UnityEvent _onTriggeredEnded;

		private bool _canTap = true;

		private bool _isTapped;

		private GtTag _gtTag;

		private BoxCollider _boxCollider;

		public static bool IsGrabbingTablet;

		public static XRNode CurrentGrabbedHand;
	}
}
