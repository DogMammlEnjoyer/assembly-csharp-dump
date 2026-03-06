using System;
using System.Collections.Generic;
using System.Linq;
using Modio.Mods;

namespace Modio.Users
{
	public class ModRepository : IDisposable
	{
		public bool HasGotSubscriptions { get; internal set; }

		internal event Action OnContentsChanged;

		public IEnumerable<Mod> GetCreatedMods()
		{
			return this._created;
		}

		public IEnumerable<Mod> GetSubscribed()
		{
			return this._subscribed;
		}

		public IEnumerable<Mod> GetPurchased()
		{
			return this._purchased;
		}

		public IEnumerable<Mod> GetDisabled()
		{
			return this._disabled;
		}

		internal ModRepository()
		{
			Mod.AddChangeListener(ModChangeType.IsSubscribed, new Action<Mod, ModChangeType>(this.OnModSubscriptionChange));
			Mod.AddChangeListener(ModChangeType.IsEnabled, new Action<Mod, ModChangeType>(this.OnModEnabledChange));
			Mod.AddChangeListener(ModChangeType.IsPurchased, new Action<Mod, ModChangeType>(this.OnModPurchasedChange));
			ModioClient.OnShutdown += this.Dispose;
		}

		private void OnModSubscriptionChange(Mod mod, ModChangeType changeType)
		{
			bool flag = false;
			if (mod.IsSubscribed)
			{
				flag |= this._subscribed.Add(mod);
			}
			else
			{
				flag |= this._subscribed.Remove(mod);
				flag |= this._disabled.Remove(mod);
			}
			if (flag)
			{
				Action onContentsChanged = this.OnContentsChanged;
				if (onContentsChanged == null)
				{
					return;
				}
				onContentsChanged();
			}
		}

		private void OnModEnabledChange(Mod mod, ModChangeType changeType)
		{
			bool flag = false;
			if (!mod.IsEnabled)
			{
				flag |= this._disabled.Add(mod);
			}
			else
			{
				flag |= this._disabled.Remove(mod);
			}
			if (flag)
			{
				Action onContentsChanged = this.OnContentsChanged;
				if (onContentsChanged == null)
				{
					return;
				}
				onContentsChanged();
			}
		}

		private void OnModPurchasedChange(Mod mod, ModChangeType changeType)
		{
			bool flag = false;
			if (mod.IsPurchased)
			{
				flag |= this._purchased.Add(mod);
			}
			else
			{
				flag |= this._purchased.Remove(mod);
			}
			if (flag)
			{
				Action onContentsChanged = this.OnContentsChanged;
				if (onContentsChanged == null)
				{
					return;
				}
				onContentsChanged();
			}
		}

		public bool IsSubscribed(ModId modId)
		{
			return this._subscribed.Any((Mod mod) => mod.Id == modId);
		}

		public bool IsDisabled(ModId modId)
		{
			return this._disabled.Any((Mod mod) => mod.Id == modId);
		}

		public bool IsPurchased(ModId modId)
		{
			return this._purchased.Any((Mod mod) => mod.Id == modId);
		}

		public void Dispose()
		{
			Mod.RemoveChangeListener(ModChangeType.IsSubscribed, new Action<Mod, ModChangeType>(this.OnModSubscriptionChange));
			Mod.RemoveChangeListener(ModChangeType.IsEnabled, new Action<Mod, ModChangeType>(this.OnModEnabledChange));
			Mod.RemoveChangeListener(ModChangeType.IsPurchased, new Action<Mod, ModChangeType>(this.OnModPurchasedChange));
			ModioClient.OnShutdown -= this.Dispose;
		}

		private readonly HashSet<Mod> _created = new HashSet<Mod>();

		private readonly HashSet<Mod> _subscribed = new HashSet<Mod>();

		private readonly HashSet<Mod> _purchased = new HashSet<Mod>();

		private readonly HashSet<Mod> _disabled = new HashSet<Mod>();
	}
}
