using System;
using System.Collections;
using System.Collections.Generic;

namespace Oculus.Interaction
{
	public class InteractableRegistry<TInteractor, TInteractable> where TInteractor : Interactor<TInteractor, TInteractable> where TInteractable : Interactable<TInteractor, TInteractable>
	{
		public InteractableRegistry()
		{
			InteractableRegistry<TInteractor, TInteractable>._interactables = new List<TInteractable>();
		}

		public virtual void Register(TInteractable interactable)
		{
			InteractableRegistry<TInteractor, TInteractable>._interactables.Add(interactable);
		}

		public virtual void Unregister(TInteractable interactable)
		{
			InteractableRegistry<TInteractor, TInteractable>._interactables.Remove(interactable);
		}

		protected InteractableRegistry<TInteractor, TInteractable>.InteractableSet List(TInteractor interactor, HashSet<TInteractable> onlyInclude)
		{
			return new InteractableRegistry<TInteractor, TInteractable>.InteractableSet(onlyInclude, interactor);
		}

		public virtual InteractableRegistry<TInteractor, TInteractable>.InteractableSet List(TInteractor interactor)
		{
			return new InteractableRegistry<TInteractor, TInteractable>.InteractableSet(null, interactor);
		}

		public virtual InteractableRegistry<TInteractor, TInteractable>.InteractableSet List()
		{
			return new InteractableRegistry<TInteractor, TInteractable>.InteractableSet(null, default(TInteractor));
		}

		private static List<TInteractable> _interactables;

		public struct InteractableSet : IEnumerable<TInteractable>, IEnumerable
		{
			public InteractableSet(ISet<TInteractable> onlyInclude, TInteractor testAgainst)
			{
				this._data = InteractableRegistry<TInteractor, TInteractable>._interactables;
				this._onlyInclude = onlyInclude;
				this._testAgainst = testAgainst;
			}

			public InteractableRegistry<TInteractor, TInteractable>.InteractableSet.Enumerator GetEnumerator()
			{
				return new InteractableRegistry<TInteractor, TInteractable>.InteractableSet.Enumerator(ref this);
			}

			IEnumerator<TInteractable> IEnumerable<!1>.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			private bool Include(TInteractable interactable)
			{
				if (this._onlyInclude != null && !this._onlyInclude.Contains(interactable))
				{
					return false;
				}
				if (this._testAgainst != null)
				{
					if (!this._testAgainst.CanSelect(interactable))
					{
						return false;
					}
					if (!interactable.CanBeSelectedBy(this._testAgainst))
					{
						return false;
					}
				}
				return true;
			}

			private readonly IReadOnlyList<TInteractable> _data;

			private readonly ISet<TInteractable> _onlyInclude;

			private readonly TInteractor _testAgainst;

			public struct Enumerator : IEnumerator<TInteractable>, IEnumerator, IDisposable
			{
				private IReadOnlyList<TInteractable> Data
				{
					get
					{
						return this._set._data;
					}
				}

				public Enumerator(in InteractableRegistry<TInteractor, TInteractable>.InteractableSet set)
				{
					this._set = set;
					this._position = -1;
				}

				public TInteractable Current
				{
					get
					{
						if (this.Data == null || this._position < 0)
						{
							throw new InvalidOperationException();
						}
						return this.Data[this._position];
					}
				}

				object IEnumerator.Current
				{
					get
					{
						return this.Current;
					}
				}

				public bool MoveNext()
				{
					if (this.Data == null)
					{
						return false;
					}
					do
					{
						this._position++;
					}
					while (this._position < this.Data.Count && !this._set.Include(this.Data[this._position]));
					return this._position < this.Data.Count;
				}

				public void Reset()
				{
					this._position = -1;
				}

				public void Dispose()
				{
				}

				private readonly InteractableRegistry<TInteractor, TInteractable>.InteractableSet _set;

				private int _position;
			}
		}
	}
}
