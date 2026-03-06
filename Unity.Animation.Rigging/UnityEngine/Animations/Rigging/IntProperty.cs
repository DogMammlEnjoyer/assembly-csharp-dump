using System;

namespace UnityEngine.Animations.Rigging
{
	public struct IntProperty : IAnimatableProperty<int>
	{
		public static IntProperty Bind(Animator animator, Component component, string name)
		{
			return new IntProperty
			{
				value = animator.BindStreamProperty(component.transform, component.GetType(), name)
			};
		}

		public static IntProperty BindCustom(Animator animator, string property)
		{
			return new IntProperty
			{
				value = animator.BindCustomStreamProperty(property, CustomStreamPropertyType.Int)
			};
		}

		public int Get(AnimationStream stream)
		{
			return this.value.GetInt(stream);
		}

		public void Set(AnimationStream stream, int v)
		{
			this.value.SetInt(stream, v);
		}

		public PropertyStreamHandle value;
	}
}
