using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.XR.MRUtilityKit
{
	[HelpURL("https://developers.meta.com/horizon/reference/mruk/latest/class_meta_x_r_m_r_utility_kit_m_r_u_k_room")]
	[Feature(Feature.Scene)]
	public class MRUKRoom : MonoBehaviour
	{
		public OVRAnchor Anchor { get; internal set; } = OVRAnchor.Null;

		public bool IsLocal
		{
			get
			{
				return this.Anchor.Handle > 0UL;
			}
		}

		internal Pose InitialPose { get; set; } = Pose.identity;

		internal Pose DeltaPose
		{
			get
			{
				Quaternion rotation = base.transform.rotation * Quaternion.Inverse(this.InitialPose.rotation);
				return new Pose(base.transform.position - rotation * this.InitialPose.position, rotation);
			}
		}

		public List<MRUKAnchor> Anchors { get; } = new List<MRUKAnchor>();

		public List<MRUKAnchor> WallAnchors { get; } = new List<MRUKAnchor>();

		public MRUKAnchor FloorAnchor { get; internal set; }

		public MRUKAnchor CeilingAnchor { get; internal set; }

		public MRUKAnchor GlobalMeshAnchor { get; internal set; }

		public List<MRUKRoom.CouchSeat> SeatPoses { get; } = new List<MRUKRoom.CouchSeat>();

		public UnityEvent<MRUKAnchor> AnchorCreatedEvent { get; private set; } = new UnityEvent<MRUKAnchor>();

		public UnityEvent<MRUKAnchor> AnchorUpdatedEvent { get; private set; } = new UnityEvent<MRUKAnchor>();

		public UnityEvent<MRUKAnchor> AnchorRemovedEvent { get; private set; } = new UnityEvent<MRUKAnchor>();

		[Obsolete("Use UnityEvent AnchorCreatedEvent directly instead")]
		public void RegisterAnchorCreatedCallback(UnityAction<MRUKAnchor> callback)
		{
			this.AnchorCreatedEvent.AddListener(callback);
		}

		[Obsolete("Use UnityEvent AnchorUpdatedEvent directly instead")]
		public void RegisterAnchorUpdatedCallback(UnityAction<MRUKAnchor> callback)
		{
			this.AnchorUpdatedEvent.AddListener(callback);
		}

		[Obsolete("Use UnityEvent AnchorRemovedEvent directly instead")]
		public void RegisterAnchorRemovedCallback(UnityAction<MRUKAnchor> callback)
		{
			this.AnchorRemovedEvent.AddListener(callback);
		}

		[Obsolete("Use UnityEvent AnchorCreatedEvent directly instead")]
		public void UnRegisterAnchorCreatedCallback(UnityAction<MRUKAnchor> callback)
		{
			this.AnchorCreatedEvent.RemoveListener(callback);
		}

		[Obsolete("Use UnityEvent AnchorUpdatedEvent directly instead")]
		public void UnRegisterAnchorUpdatedCallback(UnityAction<MRUKAnchor> callback)
		{
			this.AnchorUpdatedEvent.RemoveListener(callback);
		}

		[Obsolete("Use UnityEvent AnchorRemovedEvent directly instead")]
		public void UnRegisterAnchorRemovedCallback(UnityAction<MRUKAnchor> callback)
		{
			this.AnchorRemovedEvent.RemoveListener(callback);
		}

		public OVRTask<OVRResult<OVRAnchor.ShareResult>> ShareRoomAsync(Guid groupUuid)
		{
			MRUKRoom.<ShareRoomAsync>d__56 <ShareRoomAsync>d__;
			<ShareRoomAsync>d__.<>t__builder = OVRTaskBuilder<OVRResult<OVRAnchor.ShareResult>>.Create();
			<ShareRoomAsync>d__.<>4__this = this;
			<ShareRoomAsync>d__.groupUuid = groupUuid;
			<ShareRoomAsync>d__.<>1__state = -1;
			<ShareRoomAsync>d__.<>t__builder.Start<MRUKRoom.<ShareRoomAsync>d__56>(ref <ShareRoomAsync>d__);
			return <ShareRoomAsync>d__.<>t__builder.Task;
		}

		internal MRUKAnchor FindAnchorByUuid(Guid uuid)
		{
			foreach (MRUKAnchor mrukanchor in this.Anchors)
			{
				if (mrukanchor.Anchor.Uuid == uuid)
				{
					return mrukanchor;
				}
			}
			return null;
		}

		internal void ComputeRoomInfo()
		{
			foreach (MRUKAnchor mrukanchor in this.Anchors)
			{
				if (mrukanchor.HasAnyLabel(MRUKAnchor.SceneLabels.GLOBAL_MESH))
				{
					this.GlobalMeshAnchor = mrukanchor;
					break;
				}
			}
			this.CalculateSeatPoses();
			this.CalculateHierarchyReferences();
		}

		[Obsolete("Use Anchors property instead")]
		public List<MRUKAnchor> GetRoomAnchors()
		{
			return this.Anchors;
		}

		public void RemoveAndDestroyAnchor(MRUKAnchor anchor)
		{
			this.Anchors.Remove(anchor);
			this.WallAnchors.Remove(anchor);
			if (this.CeilingAnchor == anchor)
			{
				this.CeilingAnchor = null;
			}
			if (this.FloorAnchor == anchor)
			{
				this.FloorAnchor = null;
			}
			Utilities.DestroyGameObjectAndChildren(anchor.gameObject);
		}

		[Obsolete("Use FloorAnchor property instead")]
		public MRUKAnchor GetFloorAnchor()
		{
			return this.FloorAnchor;
		}

		[Obsolete("Use CeilingAnchor property instead")]
		public MRUKAnchor GetCeilingAnchor()
		{
			return this.CeilingAnchor;
		}

		public MRUKAnchor GetGlobalMeshAnchor()
		{
			return this.GlobalMeshAnchor;
		}

		[Obsolete("Use WallAnchors property instead")]
		public List<MRUKAnchor> GetWallAnchors()
		{
			return this.WallAnchors;
		}

		private void CalculateSeatPoses()
		{
			this.SeatPoses.Clear();
			float seatWidth = MRUK.Instance.SceneSettings.SeatWidth;
			foreach (MRUKAnchor mrukanchor in this.Anchors)
			{
				if (mrukanchor.HasAnyLabel(MRUKAnchor.SceneLabels.COUCH))
				{
					MRUKRoom.CouchSeat item = new MRUKRoom.CouchSeat
					{
						couchAnchor = mrukanchor,
						couchPoses = new List<Pose>()
					};
					Rect? rect;
					Vector2 vector = (mrukanchor.PlaneRect != null) ? rect.GetValueOrDefault().size : Vector2.one;
					float num = vector.x / vector.y;
					Vector3 facingDirection = this.GetFacingDirection(mrukanchor);
					Vector3 up = Vector3.up;
					Vector3.OrthoNormalize(ref facingDirection, ref up);
					Quaternion quaternion = Quaternion.Inverse(mrukanchor.transform.rotation);
					if (num < 2f && num > 0.5f)
					{
						Pose item2 = new Pose(Vector3.zero, quaternion * Quaternion.LookRotation(facingDirection, up));
						item.couchPoses.Add(item2);
						this.SeatPoses.Add(item);
					}
					else
					{
						bool flag = vector.x > vector.y;
						float num2 = flag ? vector.x : vector.y;
						float num3 = Mathf.Floor(num2 / seatWidth);
						float d = (num2 - num3 * seatWidth) / num3;
						int num4 = 0;
						while ((float)num4 < num3)
						{
							Vector3 a = flag ? mrukanchor.transform.right : mrukanchor.transform.up;
							Vector3 vector2 = Vector3.zero;
							vector2 -= a * num2 * 0.5f;
							vector2 += a * d * 0.5f;
							vector2 += a * seatWidth * 0.5f;
							vector2 += a * seatWidth * (float)num4;
							vector2 += a * d * (float)num4;
							Pose item3 = new Pose(quaternion * vector2, quaternion * Quaternion.LookRotation(facingDirection, up));
							item.couchPoses.Add(item3);
							this.SeatPoses.Add(item);
							num4++;
						}
					}
				}
			}
		}

		public List<Vector3> GetRoomOutline()
		{
			this.CalculateRoomOutlineAndBounds();
			return this._corners;
		}

		public MRUKAnchor GetKeyWall(out Vector2 wallScale, float tolerance = 0.1f)
		{
			wallScale = Vector3.one;
			List<MRUKAnchor> list = new List<MRUKAnchor>(this.WallAnchors);
			MRUKAnchor result = null;
			list = MRUKRoom.SortWallsByWidth(list);
			List<Vector3> roomOutline = this.GetRoomOutline();
			for (int i = list.Count - 1; i >= 0; i--)
			{
				bool flag = true;
				for (int j = 0; j < roomOutline.Count; j++)
				{
					Vector3 vector = roomOutline[j] - list[i].transform.position;
					vector += list[i].transform.forward * tolerance;
					flag &= (Vector3.Dot(list[i].transform.forward, vector) >= 0f);
					if (!flag)
					{
						break;
					}
				}
				if (flag)
				{
					wallScale = list[i].PlaneRect.Value.size;
					result = list[i];
					break;
				}
			}
			return result;
		}

		public static List<MRUKAnchor> SortWallsByWidth(List<MRUKAnchor> walls)
		{
			List<MRUKAnchor> list = new List<MRUKAnchor>();
			for (int i = 0; i < walls.Count; i++)
			{
				for (int j = i + 1; j < walls.Count; j++)
				{
					if (walls[i].PlaneRect.Value.size.x > walls[j].PlaneRect.Value.size.x)
					{
						int index = i;
						int index2 = j;
						MRUKAnchor value = walls[j];
						MRUKAnchor value2 = walls[i];
						walls[index] = value;
						walls[index2] = value2;
					}
				}
			}
			list.AddRange(walls);
			return list;
		}

		public bool RaycastAll(Ray ray, float maxDist, LabelFilter labelFilter, List<RaycastHit> raycastHits, List<MRUKAnchor> anchorList)
		{
			raycastHits.Clear();
			anchorList.Clear();
			foreach (MRUKAnchor mrukanchor in this.Anchors)
			{
				RaycastHit item;
				if (labelFilter.PassesFilter(mrukanchor.Label) && mrukanchor.Raycast(ray, maxDist, out item, labelFilter.ComponentTypes ?? MRUKAnchor.ComponentType.All))
				{
					raycastHits.Add(item);
					anchorList.Add(mrukanchor);
				}
			}
			return raycastHits.Count > 0;
		}

		public bool Raycast(Ray ray, float maxDist, LabelFilter labelFilter, out RaycastHit hit, out MRUKAnchor outAnchor)
		{
			hit = default(RaycastHit);
			outAnchor = null;
			bool result = false;
			float maxDist2 = maxDist;
			foreach (MRUKAnchor mrukanchor in this.Anchors)
			{
				RaycastHit raycastHit;
				if (labelFilter.PassesFilter(mrukanchor.Label) && mrukanchor.Raycast(ray, maxDist2, out raycastHit, labelFilter.ComponentTypes ?? MRUKAnchor.ComponentType.All))
				{
					maxDist2 = raycastHit.distance;
					hit = raycastHit;
					outAnchor = mrukanchor;
					result = true;
				}
			}
			return result;
		}

		public bool Raycast(Ray ray, float maxDist, out RaycastHit hit, out MRUKAnchor anchor)
		{
			return this.Raycast(ray, maxDist, default(LabelFilter), out hit, out anchor);
		}

		public bool Raycast(Ray ray, float maxDist, LabelFilter labelFilter, out RaycastHit hit)
		{
			MRUKAnchor mrukanchor;
			return this.Raycast(ray, maxDist, labelFilter, out hit, out mrukanchor);
		}

		public bool Raycast(Ray ray, float maxDist, out RaycastHit hit)
		{
			MRUKAnchor mrukanchor;
			return this.Raycast(ray, maxDist, default(LabelFilter), out hit, out mrukanchor);
		}

		public Pose GetBestPoseFromRaycast(Ray ray, float maxDist, LabelFilter labelFilter, out MRUKAnchor sceneAnchor, out Vector3 surfaceNormal, MRUK.PositioningMethod positioningMethod = MRUK.PositioningMethod.DEFAULT)
		{
			sceneAnchor = null;
			Pose result = default(Pose);
			surfaceNormal = Vector3.up;
			RaycastHit raycastHit;
			if (this.Raycast(ray, maxDist, labelFilter, out raycastHit, out sceneAnchor))
			{
				Vector3 position = raycastHit.point;
				surfaceNormal = raycastHit.normal;
				Vector3 up = Vector3.up;
				Vector3 vector = raycastHit.normal;
				if (Vector3.Dot(raycastHit.normal, Vector3.up) >= 0.9f && sceneAnchor.VolumeBounds != null)
				{
					Vector3 vector2 = ray.origin - sceneAnchor.transform.position;
					Vector3 vector3 = (Vector3.Dot(sceneAnchor.transform.up, vector2) > 0f) ? sceneAnchor.transform.up : (-sceneAnchor.transform.up);
					Vector3 vector4 = (Vector3.Dot(sceneAnchor.transform.right, vector2) > 0f) ? sceneAnchor.transform.right : (-sceneAnchor.transform.right);
					Vector3 forward = sceneAnchor.transform.forward;
					Vector2 vector5 = sceneAnchor.VolumeBounds.Value.size;
					Vector3 vector6 = sceneAnchor.transform.position + vector4 * vector5.x * 0.5f + vector3 * vector5.y * 0.5f;
					Vector3.OrthoNormalize(ref forward, ref vector2);
					vector6 -= sceneAnchor.transform.position;
					bool flag = Vector3.Angle(vector2, vector3) > Vector3.Angle(vector6, vector3);
					vector = (flag ? vector4 : vector3);
					float d = flag ? vector5.x : vector5.y;
					switch (positioningMethod)
					{
					case MRUK.PositioningMethod.CENTER:
						position = sceneAnchor.transform.position;
						break;
					case MRUK.PositioningMethod.EDGE:
						position = sceneAnchor.transform.position + vector * d * 0.5f;
						break;
					}
				}
				else if (Mathf.Abs(Vector3.Dot(raycastHit.normal, Vector3.up)) >= 0.9f)
				{
					vector = new Vector3(ray.origin.x - raycastHit.point.x, 0f, ray.origin.z - raycastHit.point.z).normalized;
				}
				result.position = position;
				result.rotation = Quaternion.LookRotation(vector, up);
			}
			else
			{
				Debug.Log("Best pose not found, no surface anchor detected.");
			}
			return result;
		}

		public Pose GetBestPoseFromRaycast(Ray ray, float maxDist, LabelFilter labelFilter, out MRUKAnchor sceneAnchor, MRUK.PositioningMethod positioningMethod = MRUK.PositioningMethod.DEFAULT)
		{
			Vector3 vector;
			return this.GetBestPoseFromRaycast(ray, maxDist, labelFilter, out sceneAnchor, out vector, positioningMethod);
		}

		public bool IsPositionInRoom(Vector3 queryPosition, bool testVerticalBounds = true)
		{
			if (this.FloorAnchor == null)
			{
				return false;
			}
			bool flag = false;
			Vector3 v = this.FloorAnchor.transform.InverseTransformPoint(queryPosition);
			flag |= this.FloorAnchor.IsPositionInBoundary(v);
			if (testVerticalBounds)
			{
				Bounds roomBounds = this.GetRoomBounds();
				flag &= this.TestVerticalBounds(queryPosition, roomBounds);
			}
			return flag;
		}

		private bool TestVerticalBounds(Vector3 queryPosition, Bounds roomBounds)
		{
			return queryPosition.y <= roomBounds.max.y && queryPosition.y >= roomBounds.min.y;
		}

		public Bounds GetRoomBounds()
		{
			this.CalculateRoomOutlineAndBounds();
			return this._roomBounds;
		}

		private void CalculateRoomOutlineAndBounds()
		{
			if (!this.FloorAnchor || !this.CeilingAnchor)
			{
				Debug.LogWarning("Floor or Ceiling anchor not found");
				return;
			}
			Pose? prevRoomPose = this._prevRoomPose;
			if (prevRoomPose != null)
			{
				Pose valueOrDefault = prevRoomPose.GetValueOrDefault();
				if (base.transform.position == valueOrDefault.position && base.transform.rotation == valueOrDefault.rotation)
				{
					return;
				}
			}
			this._prevRoomPose = new Pose?(new Pose(base.transform.position, base.transform.rotation));
			float num = float.PositiveInfinity;
			float num2 = float.NegativeInfinity;
			float num3 = float.PositiveInfinity;
			float num4 = float.NegativeInfinity;
			float num5 = float.PositiveInfinity;
			float num6 = float.NegativeInfinity;
			this._corners.Clear();
			foreach (Vector2 vector in this.FloorAnchor.PlaneBoundary2D)
			{
				Vector3 vector2 = this.FloorAnchor.transform.TransformPoint(new Vector3(vector.x, vector.y, 0f));
				num = Mathf.Min(num, vector2.x);
				num2 = Mathf.Max(num2, vector2.x);
				num3 = Mathf.Min(num3, vector2.y);
				num4 = Mathf.Max(num4, vector2.y);
				num5 = Mathf.Min(num5, vector2.z);
				num6 = Mathf.Max(num6, vector2.z);
				this._corners.Add(vector2);
			}
			foreach (Vector2 vector3 in this.CeilingAnchor.PlaneBoundary2D)
			{
				Vector3 vector4 = this.CeilingAnchor.transform.TransformPoint(new Vector3(vector3.x, vector3.y, 0f));
				num = Mathf.Min(num, vector4.x);
				num2 = Mathf.Max(num2, vector4.x);
				num3 = Mathf.Min(num3, vector4.y);
				num4 = Mathf.Max(num4, vector4.y);
				num5 = Mathf.Min(num5, vector4.z);
				num6 = Mathf.Max(num6, vector4.z);
			}
			this._roomBounds.center = new Vector3((num2 + num) * 0.5f, (num4 + num3) * 0.5f, (num6 + num5) * 0.5f);
			this._roomBounds.size = new Vector3(num2 - num, num4 - num3, num6 - num5);
		}

		public bool IsPositionInSceneVolume(Vector3 worldPosition, out MRUKAnchor sceneObject, bool testVerticalBounds, float distanceBuffer = 0f)
		{
			bool result = false;
			sceneObject = null;
			foreach (MRUKAnchor mrukanchor in this.Anchors)
			{
				if (mrukanchor.IsPositionInVolume(worldPosition, testVerticalBounds, distanceBuffer))
				{
					result = true;
					sceneObject = mrukanchor;
					break;
				}
			}
			return result;
		}

		public Vector3 GetFacingDirection(MRUKAnchor anchor)
		{
			if (anchor.VolumeBounds == null)
			{
				return anchor.transform.forward;
			}
			int num;
			return this.GetDirectionAwayFromClosestWall(anchor, out num, null);
		}

		internal Vector3 GetDirectionAwayFromClosestWall(MRUKAnchor anchor, out int cardinalAxisIndex, List<int> excludedAxes = null)
		{
			float maxDist = float.PositiveInfinity;
			Vector3 result = anchor.transform.up;
			cardinalAxisIndex = 0;
			for (int i = 0; i < 4; i++)
			{
				if (excludedAxes == null || !excludedAxes.Contains(i))
				{
					Vector3 vector = Quaternion.Euler(0f, 90f * (float)i, 0f) * -anchor.transform.up;
					using (List<MRUKAnchor>.Enumerator enumerator = this.WallAnchors.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							RaycastHit raycastHit;
							if (enumerator.Current.Raycast(new Ray(anchor.transform.position, vector), maxDist, out raycastHit, MRUKAnchor.ComponentType.All))
							{
								maxDist = raycastHit.distance;
								cardinalAxisIndex = i;
								result = -vector;
							}
						}
					}
				}
			}
			return result;
		}

		public bool IsPositionInSceneVolume(Vector3 worldPosition, float distanceBuffer = 0f)
		{
			MRUKAnchor mrukanchor;
			return this.IsPositionInSceneVolume(worldPosition, out mrukanchor, true, distanceBuffer);
		}

		public bool IsPositionInSceneVolume(Vector3 worldPosition, bool testVerticalBounds, float distanceBuffer = 0f)
		{
			MRUKAnchor mrukanchor;
			return this.IsPositionInSceneVolume(worldPosition, out mrukanchor, testVerticalBounds, distanceBuffer);
		}

		public bool TryGetClosestSeatPose(Ray ray, out Pose seatPose, out MRUKAnchor couch)
		{
			Pose pose = default(Pose);
			couch = null;
			float num = -1f;
			for (int i = 0; i < this.SeatPoses.Count; i++)
			{
				Quaternion rotation = this.SeatPoses[i].couchAnchor.transform.rotation;
				Vector3 position = this.SeatPoses[i].couchAnchor.transform.position;
				for (int j = 0; j < this.SeatPoses[i].couchPoses.Count; j++)
				{
					Vector3 vector = position + rotation * this.SeatPoses[i].couchPoses[j].position;
					Vector3 normalized = (vector - ray.origin).normalized;
					float num2 = Vector3.Dot(ray.direction, normalized);
					if (num2 > num)
					{
						num = num2;
						pose.position = vector;
						pose.rotation = rotation * this.SeatPoses[i].couchPoses[j].rotation;
						couch = this.SeatPoses[i].couchAnchor;
					}
				}
			}
			seatPose.position = pose.position;
			seatPose.rotation = pose.rotation;
			return this.SeatPoses.Count > 0;
		}

		public Pose[] GetSeatPoses()
		{
			List<Pose> list = new List<Pose>();
			for (int i = 0; i < this.SeatPoses.Count; i++)
			{
				Quaternion rotation = this.SeatPoses[i].couchAnchor.transform.rotation;
				Vector3 position = this.SeatPoses[i].couchAnchor.transform.position;
				for (int j = 0; j < this.SeatPoses[i].couchPoses.Count; j++)
				{
					Pose item = new Pose(position + rotation * this.SeatPoses[i].couchPoses[j].position, rotation * this.SeatPoses[i].couchPoses[j].rotation);
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		[Obsolete("Use ParentAnchor property instead")]
		public bool TryGetAnchorParent(MRUKAnchor queryAnchor, out MRUKAnchor parentAnchor)
		{
			parentAnchor = queryAnchor.ParentAnchor;
			return parentAnchor != null;
		}

		[Obsolete("Use ChildAnchors property instead")]
		public bool TryGetAnchorChildren(MRUKAnchor queryAnchor, out MRUKAnchor[] childAnchors)
		{
			List<MRUKAnchor> childAnchors2 = queryAnchor.ChildAnchors;
			childAnchors = ((childAnchors2 != null) ? childAnchors2.ToArray() : null);
			return childAnchors != null && childAnchors.Length != 0;
		}

		private void CalculateHierarchyReferences()
		{
			for (int i = 0; i < this.Anchors.Count; i++)
			{
				this.Anchors[i].ClearChildReferences();
				this.Anchors[i].ParentAnchor = null;
			}
			for (int j = 0; j < this.Anchors.Count; j++)
			{
				if (this.Anchors[j].HasAnyLabel(MRUKAnchor.SceneLabels.WALL_FACE) && this.Anchors[j].PlaneRect != null)
				{
					for (int k = 0; k < this.Anchors.Count; k++)
					{
						if (!(this.Anchors[k] == this.Anchors[j]) && this.Anchors[k].PlaneRect != null && this.Anchors[k].VolumeBounds == null)
						{
							bool flag = Vector3.Angle(this.Anchors[k].transform.right, this.Anchors[j].transform.right) <= 5f;
							Vector3 vector = this.Anchors[j].transform.InverseTransformPoint(this.Anchors[k].transform.position);
							bool flag2 = Mathf.Abs(vector.z) <= 0.1f;
							bool flag3 = vector.x <= this.Anchors[j].PlaneRect.Value.max.x && vector.x >= this.Anchors[j].PlaneRect.Value.min.x;
							if (flag && flag2 && flag3)
							{
								this.Anchors[j].AddChildReference(this.Anchors[k]);
								this.Anchors[k].ParentAnchor = this.Anchors[j];
							}
						}
					}
				}
				else if (this.Anchors[j].HasAnyLabel(MRUKAnchor.SceneLabels.FLOOR))
				{
					for (int l = 0; l < this.Anchors.Count; l++)
					{
						if (this.Anchors[l].VolumeBounds != null && (this.Anchors[l].transform.position + Vector3.up * this.Anchors[l].VolumeBounds.Value.min.z).y - this.Anchors[j].transform.position.y <= 0.1f)
						{
							this.Anchors[j].AddChildReference(this.Anchors[l]);
							this.Anchors[l].ParentAnchor = this.Anchors[j];
						}
					}
				}
				else if (this.Anchors[j].VolumeBounds != null)
				{
					Bounds value = this.Anchors[j].VolumeBounds.Value;
					for (int m = 0; m < this.Anchors.Count; m++)
					{
						if (!(this.Anchors[m] == this.Anchors[j]) && this.Anchors[m].VolumeBounds != null)
						{
							Bounds value2 = this.Anchors[m].VolumeBounds.Value;
							ref Vector3 ptr = this.Anchors[m].transform.position + Vector3.up * this.Anchors[m].VolumeBounds.Value.min.z;
							Vector3 vector2 = this.Anchors[j].transform.position + Vector3.up * this.Anchors[j].VolumeBounds.Value.max.z;
							if (Mathf.Abs(ptr.y - vector2.y) <= 0.1f)
							{
								bool flag4 = false;
								for (int n = 0; n < 4; n++)
								{
									Vector3 position = new Vector3((n < 2) ? value2.min.x : value2.max.x, (n % 2 == 0) ? value2.min.y : value2.max.y, 0f);
									position = this.Anchors[m].transform.TransformPoint(position);
									Vector3 vector3 = this.Anchors[j].transform.InverseTransformPoint(position);
									bool flag5 = 0.001f + (vector3.x - value.min.x) >= 0f;
									bool flag6 = 0.001f + (value.max.x - vector3.x) >= 0f;
									bool flag7 = 0.001f + (vector3.y - value.min.y) >= 0f;
									bool flag8 = 0.001f + (value.max.y - vector3.y) >= 0f;
									if (flag5 && flag6 && flag7 && flag8)
									{
										flag4 = true;
										break;
									}
								}
								if (flag4 && (!(this.Anchors[m].ParentAnchor != null) || !this.Anchors[m].ParentAnchor.HasAnyLabel(MRUKAnchor.SceneLabels.FLOOR)))
								{
									this.Anchors[j].AddChildReference(this.Anchors[m]);
									this.Anchors[m].ParentAnchor = this.Anchors[j];
								}
							}
						}
					}
				}
			}
		}

		[Obsolete("Use 'HasAllLabels()' instead.")]
		public bool DoesRoomHave(string[] labels)
		{
			return this.HasAllLabels(Utilities.StringLabelsToEnum(labels));
		}

		public bool HasAllLabels(MRUKAnchor.SceneLabels labelFlags)
		{
			foreach (MRUKAnchor mrukanchor in this.Anchors)
			{
				labelFlags &= ~mrukanchor.Label;
				if (labelFlags == (MRUKAnchor.SceneLabels)0)
				{
					return true;
				}
			}
			return false;
		}

		public float TryGetClosestSurfacePosition(Vector3 worldPosition, out Vector3 surfacePosition, out MRUKAnchor closestAnchor, LabelFilter labelFilter = default(LabelFilter))
		{
			Vector3 vector;
			return this.TryGetClosestSurfacePosition(worldPosition, out surfacePosition, out closestAnchor, out vector, labelFilter);
		}

		public float TryGetClosestSurfacePosition(Vector3 worldPosition, out Vector3 surfacePosition, out MRUKAnchor closestAnchor, out Vector3 normal, LabelFilter labelFilter = default(LabelFilter))
		{
			float num = float.PositiveInfinity;
			surfacePosition = Vector3.zero;
			closestAnchor = null;
			normal = Vector3.zero;
			foreach (MRUKAnchor mrukanchor in this.Anchors)
			{
				if (labelFilter.PassesFilter(mrukanchor.Label))
				{
					Vector3 vector;
					Vector3 vector2;
					float closestSurfacePosition = mrukanchor.GetClosestSurfacePosition(worldPosition, out vector, out vector2, labelFilter.ComponentTypes ?? MRUKAnchor.ComponentType.All);
					if (closestSurfacePosition < num)
					{
						num = closestSurfacePosition;
						surfacePosition = vector;
						normal = vector2;
						closestAnchor = mrukanchor.GetComponent<MRUKAnchor>();
					}
				}
			}
			return num;
		}

		[Obsolete("String-based labels are deprecated (v65). Please use the equivalent enum-based methods.")]
		public MRUKAnchor FindLargestSurface(string anchorLabel)
		{
			return this.FindLargestSurface(Utilities.StringLabelToEnum(anchorLabel));
		}

		public MRUKAnchor FindLargestSurface(MRUKAnchor.SceneLabels labelFlags)
		{
			MRUKAnchor result = null;
			float num = 0f;
			foreach (MRUKAnchor mrukanchor in this.Anchors)
			{
				if (mrukanchor.HasAnyLabel(labelFlags))
				{
					float num2 = 0f;
					if (mrukanchor.PlaneRect != null)
					{
						Vector2 size = mrukanchor.PlaneRect.Value.size;
						num2 = size.x * size.y;
					}
					else if (mrukanchor.VolumeBounds != null)
					{
						Vector3 size2 = mrukanchor.VolumeBounds.Value.size;
						num2 = size2.x * size2.y;
					}
					if (num2 > num)
					{
						num = num2;
						result = mrukanchor;
					}
				}
			}
			return result;
		}

		public Vector3? GenerateRandomPositionInRoom(float minDistanceToSurface, bool avoidVolumes)
		{
			if (!this.FloorAnchor)
			{
				return null;
			}
			Vector3 extents = this.GetRoomBounds().extents;
			float num = Mathf.Min(new float[]
			{
				extents.x,
				extents.y,
				extents.z
			});
			if (minDistanceToSurface > num)
			{
				return null;
			}
			for (int i = 0; i < 1000; i++)
			{
				Vector3 vector = new Vector3(Random.Range(this._roomBounds.min.x + minDistanceToSurface, this._roomBounds.max.x - minDistanceToSurface), Random.Range(this._roomBounds.min.y + minDistanceToSurface, this._roomBounds.max.y - minDistanceToSurface), Random.Range(this._roomBounds.min.z + minDistanceToSurface, this._roomBounds.max.z - minDistanceToSurface));
				if (this.IsPositionInRoom(vector, true))
				{
					LabelFilter labelFilter = new LabelFilter(new MRUKAnchor.SceneLabels?(MRUKAnchor.SceneLabels.WALL_FACE), null);
					Vector3 vector2;
					MRUKAnchor mrukanchor;
					if (this.TryGetClosestSurfacePosition(vector, out vector2, out mrukanchor, labelFilter) > minDistanceToSurface && (!avoidVolumes || !this.IsPositionInSceneVolume(vector, minDistanceToSurface)))
					{
						return new Vector3?(vector);
					}
				}
			}
			return null;
		}

		public bool GenerateRandomPositionOnSurface(MRUK.SurfaceType surfaceTypes, float minDistanceToEdge, LabelFilter labelFilter, out Vector3 position, out Vector3 normal)
		{
			List<MRUKRoom.Surface> list = new List<MRUKRoom.Surface>();
			float num = 0f;
			float num2 = 2f * minDistanceToEdge;
			position = Vector3.zero;
			normal = Vector3.zero;
			foreach (MRUKAnchor mrukanchor in this.Anchors)
			{
				if (labelFilter.PassesFilter(mrukanchor.Label))
				{
					if (mrukanchor.PlaneRect != null)
					{
						bool flag = false;
						if (mrukanchor.transform.forward.y >= Utilities.InvSqrt2)
						{
							if ((surfaceTypes & MRUK.SurfaceType.FACING_UP) == (MRUK.SurfaceType)0)
							{
								flag = true;
							}
						}
						else if (mrukanchor.transform.forward.y <= -Utilities.InvSqrt2)
						{
							if ((surfaceTypes & MRUK.SurfaceType.FACING_DOWN) == (MRUK.SurfaceType)0)
							{
								flag = true;
							}
						}
						else if ((surfaceTypes & MRUK.SurfaceType.VERTICAL) == (MRUK.SurfaceType)0)
						{
							flag = true;
						}
						if (!flag)
						{
							Vector2 size = mrukanchor.PlaneRect.Value.size;
							if (size.x > num2 && size.y > num2)
							{
								float num3 = (size.x - num2) * (size.y - num2);
								num += num3;
								list.Add(new MRUKRoom.Surface
								{
									Anchor = mrukanchor,
									UsableArea = num3,
									IsPlane = true,
									Bounds = mrukanchor.PlaneRect.Value,
									Transform = mrukanchor.transform.localToWorldMatrix
								});
							}
						}
					}
					if (mrukanchor.VolumeBounds != null)
					{
						int i = 0;
						while (i < 6)
						{
							if (i == 0)
							{
								if ((surfaceTypes & MRUK.SurfaceType.FACING_UP) != (MRUK.SurfaceType)0)
								{
									goto IL_1A4;
								}
							}
							else if (i == 1)
							{
								if ((surfaceTypes & MRUK.SurfaceType.FACING_DOWN) != (MRUK.SurfaceType)0)
								{
									goto IL_1A4;
								}
							}
							else if ((surfaceTypes & MRUK.SurfaceType.VERTICAL) != (MRUK.SurfaceType)0)
							{
								goto IL_1A4;
							}
							IL_7F2:
							i++;
							continue;
							IL_1A4:
							Rect bounds;
							Matrix4x4 rhs;
							switch (i)
							{
							case 0:
								bounds = new Rect
								{
									xMin = mrukanchor.VolumeBounds.Value.min.x,
									xMax = mrukanchor.VolumeBounds.Value.max.x,
									yMin = mrukanchor.VolumeBounds.Value.min.y,
									yMax = mrukanchor.VolumeBounds.Value.max.y
								};
								rhs = Matrix4x4.TRS(new Vector3(0f, 0f, mrukanchor.VolumeBounds.Value.max.z), Quaternion.identity, Vector3.one);
								break;
							case 1:
								bounds = new Rect
								{
									xMin = -mrukanchor.VolumeBounds.Value.max.x,
									xMax = -mrukanchor.VolumeBounds.Value.min.x,
									yMin = mrukanchor.VolumeBounds.Value.min.y,
									yMax = mrukanchor.VolumeBounds.Value.max.y
								};
								rhs = Matrix4x4.TRS(new Vector3(0f, 0f, mrukanchor.VolumeBounds.Value.min.z), Quaternion.Euler(0f, 180f, 0f), Vector3.one);
								break;
							case 2:
								bounds = new Rect
								{
									xMin = -mrukanchor.VolumeBounds.Value.max.z,
									xMax = -mrukanchor.VolumeBounds.Value.min.z,
									yMin = mrukanchor.VolumeBounds.Value.min.y,
									yMax = mrukanchor.VolumeBounds.Value.max.y
								};
								rhs = Matrix4x4.TRS(new Vector3(mrukanchor.VolumeBounds.Value.max.x, 0f, 0f), Quaternion.Euler(0f, 90f, 0f), Vector3.one);
								break;
							case 3:
								bounds = new Rect
								{
									xMin = mrukanchor.VolumeBounds.Value.min.z,
									xMax = mrukanchor.VolumeBounds.Value.max.z,
									yMin = mrukanchor.VolumeBounds.Value.min.y,
									yMax = mrukanchor.VolumeBounds.Value.max.y
								};
								rhs = Matrix4x4.TRS(new Vector3(mrukanchor.VolumeBounds.Value.min.x, 0f, 0f), Quaternion.Euler(0f, -90f, 0f), Vector3.one);
								break;
							case 4:
								bounds = new Rect
								{
									xMin = mrukanchor.VolumeBounds.Value.min.x,
									xMax = mrukanchor.VolumeBounds.Value.max.x,
									yMin = -mrukanchor.VolumeBounds.Value.max.z,
									yMax = -mrukanchor.VolumeBounds.Value.min.z
								};
								rhs = Matrix4x4.TRS(new Vector3(0f, mrukanchor.VolumeBounds.Value.max.y, 0f), Quaternion.Euler(-90f, 0f, 0f), Vector3.one);
								break;
							case 5:
								bounds = new Rect
								{
									xMin = mrukanchor.VolumeBounds.Value.min.x,
									xMax = mrukanchor.VolumeBounds.Value.max.x,
									yMin = mrukanchor.VolumeBounds.Value.min.z,
									yMax = mrukanchor.VolumeBounds.Value.max.z
								};
								rhs = Matrix4x4.TRS(new Vector3(0f, mrukanchor.VolumeBounds.Value.min.y, 0f), Quaternion.Euler(90f, 0f, 0f), Vector3.one);
								break;
							default:
								throw new SwitchExpressionException();
							}
							Vector2 size2 = bounds.size;
							if (size2.x > num2 && size2.y > num2)
							{
								float num4 = (size2.x - num2) * (size2.y - num2);
								num += num4;
								list.Add(new MRUKRoom.Surface
								{
									Anchor = mrukanchor,
									UsableArea = num4,
									IsPlane = false,
									Bounds = bounds,
									Transform = mrukanchor.transform.localToWorldMatrix * rhs
								});
								goto IL_7F2;
							}
							goto IL_7F2;
						}
					}
				}
			}
			if (list.Count == 0)
			{
				return false;
			}
			for (int j = 0; j < 1000; j++)
			{
				float num5 = Random.Range(0f, num);
				int k;
				for (k = 0; k < list.Count - 1; k++)
				{
					num5 -= list[k].UsableArea;
					if (num5 <= 0f)
					{
						break;
					}
				}
				MRUKRoom.Surface surface = list[k];
				Rect bounds2 = surface.Bounds;
				Vector2 vector = new Vector2(Random.Range(bounds2.xMin + minDistanceToEdge, bounds2.xMax - minDistanceToEdge), Random.Range(bounds2.yMin + minDistanceToEdge, bounds2.yMax - minDistanceToEdge));
				if (!surface.IsPlane || surface.Anchor.IsPositionInBoundary(vector))
				{
					position = surface.Transform.MultiplyPoint3x4(new Vector3(vector.x, vector.y, 0f));
					normal = surface.Transform.MultiplyVector(Vector3.forward);
					return true;
				}
			}
			return false;
		}

		private void OnDestroy()
		{
			MRUK instance = MRUK.Instance;
			if (instance != null)
			{
				instance.OnRoomDestroyed(this);
			}
			this.AnchorCreatedEvent.RemoveAllListeners();
			this.AnchorRemovedEvent.RemoveAllListeners();
			this.AnchorUpdatedEvent.RemoveAllListeners();
		}

		private Bounds _roomBounds;

		private List<Vector3> _corners = new List<Vector3>();

		private Pose? _prevRoomPose;

		public struct CouchSeat
		{
			public MRUKAnchor couchAnchor { readonly get; internal set; }

			public List<Pose> couchPoses { readonly get; internal set; }
		}

		private struct Surface
		{
			public MRUKAnchor Anchor;

			public float UsableArea;

			public bool IsPlane;

			public Rect Bounds;

			public Matrix4x4 Transform;
		}
	}
}
