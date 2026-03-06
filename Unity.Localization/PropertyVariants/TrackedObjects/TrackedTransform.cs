using System;
using System.Collections.Generic;
using UnityEngine.Localization.PropertyVariants.TrackedProperties;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.PropertyVariants.TrackedObjects
{
	[DisplayName("Transform", null)]
	[CustomTrackedObject(typeof(Transform), false)]
	[Serializable]
	public class TrackedTransform : TrackedObject
	{
		protected virtual void AddPropertyHandlers(Dictionary<string, Action<float>> handlers)
		{
			handlers["m_LocalPosition.x"] = delegate(float val)
			{
				this.m_PositionToApply.x = val;
			};
			handlers["m_LocalPosition.y"] = delegate(float val)
			{
				this.m_PositionToApply.y = val;
			};
			handlers["m_LocalPosition.z"] = delegate(float val)
			{
				this.m_PositionToApply.z = val;
			};
			handlers["m_LocalRotation.x"] = delegate(float val)
			{
				this.m_RotationToApply.x = val;
			};
			handlers["m_LocalRotation.y"] = delegate(float val)
			{
				this.m_RotationToApply.y = val;
			};
			handlers["m_LocalRotation.z"] = delegate(float val)
			{
				this.m_RotationToApply.z = val;
			};
			handlers["m_LocalRotation.w"] = delegate(float val)
			{
				this.m_RotationToApply.w = val;
			};
			handlers["m_LocalScale.x"] = delegate(float val)
			{
				this.m_ScaleToApply.x = val;
			};
			handlers["m_LocalScale.y"] = delegate(float val)
			{
				this.m_ScaleToApply.y = val;
			};
			handlers["m_LocalScale.z"] = delegate(float val)
			{
				this.m_ScaleToApply.z = val;
			};
		}

		public override bool CanTrackProperty(string propertyPath)
		{
			return !propertyPath.StartsWith("m_LocalEulerAnglesHint") && !(propertyPath == "m_RootOrder");
		}

		public override AsyncOperationHandle ApplyLocale(Locale variantLocale, Locale defaultLocale)
		{
			Transform transform = (Transform)base.Target;
			if (this.m_PropertyHandlers == null)
			{
				this.m_PropertyHandlers = new Dictionary<string, Action<float>>();
				this.AddPropertyHandlers(this.m_PropertyHandlers);
			}
			this.m_PositionToApply = transform.localPosition;
			this.m_RotationToApply = transform.localRotation;
			this.m_ScaleToApply = transform.localScale;
			LocaleIdentifier identifier = variantLocale.Identifier;
			LocaleIdentifier fallback = (defaultLocale != null) ? defaultLocale.Identifier : default(LocaleIdentifier);
			foreach (ITrackedProperty trackedProperty in base.TrackedProperties)
			{
				float obj;
				Action<float> action;
				if (((FloatTrackedProperty)trackedProperty).GetValue(identifier, fallback, out obj) && this.m_PropertyHandlers.TryGetValue(trackedProperty.PropertyPath, out action))
				{
					action(obj);
				}
			}
			transform.localScale = this.m_ScaleToApply;
			transform.localPosition = this.m_PositionToApply;
			transform.localRotation = this.m_RotationToApply;
			return default(AsyncOperationHandle);
		}

		private Vector3 m_PositionToApply;

		private Quaternion m_RotationToApply;

		private Vector3 m_ScaleToApply;

		private Dictionary<string, Action<float>> m_PropertyHandlers;
	}
}
