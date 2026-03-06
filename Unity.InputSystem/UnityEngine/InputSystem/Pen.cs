using System;
using System.ComponentModel;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(stateType = typeof(PenState), isGenericTypeOfDevice = true)]
	public class Pen : Pointer
	{
		public ButtonControl tip { get; protected set; }

		public ButtonControl eraser { get; protected set; }

		public ButtonControl firstBarrelButton { get; protected set; }

		public ButtonControl secondBarrelButton { get; protected set; }

		public ButtonControl thirdBarrelButton { get; protected set; }

		public ButtonControl fourthBarrelButton { get; protected set; }

		public ButtonControl inRange { get; protected set; }

		public Vector2Control tilt { get; protected set; }

		public AxisControl twist { get; protected set; }

		public new static Pen current { get; internal set; }

		public ButtonControl this[PenButton button]
		{
			get
			{
				switch (button)
				{
				case PenButton.Tip:
					return this.tip;
				case PenButton.Eraser:
					return this.eraser;
				case PenButton.BarrelFirst:
					return this.firstBarrelButton;
				case PenButton.BarrelSecond:
					return this.secondBarrelButton;
				case PenButton.InRange:
					return this.inRange;
				case PenButton.BarrelThird:
					return this.thirdBarrelButton;
				case PenButton.BarrelFourth:
					return this.fourthBarrelButton;
				default:
					throw new InvalidEnumArgumentException("button", (int)button, typeof(PenButton));
				}
			}
		}

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			Pen.current = this;
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (Pen.current == this)
			{
				Pen.current = null;
			}
		}

		protected override void FinishSetup()
		{
			this.tip = base.GetChildControl<ButtonControl>("tip");
			this.eraser = base.GetChildControl<ButtonControl>("eraser");
			this.firstBarrelButton = base.GetChildControl<ButtonControl>("barrel1");
			this.secondBarrelButton = base.GetChildControl<ButtonControl>("barrel2");
			this.thirdBarrelButton = base.GetChildControl<ButtonControl>("barrel3");
			this.fourthBarrelButton = base.GetChildControl<ButtonControl>("barrel4");
			this.inRange = base.GetChildControl<ButtonControl>("inRange");
			this.tilt = base.GetChildControl<Vector2Control>("tilt");
			this.twist = base.GetChildControl<AxisControl>("twist");
			base.displayIndex = base.GetChildControl<IntegerControl>("displayIndex");
			base.FinishSetup();
		}
	}
}
