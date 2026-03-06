using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Oculus.Interaction
{
	public class PointableCanvasModule : PointerInputModule
	{
		public static event Action<PointableCanvasEventArgs> WhenSelected;

		public static event Action<PointableCanvasEventArgs> WhenUnselected;

		public static event Action<PointableCanvasEventArgs> WhenSelectableHovered;

		public static event Action<PointableCanvasEventArgs> WhenSelectableUnhovered;

		public static event Action<PointableCanvasModule.Pointer> WhenPointerStarted;

		public bool ExclusiveMode
		{
			get
			{
				return this._exclusiveMode;
			}
			set
			{
				this._exclusiveMode = value;
			}
		}

		private static PointableCanvasModule Instance
		{
			get
			{
				return PointableCanvasModule._instance;
			}
		}

		public static void RegisterPointableCanvas(IPointableCanvas pointerCanvas)
		{
			PointableCanvasModule.Instance.AddPointerCanvas(pointerCanvas);
		}

		public static void UnregisterPointableCanvas(IPointableCanvas pointerCanvas)
		{
			PointableCanvasModule instance = PointableCanvasModule.Instance;
			if (instance == null)
			{
				return;
			}
			instance.RemovePointerCanvas(pointerCanvas);
		}

		private void AddPointerCanvas(IPointableCanvas pointerCanvas)
		{
			Action<PointerEvent> value = delegate(PointerEvent args)
			{
				this.HandlePointerEvent(pointerCanvas.Canvas, args);
			};
			this._pointerCanvasActionMap.Add(pointerCanvas, value);
			pointerCanvas.WhenPointerEventRaised += value;
		}

		private void RemovePointerCanvas(IPointableCanvas pointerCanvas)
		{
			Action<PointerEvent> value = this._pointerCanvasActionMap[pointerCanvas];
			this._pointerCanvasActionMap.Remove(pointerCanvas);
			pointerCanvas.WhenPointerEventRaised -= value;
			foreach (int key in new List<int>(this._pointerMap.Keys))
			{
				PointableCanvasModule.PointerImpl pointerImpl = this._pointerMap[key];
				if (!(pointerImpl.Canvas != pointerCanvas.Canvas))
				{
					this.ClearPointerSelection(pointerImpl.PointerEventData);
					pointerImpl.MarkForDeletion();
					this._pointersForDeletion.Add(pointerImpl);
					this._pointerMap.Remove(key);
				}
			}
		}

		private void HandlePointerEvent(Canvas canvas, PointerEvent evt)
		{
			switch (evt.Type)
			{
			case PointerEventType.Hover:
			{
				PointableCanvasModule.PointerImpl pointerImpl = new PointableCanvasModule.PointerImpl(evt.Identifier, canvas);
				pointerImpl.PointerEventData = new PointerEventData(base.eventSystem);
				pointerImpl.SetPosition(evt.Pose.position);
				this._pointerMap.Add(evt.Identifier, pointerImpl);
				Action<PointableCanvasModule.Pointer> whenPointerStarted = PointableCanvasModule.WhenPointerStarted;
				if (whenPointerStarted == null)
				{
					return;
				}
				whenPointerStarted(pointerImpl);
				return;
			}
			case PointerEventType.Unhover:
			{
				PointableCanvasModule.PointerImpl pointerImpl;
				if (this._pointerMap.TryGetValue(evt.Identifier, out pointerImpl))
				{
					this._pointerMap.Remove(evt.Identifier);
					pointerImpl.MarkForDeletion();
					this._pointersForDeletion.Add(pointerImpl);
					return;
				}
				break;
			}
			case PointerEventType.Select:
			{
				PointableCanvasModule.PointerImpl pointerImpl;
				if (this._pointerMap.TryGetValue(evt.Identifier, out pointerImpl))
				{
					pointerImpl.SetPosition(evt.Pose.position);
					pointerImpl.Press();
					return;
				}
				break;
			}
			case PointerEventType.Unselect:
			{
				PointableCanvasModule.PointerImpl pointerImpl;
				if (this._pointerMap.TryGetValue(evt.Identifier, out pointerImpl))
				{
					pointerImpl.SetPosition(evt.Pose.position);
					pointerImpl.Release();
					return;
				}
				break;
			}
			case PointerEventType.Move:
			{
				PointableCanvasModule.PointerImpl pointerImpl;
				if (this._pointerMap.TryGetValue(evt.Identifier, out pointerImpl))
				{
					pointerImpl.SetPosition(evt.Pose.position);
					return;
				}
				break;
			}
			case PointerEventType.Cancel:
			{
				PointableCanvasModule.PointerImpl pointerImpl;
				if (this._pointerMap.TryGetValue(evt.Identifier, out pointerImpl))
				{
					this._pointerMap.Remove(evt.Identifier);
					this.ClearPointerSelection(pointerImpl.PointerEventData);
					pointerImpl.MarkForDeletion();
					this._pointersForDeletion.Add(pointerImpl);
				}
				break;
			}
			default:
				return;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			PointableCanvasModule._instance = this;
		}

		protected override void OnDestroy()
		{
			PointableCanvasModule._instance = null;
			base.OnDestroy();
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			if (this._exclusiveMode)
			{
				this.DisableOtherModules();
			}
			this.EndStart(ref this._started);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this._started)
			{
				this._pointerEventCamera = base.gameObject.AddComponent<Camera>();
				this._pointerEventCamera.nearClipPlane = 0.1f;
				this._pointerEventCamera.enabled = false;
			}
		}

		protected override void OnDisable()
		{
			if (this._started)
			{
				Object.Destroy(this._pointerEventCamera);
				this._pointerEventCamera = null;
			}
			base.OnDisable();
		}

		private void DisableOtherModules()
		{
			base.GetComponents<BaseInputModule>(this._inputModules);
			foreach (BaseInputModule baseInputModule in this._inputModules)
			{
				if (baseInputModule != this && baseInputModule.enabled)
				{
					baseInputModule.enabled = false;
					Debug.Log("PointableCanvasModule: Disabling " + baseInputModule.GetType().Name + ".");
				}
			}
		}

		public override void UpdateModule()
		{
			base.UpdateModule();
			if (this._exclusiveMode && base.eventSystem.currentInputModule != null && base.eventSystem.currentInputModule != this)
			{
				this.DisableOtherModules();
			}
		}

		protected static RaycastResult FindFirstRaycastWithinCanvas(List<RaycastResult> candidates, Canvas canvas)
		{
			for (int i = 0; i < candidates.Count; i++)
			{
				GameObject gameObject = candidates[i].gameObject;
				if (!(gameObject == null))
				{
					Canvas componentInParent = gameObject.GetComponentInParent<Canvas>();
					if (!(componentInParent == null) && !(componentInParent.rootCanvas != canvas))
					{
						return candidates[i];
					}
				}
			}
			return default(RaycastResult);
		}

		private void UpdateRaycasts(PointableCanvasModule.PointerImpl pointer, out bool pressed, out bool released)
		{
			PointerEventData pointerEventData = pointer.PointerEventData;
			Vector2 position = pointerEventData.position;
			pointerEventData.Reset();
			Vector3 position2 = pointer.Position;
			pointer.ReadAndResetPressedReleased(out pressed, out released);
			if (pointer.MarkedForDeletion)
			{
				pointerEventData.pointerCurrentRaycast = default(RaycastResult);
				return;
			}
			Canvas canvas = pointer.Canvas;
			canvas.worldCamera = this._pointerEventCamera;
			Vector3 position3 = Vector3.zero;
			Plane plane = new Plane(-1f * canvas.transform.forward, canvas.transform.position);
			Ray ray = new Ray(position2 - canvas.transform.forward, canvas.transform.forward);
			float distance;
			if (plane.Raycast(ray, out distance))
			{
				position3 = ray.GetPoint(distance);
			}
			this._pointerEventCamera.transform.position = position2 - canvas.transform.forward;
			this._pointerEventCamera.transform.LookAt(position2, canvas.transform.up);
			Vector2 position4 = this._pointerEventCamera.WorldToScreenPoint(position3);
			pointerEventData.position = position4;
			base.eventSystem.RaycastAll(pointerEventData, this._raycastResultCache);
			RaycastResult pointerCurrentRaycast = PointableCanvasModule.FindFirstRaycastWithinCanvas(this._raycastResultCache, canvas);
			pointer.PointerEventData.pointerCurrentRaycast = pointerCurrentRaycast;
			this._raycastResultCache.Clear();
			this._pointerEventCamera.transform.position = canvas.transform.position - canvas.transform.forward;
			this._pointerEventCamera.transform.LookAt(canvas.transform.position, canvas.transform.up);
			position4 = this._pointerEventCamera.WorldToScreenPoint(position3);
			pointerEventData.position = position4;
			if (pressed)
			{
				pointerEventData.delta = Vector2.zero;
			}
			else
			{
				pointerEventData.delta = pointerEventData.position - position;
			}
			pointerEventData.button = PointerEventData.InputButton.Left;
		}

		public override void Process()
		{
			this.ProcessPointers(this._pointersForDeletion, true);
			this.ProcessPointers(this._pointerMap.Values, false);
		}

		private void ProcessPointers(ICollection<PointableCanvasModule.PointerImpl> pointers, bool clearAndReleasePointers)
		{
			int count = pointers.Count;
			if (count == 0)
			{
				return;
			}
			if (count > this._pointersToProcessScratch.Length)
			{
				this._pointersToProcessScratch = new PointableCanvasModule.PointerImpl[count];
			}
			pointers.CopyTo(this._pointersToProcessScratch, 0);
			if (clearAndReleasePointers)
			{
				pointers.Clear();
			}
			foreach (PointableCanvasModule.PointerImpl pointerImpl in this._pointersToProcessScratch)
			{
				this.ProcessPointer(pointerImpl, clearAndReleasePointers);
				if (clearAndReleasePointers)
				{
					pointerImpl.InvokeWhenDisposed();
				}
			}
		}

		private void ProcessPointer(PointableCanvasModule.PointerImpl pointer, bool forceRelease = false)
		{
			bool pressed = false;
			bool flag = false;
			bool dragging = pointer.PointerEventData.dragging;
			this.UpdateRaycasts(pointer, out pressed, out flag);
			PointerEventData pointerEventData = pointer.PointerEventData;
			this.UpdatePointerEventData(pointerEventData, pressed, flag);
			flag = (flag || forceRelease);
			if (!flag)
			{
				this.ProcessMove(pointerEventData);
				this.ProcessDrag(pointerEventData);
			}
			else
			{
				base.HandlePointerExitAndEnter(pointerEventData, null);
				base.RemovePointerData(pointerEventData);
			}
			this.HandleSelectableHover(pointer, dragging);
			this.HandleSelectablePress(pointer, pressed, flag, dragging);
			pointer.InvokeWhenUpdated();
		}

		private void HandleSelectableHover(PointableCanvasModule.PointerImpl pointer, bool wasDragging)
		{
			bool dragging = pointer.PointerEventData.dragging || wasDragging;
			GameObject gameObject = pointer.PointerEventData.pointerCurrentRaycast.gameObject;
			GameObject hoveredSelectable = pointer.HoveredSelectable;
			GameObject eventHandler = ExecuteEvents.GetEventHandler<ISelectHandler>(gameObject);
			pointer.SetHoveredSelectable(eventHandler);
			if (!(eventHandler != null) || !(eventHandler != hoveredSelectable))
			{
				if (hoveredSelectable != null && eventHandler == null)
				{
					Action<PointableCanvasEventArgs> whenSelectableUnhovered = PointableCanvasModule.WhenSelectableUnhovered;
					if (whenSelectableUnhovered == null)
					{
						return;
					}
					whenSelectableUnhovered(new PointableCanvasEventArgs(pointer.Canvas, pointer.HoveredSelectable, dragging));
				}
				return;
			}
			Action<PointableCanvasEventArgs> whenSelectableHovered = PointableCanvasModule.WhenSelectableHovered;
			if (whenSelectableHovered == null)
			{
				return;
			}
			whenSelectableHovered(new PointableCanvasEventArgs(pointer.Canvas, pointer.HoveredSelectable, dragging));
		}

		private void HandleSelectablePress(PointableCanvasModule.PointerImpl pointer, bool pressed, bool released, bool wasDragging)
		{
			bool dragging = pointer.PointerEventData.dragging || wasDragging;
			if (!pressed)
			{
				if (released && !pointer.MarkedForDeletion)
				{
					GameObject hovered = (pointer.HoveredSelectable != null && pointer.HoveredSelectable == pointer.PointerEventData.selectedObject) ? pointer.HoveredSelectable : null;
					Action<PointableCanvasEventArgs> whenUnselected = PointableCanvasModule.WhenUnselected;
					if (whenUnselected == null)
					{
						return;
					}
					whenUnselected(new PointableCanvasEventArgs(pointer.Canvas, hovered, dragging));
				}
				return;
			}
			Action<PointableCanvasEventArgs> whenSelected = PointableCanvasModule.WhenSelected;
			if (whenSelected == null)
			{
				return;
			}
			whenSelected(new PointableCanvasEventArgs(pointer.Canvas, pointer.HoveredSelectable, dragging));
		}

		protected void UpdatePointerEventData(PointerEventData pointerEvent, bool pressed, bool released)
		{
			GameObject gameObject = pointerEvent.pointerCurrentRaycast.gameObject;
			if (pressed)
			{
				pointerEvent.eligibleForClick = true;
				pointerEvent.delta = Vector2.zero;
				pointerEvent.dragging = false;
				pointerEvent.useDragThreshold = true;
				pointerEvent.pressPosition = pointerEvent.position;
				pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
				base.DeselectIfSelectionChanged(gameObject, pointerEvent);
				if (pointerEvent.pointerEnter != gameObject)
				{
					base.HandlePointerExitAndEnter(pointerEvent, gameObject);
					pointerEvent.pointerEnter = gameObject;
				}
				GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject, pointerEvent, ExecuteEvents.pointerDownHandler);
				if (gameObject2 == null)
				{
					gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				}
				float unscaledTime = Time.unscaledTime;
				if (gameObject2 == pointerEvent.lastPress)
				{
					if (unscaledTime - pointerEvent.clickTime < 0.3f)
					{
						int clickCount = pointerEvent.clickCount + 1;
						pointerEvent.clickCount = clickCount;
					}
					else
					{
						pointerEvent.clickCount = 1;
					}
					pointerEvent.clickTime = unscaledTime;
				}
				else
				{
					pointerEvent.clickCount = 1;
				}
				pointerEvent.pointerPress = gameObject2;
				pointerEvent.rawPointerPress = gameObject;
				pointerEvent.clickTime = unscaledTime;
				pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
				if (pointerEvent.pointerDrag != null)
				{
					ExecuteEvents.Execute<IInitializePotentialDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
				}
			}
			if (released)
			{
				ExecuteEvents.Execute<IPointerUpHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
				GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				if (pointerEvent.pointerPress == eventHandler && pointerEvent.eligibleForClick)
				{
					ExecuteEvents.Execute<IPointerClickHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
				}
				if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
				{
					ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject, pointerEvent, ExecuteEvents.dropHandler);
				}
				pointerEvent.eligibleForClick = false;
				pointerEvent.pointerPress = null;
				pointerEvent.rawPointerPress = null;
				if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
				{
					ExecuteEvents.Execute<IEndDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
				}
				pointerEvent.dragging = false;
				pointerEvent.pointerDrag = null;
				ExecuteEvents.ExecuteHierarchy<IPointerExitHandler>(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
				pointerEvent.pointerEnter = null;
			}
		}

		protected override void ProcessDrag(PointerEventData pointerEvent)
		{
			if (!pointerEvent.IsPointerMoving() || pointerEvent.pointerDrag == null)
			{
				return;
			}
			if (!pointerEvent.dragging && PointableCanvasModule.ShouldStartDrag(pointerEvent.pressPosition, pointerEvent.position, (float)base.eventSystem.pixelDragThreshold, pointerEvent.useDragThreshold))
			{
				if (this._useInitialPressPositionForDrag)
				{
					pointerEvent.position = pointerEvent.pressPosition;
				}
				ExecuteEvents.Execute<IBeginDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.beginDragHandler);
				pointerEvent.dragging = true;
			}
			if (pointerEvent.dragging)
			{
				if (pointerEvent.pointerPress != pointerEvent.pointerDrag)
				{
					this.ClearPointerSelection(pointerEvent);
				}
				ExecuteEvents.Execute<IDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.dragHandler);
			}
		}

		private void ClearPointerSelection(PointerEventData pointerEvent)
		{
			ExecuteEvents.Execute<IPointerUpHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
			pointerEvent.eligibleForClick = false;
			pointerEvent.pointerPress = null;
			pointerEvent.rawPointerPress = null;
		}

		protected static bool ShouldStartDrag(Vector2 pressPos, Vector2 currentPos, float threshold, bool useDragThreshold)
		{
			return !useDragThreshold || (pressPos - currentPos).sqrMagnitude >= threshold * threshold;
		}

		[Tooltip("If true, the initial press position will be used as the drag start position, rather than the position when drag threshold is exceeded. This is used to prevent the pointer position shifting relative to the surface while dragging.")]
		[SerializeField]
		private bool _useInitialPressPositionForDrag = true;

		[Tooltip("If true, this module will disable other input modules in the event system and will be the only input module used in the scene.")]
		[SerializeField]
		private bool _exclusiveMode;

		private Camera _pointerEventCamera;

		private static PointableCanvasModule _instance;

		private Dictionary<int, PointableCanvasModule.PointerImpl> _pointerMap = new Dictionary<int, PointableCanvasModule.PointerImpl>();

		private List<RaycastResult> _raycastResultCache = new List<RaycastResult>();

		private List<PointableCanvasModule.PointerImpl> _pointersForDeletion = new List<PointableCanvasModule.PointerImpl>();

		private Dictionary<IPointableCanvas, Action<PointerEvent>> _pointerCanvasActionMap = new Dictionary<IPointableCanvas, Action<PointerEvent>>();

		private List<BaseInputModule> _inputModules = new List<BaseInputModule>();

		private PointableCanvasModule.PointerImpl[] _pointersToProcessScratch = Array.Empty<PointableCanvasModule.PointerImpl>();

		protected bool _started;

		public class Pointer
		{
			internal Pointer(int identifier)
			{
				this.Identifier = identifier;
			}

			public int Identifier { get; }

			internal PointerEventData PointerEventData { get; set; }

			public event Action<PointerEventData> WhenUpdated = delegate(PointerEventData _)
			{
			};

			public event Action WhenDisposed = delegate()
			{
			};

			internal void InvokeWhenUpdated()
			{
				this.WhenUpdated(this.PointerEventData);
			}

			internal void InvokeWhenDisposed()
			{
				this.WhenDisposed();
			}
		}

		private class PointerImpl : PointableCanvasModule.Pointer
		{
			public bool MarkedForDeletion { get; private set; }

			public Canvas Canvas
			{
				get
				{
					return this._canvas;
				}
			}

			public Vector3 Position
			{
				get
				{
					return this._position;
				}
			}

			public GameObject HoveredSelectable
			{
				get
				{
					return this._hoveredSelectable;
				}
			}

			public PointerImpl(int identifier, Canvas canvas) : base(identifier)
			{
				this._canvas = canvas;
				this._pressed = (this._released = false);
			}

			public void Press()
			{
				if (this._pressing)
				{
					return;
				}
				this._pressing = true;
				this._pressed = true;
			}

			public void Release()
			{
				if (!this._pressing)
				{
					return;
				}
				this._pressing = false;
				this._released = true;
			}

			public void ReadAndResetPressedReleased(out bool pressed, out bool released)
			{
				pressed = this._pressed;
				released = this._released;
				this._pressed = (this._released = false);
				this._position = this._targetPosition;
			}

			public void MarkForDeletion()
			{
				this.MarkedForDeletion = true;
				this.Release();
			}

			public void SetPosition(Vector3 position)
			{
				this._targetPosition = position;
				if (!this._released)
				{
					this._position = position;
				}
			}

			public void SetHoveredSelectable(GameObject hoveredSelectable)
			{
				this._hoveredSelectable = hoveredSelectable;
			}

			private Canvas _canvas;

			private Vector3 _position;

			private Vector3 _targetPosition;

			private GameObject _hoveredSelectable;

			private bool _pressing;

			private bool _pressed;

			private bool _released;
		}
	}
}
