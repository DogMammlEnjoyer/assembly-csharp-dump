using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Meta.WitAi
{
	public static class CoroutineUtility
	{
		public static CoroutineUtility.CoroutinePerformer StartCoroutine(IEnumerator asyncMethod, bool useUpdate = false)
		{
			CoroutineUtility.CoroutinePerformer performer = CoroutineUtility.GetPerformer();
			performer.CoroutineBegin(asyncMethod, useUpdate);
			return performer;
		}

		private static CoroutineUtility.CoroutinePerformer GetPerformer()
		{
			CoroutineUtility.CoroutinePerformer coroutinePerformer = new GameObject("Coroutine").AddComponent<CoroutineUtility.CoroutinePerformer>();
			coroutinePerformer.gameObject.hideFlags = HideFlags.HideAndDontSave;
			return coroutinePerformer;
		}

		public class CoroutinePerformer : MonoBehaviour
		{
			public bool IsRunning { get; private set; }

			private void Awake()
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}

			public void CoroutineBegin(IEnumerator asyncMethod, bool useUpdate)
			{
				if (this.IsRunning)
				{
					return;
				}
				this.IsRunning = true;
				if (Application.isBatchMode)
				{
					useUpdate = true;
				}
				this._useUpdate = useUpdate;
				this._method = asyncMethod;
				if (this._useUpdate)
				{
					this.CoroutineIterateUpdate();
					return;
				}
				this._coroutine = base.StartCoroutine(this.CoroutineIterateEnumerator());
			}

			private IEnumerator CoroutineIterateEnumerator()
			{
				yield return this._method;
				this.CoroutineComplete();
				yield break;
			}

			private void Update()
			{
				if (this._useUpdate)
				{
					this.CoroutineIterateUpdate();
				}
			}

			private void CoroutineIterateUpdate()
			{
				if (this == null || this._method == null)
				{
					this.CoroutineCancel();
					return;
				}
				if (!this.MoveNext(this._method))
				{
					this.CoroutineComplete();
				}
			}

			private bool MoveNext(IEnumerator method)
			{
				object obj = method.Current;
				return (obj != null && obj.GetType().GetInterfaces().Contains(typeof(IEnumerator)) && this.MoveNext(obj as IEnumerator)) || method.MoveNext();
			}

			private void OnDestroy()
			{
				this.CoroutineUnload();
			}

			public void CoroutineCancel()
			{
				this.CoroutineComplete();
			}

			private void CoroutineComplete()
			{
				if (!this.IsRunning)
				{
					return;
				}
				this.CoroutineUnload();
				if (this != null && base.gameObject != null)
				{
					base.gameObject.DestroySafely();
				}
			}

			private void CoroutineUnload()
			{
				this.IsRunning = false;
				if (this._method != null)
				{
					this._method = null;
				}
				if (this._coroutine != null)
				{
					base.StopCoroutine(this._coroutine);
					this._coroutine = null;
				}
			}

			private bool _useUpdate;

			private IEnumerator _method;

			private Coroutine _coroutine;
		}
	}
}
