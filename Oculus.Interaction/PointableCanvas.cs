using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class PointableCanvas : PointableElement, IPointableCanvas, IPointableElement, IPointable
	{
		public Canvas Canvas
		{
			get
			{
				return this._canvas;
			}
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this.EndStart(ref this._started);
		}

		private void Register()
		{
			PointableCanvasModule.RegisterPointableCanvas(this);
			this._registered = true;
		}

		private void Unregister()
		{
			if (!this._registered)
			{
				return;
			}
			PointableCanvasModule.UnregisterPointableCanvas(this);
			this._registered = false;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this._started)
			{
				this.Register();
			}
		}

		protected override void OnDisable()
		{
			if (this._started)
			{
				this.Unregister();
			}
			base.OnDisable();
		}

		public void InjectAllPointableCanvas(Canvas canvas)
		{
			this.InjectCanvas(canvas);
		}

		public void InjectCanvas(Canvas canvas)
		{
			this._canvas = canvas;
		}

		[Tooltip("PointerEvents will be forwarded to this Unity Canvas.")]
		[SerializeField]
		private Canvas _canvas;

		private bool _registered;
	}
}
