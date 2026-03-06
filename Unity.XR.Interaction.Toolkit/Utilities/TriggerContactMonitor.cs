using System;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
	internal class TriggerContactMonitor
	{
		public event Action<IXRInteractable> contactAdded;

		public event Action<IXRInteractable> contactRemoved;

		public XRInteractionManager interactionManager { get; set; }

		public void AddCollider(Collider collider)
		{
			if (this.interactionManager == null)
			{
				return;
			}
			IXRInteractable ixrinteractable;
			if (!this.interactionManager.TryGetInteractableForCollider(collider, out ixrinteractable))
			{
				this.m_EnteredUnassociatedColliders.Add(collider);
				return;
			}
			this.m_EnteredColliders[collider] = ixrinteractable;
			if (this.m_UnorderedInteractables.Add(ixrinteractable))
			{
				Action<IXRInteractable> action = this.contactAdded;
				if (action == null)
				{
					return;
				}
				action(ixrinteractable);
			}
		}

		public void RemoveCollider(Collider collider)
		{
			if (this.m_EnteredUnassociatedColliders.Remove(collider))
			{
				return;
			}
			IXRInteractable ixrinteractable;
			if (this.m_EnteredColliders.TryGetValue(collider, out ixrinteractable))
			{
				this.m_EnteredColliders.Remove(collider);
				if (ixrinteractable == null)
				{
					return;
				}
				foreach (KeyValuePair<Collider, IXRInteractable> keyValuePair in this.m_EnteredColliders)
				{
					if (keyValuePair.Value == ixrinteractable && keyValuePair.Key != null)
					{
						return;
					}
				}
				if (this.m_UnorderedInteractables.Remove(ixrinteractable))
				{
					Action<IXRInteractable> action = this.contactRemoved;
					if (action == null)
					{
						return;
					}
					action(ixrinteractable);
				}
			}
		}

		public void ResolveUnassociatedColliders()
		{
			this.m_EnteredUnassociatedColliders.RemoveWhere(new Predicate<Collider>(TriggerContactMonitor.IsDestroyed));
			if (this.m_EnteredUnassociatedColliders.Count == 0 || this.interactionManager == null)
			{
				return;
			}
			TriggerContactMonitor.s_ScratchColliders.Clear();
			foreach (Collider collider in this.m_EnteredUnassociatedColliders)
			{
				IXRInteractable ixrinteractable;
				if (this.interactionManager.TryGetInteractableForCollider(collider, out ixrinteractable))
				{
					TriggerContactMonitor.s_ScratchColliders.Add(collider);
					this.m_EnteredColliders[collider] = ixrinteractable;
					if (this.m_UnorderedInteractables.Add(ixrinteractable))
					{
						Action<IXRInteractable> action = this.contactAdded;
						if (action != null)
						{
							action(ixrinteractable);
						}
					}
				}
			}
			TriggerContactMonitor.s_ScratchColliders.ForEach(new Action<Collider>(this.RemoveFromUnassociatedColliders));
			TriggerContactMonitor.s_ScratchColliders.Clear();
		}

		private void RemoveFromUnassociatedColliders(Collider col)
		{
			this.m_EnteredUnassociatedColliders.Remove(col);
		}

		public void ResolveUnassociatedColliders(IXRInteractable interactable)
		{
			this.m_EnteredUnassociatedColliders.RemoveWhere(new Predicate<Collider>(TriggerContactMonitor.IsDestroyed));
			if (this.m_EnteredUnassociatedColliders.Count == 0 || this.interactionManager == null)
			{
				return;
			}
			int i = 0;
			int count = interactable.colliders.Count;
			while (i < count)
			{
				Collider collider = interactable.colliders[i];
				IXRInteractable ixrinteractable;
				if (!(collider == null) && this.m_EnteredUnassociatedColliders.Contains(collider) && this.interactionManager.TryGetInteractableForCollider(collider, out ixrinteractable) && ixrinteractable == interactable)
				{
					this.m_EnteredUnassociatedColliders.Remove(collider);
					this.m_EnteredColliders[collider] = interactable;
					if (this.m_UnorderedInteractables.Add(interactable))
					{
						Action<IXRInteractable> action = this.contactAdded;
						if (action != null)
						{
							action(interactable);
						}
					}
				}
				i++;
			}
		}

		public void UpdateStayedColliders(HashSet<Collider> stayedColliders)
		{
			TriggerContactMonitor.s_ExitedColliders.Clear();
			if (this.m_EnteredColliders.Count > 0)
			{
				foreach (Collider item in this.m_EnteredColliders.Keys)
				{
					if (!stayedColliders.Contains(item))
					{
						TriggerContactMonitor.s_ExitedColliders.Add(item);
					}
					else
					{
						stayedColliders.Remove(item);
					}
				}
			}
			if (stayedColliders.Count > 0)
			{
				foreach (Collider collider in stayedColliders)
				{
					this.AddCollider(collider);
				}
			}
			if (TriggerContactMonitor.s_ExitedColliders.Count > 0)
			{
				TriggerContactMonitor.s_ExitedColliders.ForEach(new Action<Collider>(this.RemoveCollider));
				TriggerContactMonitor.s_ExitedColliders.Clear();
			}
		}

		public bool IsContacting(IXRInteractable interactable)
		{
			return this.m_UnorderedInteractables.Contains(interactable);
		}

		private static bool IsDestroyed(Collider col)
		{
			return col == null;
		}

		private readonly Dictionary<Collider, IXRInteractable> m_EnteredColliders = new Dictionary<Collider, IXRInteractable>();

		private readonly HashSet<IXRInteractable> m_UnorderedInteractables = new HashSet<IXRInteractable>();

		private readonly HashSet<Collider> m_EnteredUnassociatedColliders = new HashSet<Collider>();

		private static readonly List<Collider> s_ScratchColliders = new List<Collider>();

		private static readonly List<Collider> s_ExitedColliders = new List<Collider>();
	}
}
