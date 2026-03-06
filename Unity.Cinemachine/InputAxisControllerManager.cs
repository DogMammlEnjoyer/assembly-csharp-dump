using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unity.Cinemachine
{
	[Serializable]
	internal class InputAxisControllerManager<T> where T : IInputAxisReader, new()
	{
		public void Validate()
		{
			for (int i = 0; i < this.Controllers.Count; i++)
			{
				if (this.Controllers[i] != null)
				{
					this.Controllers[i].Driver.Validate();
				}
			}
		}

		public void OnDisable()
		{
			for (int i = 0; i < this.m_AxisResetters.Count; i++)
			{
				if (this.m_AxisResetters[i] as Object != null)
				{
					this.m_AxisResetters[i].UnregisterResetHandler(new Action(this.OnResetInput));
				}
			}
			this.m_Axes.Clear();
			this.m_AxisOwners.Clear();
			this.m_AxisResetters.Clear();
		}

		public void Reset()
		{
			this.OnDisable();
			this.Controllers.Clear();
		}

		private void OnResetInput()
		{
			for (int i = 0; i < this.Controllers.Count; i++)
			{
				this.Controllers[i].Driver.Reset(this.m_Axes[i].DrivenAxis());
			}
		}

		public void CreateControllers(GameObject root, bool scanRecursively, bool enabled, InputAxisControllerManager<T>.DefaultInitializer defaultInitializer)
		{
			this.OnDisable();
			if (scanRecursively)
			{
				root.GetComponentsInChildren<IInputAxisOwner>(this.m_AxisOwners);
			}
			else
			{
				root.GetComponents<IInputAxisOwner>(this.m_AxisOwners);
			}
			for (int i = this.Controllers.Count - 1; i >= 0; i--)
			{
				if (!this.m_AxisOwners.Contains(this.Controllers[i].Owner as IInputAxisOwner))
				{
					this.Controllers.RemoveAt(i);
				}
			}
			List<InputAxisControllerBase<T>.Controller> list = new List<InputAxisControllerBase<T>.Controller>();
			for (int j = 0; j < this.m_AxisOwners.Count; j++)
			{
				IInputAxisOwner inputAxisOwner = this.m_AxisOwners[j];
				int count = this.m_Axes.Count;
				inputAxisOwner.GetInputAxes(this.m_Axes);
				for (int k = count; k < this.m_Axes.Count; k++)
				{
					int num = InputAxisControllerManager<T>.<CreateControllers>g__GetControllerIndex|9_0(this.Controllers, inputAxisOwner, this.m_Axes[k].Name);
					if (num < 0)
					{
						InputAxisControllerBase<T>.Controller controller = new InputAxisControllerBase<T>.Controller
						{
							Enabled = true,
							Name = this.m_Axes[k].Name,
							Owner = (inputAxisOwner as Object),
							Input = Activator.CreateInstance<T>()
						};
						if (defaultInitializer != null)
						{
							IInputAxisOwner.AxisDescriptor axisDescriptor = this.m_Axes[k];
							defaultInitializer(axisDescriptor, controller);
						}
						list.Add(controller);
					}
					else
					{
						list.Add(this.Controllers[num]);
						this.Controllers.RemoveAt(num);
					}
				}
			}
			this.Controllers = list;
			if (enabled)
			{
				this.RegisterResetHandlers(root, scanRecursively);
			}
		}

		private void RegisterResetHandlers(GameObject root, bool scanRecursively)
		{
			this.m_AxisResetters.Clear();
			if (scanRecursively)
			{
				root.GetComponentsInChildren<IInputAxisResetSource>(this.m_AxisResetters);
			}
			else
			{
				root.GetComponents<IInputAxisResetSource>(this.m_AxisResetters);
			}
			for (int i = 0; i < this.m_AxisResetters.Count; i++)
			{
				this.m_AxisResetters[i].UnregisterResetHandler(new Action(this.OnResetInput));
				this.m_AxisResetters[i].RegisterResetHandler(new Action(this.OnResetInput));
			}
		}

		public void UpdateControllers(Object context, float deltaTime)
		{
			for (int i = 0; i < this.Controllers.Count; i++)
			{
				InputAxisControllerBase<T>.Controller controller = this.Controllers[i];
				if (controller.Enabled && controller.Input != null)
				{
					IInputAxisOwner.AxisDescriptor.Hints hint = (i < this.m_Axes.Count) ? this.m_Axes[i].Hint : IInputAxisOwner.AxisDescriptor.Hints.Default;
					if (controller.Input != null)
					{
						controller.InputValue = controller.Input.GetValue(context, hint);
					}
					controller.Driver.ProcessInput(this.m_Axes[i].DrivenAxis(), controller.InputValue, deltaTime);
				}
			}
		}

		[CompilerGenerated]
		internal static int <CreateControllers>g__GetControllerIndex|9_0(List<InputAxisControllerBase<T>.Controller> list, IInputAxisOwner owner, string axisName)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Owner as IInputAxisOwner == owner && list[i].Name == axisName)
				{
					return i;
				}
			}
			return -1;
		}

		[NonReorderable]
		public List<InputAxisControllerBase<T>.Controller> Controllers = new List<InputAxisControllerBase<T>.Controller>();

		private readonly List<IInputAxisOwner.AxisDescriptor> m_Axes = new List<IInputAxisOwner.AxisDescriptor>();

		private readonly List<IInputAxisOwner> m_AxisOwners = new List<IInputAxisOwner>();

		private readonly List<IInputAxisResetSource> m_AxisResetters = new List<IInputAxisResetSource>();

		public delegate void DefaultInitializer(in IInputAxisOwner.AxisDescriptor axis, InputAxisControllerBase<T>.Controller controller);
	}
}
