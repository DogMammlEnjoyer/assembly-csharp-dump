using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ParticleSystemJobs;

namespace Cysharp.Threading.Tasks.Triggers
{
	public sealed class AsyncTriggerHandler<T> : IAsyncOneShotTrigger, IUniTaskSource<!0>, IUniTaskSource, ITriggerHandler<!0>, IDisposable, IAsyncFixedUpdateHandler, IAsyncLateUpdateHandler, IAsyncOnAnimatorIKHandler, IAsyncOnAnimatorMoveHandler, IAsyncOnApplicationFocusHandler, IAsyncOnApplicationPauseHandler, IAsyncOnApplicationQuitHandler, IAsyncOnAudioFilterReadHandler, IAsyncOnBecameInvisibleHandler, IAsyncOnBecameVisibleHandler, IAsyncOnBeforeTransformParentChangedHandler, IAsyncOnCanvasGroupChangedHandler, IAsyncOnCollisionEnterHandler, IAsyncOnCollisionEnter2DHandler, IAsyncOnCollisionExitHandler, IAsyncOnCollisionExit2DHandler, IAsyncOnCollisionStayHandler, IAsyncOnCollisionStay2DHandler, IAsyncOnControllerColliderHitHandler, IAsyncOnDisableHandler, IAsyncOnDrawGizmosHandler, IAsyncOnDrawGizmosSelectedHandler, IAsyncOnEnableHandler, IAsyncOnGUIHandler, IAsyncOnJointBreakHandler, IAsyncOnJointBreak2DHandler, IAsyncOnMouseDownHandler, IAsyncOnMouseDragHandler, IAsyncOnMouseEnterHandler, IAsyncOnMouseExitHandler, IAsyncOnMouseOverHandler, IAsyncOnMouseUpHandler, IAsyncOnMouseUpAsButtonHandler, IAsyncOnParticleCollisionHandler, IAsyncOnParticleSystemStoppedHandler, IAsyncOnParticleTriggerHandler, IAsyncOnParticleUpdateJobScheduledHandler, IAsyncOnPostRenderHandler, IAsyncOnPreCullHandler, IAsyncOnPreRenderHandler, IAsyncOnRectTransformDimensionsChangeHandler, IAsyncOnRectTransformRemovedHandler, IAsyncOnRenderImageHandler, IAsyncOnRenderObjectHandler, IAsyncOnServerInitializedHandler, IAsyncOnTransformChildrenChangedHandler, IAsyncOnTransformParentChangedHandler, IAsyncOnTriggerEnterHandler, IAsyncOnTriggerEnter2DHandler, IAsyncOnTriggerExitHandler, IAsyncOnTriggerExit2DHandler, IAsyncOnTriggerStayHandler, IAsyncOnTriggerStay2DHandler, IAsyncOnValidateHandler, IAsyncOnWillRenderObjectHandler, IAsyncResetHandler, IAsyncUpdateHandler, IAsyncOnBeginDragHandler, IAsyncOnCancelHandler, IAsyncOnDeselectHandler, IAsyncOnDragHandler, IAsyncOnDropHandler, IAsyncOnEndDragHandler, IAsyncOnInitializePotentialDragHandler, IAsyncOnMoveHandler, IAsyncOnPointerClickHandler, IAsyncOnPointerDownHandler, IAsyncOnPointerEnterHandler, IAsyncOnPointerExitHandler, IAsyncOnPointerUpHandler, IAsyncOnScrollHandler, IAsyncOnSelectHandler, IAsyncOnSubmitHandler, IAsyncOnUpdateSelectedHandler
	{
		UniTask IAsyncOneShotTrigger.OneShotAsync()
		{
			this.core.Reset();
			return new UniTask(this, this.core.Version);
		}

		internal CancellationToken CancellationToken
		{
			get
			{
				return this.cancellationToken;
			}
		}

		ITriggerHandler<T> ITriggerHandler<!0>.Prev { get; set; }

		ITriggerHandler<T> ITriggerHandler<!0>.Next { get; set; }

		internal AsyncTriggerHandler(AsyncTriggerBase<T> trigger, bool callOnce)
		{
			if (this.cancellationToken.IsCancellationRequested)
			{
				this.isDisposed = true;
				return;
			}
			this.trigger = trigger;
			this.cancellationToken = default(CancellationToken);
			this.registration = default(CancellationTokenRegistration);
			this.callOnce = callOnce;
			trigger.AddHandler(this);
		}

