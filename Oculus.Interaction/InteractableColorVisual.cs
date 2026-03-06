using System;
using System.Collections;
using UnityEngine;

namespace Oculus.Interaction
{
	public class InteractableColorVisual : MonoBehaviour
	{
		private IInteractableView InteractableView { get; set; }

		protected virtual void Awake()
		{
			this.InteractableView = (this._interactableView as IInteractableView);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._colorShaderID = Shader.PropertyToID(this._colorShaderPropertyName);
			this.UpdateVisual();
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.UpdateVisual();
				this.InteractableView.WhenStateChanged += this.UpdateVisualState;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.InteractableView.WhenStateChanged -= this.UpdateVisualState;
			}
		}

		private void UpdateVisualState(InteractableStateChangeArgs args)
		{
			this.UpdateVisual();
		}

		protected virtual void UpdateVisual()
		{
			InteractableColorVisual.ColorState colorState = this.ColorForState(this.InteractableView.State);
			if (colorState != this._target)
			{
				this._target = colorState;
				this.CancelRoutine();
				this._routine = base.StartCoroutine(this.ChangeColor(colorState));
			}
		}

		private InteractableColorVisual.ColorState ColorForState(InteractableState state)
		{
			switch (state)
			{
			case InteractableState.Normal:
				return this._normalColorState;
			case InteractableState.Hover:
				return this._hoverColorState;
			case InteractableState.Select:
				return this._selectColorState;
			case InteractableState.Disabled:
				return this._disabledColorState;
			default:
				return this._normalColorState;
			}
		}

		private IEnumerator ChangeColor(InteractableColorVisual.ColorState targetState)
		{
			Color startColor = this._currentColor;
			float timer = 0f;
			do
			{
				timer += Time.deltaTime;
				float time = Mathf.Clamp01(timer / targetState.ColorTime);
				float t = targetState.ColorCurve.Evaluate(time);
				this.SetColor(Color.Lerp(startColor, targetState.Color, t));
				yield return InteractableColorVisual._waiter;
			}
			while (timer <= targetState.ColorTime);
			yield break;
		}

		private void SetColor(Color color)
		{
			this._currentColor = color;
			this._editor.MaterialPropertyBlock.SetColor(this._colorShaderID, color);
		}

		private void CancelRoutine()
		{
			if (this._routine != null)
			{
				base.StopCoroutine(this._routine);
				this._routine = null;
			}
		}

		public void InjectAllInteractableColorVisual(IInteractableView interactableView, MaterialPropertyBlockEditor editor)
		{
			this.InjectInteractableView(interactableView);
			this.InjectMaterialPropertyBlockEditor(editor);
		}

		public void InjectInteractableView(IInteractableView interactableview)
		{
			this._interactableView = (interactableview as Object);
			this.InteractableView = interactableview;
		}

		public void InjectMaterialPropertyBlockEditor(MaterialPropertyBlockEditor editor)
		{
			this._editor = editor;
		}

		public void InjectOptionalColorShaderPropertyName(string colorShaderPropertyName)
		{
			this._colorShaderPropertyName = colorShaderPropertyName;
		}

		public void InjectOptionalNormalColorState(InteractableColorVisual.ColorState normalColorState)
		{
			this._normalColorState = normalColorState;
		}

		public void InjectOptionalHoverColorState(InteractableColorVisual.ColorState hoverColorState)
		{
			this._hoverColorState = hoverColorState;
		}

		public void InjectOptionalSelectColorState(InteractableColorVisual.ColorState selectColorState)
		{
			this._selectColorState = selectColorState;
		}

		[SerializeField]
		[Interface(typeof(IInteractableView), new Type[]
		{

		})]
		private Object _interactableView;

		[SerializeField]
		private MaterialPropertyBlockEditor _editor;

		[SerializeField]
		private string _colorShaderPropertyName = "_Color";

		[SerializeField]
		private InteractableColorVisual.ColorState _normalColorState = new InteractableColorVisual.ColorState
		{
			Color = Color.white
		};

		[SerializeField]
		private InteractableColorVisual.ColorState _hoverColorState = new InteractableColorVisual.ColorState
		{
			Color = Color.blue
		};

		[SerializeField]
		private InteractableColorVisual.ColorState _selectColorState = new InteractableColorVisual.ColorState
		{
			Color = Color.green
		};

		[SerializeField]
		private InteractableColorVisual.ColorState _disabledColorState = new InteractableColorVisual.ColorState
		{
			Color = Color.grey
		};

		private Color _currentColor;

		private InteractableColorVisual.ColorState _target;

		private int _colorShaderID;

		private Coroutine _routine;

		private static readonly YieldInstruction _waiter = new WaitForEndOfFrame();

		protected bool _started;

		[Serializable]
		public class ColorState
		{
			public Color Color = Color.white;

			public AnimationCurve ColorCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

			public float ColorTime = 0.1f;
		}
	}
}
