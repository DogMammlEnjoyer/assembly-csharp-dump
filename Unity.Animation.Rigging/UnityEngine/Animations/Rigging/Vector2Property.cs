using System;

namespace UnityEngine.Animations.Rigging
{
	public struct Vector2Property : IAnimatableProperty<Vector2>
	{
		public static Vector2Property Bind(Animator animator, Component component, string name)
		{
			Type type = component.GetType();
			return new Vector2Property
			{
				x = animator.BindStreamProperty(component.transform, type, name + ".x"),
				y = animator.BindStreamProperty(component.transform, type, name + ".y")
			};
		}

		public static Vector2Property BindCustom(Animator animator, string name)
		{
			return new Vector2Property
			{
				x = animator.BindCustomStreamProperty(name + ".x", CustomStreamPropertyType.Float),
				y = animator.BindCustomStreamProperty(name + ".y", CustomStreamPropertyType.Float)
			};
		}

		public Vector2 Get(AnimationStream stream)
		{
			return new Vector2(this.x.GetFloat(stream), this.y.GetFloat(stream));
		}

		public void Set(AnimationStream stream, Vector2 value)
		{
			this.x.SetFloat(stream, value.x);
			this.y.SetFloat(stream, value.y);
		}

		public PropertyStreamHandle x;

		public PropertyStreamHandle y;
	}
}