		internal AsyncTriggerHandler(AsyncTriggerBase<T> trigger, CancellationToken cancellationToken, bool callOnce)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				this.isDisposed = true;
				return;
			}
			this.trigger = trigger;
			this.cancellationToken = cancellationToken;
			this.callOnce = callOnce;
			trigger.AddHandler(this);
			if (cancellationToken.CanBeCanceled)
			{
				this.registration = cancellationToken.RegisterWithoutCaptureExecutionContext(AsyncTriggerHandler<T>.cancellationCallback, this);
			}
		}

		private static void CancellationCallback(object state)
		{
			AsyncTriggerHandler<T> asyncTriggerHandler = (AsyncTriggerHandler<T>)state;
			asyncTriggerHandler.Dispose();
			asyncTriggerHandler.core.TrySetCanceled(asyncTriggerHandler.cancellationToken);
		}

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.registration.Dispose();
				this.trigger.RemoveHandler(this);
			}
		}

		T IUniTaskSource<!0>.GetResult(short token)
		{
			T result;
			try
			{
				result = this.core.GetResult(token);
			}
			finally
			{
				if (this.callOnce)
				{
					this.Dispose();
				}
			}
			return result;
		}

		void ITriggerHandler<!0>.OnNext(T value)
		{
			this.core.TrySetResult(value);
		}

		void ITriggerHandler<!0>.OnCanceled(CancellationToken cancellationToken)
		{
			this.core.TrySetCanceled(cancellationToken);
		}

		void ITriggerHandler<!0>.OnCompleted()
		{
			this.core.TrySetCanceled(CancellationToken.None);
		}

		void ITriggerHandler<!0>.OnError(Exception ex)
		{
			this.core.TrySetException(ex);
		}

		void IUniTaskSource.GetResult(short token)
		{
			((IUniTaskSource<!0>)this).GetResult(token);
		}

		UniTaskStatus IUniTaskSource.GetStatus(short token)
		{
			return this.core.GetStatus(token);
		}

		UniTaskStatus IUniTaskSource.UnsafeGetStatus()
		{
			return this.core.UnsafeGetStatus();
		}

		void IUniTaskSource.OnCompleted(Action<object> continuation, object state, short token)
		{
			this.core.OnCompleted(continuation, state, token);
		}

		UniTask IAsyncFixedUpdateHandler.FixedUpdateAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncLateUpdateHandler.LateUpdateAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask<int> IAsyncOnAnimatorIKHandler.OnAnimatorIKAsync()
		{
			this.core.Reset();
			return new UniTask<int>((IUniTaskSource<int>)this, this.core.Version);
		}

		UniTask IAsyncOnAnimatorMoveHandler.OnAnimatorMoveAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask<bool> IAsyncOnApplicationFocusHandler.OnApplicationFocusAsync()
		{
			this.core.Reset();
			return new UniTask<bool>((IUniTaskSource<bool>)this, this.core.Version);
		}

		UniTask<bool> IAsyncOnApplicationPauseHandler.OnApplicationPauseAsync()
		{
			this.core.Reset();
			return new UniTask<bool>((IUniTaskSource<bool>)this, this.core.Version);
		}

		UniTask IAsyncOnApplicationQuitHandler.OnApplicationQuitAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		[return: TupleElementNames(new string[]
		{
			"data",
			"channels"
		})]
		UniTask<ValueTuple<float[], int>> IAsyncOnAudioFilterReadHandler.OnAudioFilterReadAsync()
		{
			this.core.Reset();
			return new UniTask<ValueTuple<float[], int>>((IUniTaskSource<ValueTuple<float[], int>>)this, this.core.Version);
		}

		UniTask IAsyncOnBecameInvisibleHandler.OnBecameInvisibleAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnBecameVisibleHandler.OnBecameVisibleAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnBeforeTransformParentChangedHandler.OnBeforeTransformParentChangedAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnCanvasGroupChangedHandler.OnCanvasGroupChangedAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask<Collision> IAsyncOnCollisionEnterHandler.OnCollisionEnterAsync()
		{
			this.core.Reset();
			return new UniTask<Collision>((IUniTaskSource<Collision>)this, this.core.Version);
		}

		UniTask<Collision2D> IAsyncOnCollisionEnter2DHandler.OnCollisionEnter2DAsync()
		{
			this.core.Reset();
			return new UniTask<Collision2D>((IUniTaskSource<Collision2D>)this, this.core.Version);
		}

		UniTask<Collision> IAsyncOnCollisionExitHandler.OnCollisionExitAsync()
		{
			this.core.Reset();
			return new UniTask<Collision>((IUniTaskSource<Collision>)this, this.core.Version);
		}

		UniTask<Collision2D> IAsyncOnCollisionExit2DHandler.OnCollisionExit2DAsync()
		{
			this.core.Reset();
			return new UniTask<Collision2D>((IUniTaskSource<Collision2D>)this, this.core.Version);
		}

		UniTask<Collision> IAsyncOnCollisionStayHandler.OnCollisionStayAsync()
		{
			this.core.Reset();
			return new UniTask<Collision>((IUniTaskSource<Collision>)this, this.core.Version);
		}

		UniTask<Collision2D> IAsyncOnCollisionStay2DHandler.OnCollisionStay2DAsync()
		{
			this.core.Reset();
			return new UniTask<Collision2D>((IUniTaskSource<Collision2D>)this, this.core.Version);
		}

		UniTask<ControllerColliderHit> IAsyncOnControllerColliderHitHandler.OnControllerColliderHitAsync()
		{
			this.core.Reset();
			return new UniTask<ControllerColliderHit>((IUniTaskSource<ControllerColliderHit>)this, this.core.Version);
		}

		UniTask IAsyncOnDisableHandler.OnDisableAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnDrawGizmosHandler.OnDrawGizmosAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnDrawGizmosSelectedHandler.OnDrawGizmosSelectedAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnEnableHandler.OnEnableAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnGUIHandler.OnGUIAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask<float> IAsyncOnJointBreakHandler.OnJointBreakAsync()
		{
			this.core.Reset();
			return new UniTask<float>((IUniTaskSource<float>)this, this.core.Version);
		}

		UniTask<Joint2D> IAsyncOnJointBreak2DHandler.OnJointBreak2DAsync()
		{
			this.core.Reset();
			return new UniTask<Joint2D>((IUniTaskSource<Joint2D>)this, this.core.Version);
		}

		UniTask IAsyncOnMouseDownHandler.OnMouseDownAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnMouseDragHandler.OnMouseDragAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnMouseEnterHandler.OnMouseEnterAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnMouseExitHandler.OnMouseExitAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnMouseOverHandler.OnMouseOverAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnMouseUpHandler.OnMouseUpAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnMouseUpAsButtonHandler.OnMouseUpAsButtonAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask<GameObject> IAsyncOnParticleCollisionHandler.OnParticleCollisionAsync()
		{
			this.core.Reset();
			return new UniTask<GameObject>((IUniTaskSource<GameObject>)this, this.core.Version);
		}

		UniTask IAsyncOnParticleSystemStoppedHandler.OnParticleSystemStoppedAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnParticleTriggerHandler.OnParticleTriggerAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask<ParticleSystemJobData> IAsyncOnParticleUpdateJobScheduledHandler.OnParticleUpdateJobScheduledAsync()
		{
			this.core.Reset();
			return new UniTask<ParticleSystemJobData>((IUniTaskSource<ParticleSystemJobData>)this, this.core.Version);
		}

		UniTask IAsyncOnPostRenderHandler.OnPostRenderAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnPreCullHandler.OnPreCullAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnPreRenderHandler.OnPreRenderAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnRectTransformDimensionsChangeHandler.OnRectTransformDimensionsChangeAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnRectTransformRemovedHandler.OnRectTransformRemovedAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		[return: TupleElementNames(new string[]
		{
			"source",
			"destination"
		})]
		UniTask<ValueTuple<RenderTexture, RenderTexture>> IAsyncOnRenderImageHandler.OnRenderImageAsync()
		{
			this.core.Reset();
			return new UniTask<ValueTuple<RenderTexture, RenderTexture>>((IUniTaskSource<ValueTuple<RenderTexture, RenderTexture>>)this, this.core.Version);
		}

		UniTask IAsyncOnRenderObjectHandler.OnRenderObjectAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnServerInitializedHandler.OnServerInitializedAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnTransformChildrenChangedHandler.OnTransformChildrenChangedAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnTransformParentChangedHandler.OnTransformParentChangedAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask<Collider> IAsyncOnTriggerEnterHandler.OnTriggerEnterAsync()
		{
			this.core.Reset();
			return new UniTask<Collider>((IUniTaskSource<Collider>)this, this.core.Version);
		}

		UniTask<Collider2D> IAsyncOnTriggerEnter2DHandler.OnTriggerEnter2DAsync()
		{
			this.core.Reset();
			return new UniTask<Collider2D>((IUniTaskSource<Collider2D>)this, this.core.Version);
		}

		UniTask<Collider> IAsyncOnTriggerExitHandler.OnTriggerExitAsync()
		{
			this.core.Reset();
			return new UniTask<Collider>((IUniTaskSource<Collider>)this, this.core.Version);
		}

		UniTask<Collider2D> IAsyncOnTriggerExit2DHandler.OnTriggerExit2DAsync()
		{
			this.core.Reset();
			return new UniTask<Collider2D>((IUniTaskSource<Collider2D>)this, this.core.Version);
		}

		UniTask<Collider> IAsyncOnTriggerStayHandler.OnTriggerStayAsync()
		{
			this.core.Reset();
			return new UniTask<Collider>((IUniTaskSource<Collider>)this, this.core.Version);
		}

		UniTask<Collider2D> IAsyncOnTriggerStay2DHandler.OnTriggerStay2DAsync()
		{
			this.core.Reset();
			return new UniTask<Collider2D>((IUniTaskSource<Collider2D>)this, this.core.Version);
		}

		UniTask IAsyncOnValidateHandler.OnValidateAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncOnWillRenderObjectHandler.OnWillRenderObjectAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncResetHandler.ResetAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask IAsyncUpdateHandler.UpdateAsync()
		{
			this.core.Reset();
			return new UniTask((IUniTaskSource)this, this.core.Version);
		}

		UniTask<PointerEventData> IAsyncOnBeginDragHandler.OnBeginDragAsync()
		{
			this.core.Reset();
			return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)this, this.core.Version);
		}

		UniTask<BaseEventData> IAsyncOnCancelHandler.OnCancelAsync()
		{
			this.core.Reset();
			return new UniTask<BaseEventData>((IUniTaskSource<BaseEventData>)this, this.core.Version);
		}

		UniTask<BaseEventData> IAsyncOnDeselectHandler.OnDeselectAsync()
		{
			this.core.Reset();
			return new UniTask<BaseEventData>((IUniTaskSource<BaseEventData>)this, this.core.Version);
		}

		UniTask<PointerEventData> IAsyncOnDragHandler.OnDragAsync()
		{
			this.core.Reset();
			return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)this, this.core.Version);
		}

		UniTask<PointerEventData> IAsyncOnDropHandler.OnDropAsync()
		{
			this.core.Reset();
			return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)this, this.core.Version);
		}

		UniTask<PointerEventData> IAsyncOnEndDragHandler.OnEndDragAsync()
		{
			this.core.Reset();
			return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)this, this.core.Version);
		}

		UniTask<PointerEventData> IAsyncOnInitializePotentialDragHandler.OnInitializePotentialDragAsync()
		{
			this.core.Reset();
			return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)this, this.core.Version);
		}

		UniTask<AxisEventData> IAsyncOnMoveHandler.OnMoveAsync()
		{
			this.core.Reset();
			return new UniTask<AxisEventData>((IUniTaskSource<AxisEventData>)this, this.core.Version);
		}

		UniTask<PointerEventData> IAsyncOnPointerClickHandler.OnPointerClickAsync()
		{
			this.core.Reset();
			return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)this, this.core.Version);
		}

		UniTask<PointerEventData> IAsyncOnPointerDownHandler.OnPointerDownAsync()
		{
			this.core.Reset();
			return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)this, this.core.Version);
		}

		UniTask<PointerEventData> IAsyncOnPointerEnterHandler.OnPointerEnterAsync()
		{
			this.core.Reset();
			return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)this, this.core.Version);
		}

		UniTask<PointerEventData> IAsyncOnPointerExitHandler.OnPointerExitAsync()
		{
			this.core.Reset();
			return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)this, this.core.Version);
		}

		UniTask<PointerEventData> IAsyncOnPointerUpHandler.OnPointerUpAsync()
		{
			this.core.Reset();
			return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)this, this.core.Version);
		}

		UniTask<PointerEventData> IAsyncOnScrollHandler.OnScrollAsync()
		{
			this.core.Reset();
			return new UniTask<PointerEventData>((IUniTaskSource<PointerEventData>)this, this.core.Version);
		}

		UniTask<BaseEventData> IAsyncOnSelectHandler.OnSelectAsync()
		{
			this.core.Reset();
			return new UniTask<BaseEventData>((IUniTaskSource<BaseEventData>)this, this.core.Version);
		}

		UniTask<BaseEventData> IAsyncOnSubmitHandler.OnSubmitAsync()
		{
			this.core.Reset();
			return new UniTask<BaseEventData>((IUniTaskSource<BaseEventData>)this, this.core.Version);
		}

		UniTask<BaseEventData> IAsyncOnUpdateSelectedHandler.OnUpdateSelectedAsync()
		{
			this.core.Reset();
			return new UniTask<BaseEventData>((IUniTaskSource<BaseEventData>)this, this.core.Version);
		}

		private static Action<object> cancellationCallback = new Action<object>(AsyncTriggerHandler<T>.CancellationCallback);

		private readonly AsyncTriggerBase<T> trigger;

		private CancellationToken cancellationToken;

		private CancellationTokenRegistration registration;

		private bool isDisposed;

		private bool callOnce;

		private UniTaskCompletionSourceCore<T> core;
	}
}
