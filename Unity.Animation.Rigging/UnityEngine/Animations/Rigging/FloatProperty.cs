using System;

namespace UnityEngine.Animations.Rigging
{
	public struct FloatProperty : IAnimatableProperty<float>
	{
		public static FloatProperty Bind(Animator animator, Component component, string name)
		{
			return new FloatProperty
			{
				value = animator.BindStreamProperty(component.transform, component.GetType(), name)
			};
		}

		public static FloatProperty BindCustom(Animator animator, string property)
		{
			return new FloatProperty
			{
				value = animator.BindCustomStreamProperty(property, CustomStreamPropertyType.Float)
			};
		}

		public float Get(AnimationStream stream)
		{
			return this.value.GetFloat(stream);
		}

		public void Set(AnimationStream stream, float v)
		{
			this.value.SetFloat(stream, v);
		}

		public PropertyStreamHandle value;
	}
}
