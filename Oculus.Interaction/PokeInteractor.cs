using System;
using System.Collections.Generic;
using Oculus.Interaction.Surfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
	public class PokeInteractor : PointerInteractor<PokeInteractor, PokeInteractable>, ITimeConsumer
	{
		public Vector3 ClosestPoint { get; private set; }

		public Vector3 TouchPoint { get; private set; }

		public Vector3 TouchNormal { get; private set; }

		public float Radius
		{
			get
			{
				return this._radius;
			}
		}

		public Vector3 Origin { get; private set; }

		public void SetTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		public bool IsPassedSurface
		{
			get
			{
				return this._isPassedSurface;
			}
			set
			{
				bool isPassedSurface = this._isPassedSurface;
				this._isPassedSurface = value;
				if (value != isPassedSurface)
				{
					this.WhenPassedSurfaceChanged(value);
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this._nativeId = 5795969328217354098UL;
		}

		protected override void Start()
		{
			base.Start();
			this._dragEaseCurve = new ProgressCurve();
			this._pinningResyncCurve = new ProgressCurve();
			this._hitCache = new PokeInteractor.SurfaceHitCache();
			this._previousSurfaceTransformMap = new Dictionary<PokeInteractable, Matrix4x4>();
		}

		protected override void DoPreprocess()
		{
			base.DoPreprocess();
			this._previousPokeOrigin = this.Origin;
			this.Origin = this._pointTransform.position;
			this._hitCache.Reset(this.Origin);
		}

		protected override void DoPostprocess()
		{
			base.DoPostprocess();
			foreach (PokeInteractable pokeInteractable in Interactable<PokeInteractor, PokeInteractable>.Registry.List(this))
			{
				this._previousSurfaceTransformMap[pokeInteractable] = pokeInteractable.SurfacePatch.BackingSurface.Transform.worldToLocalMatrix;
			}
			this._lastUpdateTime = this._timeProvider();
		}

		protected override bool ComputeShouldSelect()
		{
			if (this._recoilInteractable != null)
			{
				float num = this.ComputePokeDepth(this._recoilInteractable, this.Origin);
				this._reEnterDepth = Mathf.Min(num + this._recoilInteractable.RecoilAssist.ReEnterDistance, this._reEnterDepth);
				this._hitInteractable = ((num > this._reEnterDepth) ? this._recoilInteractable : null);
			}
			return this._hitInteractable != null;
		}

		protected override bool ComputeShouldUnselect()
		{
			return this._hitInteractable == null;
		}

		private bool GetBackingHit(PokeInteractable interactable, out SurfaceHit hit)
		{
			return this._hitCache.GetBackingHit(interactable, out hit);
		}

		private bool GetPatchHit(PokeInteractable interactable, out SurfaceHit hit)
		{
			return this._hitCache.GetPatchHit(interactable, out hit);
		}

		private bool InteractableInRange(PokeInteractable interactable)
		{
			if (!this._previousSurfaceTransformMap.ContainsKey(interactable))
			{
				return true;
			}
			Vector3 position = this._previousSurfaceTransformMap[interactable].MultiplyPoint(this._previousPokeOrigin);
			Vector3 b = interactable.SurfacePatch.BackingSurface.Transform.TransformPoint(position);
			float num = (interactable == base.Interactable) ? Mathf.Max(interactable.ExitHoverTangent, interactable.ExitHoverNormal) : Mathf.Max(interactable.EnterHoverTangent, interactable.EnterHoverNormal);
			float maxDistance = Vector3.Distance(this.Origin, b) + this.Radius + num + this._equalDistanceThreshold + interactable.CloseDistanceThreshold;
			ISurface surfacePatch = interactable.SurfacePatch;
			Vector3 origin = this.Origin;
			SurfaceHit surfaceHit;
			return surfacePatch.ClosestSurfacePoint(origin, out surfaceHit, maxDistance);
		}

		protected override void DoHoverUpdate()
		{
			SurfaceHit surfaceHit;
			if (this._interactable != null && this.GetBackingHit(this._interactable, out surfaceHit))
			{
				this.TouchPoint = surfaceHit.Point;
				this.TouchNormal = surfaceHit.Normal;
			}
			if (this._recoilInteractable != null)
			{
				if (!this.SurfaceUpdate(this._recoilInteractable))
				{
					this._isRecoiled = false;
					this._recoilInteractable = null;
					this._recoilVelocityExpansion = 0f;
					this.IsPassedSurface = false;
					return;
				}
				if (this.ShouldCancel(this._recoilInteractable))
				{
					base.GeneratePointerEvent(PointerEventType.Cancel, this._recoilInteractable);
					this._previousPokeOrigin = this.Origin;
					this._previousCandidate = null;
					this._hitInteractable = null;
					this._recoilInteractable = null;
					this._recoilVelocityExpansion = 0f;
					this.IsPassedSurface = false;
					this._isRecoiled = false;
				}
			}
		}

		protected override PokeInteractable ComputeCandidate()
		{
			if (this._recoilInteractable != null)
			{
				return this._recoilInteractable;
			}
			if (this._hitInteractable != null)
			{
				return this._hitInteractable;
			}
			this.UpdateInteractablesInRange(ref this._cachedInteractablesInRange);
			PokeInteractable pokeInteractable = this.ComputeSelectCandidate(this._cachedInteractablesInRange);
			if (pokeInteractable != null)
			{
				this._hitInteractable = pokeInteractable;
				this._previousCandidate = pokeInteractable;
				return this._hitInteractable;
			}
			pokeInteractable = this.ComputeHoverCandidate(this._cachedInteractablesInRange);
			this._previousCandidate = pokeInteractable;
			return pokeInteractable;
		}

		protected override int ComputeCandidateTiebreaker(PokeInteractable a, PokeInteractable b)
		{
			int num = base.ComputeCandidateTiebreaker(a, b);
			if (num != 0)
			{
				return num;
			}
			return a.TiebreakerScore.CompareTo(b.TiebreakerScore);
		}

		private void UpdateInteractablesInRange(ref List<PokeInteractor.CachedInteractable> cachedInteractables)
		{
			cachedInteractables.Clear();
			foreach (PokeInteractable interactable in Interactable<PokeInteractor, PokeInteractable>.Registry.List(this))
			{
				SurfaceHit backingHit;
				SurfaceHit patchHit;
				if (this.InteractableInRange(interactable) && this.GetBackingHit(interactable, out backingHit) && this.GetPatchHit(interactable, out patchHit))
				{
					cachedInteractables.Add(new PokeInteractor.CachedInteractable
					{
						interactable = interactable,
						backingHit = backingHit,
						patchHit = patchHit
					});
				}
			}
		}

		private PokeInteractable ComputeSelectCandidate(List<PokeInteractor.CachedInteractable> interactables)
		{
			PokeInteractable pokeInteractable = null;
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			foreach (PokeInteractor.CachedInteractable cachedInteractable in interactables)
			{
				PokeInteractable interactable = cachedInteractable.interactable;
				Vector3 position = (this._previousSurfaceTransformMap.ContainsKey(interactable) ? this._previousSurfaceTransformMap[interactable] : interactable.SurfacePatch.BackingSurface.Transform.worldToLocalMatrix).MultiplyPoint(this._previousPokeOrigin);
				Vector3 vector = interactable.SurfacePatch.BackingSurface.Transform.TransformPoint(position);
				if (this.PassesEnterHoverDistanceCheck(vector, interactable))
				{
					Vector3 vector2 = this.Origin - vector;
					float magnitude = vector2.magnitude;
					if (magnitude != 0f)
					{
						vector2 /= magnitude;
						Ray ray = new Ray(vector, vector2);
						SurfaceHit backingHit = cachedInteractable.backingHit;
						Vector3 normal = backingHit.Normal;
						if (Vector3.Dot(vector2, normal) < 0f)
						{
							SurfaceHit surfaceHit;
							bool flag = interactable.SurfacePatch.BackingSurface.Raycast(ray, out surfaceHit, 0f);
							flag = (flag && surfaceHit.Distance <= magnitude);
							if (!flag)
							{
								float num3 = this.ComputeDistanceAbove(interactable, this.Origin);
								if (num3 <= 0f)
								{
									Vector3 point = backingHit.Point;
									flag = true;
									surfaceHit = new SurfaceHit
									{
										Point = point,
										Normal = backingHit.Normal,
										Distance = num3
									};
								}
							}
							if (flag)
							{
								float num4 = this.ComputeTangentDistance(interactable, surfaceHit.Point);
								if (num4 <= ((interactable != this._previousCandidate) ? interactable.EnterHoverTangent : interactable.ExitHoverTangent))
								{
									float num5 = Vector3.Dot(vector - surfaceHit.Point, surfaceHit.Normal);
									if (Mathf.Abs(num5 - num) < this._equalDistanceThreshold)
									{
										int num6 = this.ComputeCandidateTiebreaker(interactable, pokeInteractable);
										if (num6 > 0)
										{
											num = num5;
											num2 = num4;
											pokeInteractable = interactable;
										}
										if (num6 != 0)
										{
											continue;
										}
									}
									if (num5 <= num + interactable.CloseDistanceThreshold)
									{
										if (pokeInteractable == null || num5 < num - pokeInteractable.CloseDistanceThreshold)
										{
											num = num5;
											num2 = num4;
											pokeInteractable = interactable;
										}
										else if (num4 < num2)
										{
											num = num5;
											num2 = num4;
											pokeInteractable = interactable;
										}
									}
								}
							}
						}
					}
				}
			}
			if (pokeInteractable != null)
			{
				SurfaceHit surfaceHit2;
				this.GetBackingHit(pokeInteractable, out surfaceHit2);
				SurfaceHit surfaceHit3;
				this.GetPatchHit(pokeInteractable, out surfaceHit3);
				this.ClosestPoint = surfaceHit3.Point;
				this.TouchPoint = surfaceHit2.Point;
				this.TouchNormal = surfaceHit2.Normal;
				foreach (PokeInteractor.CachedInteractable cachedInteractable2 in this._cachedInteractablesInRange)
				{
					PokeInteractable interactable2 = cachedInteractable2.interactable;
					if (!(interactable2 == pokeInteractable))
					{
						Vector3 position2 = (this._previousSurfaceTransformMap.ContainsKey(interactable2) ? this._previousSurfaceTransformMap[interactable2] : interactable2.SurfacePatch.BackingSurface.Transform.worldToLocalMatrix).MultiplyPoint(this._previousPokeOrigin);
						Vector3 position3 = interactable2.SurfacePatch.BackingSurface.Transform.TransformPoint(position2);
						if (this.PassesEnterHoverDistanceCheck(position3, interactable2))
						{
							SurfaceHit backingHit2 = cachedInteractable2.backingHit;
							float num7 = Vector3.Dot(this.TouchPoint - backingHit2.Point, backingHit2.Normal);
							if ((Mathf.Abs(num7) >= this._equalDistanceThreshold || this.ComputeCandidateTiebreaker(pokeInteractable, interactable2) <= 0) && num7 > 0f && num7 <= interactable2.CloseDistanceThreshold)
							{
								float num8 = this.ComputeTangentDistance(interactable2, this.TouchPoint);
								if (num8 <= interactable2.EnterHoverTangent && num8 <= num2)
								{
									return null;
								}
							}
						}
					}
				}
				return pokeInteractable;
			}
			return pokeInteractable;
		}

		private bool PassesEnterHoverDistanceCheck(Vector3 position, PokeInteractable interactable)
		{
			if (interactable == this._previousCandidate)
			{
				return true;
			}
			float num = 0f;
			if (interactable.MinThresholds.Enabled)
			{
				num = Mathf.Min(interactable.MinThresholds.MinNormal, this.MinPokeDepth(interactable));
			}
			return this.ComputeDistanceAbove(interactable, position) > num;
		}

		public float MinPokeDepth(PokeInteractable interactable)
		{
			float num = interactable.ExitHoverNormal;
			foreach (PokeInteractor pokeInteractor in interactable.Interactors)
			{
				num = Mathf.Min(this.ComputePokeDepth(interactable, pokeInteractor.Origin), num);
			}
			return num;
		}

		private PokeInteractable ComputeHoverCandidate(List<PokeInteractor.CachedInteractable> interactables)
		{
			PokeInteractable pokeInteractable = null;
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			foreach (PokeInteractor.CachedInteractable cachedInteractable in interactables)
			{
				PokeInteractable interactable = cachedInteractable.interactable;
				if (this.PassesEnterHoverDistanceCheck(this.Origin, interactable) || this.PassesEnterHoverDistanceCheck(this._previousPokeOrigin, interactable))
				{
					SurfaceHit backingHit = cachedInteractable.backingHit;
					Vector3 point = backingHit.Point;
					Vector3 normal = backingHit.Normal;
					Vector3 lhs = this.Origin - point;
					if (lhs.magnitude != 0f && Vector3.Dot(lhs, normal) > 0f)
					{
						float num3 = this.ComputeDistanceAbove(interactable, this.Origin);
						if (num3 <= ((this._previousCandidate != interactable) ? interactable.EnterHoverNormal : interactable.ExitHoverNormal))
						{
							float num4 = this.ComputeTangentDistance(interactable, this.Origin);
							if (num4 <= ((this._previousCandidate != interactable) ? interactable.EnterHoverTangent : interactable.ExitHoverTangent))
							{
								if (Mathf.Abs(num3 - num) < this._equalDistanceThreshold && pokeInteractable != null)
								{
									int num5 = this.ComputeCandidateTiebreaker(interactable, pokeInteractable);
									if (num5 > 0)
									{
										pokeInteractable = interactable;
										num = num3;
										num2 = num4;
									}
									if (num5 != 0)
									{
										continue;
									}
								}
								if (num3 <= num + interactable.CloseDistanceThreshold)
								{
									if (pokeInteractable == null || num3 < num - pokeInteractable.CloseDistanceThreshold)
									{
										pokeInteractable = interactable;
										num = num3;
										num2 = num4;
									}
									else if (num4 < num2)
									{
										pokeInteractable = interactable;
										num = num3;
										num2 = num4;
									}
								}
							}
						}
					}
				}
			}
			if (pokeInteractable != null)
			{
				SurfaceHit surfaceHit;
				this.GetBackingHit(pokeInteractable, out surfaceHit);
				SurfaceHit surfaceHit2;
				this.GetPatchHit(pokeInteractable, out surfaceHit2);
				this.ClosestPoint = surfaceHit2.Point;
				this.TouchPoint = surfaceHit.Point;
				this.TouchNormal = surfaceHit.Normal;
			}
			return pokeInteractable;
		}

		protected override void InteractableSelected(PokeInteractable interactable)
		{
			SurfaceHit surfaceHit;
			if (interactable != null && this.GetBackingHit(interactable, out surfaceHit))
			{
				this._previousSurfacePointLocal = (this._firstTouchPointLocal = (this._easeTouchPointLocal = (this._targetTouchPointLocal = interactable.SurfacePatch.BackingSurface.Transform.InverseTransformPoint(this.TouchPoint))));
				Vector3 point = surfaceHit.Point;
				this._dragCompareSurfacePointLocal = interactable.SurfacePatch.BackingSurface.Transform.InverseTransformPoint(point);
				this._dragEaseCurve.Copy(interactable.DragThresholds.DragEaseCurve);
				this._pinningResyncCurve.Copy(interactable.PositionPinning.ResyncCurve);
				this._isDragging = false;
				this._isRecoiled = false;
				this._maxDistanceFromFirstTouchPoint = 0f;
				this._selectMaxDepth = 0f;
			}
			this.IsPassedSurface = true;
			base.InteractableSelected(interactable);
		}

		protected override void HandleDisabled()
		{
			this._hitInteractable = null;
			this.IsPassedSurface = false;
			base.HandleDisabled();
		}

		protected override Pose ComputePointerPose()
		{
			if (base.Interactable == null)
			{
				return Pose.identity;
			}
			SurfaceHit surfaceHit;
			if (!base.Interactable.ClosestBackingSurfaceHit(this.TouchPoint, out surfaceHit))
			{
				return Pose.identity;
			}
			return new Pose(this.TouchPoint, Quaternion.LookRotation(surfaceHit.Normal));
		}

		private float ComputeDistanceAbove(PokeInteractable interactable, Vector3 point)
		{
			return SurfaceUtils.ComputeDistanceAbove(interactable.SurfacePatch, point, this._radius);
		}

		[Obsolete("This will be removed in a future version of Interaction SDK. Please use SurfaceUtils.ComputeDepth instead")]
		public float ComputeDepth(PokeInteractable interactable, Vector3 point)
		{
			return SurfaceUtils.ComputeDepth(interactable.SurfacePatch, point, this._radius);
		}

		private float ComputePokeDepth(PokeInteractable interactable, Vector3 point)
		{
			return SurfaceUtils.ComputeDepth(interactable.SurfacePatch, point, this._radius);
		}

		private float ComputeDistanceFrom(PokeInteractable interactable, Vector3 point)
		{
			return SurfaceUtils.ComputeDistanceFrom(interactable.SurfacePatch, point, this._radius);
		}

		private float ComputeTangentDistance(PokeInteractable interactable, Vector3 point)
		{
			return SurfaceUtils.ComputeTangentDistance(interactable.SurfacePatch, point, this._radius);
		}

		protected virtual bool SurfaceUpdate(PokeInteractable interactable)
		{
			if (interactable == null)
			{
				return false;
			}
			SurfaceHit surfaceHit;
			if (!this.GetBackingHit(interactable, out surfaceHit))
			{
				return false;
			}
			if (this.ComputeDistanceAbove(interactable, this.Origin) > this._touchReleaseThreshold)
			{
				return false;
			}
			bool isRecoiled = this._isRecoiled;
			this._isRecoiled = (this._hitInteractable == null && this._recoilInteractable != null);
			Vector3 point = surfaceHit.Point;
			Vector3 vector = interactable.SurfacePatch.BackingSurface.Transform.InverseTransformPoint(point);
			if (interactable.DragThresholds.Enabled)
			{
				float num = Mathf.Abs(this.ComputePokeDepth(interactable, this.Origin) - this.ComputePokeDepth(interactable, this._previousPokeOrigin));
				Vector3 vector2 = vector - this._previousSurfacePointLocal;
				bool flag = num > interactable.SurfacePatch.BackingSurface.Transform.TransformVector(vector2).magnitude && num > interactable.DragThresholds.DragNormal;
				if (flag)
				{
					this._dragCompareSurfacePointLocal = vector;
				}
				if (!this._isDragging)
				{
					if (!flag)
					{
						Vector3 vector3 = vector - this._dragCompareSurfacePointLocal;
						if (interactable.SurfacePatch.BackingSurface.Transform.TransformVector(vector3).magnitude > interactable.DragThresholds.DragTangent)
						{
							this._isDragging = true;
							this._dragEaseCurve.Start();
							this._previousDragCurveProgress = 0f;
							this._targetTouchPointLocal = vector;
						}
					}
				}
				else if (flag)
				{
					this._isDragging = false;
				}
				else
				{
					this._targetTouchPointLocal = vector;
				}
			}
			else
			{
				this._targetTouchPointLocal = vector;
			}
			Vector3 vector4 = this._targetTouchPointLocal;
			if (interactable.PositionPinning.Enabled)
			{
				if (!this._isRecoiled)
				{
					Vector3 vector5 = vector4 - this._firstTouchPointLocal;
					this._maxDistanceFromFirstTouchPoint = Mathf.Max(interactable.SurfacePatch.BackingSurface.Transform.TransformVector(vector5).magnitude, this._maxDistanceFromFirstTouchPoint);
					float num2 = 1f;
					if (interactable.PositionPinning.MaxPinDistance != 0f)
					{
						num2 = Mathf.Clamp01(this._maxDistanceFromFirstTouchPoint / interactable.PositionPinning.MaxPinDistance);
						num2 = interactable.PositionPinning.PinningEaseCurve.Evaluate(num2);
					}
					vector4 = this._firstTouchPointLocal + vector5 * num2;
				}
				else
				{
					if (!isRecoiled)
					{
						this._pinningResyncCurve.Start();
						this._previousPinningCurveProgress = 0f;
					}
					float num3 = this._pinningResyncCurve.Progress();
					if (num3 != 1f)
					{
						float num4 = num3 - this._previousPinningCurveProgress;
						Vector3 a = vector4 - this._easeTouchPointLocal;
						vector4 = this._easeTouchPointLocal + num4 / (1f - this._previousPinningCurveProgress) * a;
						this._previousPinningCurveProgress = num3;
					}
				}
			}
			float num5 = this._dragEaseCurve.Progress();
			if (num5 != 1f)
			{
				float num6 = num5 - this._previousDragCurveProgress;
				Vector3 a2 = vector4 - this._easeTouchPointLocal;
				this._easeTouchPointLocal += num6 / (1f - this._previousDragCurveProgress) * a2;
				this._previousDragCurveProgress = num5;
			}
			else
			{
				this._easeTouchPointLocal = vector4;
			}
			this.TouchPoint = interactable.SurfacePatch.BackingSurface.Transform.TransformPoint(this._easeTouchPointLocal);
			SurfaceHit surfaceHit2;
			interactable.ClosestBackingSurfaceHit(this.TouchPoint, out surfaceHit2);
			this.TouchNormal = surfaceHit2.Normal;
			this._previousSurfacePointLocal = vector;
			return true;
		}

		protected virtual bool ShouldCancel(PokeInteractable interactable)
		{
			return (interactable.CancelSelectNormal > 0f && this.ComputePokeDepth(interactable, this.Origin) > interactable.CancelSelectNormal) || (interactable.CancelSelectTangent > 0f && this.ComputeTangentDistance(interactable, this.Origin) > interactable.CancelSelectTangent);
		}

		protected virtual bool ShouldRecoil(PokeInteractable interactable)
		{
			if (!interactable.RecoilAssist.Enabled)
			{
				return false;
			}
			float num = this.ComputePokeDepth(interactable, this.Origin);
			float num2 = this._timeProvider() - this._lastUpdateTime;
			float num3 = interactable.RecoilAssist.ExitDistance;
			if (interactable.RecoilAssist.UseVelocityExpansion)
			{
				Vector3 lhs = this.Origin - this._previousPokeOrigin;
				float num4 = Mathf.Max(0f, Vector3.Dot(lhs, -this.TouchNormal));
				num4 = ((num2 > 0f) ? (num4 / num2) : 0f);
				float num5 = Mathf.Clamp01(Mathf.InverseLerp(interactable.RecoilAssist.VelocityExpansionMinSpeed, interactable.RecoilAssist.VelocityExpansionMaxSpeed, num4)) * interactable.RecoilAssist.VelocityExpansionDistance;
				if (num5 > this._recoilVelocityExpansion)
				{
					this._recoilVelocityExpansion = num5;
				}
				else
				{
					float num6 = interactable.RecoilAssist.VelocityExpansionDecayRate * num2;
					this._recoilVelocityExpansion = Math.Max(num5, this._recoilVelocityExpansion - num6);
				}
				num3 += this._recoilVelocityExpansion;
			}
			if (num > this._selectMaxDepth)
			{
				this._selectMaxDepth = num;
			}
			else
			{
				if (interactable.RecoilAssist.UseDynamicDecay)
				{
					Vector3 vector = this.Origin - this._previousPokeOrigin;
					Vector3 vector2 = Vector3.Project(vector, this.TouchNormal);
					float time = (vector.sqrMagnitude > 1E-07f) ? (vector2.magnitude / vector.magnitude) : 1f;
					float num7 = interactable.RecoilAssist.DynamicDecayCurve.Evaluate(time);
					this._selectMaxDepth = Mathf.Lerp(this._selectMaxDepth, num, num7 * num2);
				}
				if (num < this._selectMaxDepth - num3)
				{
					this._reEnterDepth = num + interactable.RecoilAssist.ReEnterDistance;
					return true;
				}
			}
			return false;
		}

		protected override void DoSelectUpdate()
		{
			if (!this.SurfaceUpdate(this._selectedInteractable))
			{
				this._hitInteractable = null;
				this.IsPassedSurface = (this._recoilInteractable != null);
				return;
			}
			if (this.ShouldCancel(this._selectedInteractable))
			{
				base.GeneratePointerEvent(PointerEventType.Cancel, this._selectedInteractable);
				this._previousPokeOrigin = this.Origin;
				this._previousCandidate = null;
				this._hitInteractable = null;
				this._recoilInteractable = null;
				this._recoilVelocityExpansion = 0f;
				this.IsPassedSurface = false;
				this._isRecoiled = false;
				return;
			}
			if (this.ShouldRecoil(this._selectedInteractable))
			{
				this._hitInteractable = null;
				this._recoilInteractable = this._selectedInteractable;
				this._selectMaxDepth = 0f;
			}
		}

		public void InjectAllPokeInteractor(Transform pointTransform, float radius = 0.005f)
		{
			this.InjectPointTransform(pointTransform);
			this.InjectRadius(radius);
		}

		public void InjectPointTransform(Transform pointTransform)
		{
			this._pointTransform = pointTransform;
		}

		public void InjectRadius(float radius)
		{
			this._radius = radius;
		}

		public void InjectOptionalTouchReleaseThreshold(float touchReleaseThreshold)
		{
			this._touchReleaseThreshold = touchReleaseThreshold;
		}

		public void InjectOptionalEqualDistanceThreshold(float equalDistanceThreshold)
		{
			this._equalDistanceThreshold = equalDistanceThreshold;
		}

		[Obsolete("Use SetTimeProvider()")]
		public void InjectOptionalTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		[SerializeField]
		[Tooltip("The poke origin tracks the provided transform.")]
		private Transform _pointTransform;

		[SerializeField]
		[Tooltip("(Meters, World) The radius of the sphere positioned at the origin.")]
		private float _radius = 0.005f;

		[SerializeField]
		[Tooltip("(Meters, World) A poke unselect fires when the poke origin surpasses this distance above a surface.")]
		private float _touchReleaseThreshold = 0.002f;

		[FormerlySerializedAs("_zThreshold")]
		[SerializeField]
		[Tooltip("(Meters, World) The threshold below which distances to a surface will use tiebreaker score to decide candidate.")]
		private float _equalDistanceThreshold = 0.001f;

		private Vector3 _previousPokeOrigin;

		private PokeInteractable _previousCandidate;

		private PokeInteractable _hitInteractable;

		private PokeInteractable _recoilInteractable;

		private Vector3 _previousSurfacePointLocal;

		private Vector3 _firstTouchPointLocal;

		private Vector3 _targetTouchPointLocal;

		private Vector3 _easeTouchPointLocal;

		private bool _isRecoiled;

		private bool _isDragging;

		private ProgressCurve _dragEaseCurve;

		private ProgressCurve _pinningResyncCurve;

		private Vector3 _dragCompareSurfacePointLocal;

		private float _maxDistanceFromFirstTouchPoint;

		private float _recoilVelocityExpansion;

		private float _selectMaxDepth;

		private float _reEnterDepth;

		private float _lastUpdateTime;

		private Func<float> _timeProvider = () => Time.time;

		private bool _isPassedSurface;

		public Action<bool> WhenPassedSurfaceChanged = delegate(bool <p0>)
		{
		};

		private PokeInteractor.SurfaceHitCache _hitCache;

		private Dictionary<PokeInteractable, Matrix4x4> _previousSurfaceTransformMap;

		private float _previousDragCurveProgress;

		private float _previousPinningCurveProgress;

		private List<PokeInteractor.CachedInteractable> _cachedInteractablesInRange = new List<PokeInteractor.CachedInteractable>();

		private class SurfaceHitCache
		{
			public bool GetPatchHit(PokeInteractable interactable, out SurfaceHit hit)
			{
				if (!this._surfacePatchHitCache.ContainsKey(interactable))
				{
					SurfaceHit hit2;
					bool isValid = interactable.SurfacePatch.ClosestSurfacePoint(this._origin, out hit2, 0f);
					PokeInteractor.SurfaceHitCache.HitInfo value = new PokeInteractor.SurfaceHitCache.HitInfo(isValid, hit2);
					this._surfacePatchHitCache.Add(interactable, value);
				}
				hit = this._surfacePatchHitCache[interactable].Hit;
				return this._surfacePatchHitCache[interactable].IsValid;
			}

			public bool GetBackingHit(PokeInteractable interactable, out SurfaceHit hit)
			{
				if (!this._backingSurfaceHitCache.ContainsKey(interactable))
				{
					SurfaceHit hit2;
					bool isValid = interactable.SurfacePatch.BackingSurface.ClosestSurfacePoint(this._origin, out hit2, 0f);
					PokeInteractor.SurfaceHitCache.HitInfo value = new PokeInteractor.SurfaceHitCache.HitInfo(isValid, hit2);
					this._backingSurfaceHitCache.Add(interactable, value);
				}
				hit = this._backingSurfaceHitCache[interactable].Hit;
				return this._backingSurfaceHitCache[interactable].IsValid;
			}

			public SurfaceHitCache()
			{
				this._surfacePatchHitCache = new Dictionary<PokeInteractable, PokeInteractor.SurfaceHitCache.HitInfo>();
				this._backingSurfaceHitCache = new Dictionary<PokeInteractable, PokeInteractor.SurfaceHitCache.HitInfo>();
			}

			public void Reset(Vector3 origin)
			{
				this._origin = origin;
				this._surfacePatchHitCache.Clear();
				this._backingSurfaceHitCache.Clear();
			}

			private Dictionary<PokeInteractable, PokeInteractor.SurfaceHitCache.HitInfo> _surfacePatchHitCache;

			private Dictionary<PokeInteractable, PokeInteractor.SurfaceHitCache.HitInfo> _backingSurfaceHitCache;

			private Vector3 _origin;

			private readonly struct HitInfo
			{
				public HitInfo(bool isValid, SurfaceHit hit)
				{
					this.IsValid = isValid;
					this.Hit = hit;
				}

				public readonly bool IsValid;

				public readonly SurfaceHit Hit;
			}
		}

		private struct CachedInteractable
		{
			public PokeInteractable interactable;

			public SurfaceHit backingHit;

			public SurfaceHit patchHit;
		}
	}
}
