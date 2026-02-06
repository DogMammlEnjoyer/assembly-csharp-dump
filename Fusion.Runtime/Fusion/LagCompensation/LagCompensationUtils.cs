using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion.LagCompensation
{
	internal static class LagCompensationUtils
	{
		internal static bool NarrowBoxBox(ref LagCompensationUtils.BoxNarrowData aNarrow, ref LagCompensationUtils.BoxNarrowData bNarrow, bool detailedManifold, out Vector3 hitPoint, out Vector3 normal)
		{
			hitPoint = default(Vector3);
			normal = default(Vector3);
			Vector3 extents = aNarrow.Extents;
			Vector3 extents2 = bNarrow.Extents;
			LagCompensationUtils.RotationMatrix rotationMatrix;
			rotationMatrix.M00 = Vector3.Dot(aNarrow.RotatedRight, bNarrow.RotatedRight);
			rotationMatrix.M01 = Vector3.Dot(aNarrow.RotatedRight, bNarrow.RotatedUp);
			rotationMatrix.M02 = Vector3.Dot(aNarrow.RotatedRight, bNarrow.RotatedForward);
			rotationMatrix.M10 = Vector3.Dot(aNarrow.RotatedUp, bNarrow.RotatedRight);
			rotationMatrix.M11 = Vector3.Dot(aNarrow.RotatedUp, bNarrow.RotatedUp);
			rotationMatrix.M12 = Vector3.Dot(aNarrow.RotatedUp, bNarrow.RotatedForward);
			rotationMatrix.M20 = Vector3.Dot(aNarrow.RotatedForward, bNarrow.RotatedRight);
			rotationMatrix.M21 = Vector3.Dot(aNarrow.RotatedForward, bNarrow.RotatedUp);
			rotationMatrix.M22 = Vector3.Dot(aNarrow.RotatedForward, bNarrow.RotatedForward);
			LagCompensationUtils.RotationMatrix rotationMatrix2;
			rotationMatrix2.M00 = Mathf.Abs(rotationMatrix.M00) + 0.0001f;
			rotationMatrix2.M01 = Mathf.Abs(rotationMatrix.M01) + 0.0001f;
			rotationMatrix2.M02 = Mathf.Abs(rotationMatrix.M02) + 0.0001f;
			rotationMatrix2.M10 = Mathf.Abs(rotationMatrix.M10) + 0.0001f;
			rotationMatrix2.M11 = Mathf.Abs(rotationMatrix.M11) + 0.0001f;
			rotationMatrix2.M12 = Mathf.Abs(rotationMatrix.M12) + 0.0001f;
			rotationMatrix2.M20 = Mathf.Abs(rotationMatrix.M20) + 0.0001f;
			rotationMatrix2.M21 = Mathf.Abs(rotationMatrix.M21) + 0.0001f;
			rotationMatrix2.M22 = Mathf.Abs(rotationMatrix.M22) + 0.0001f;
			float num = float.MaxValue;
			Vector3 lhs = bNarrow.Position - aNarrow.Position;
			Vector3 vector = new Vector3(Vector3.Dot(lhs, aNarrow.RotatedRight), Vector3.Dot(lhs, aNarrow.RotatedUp), Vector3.Dot(lhs, aNarrow.RotatedForward));
			float num2 = extents.x;
			float num3 = extents2.x * rotationMatrix2.M00 + extents2.y * rotationMatrix2.M01 + extents2.z * rotationMatrix2.M02;
			float num4 = num2 + num3 - Mathf.Abs(vector.x);
			bool flag = num4 <= 0f;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = detailedManifold && num4 < num;
				if (flag2)
				{
					num = num4;
					normal = aNarrow.RotatedRight;
				}
				num2 = extents.y;
				num3 = extents2.x * rotationMatrix2.M10 + extents2.y * rotationMatrix2.M11 + extents2.z * rotationMatrix2.M12;
				num4 = num2 + num3 - Mathf.Abs(vector.y);
				bool flag3 = num4 <= 0f;
				if (flag3)
				{
					result = false;
				}
				else
				{
					bool flag4 = detailedManifold && num4 < num;
					if (flag4)
					{
						num = num4;
						normal = aNarrow.RotatedUp;
					}
					num2 = extents.z;
					num3 = extents2.x * rotationMatrix2.M20 + extents2.y * rotationMatrix2.M21 + extents2.z * rotationMatrix2.M22;
					num4 = num2 + num3 - Mathf.Abs(vector.z);
					bool flag5 = num4 <= 0f;
					if (flag5)
					{
						result = false;
					}
					else
					{
						bool flag6 = detailedManifold && num4 < num;
						if (flag6)
						{
							num = num4;
							normal = aNarrow.RotatedForward;
						}
						num2 = extents.x * rotationMatrix2.M00 + extents.y * rotationMatrix2.M10 + extents.z * rotationMatrix2.M20;
						num3 = extents2.x;
						num4 = num2 + num3 - Mathf.Abs(vector.x * rotationMatrix.M00 + vector.y * rotationMatrix.M10 + vector.z * rotationMatrix.M20);
						bool flag7 = num4 <= 0f;
						if (flag7)
						{
							result = false;
						}
						else
						{
							bool flag8 = detailedManifold && num4 < num;
							if (flag8)
							{
								num = num4;
								normal = bNarrow.RotatedRight;
							}
							num2 = extents.x * rotationMatrix2.M01 + extents.y * rotationMatrix2.M11 + extents.z * rotationMatrix2.M21;
							num3 = extents2.y;
							num4 = num2 + num3 - Mathf.Abs(vector.x * rotationMatrix.M01 + vector.y * rotationMatrix.M11 + vector.z * rotationMatrix.M21);
							bool flag9 = num4 <= 0f;
							if (flag9)
							{
								result = false;
							}
							else
							{
								bool flag10 = detailedManifold && num4 < num;
								if (flag10)
								{
									num = num4;
									normal = bNarrow.RotatedUp;
								}
								num2 = extents.x * rotationMatrix2.M02 + extents.y * rotationMatrix2.M12 + extents.z * rotationMatrix2.M22;
								num3 = extents2.z;
								num4 = num2 + num3 - Mathf.Abs(vector.x * rotationMatrix.M02 + vector.y * rotationMatrix.M12 + vector.z * rotationMatrix.M22);
								bool flag11 = num4 <= 0f;
								if (flag11)
								{
									result = false;
								}
								else
								{
									bool flag12 = detailedManifold && num4 < num;
									if (flag12)
									{
										num = num4;
										normal = bNarrow.RotatedForward;
									}
									bool flag13 = rotationMatrix2.M00 < 0.975f;
									if (flag13)
									{
										num2 = extents.y * rotationMatrix2.M20 + extents.z * rotationMatrix2.M10;
										num3 = extents2.y * rotationMatrix2.M02 + extents2.z * rotationMatrix2.M01;
										num4 = num2 + num3 - Mathf.Abs(vector.z * rotationMatrix.M10 - vector.y * rotationMatrix.M20);
										bool flag14 = num4 <= 0f;
										if (flag14)
										{
											return false;
										}
										bool flag15 = detailedManifold && num4 < num;
										if (flag15)
										{
											Vector3 vector2 = Vector3.Cross(aNarrow.RotatedRight, bNarrow.RotatedRight);
											bool flag16 = vector2.sqrMagnitude > 0.0001f;
											if (flag16)
											{
												normal = vector2;
												num = num4;
											}
										}
									}
									bool flag17 = rotationMatrix2.M01 < 0.975f;
									if (flag17)
									{
										num2 = extents.y * rotationMatrix2.M21 + extents.z * rotationMatrix2.M11;
										num3 = extents2.x * rotationMatrix2.M02 + extents2.z * rotationMatrix2.M00;
										num4 = num2 + num3 - Mathf.Abs(vector.z * rotationMatrix.M11 - vector.y * rotationMatrix.M21);
										bool flag18 = num4 <= 0f;
										if (flag18)
										{
											return false;
										}
										bool flag19 = detailedManifold && num4 < num;
										if (flag19)
										{
											Vector3 vector3 = Vector3.Cross(aNarrow.RotatedRight, bNarrow.RotatedUp);
											bool flag20 = vector3.sqrMagnitude > 0.0001f;
											if (flag20)
											{
												normal = vector3;
												num = num4;
											}
										}
									}
									bool flag21 = rotationMatrix2.M02 < 0.975f;
									if (flag21)
									{
										num2 = extents.y * rotationMatrix2.M22 + extents.z * rotationMatrix2.M12;
										num3 = extents2.x * rotationMatrix2.M01 + extents2.y * rotationMatrix2.M00;
										num4 = num2 + num3 - Mathf.Abs(vector.z * rotationMatrix.M12 - vector.y * rotationMatrix.M22);
										bool flag22 = num4 <= 0f;
										if (flag22)
										{
											return false;
										}
										bool flag23 = detailedManifold && num4 < num;
										if (flag23)
										{
											Vector3 vector4 = Vector3.Cross(aNarrow.RotatedRight, bNarrow.RotatedForward);
											bool flag24 = vector4.sqrMagnitude > 0.0001f;
											if (flag24)
											{
												normal = vector4;
												num = num4;
											}
										}
									}
									bool flag25 = rotationMatrix2.M10 < 0.975f;
									if (flag25)
									{
										num2 = extents.x * rotationMatrix2.M20 + extents.z * rotationMatrix2.M00;
										num3 = extents2.y * rotationMatrix2.M12 + extents2.z * rotationMatrix2.M11;
										num4 = num2 + num3 - Mathf.Abs(vector.x * rotationMatrix.M20 - vector.z * rotationMatrix.M00);
										bool flag26 = num4 <= 0f;
										if (flag26)
										{
											return false;
										}
										bool flag27 = detailedManifold && num4 < num;
										if (flag27)
										{
											Vector3 vector5 = Vector3.Cross(aNarrow.RotatedUp, bNarrow.RotatedRight);
											bool flag28 = vector5.sqrMagnitude > 0.0001f;
											if (flag28)
											{
												normal = vector5;
												num = num4;
											}
										}
									}
									bool flag29 = rotationMatrix2.M11 < 0.975f;
									if (flag29)
									{
										num2 = extents.x * rotationMatrix2.M21 + extents.z * rotationMatrix2.M01;
										num3 = extents2.x * rotationMatrix2.M12 + extents2.z * rotationMatrix2.M10;
										num4 = num2 + num3 - Mathf.Abs(vector.x * rotationMatrix.M21 - vector.z * rotationMatrix.M01);
										bool flag30 = num4 <= 0f;
										if (flag30)
										{
											return false;
										}
										bool flag31 = detailedManifold && num4 < num;
										if (flag31)
										{
											Vector3 vector6 = Vector3.Cross(aNarrow.RotatedUp, bNarrow.RotatedUp);
											bool flag32 = vector6.sqrMagnitude > 0.0001f;
											if (flag32)
											{
												normal = vector6;
												num = num4;
											}
										}
									}
									bool flag33 = rotationMatrix2.M12 < 0.975f;
									if (flag33)
									{
										num2 = extents.x * rotationMatrix2.M22 + extents.z * rotationMatrix2.M02;
										num3 = extents2.x * rotationMatrix2.M11 + extents2.y * rotationMatrix2.M10;
										num4 = num2 + num3 - Mathf.Abs(vector.x * rotationMatrix.M22 - vector.z * rotationMatrix.M02);
										bool flag34 = num4 <= 0f;
										if (flag34)
										{
											return false;
										}
										bool flag35 = detailedManifold && num4 < num;
										if (flag35)
										{
											Vector3 vector7 = Vector3.Cross(aNarrow.RotatedUp, bNarrow.RotatedForward);
											bool flag36 = vector7.sqrMagnitude > 0.0001f;
											if (flag36)
											{
												normal = vector7;
												num = num4;
											}
										}
									}
									bool flag37 = rotationMatrix2.M20 < 0.975f;
									if (flag37)
									{
										num2 = extents.x * rotationMatrix2.M10 + extents.y * rotationMatrix2.M00;
										num3 = extents2.y * rotationMatrix2.M22 + extents2.z * rotationMatrix2.M21;
										num4 = num2 + num3 - Mathf.Abs(vector.y * rotationMatrix.M00 - vector.x * rotationMatrix.M10);
										bool flag38 = num4 <= 0f;
										if (flag38)
										{
											return false;
										}
										bool flag39 = detailedManifold && num4 < num;
										if (flag39)
										{
											Vector3 vector8 = Vector3.Cross(aNarrow.RotatedForward, bNarrow.RotatedRight);
											bool flag40 = vector8.sqrMagnitude > 0.0001f;
											if (flag40)
											{
												normal = vector8;
												num = num4;
											}
										}
									}
									bool flag41 = rotationMatrix2.M21 < 0.975f;
									if (flag41)
									{
										num2 = extents.x * rotationMatrix2.M11 + extents.y * rotationMatrix2.M01;
										num3 = extents2.x * rotationMatrix2.M22 + extents2.z * rotationMatrix2.M20;
										num4 = num2 + num3 - Mathf.Abs(vector.y * rotationMatrix.M01 - vector.x * rotationMatrix.M11);
										bool flag42 = num4 <= 0f;
										if (flag42)
										{
											return false;
										}
										bool flag43 = detailedManifold && num4 < num;
										if (flag43)
										{
											Vector3 vector9 = Vector3.Cross(aNarrow.RotatedForward, bNarrow.RotatedUp);
											bool flag44 = vector9.sqrMagnitude > 0.0001f;
											if (flag44)
											{
												normal = vector9;
												num = num4;
											}
										}
									}
									bool flag45 = rotationMatrix2.M22 < 0.975f;
									if (flag45)
									{
										num2 = extents.x * rotationMatrix2.M12 + extents.y * rotationMatrix2.M02;
										num3 = extents2.x * rotationMatrix2.M21 + extents2.y * rotationMatrix2.M20;
										num4 = num2 + num3 - Mathf.Abs(vector.y * rotationMatrix.M02 - vector.x * rotationMatrix.M12);
										bool flag46 = num4 <= 0f;
										if (flag46)
										{
											return false;
										}
										bool flag47 = detailedManifold && num4 < num;
										if (flag47)
										{
											Vector3 vector10 = Vector3.Cross(aNarrow.RotatedForward, bNarrow.RotatedForward);
											bool flag48 = vector10.sqrMagnitude > 0.0001f;
											if (flag48)
											{
												normal = vector10;
											}
										}
									}
									LagCompensationUtils.CustomPlanesBox planesBox = LagCompensationUtils.GetPlanesBox(bNarrow.BoxPlanesRotated, ref lhs);
									LagCompensationUtils.CustomEdgesBox edgesBox = LagCompensationUtils.GetEdgesBox(bNarrow.BoxEdgesRotated, ref lhs);
									bool hitPoint2 = LagCompensationUtils.GetHitPoint(ref aNarrow.BoxPlanesRotated, ref planesBox, ref aNarrow.BoxEdgesRotated, ref edgesBox, ref aNarrow, ref bNarrow, ref lhs, detailedManifold, out hitPoint);
									if (hitPoint2)
									{
										if (detailedManifold)
										{
											bool flag49 = Vector3.Dot(lhs, normal) < 0f;
											if (flag49)
											{
												normal = -normal;
											}
											normal = normal.normalized;
										}
										result = true;
									}
									else
									{
										result = false;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		private static LagCompensationUtils.CustomEdgesBox GetEdgesBox(LagCompensationUtils.CustomEdgesBox edges, ref Vector3 translation)
		{
			edges.P0 += translation;
			edges.P1 += translation;
			edges.P2 += translation;
			edges.P3 += translation;
			edges.P4 += translation;
			edges.P5 += translation;
			edges.P6 += translation;
			edges.P7 += translation;
			return edges;
		}

		private static LagCompensationUtils.CustomPlanesBox GetPlanesBox(LagCompensationUtils.CustomPlanesBox planes, ref Vector3 translation)
		{
			planes.P0.PointOnPlane = planes.P0.PointOnPlane + translation;
			planes.P2.PointOnPlane = planes.P2.PointOnPlane + translation;
			planes.P4.PointOnPlane = planes.P4.PointOnPlane + translation;
			planes.P1.PointOnPlane = planes.P1.PointOnPlane + translation;
			planes.P3.PointOnPlane = planes.P3.PointOnPlane + translation;
			planes.P5.PointOnPlane = planes.P5.PointOnPlane + translation;
			return planes;
		}

		private static bool GetHitPoint(ref LagCompensationUtils.CustomPlanesBox planesA, ref LagCompensationUtils.CustomPlanesBox planesB, ref LagCompensationUtils.CustomEdgesBox edgesA, ref LagCompensationUtils.CustomEdgesBox edgesB, ref LagCompensationUtils.BoxNarrowData boxNarrowA, ref LagCompensationUtils.BoxNarrowData boxNarrowB, ref Vector3 boxAToBoxBOffset, bool computeDetailedInfo, out Vector3 contactPoint)
		{
			Vector3 vector = default(Vector3);
			contactPoint = default(Vector3);
			int num = 0;
			LagCompensationUtils.GetContactPointPlaneEdge(ref planesA, ref edgesB, ref boxNarrowA, ref vector, ref boxNarrowA.Position, computeDetailedInfo, ref num, ref contactPoint);
			bool flag = num == 0 || (computeDetailedInfo && num < 4);
			if (flag)
			{
				LagCompensationUtils.GetContactPointPlaneEdge(ref planesB, ref edgesA, ref boxNarrowB, ref boxAToBoxBOffset, ref boxNarrowA.Position, computeDetailedInfo, ref num, ref contactPoint);
			}
			bool flag2 = num > 0;
			bool result;
			if (flag2)
			{
				contactPoint /= (float)num;
				result = true;
			}
			else
			{
				bool flag3 = LagCompensationUtils.BoxInAABB(ref edgesB, ref boxNarrowA, ref vector);
				if (flag3)
				{
					contactPoint = boxAToBoxBOffset + boxNarrowA.Position;
					result = true;
				}
				else
				{
					bool flag4 = LagCompensationUtils.BoxInAABB(ref edgesA, ref boxNarrowB, ref boxAToBoxBOffset);
					if (flag4)
					{
						contactPoint = boxNarrowA.Position;
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		private static void GetContactPointPlaneEdge(ref LagCompensationUtils.CustomPlanesBox planes, ref LagCompensationUtils.CustomEdgesBox edges, ref LagCompensationUtils.BoxNarrowData boxNarrow, ref Vector3 offset, ref Vector3 boxAPosition, bool detailedManifold, ref int cpCount, ref Vector3 contactPoint)
		{
			Vector3 vector = boxNarrow.Extents * 1.025f;
			Vector3 vector2 = default(Vector3);
			bool flag = LagCompensationUtils.ClipToPlane(ref planes.P0, ref edges.P0, ref edges.P1, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag2 = !detailedManifold || cpCount >= 4;
				if (flag2)
				{
					return;
				}
			}
			bool flag3 = LagCompensationUtils.ClipToPlane(ref planes.P0, ref edges.P1, ref edges.P2, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag3)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag4 = !detailedManifold || cpCount >= 4;
				if (flag4)
				{
					return;
				}
			}
			bool flag5 = LagCompensationUtils.ClipToPlane(ref planes.P0, ref edges.P2, ref edges.P3, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag5)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag6 = !detailedManifold || cpCount >= 4;
				if (flag6)
				{
					return;
				}
			}
			bool flag7 = LagCompensationUtils.ClipToPlane(ref planes.P0, ref edges.P3, ref edges.P0, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag7)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag8 = !detailedManifold || cpCount >= 4;
				if (flag8)
				{
					return;
				}
			}
			bool flag9 = LagCompensationUtils.ClipToPlane(ref planes.P0, ref edges.P4, ref edges.P5, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag9)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag10 = !detailedManifold || cpCount >= 4;
				if (flag10)
				{
					return;
				}
			}
			bool flag11 = LagCompensationUtils.ClipToPlane(ref planes.P0, ref edges.P5, ref edges.P6, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag11)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag12 = !detailedManifold || cpCount >= 4;
				if (flag12)
				{
					return;
				}
			}
			bool flag13 = LagCompensationUtils.ClipToPlane(ref planes.P0, ref edges.P6, ref edges.P7, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag13)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag14 = !detailedManifold || cpCount >= 4;
				if (flag14)
				{
					return;
				}
			}
			bool flag15 = LagCompensationUtils.ClipToPlane(ref planes.P0, ref edges.P7, ref edges.P4, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag15)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag16 = !detailedManifold || cpCount >= 4;
				if (flag16)
				{
					return;
				}
			}
			bool flag17 = LagCompensationUtils.ClipToPlane(ref planes.P0, ref edges.P4, ref edges.P0, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag17)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag18 = !detailedManifold || cpCount >= 4;
				if (flag18)
				{
					return;
				}
			}
			bool flag19 = LagCompensationUtils.ClipToPlane(ref planes.P0, ref edges.P5, ref edges.P1, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag19)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag20 = !detailedManifold || cpCount >= 4;
				if (flag20)
				{
					return;
				}
			}
			bool flag21 = LagCompensationUtils.ClipToPlane(ref planes.P0, ref edges.P6, ref edges.P2, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag21)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag22 = !detailedManifold || cpCount >= 4;
				if (flag22)
				{
					return;
				}
			}
			bool flag23 = LagCompensationUtils.ClipToPlane(ref planes.P0, ref edges.P7, ref edges.P3, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag23)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag24 = !detailedManifold || cpCount >= 4;
				if (flag24)
				{
					return;
				}
			}
			bool flag25 = LagCompensationUtils.ClipToPlane(ref planes.P1, ref edges.P0, ref edges.P1, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag25)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag26 = !detailedManifold || cpCount >= 4;
				if (flag26)
				{
					return;
				}
			}
			bool flag27 = LagCompensationUtils.ClipToPlane(ref planes.P1, ref edges.P1, ref edges.P2, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag27)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag28 = !detailedManifold || cpCount >= 4;
				if (flag28)
				{
					return;
				}
			}
			bool flag29 = LagCompensationUtils.ClipToPlane(ref planes.P1, ref edges.P2, ref edges.P3, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag29)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag30 = !detailedManifold || cpCount >= 4;
				if (flag30)
				{
					return;
				}
			}
			bool flag31 = LagCompensationUtils.ClipToPlane(ref planes.P1, ref edges.P3, ref edges.P0, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag31)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag32 = !detailedManifold || cpCount >= 4;
				if (flag32)
				{
					return;
				}
			}
			bool flag33 = LagCompensationUtils.ClipToPlane(ref planes.P1, ref edges.P4, ref edges.P5, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag33)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag34 = !detailedManifold || cpCount >= 4;
				if (flag34)
				{
					return;
				}
			}
			bool flag35 = LagCompensationUtils.ClipToPlane(ref planes.P1, ref edges.P5, ref edges.P6, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag35)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag36 = !detailedManifold || cpCount >= 4;
				if (flag36)
				{
					return;
				}
			}
			bool flag37 = LagCompensationUtils.ClipToPlane(ref planes.P1, ref edges.P6, ref edges.P7, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag37)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag38 = !detailedManifold || cpCount >= 4;
				if (flag38)
				{
					return;
				}
			}
			bool flag39 = LagCompensationUtils.ClipToPlane(ref planes.P1, ref edges.P7, ref edges.P4, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag39)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag40 = !detailedManifold || cpCount >= 4;
				if (flag40)
				{
					return;
				}
			}
			bool flag41 = LagCompensationUtils.ClipToPlane(ref planes.P1, ref edges.P4, ref edges.P0, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag41)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag42 = !detailedManifold || cpCount >= 4;
				if (flag42)
				{
					return;
				}
			}
			bool flag43 = LagCompensationUtils.ClipToPlane(ref planes.P1, ref edges.P5, ref edges.P1, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag43)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag44 = !detailedManifold || cpCount >= 4;
				if (flag44)
				{
					return;
				}
			}
			bool flag45 = LagCompensationUtils.ClipToPlane(ref planes.P1, ref edges.P6, ref edges.P2, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag45)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag46 = !detailedManifold || cpCount >= 4;
				if (flag46)
				{
					return;
				}
			}
			bool flag47 = LagCompensationUtils.ClipToPlane(ref planes.P1, ref edges.P7, ref edges.P3, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag47)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag48 = !detailedManifold || cpCount >= 4;
				if (flag48)
				{
					return;
				}
			}
			bool flag49 = LagCompensationUtils.ClipToPlane(ref planes.P2, ref edges.P0, ref edges.P1, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag49)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag50 = !detailedManifold || cpCount >= 4;
				if (flag50)
				{
					return;
				}
			}
			bool flag51 = LagCompensationUtils.ClipToPlane(ref planes.P2, ref edges.P1, ref edges.P2, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag51)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag52 = !detailedManifold || cpCount >= 4;
				if (flag52)
				{
					return;
				}
			}
			bool flag53 = LagCompensationUtils.ClipToPlane(ref planes.P2, ref edges.P2, ref edges.P3, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag53)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag54 = !detailedManifold || cpCount >= 4;
				if (flag54)
				{
					return;
				}
			}
			bool flag55 = LagCompensationUtils.ClipToPlane(ref planes.P2, ref edges.P3, ref edges.P0, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag55)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag56 = !detailedManifold || cpCount >= 4;
				if (flag56)
				{
					return;
				}
			}
			bool flag57 = LagCompensationUtils.ClipToPlane(ref planes.P2, ref edges.P4, ref edges.P5, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag57)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag58 = !detailedManifold || cpCount >= 4;
				if (flag58)
				{
					return;
				}
			}
			bool flag59 = LagCompensationUtils.ClipToPlane(ref planes.P2, ref edges.P5, ref edges.P6, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag59)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag60 = !detailedManifold || cpCount >= 4;
				if (flag60)
				{
					return;
				}
			}
			bool flag61 = LagCompensationUtils.ClipToPlane(ref planes.P2, ref edges.P6, ref edges.P7, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag61)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag62 = !detailedManifold || cpCount >= 4;
				if (flag62)
				{
					return;
				}
			}
			bool flag63 = LagCompensationUtils.ClipToPlane(ref planes.P2, ref edges.P7, ref edges.P4, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag63)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag64 = !detailedManifold || cpCount >= 4;
				if (flag64)
				{
					return;
				}
			}
			bool flag65 = LagCompensationUtils.ClipToPlane(ref planes.P2, ref edges.P4, ref edges.P0, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag65)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag66 = !detailedManifold || cpCount >= 4;
				if (flag66)
				{
					return;
				}
			}
			bool flag67 = LagCompensationUtils.ClipToPlane(ref planes.P2, ref edges.P5, ref edges.P1, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag67)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag68 = !detailedManifold || cpCount >= 4;
				if (flag68)
				{
					return;
				}
			}
			bool flag69 = LagCompensationUtils.ClipToPlane(ref planes.P2, ref edges.P6, ref edges.P2, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag69)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag70 = !detailedManifold || cpCount >= 4;
				if (flag70)
				{
					return;
				}
			}
			bool flag71 = LagCompensationUtils.ClipToPlane(ref planes.P2, ref edges.P7, ref edges.P3, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag71)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag72 = !detailedManifold || cpCount >= 4;
				if (flag72)
				{
					return;
				}
			}
			bool flag73 = LagCompensationUtils.ClipToPlane(ref planes.P3, ref edges.P0, ref edges.P1, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag73)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag74 = !detailedManifold || cpCount >= 4;
				if (flag74)
				{
					return;
				}
			}
			bool flag75 = LagCompensationUtils.ClipToPlane(ref planes.P3, ref edges.P1, ref edges.P2, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag75)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag76 = !detailedManifold || cpCount >= 4;
				if (flag76)
				{
					return;
				}
			}
			bool flag77 = LagCompensationUtils.ClipToPlane(ref planes.P3, ref edges.P2, ref edges.P3, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag77)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag78 = !detailedManifold || cpCount >= 4;
				if (flag78)
				{
					return;
				}
			}
			bool flag79 = LagCompensationUtils.ClipToPlane(ref planes.P3, ref edges.P3, ref edges.P0, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag79)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag80 = !detailedManifold || cpCount >= 4;
				if (flag80)
				{
					return;
				}
			}
			bool flag81 = LagCompensationUtils.ClipToPlane(ref planes.P3, ref edges.P4, ref edges.P5, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag81)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag82 = !detailedManifold || cpCount >= 4;
				if (flag82)
				{
					return;
				}
			}
			bool flag83 = LagCompensationUtils.ClipToPlane(ref planes.P3, ref edges.P5, ref edges.P6, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag83)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag84 = !detailedManifold || cpCount >= 4;
				if (flag84)
				{
					return;
				}
			}
			bool flag85 = LagCompensationUtils.ClipToPlane(ref planes.P3, ref edges.P6, ref edges.P7, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag85)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag86 = !detailedManifold || cpCount >= 4;
				if (flag86)
				{
					return;
				}
			}
			bool flag87 = LagCompensationUtils.ClipToPlane(ref planes.P3, ref edges.P7, ref edges.P4, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag87)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag88 = !detailedManifold || cpCount >= 4;
				if (flag88)
				{
					return;
				}
			}
			bool flag89 = LagCompensationUtils.ClipToPlane(ref planes.P3, ref edges.P4, ref edges.P0, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag89)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag90 = !detailedManifold || cpCount >= 4;
				if (flag90)
				{
					return;
				}
			}
			bool flag91 = LagCompensationUtils.ClipToPlane(ref planes.P3, ref edges.P5, ref edges.P1, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag91)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag92 = !detailedManifold || cpCount >= 4;
				if (flag92)
				{
					return;
				}
			}
			bool flag93 = LagCompensationUtils.ClipToPlane(ref planes.P3, ref edges.P6, ref edges.P2, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag93)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag94 = !detailedManifold || cpCount >= 4;
				if (flag94)
				{
					return;
				}
			}
			bool flag95 = LagCompensationUtils.ClipToPlane(ref planes.P3, ref edges.P7, ref edges.P3, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag95)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag96 = !detailedManifold || cpCount >= 4;
				if (flag96)
				{
					return;
				}
			}
			bool flag97 = LagCompensationUtils.ClipToPlane(ref planes.P4, ref edges.P0, ref edges.P1, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag97)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag98 = !detailedManifold || cpCount >= 4;
				if (flag98)
				{
					return;
				}
			}
			bool flag99 = LagCompensationUtils.ClipToPlane(ref planes.P4, ref edges.P1, ref edges.P2, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag99)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag100 = !detailedManifold || cpCount >= 4;
				if (flag100)
				{
					return;
				}
			}
			bool flag101 = LagCompensationUtils.ClipToPlane(ref planes.P4, ref edges.P2, ref edges.P3, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag101)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag102 = !detailedManifold || cpCount >= 4;
				if (flag102)
				{
					return;
				}
			}
			bool flag103 = LagCompensationUtils.ClipToPlane(ref planes.P4, ref edges.P3, ref edges.P0, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag103)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag104 = !detailedManifold || cpCount >= 4;
				if (flag104)
				{
					return;
				}
			}
			bool flag105 = LagCompensationUtils.ClipToPlane(ref planes.P4, ref edges.P4, ref edges.P5, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag105)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag106 = !detailedManifold || cpCount >= 4;
				if (flag106)
				{
					return;
				}
			}
			bool flag107 = LagCompensationUtils.ClipToPlane(ref planes.P4, ref edges.P5, ref edges.P6, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag107)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag108 = !detailedManifold || cpCount >= 4;
				if (flag108)
				{
					return;
				}
			}
			bool flag109 = LagCompensationUtils.ClipToPlane(ref planes.P4, ref edges.P6, ref edges.P7, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag109)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag110 = !detailedManifold || cpCount >= 4;
				if (flag110)
				{
					return;
				}
			}
			bool flag111 = LagCompensationUtils.ClipToPlane(ref planes.P4, ref edges.P7, ref edges.P4, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag111)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag112 = !detailedManifold || cpCount >= 4;
				if (flag112)
				{
					return;
				}
			}
			bool flag113 = LagCompensationUtils.ClipToPlane(ref planes.P4, ref edges.P4, ref edges.P0, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag113)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag114 = !detailedManifold || cpCount >= 4;
				if (flag114)
				{
					return;
				}
			}
			bool flag115 = LagCompensationUtils.ClipToPlane(ref planes.P4, ref edges.P5, ref edges.P1, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag115)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag116 = !detailedManifold || cpCount >= 4;
				if (flag116)
				{
					return;
				}
			}
			bool flag117 = LagCompensationUtils.ClipToPlane(ref planes.P4, ref edges.P6, ref edges.P2, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag117)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag118 = !detailedManifold || cpCount >= 4;
				if (flag118)
				{
					return;
				}
			}
			bool flag119 = LagCompensationUtils.ClipToPlane(ref planes.P4, ref edges.P7, ref edges.P3, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag119)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag120 = !detailedManifold || cpCount >= 4;
				if (flag120)
				{
					return;
				}
			}
			bool flag121 = LagCompensationUtils.ClipToPlane(ref planes.P5, ref edges.P0, ref edges.P1, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag121)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag122 = !detailedManifold || cpCount >= 4;
				if (flag122)
				{
					return;
				}
			}
			bool flag123 = LagCompensationUtils.ClipToPlane(ref planes.P5, ref edges.P1, ref edges.P2, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag123)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag124 = !detailedManifold || cpCount >= 4;
				if (flag124)
				{
					return;
				}
			}
			bool flag125 = LagCompensationUtils.ClipToPlane(ref planes.P5, ref edges.P2, ref edges.P3, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag125)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag126 = !detailedManifold || cpCount >= 4;
				if (flag126)
				{
					return;
				}
			}
			bool flag127 = LagCompensationUtils.ClipToPlane(ref planes.P5, ref edges.P3, ref edges.P0, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag127)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag128 = !detailedManifold || cpCount >= 4;
				if (flag128)
				{
					return;
				}
			}
			bool flag129 = LagCompensationUtils.ClipToPlane(ref planes.P5, ref edges.P4, ref edges.P5, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag129)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag130 = !detailedManifold || cpCount >= 4;
				if (flag130)
				{
					return;
				}
			}
			bool flag131 = LagCompensationUtils.ClipToPlane(ref planes.P5, ref edges.P5, ref edges.P6, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag131)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag132 = !detailedManifold || cpCount >= 4;
				if (flag132)
				{
					return;
				}
			}
			bool flag133 = LagCompensationUtils.ClipToPlane(ref planes.P5, ref edges.P6, ref edges.P7, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag133)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag134 = !detailedManifold || cpCount >= 4;
				if (flag134)
				{
					return;
				}
			}
			bool flag135 = LagCompensationUtils.ClipToPlane(ref planes.P5, ref edges.P7, ref edges.P4, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag135)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag136 = !detailedManifold || cpCount >= 4;
				if (flag136)
				{
					return;
				}
			}
			bool flag137 = LagCompensationUtils.ClipToPlane(ref planes.P5, ref edges.P4, ref edges.P0, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag137)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag138 = !detailedManifold || cpCount >= 4;
				if (flag138)
				{
					return;
				}
			}
			bool flag139 = LagCompensationUtils.ClipToPlane(ref planes.P5, ref edges.P5, ref edges.P1, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag139)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag140 = !detailedManifold || cpCount >= 4;
				if (flag140)
				{
					return;
				}
			}
			bool flag141 = LagCompensationUtils.ClipToPlane(ref planes.P5, ref edges.P6, ref edges.P2, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag141)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag142 = !detailedManifold || cpCount >= 4;
				if (flag142)
				{
					return;
				}
			}
			bool flag143 = LagCompensationUtils.ClipToPlane(ref planes.P5, ref edges.P7, ref edges.P3, ref vector2) && LagCompensationUtils.PointInAABB(vector2, ref boxNarrow, ref vector, ref offset);
			if (flag143)
			{
				contactPoint += vector2 + boxAPosition;
				cpCount++;
				bool flag144 = !detailedManifold || cpCount >= 4;
				if (flag144)
				{
				}
			}
		}

		private static bool ClipToPlane(ref LagCompensationUtils.CustomPlane plane, ref Vector3 lineStart, ref Vector3 lineEnd, ref Vector3 intersection)
		{
			Vector3 vector = lineEnd - lineStart;
			float num = Vector3.Dot(vector, plane.Normal);
			bool flag = num > -0.0001f && num < 0.0001f;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				float num2 = Vector3.Dot(plane.PointOnPlane - lineStart, plane.Normal) / num;
				bool flag2 = num2 >= 0f && num2 <= 1f;
				if (flag2)
				{
					intersection = lineStart + vector * num2;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private static bool BoxInAABB(ref LagCompensationUtils.CustomEdgesBox boxEdges, ref LagCompensationUtils.BoxNarrowData boxNarrow, ref Vector3 offset)
		{
			Vector3 vector = boxNarrow.Extents * 1.025f;
			bool flag = !LagCompensationUtils.PointInAABB(boxEdges.P0, ref boxNarrow, ref vector, ref offset);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !LagCompensationUtils.PointInAABB(boxEdges.P1, ref boxNarrow, ref vector, ref offset);
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !LagCompensationUtils.PointInAABB(boxEdges.P2, ref boxNarrow, ref vector, ref offset);
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = !LagCompensationUtils.PointInAABB(boxEdges.P3, ref boxNarrow, ref vector, ref offset);
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool flag5 = !LagCompensationUtils.PointInAABB(boxEdges.P4, ref boxNarrow, ref vector, ref offset);
							if (flag5)
							{
								result = false;
							}
							else
							{
								bool flag6 = !LagCompensationUtils.PointInAABB(boxEdges.P5, ref boxNarrow, ref vector, ref offset);
								if (flag6)
								{
									result = false;
								}
								else
								{
									bool flag7 = !LagCompensationUtils.PointInAABB(boxEdges.P6, ref boxNarrow, ref vector, ref offset);
									if (flag7)
									{
										result = false;
									}
									else
									{
										bool flag8 = !LagCompensationUtils.PointInAABB(boxEdges.P7, ref boxNarrow, ref vector, ref offset);
										result = !flag8;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		private static bool PointInAABB(Vector3 point, ref LagCompensationUtils.BoxNarrowData boxNarrow, ref Vector3 max, ref Vector3 offset)
		{
			point = boxNarrow.WorldToLocalVector(point - offset);
			Vector3 vector = -max;
			bool flag = point.x < vector.x || point.x > max.x || point.y < vector.y || point.y > max.y || point.z < vector.z || point.z > max.z;
			return !flag;
		}

		public static bool LocalAABBSphereIntersection(Vector3 aabbExtents, Vector3 sphereCenter, float sphereRadius)
		{
			Vector3 vector = sphereCenter;
			bool flag = vector.x > aabbExtents.x;
			if (flag)
			{
				vector.x = aabbExtents.x;
			}
			else
			{
				bool flag2 = vector.x < -aabbExtents.x;
				if (flag2)
				{
					vector.x = -aabbExtents.x;
				}
			}
			bool flag3 = vector.y > aabbExtents.y;
			if (flag3)
			{
				vector.y = aabbExtents.y;
			}
			else
			{
				bool flag4 = vector.y < -aabbExtents.y;
				if (flag4)
				{
					vector.y = -aabbExtents.y;
				}
			}
			bool flag5 = vector.z > aabbExtents.z;
			if (flag5)
			{
				vector.z = aabbExtents.z;
			}
			else
			{
				bool flag6 = vector.z < -aabbExtents.z;
				if (flag6)
				{
					vector.z = -aabbExtents.z;
				}
			}
			sphereCenter.x -= vector.x;
			sphereCenter.y -= vector.y;
			sphereCenter.z -= vector.z;
			return sphereCenter.sqrMagnitude < sphereRadius * sphereRadius;
		}

		public static bool LocalAABBSphereContact(Vector3 aabbExtents, Vector3 sphereCenter, float sphereRadius, out LagCompensationUtils.ContactData contact)
		{
			bool flag = true;
			Vector3 vector = sphereCenter;
			bool flag2 = vector.x < -aabbExtents.x;
			if (flag2)
			{
				flag = false;
				vector.x = -aabbExtents.x;
			}
			else
			{
				bool flag3 = vector.x > aabbExtents.x;
				if (flag3)
				{
					flag = false;
					vector.x = aabbExtents.x;
				}
			}
			bool flag4 = vector.y < -aabbExtents.y;
			if (flag4)
			{
				flag = false;
				vector.y = -aabbExtents.y;
			}
			else
			{
				bool flag5 = vector.y > aabbExtents.y;
				if (flag5)
				{
					flag = false;
					vector.y = aabbExtents.y;
				}
			}
			bool flag6 = vector.z < -aabbExtents.z;
			if (flag6)
			{
				flag = false;
				vector.z = -aabbExtents.z;
			}
			else
			{
				bool flag7 = vector.z > aabbExtents.z;
				if (flag7)
				{
					flag = false;
					vector.z = aabbExtents.z;
				}
			}
			bool flag8 = flag;
			bool result;
			if (flag8)
			{
				contact.Point = sphereCenter;
				contact.Normal = default(Vector3);
				Vector3 b = new Vector3(Math.Abs(vector.x), Math.Abs(vector.y), Math.Abs(vector.z));
				Vector3 vector2 = aabbExtents - b;
				bool flag9 = vector2.y < vector2.x;
				if (flag9)
				{
					bool flag10 = vector2.y < vector2.z;
					if (flag10)
					{
						contact.Normal.y = ((vector.y > 0f) ? 1f : -1f);
						contact.Penetration = vector2.y;
					}
					else
					{
						contact.Normal.z = ((vector.z > 0f) ? 1f : -1f);
						contact.Penetration = vector2.z;
					}
				}
				else
				{
					bool flag11 = vector2.x < vector2.z;
					if (flag11)
					{
						contact.Normal.x = ((vector.x > 0f) ? 1f : -1f);
						contact.Penetration = vector2.x;
					}
					else
					{
						contact.Normal.z = ((vector.z > 0f) ? 1f : -1f);
						contact.Penetration = vector2.z;
					}
				}
				contact.Penetration += sphereRadius;
				result = true;
			}
			else
			{
				contact.Point = vector;
				contact.Normal = sphereCenter - vector;
				float sqrMagnitude = contact.Normal.sqrMagnitude;
				bool flag12 = sqrMagnitude >= sphereRadius * sphereRadius;
				if (flag12)
				{
					contact = default(LagCompensationUtils.ContactData);
					result = false;
				}
				else
				{
					contact.Penetration = (float)(1.0 / Math.Sqrt((double)sqrMagnitude));
					contact.Normal *= contact.Penetration;
					contact.Penetration = sphereRadius - contact.Penetration * sqrMagnitude;
					result = true;
				}
			}
			return result;
		}

		internal static bool LocalSphereCapsuleIntersection(Vector3 capsuleTopCenter, Vector3 capsuleBottomCenter, float capsuleRadius, Vector3 sphereCenter, float sphereRadius, out LagCompensationUtils.ContactData contactData)
		{
			Vector3 vector = LagCompensationUtils.ClosestPtPointSegment(sphereCenter, capsuleBottomCenter, capsuleTopCenter);
			Vector3 normalized = (sphereCenter - vector).normalized;
			bool flag = Vector3.Distance(vector, sphereCenter) <= capsuleRadius + sphereRadius;
			bool result;
			if (flag)
			{
				contactData.Point = vector + normalized * capsuleRadius;
				contactData.Normal = normalized;
				contactData.Penetration = sphereRadius - Vector3.Distance(contactData.Point, sphereCenter);
				result = true;
			}
			else
			{
				contactData.Point = default(Vector3);
				contactData.Normal = default(Vector3);
				contactData.Penetration = 0f;
				result = false;
			}
			return result;
		}

		internal static bool LocalRayCapsuleIntersection(Vector3 capsuleTopCenter, Vector3 capsuleBottomCenter, float capsuleRadius, Vector3 rayLocalOrigin, Vector3 rayLocalDir, float maxDistance, out Vector3 point, out Vector3 normal, out float distance)
		{
			float num = LagCompensationUtils.RayCapsuleIntersect(rayLocalOrigin, rayLocalDir, capsuleBottomCenter, capsuleTopCenter, capsuleRadius);
			bool flag = num > 0f && num <= maxDistance;
			bool result;
			if (flag)
			{
				point = rayLocalOrigin + rayLocalDir * num;
				normal = (point - LagCompensationUtils.ClosestPtPointSegment(point, capsuleBottomCenter, capsuleTopCenter)).normalized;
				distance = num;
				result = true;
			}
			else
			{
				point = default(Vector3);
				normal = default(Vector3);
				distance = 0f;
				result = false;
			}
			return result;
		}

		internal static bool LocalAABBCapsuleIntersection(Vector3 localCapsuleCenter, Vector3 localCapsulePointA, Vector3 localCapsulePointB, float capsuleRadius, Vector3 aabbExtents, out LagCompensationUtils.ContactData contactData)
		{
			Vector3 point;
			bool flag = LagCompensationUtils.ClampPointToAABB(localCapsuleCenter, aabbExtents, out point);
			Vector3 vector;
			LagCompensationUtils.ClampPointToAABB(localCapsulePointA, aabbExtents, out vector);
			Vector3 vector2;
			LagCompensationUtils.ClampPointToAABB(localCapsulePointB, aabbExtents, out vector2);
			bool flag2 = flag;
			bool result;
			if (flag2)
			{
				contactData.Normal = -point.normalized;
				contactData.Point = point;
				contactData.Penetration = 0f;
				result = true;
			}
			else
			{
				float num = Vector3.Distance(localCapsulePointA, vector);
				float num2 = Vector3.Distance(localCapsulePointB, vector2);
				bool flag3 = num <= capsuleRadius;
				if (flag3)
				{
					contactData.Normal = -vector.normalized;
					contactData.Point = vector;
					contactData.Penetration = 0f;
					result = true;
				}
				else
				{
					bool flag4 = num2 <= capsuleRadius;
					if (flag4)
					{
						contactData.Normal = -vector2.normalized;
						contactData.Point = vector2;
						contactData.Penetration = 0f;
						result = true;
					}
					else
					{
						Vector3 aabbsupportPoint = LagCompensationUtils.GetAABBSupportPoint(vector, vector2, aabbExtents);
						ValueTuple<Vector3, Vector3, float> valueTuple = LagCompensationUtils.ClosestDistanceBetweenLines(vector, aabbsupportPoint, localCapsulePointA, localCapsulePointB, true, false, false, false, false);
						ValueTuple<Vector3, Vector3, float> valueTuple2 = LagCompensationUtils.ClosestDistanceBetweenLines(vector2, aabbsupportPoint, localCapsulePointA, localCapsulePointB, true, false, false, false, false);
						Vector3 vector3 = (valueTuple.Item3 <= valueTuple2.Item3) ? valueTuple.Item2 : valueTuple2.Item2;
						Vector3 a = (valueTuple.Item3 <= valueTuple2.Item3) ? valueTuple.Item1 : valueTuple2.Item1;
						bool flag5 = valueTuple.Item3 <= capsuleRadius || valueTuple2.Item3 <= capsuleRadius;
						if (flag5)
						{
							contactData.Normal = (a - vector3).normalized;
							contactData.Point = vector3 + contactData.Normal * capsuleRadius;
							contactData.Penetration = 0f;
							result = true;
						}
						else
						{
							contactData.Point = default(Vector3);
							contactData.Penetration = 0f;
							contactData.Normal = default(Vector3);
							result = false;
						}
					}
				}
			}
			return result;
		}

		private static Vector3 GetAABBSupportPoint(Vector3 pointA, Vector3 pointB, Vector3 extents)
		{
			Vector3 result = default(Vector3);
			bool flag = Mathf.Abs(pointA.x - extents.x) <= float.Epsilon;
			if (flag)
			{
				result.x = pointA.x;
			}
			else
			{
				bool flag2 = Mathf.Abs(pointB.x - extents.x) <= float.Epsilon;
				if (flag2)
				{
					result.x = pointB.x;
				}
				else
				{
					result.x = Mathf.Lerp(pointA.x, pointB.x, 0.5f);
				}
			}
			bool flag3 = Mathf.Abs(pointA.y - extents.y) <= float.Epsilon;
			if (flag3)
			{
				result.y = pointA.y;
			}
			else
			{
				bool flag4 = Mathf.Abs(pointB.y - extents.y) <= float.Epsilon;
				if (flag4)
				{
					result.y = pointB.y;
				}
				else
				{
					result.y = Mathf.Lerp(pointA.y, pointB.y, 0.5f);
				}
			}
			bool flag5 = Mathf.Abs(pointA.z - extents.z) <= float.Epsilon;
			if (flag5)
			{
				result.z = pointA.z;
			}
			else
			{
				bool flag6 = Mathf.Abs(pointB.z - extents.z) <= float.Epsilon;
				if (flag6)
				{
					result.z = pointB.z;
				}
				else
				{
					result.z = Mathf.Lerp(pointA.z, pointB.z, 0.5f);
				}
			}
			return result;
		}

		internal static bool ClampPointToAABB(Vector3 point, Vector3 aabbExtents, out Vector3 clampedPoint)
		{
			bool result = true;
			bool flag = point.x < -aabbExtents.x;
			if (flag)
			{
				result = false;
				point.x = -aabbExtents.x;
			}
			else
			{
				bool flag2 = point.x > aabbExtents.x;
				if (flag2)
				{
					result = false;
					point.x = aabbExtents.x;
				}
			}
			bool flag3 = point.y < -aabbExtents.y;
			if (flag3)
			{
				result = false;
				point.y = -aabbExtents.y;
			}
			else
			{
				bool flag4 = point.y > aabbExtents.y;
				if (flag4)
				{
					result = false;
					point.y = aabbExtents.y;
				}
			}
			bool flag5 = point.z < -aabbExtents.z;
			if (flag5)
			{
				result = false;
				point.z = -aabbExtents.z;
			}
			else
			{
				bool flag6 = point.z > aabbExtents.z;
				if (flag6)
				{
					result = false;
					point.z = aabbExtents.z;
				}
			}
			clampedPoint = point;
			return result;
		}

		public static ValueTuple<Vector3, Vector3, float> ClosestDistanceBetweenLines(Vector3 a0, Vector3 a1, Vector3 b0, Vector3 b1, bool clampAll = false, bool clampA0 = false, bool clampA1 = false, bool clampB0 = false, bool clampB1 = false)
		{
			if (clampAll)
			{
				clampA0 = true;
				clampA1 = true;
				clampB0 = true;
				clampB1 = true;
			}
			Vector3 a2 = a1 - a0;
			Vector3 a3 = b1 - b0;
			float magnitude = a2.magnitude;
			float magnitude2 = a3.magnitude;
			Vector3 vector = a2 / magnitude;
			Vector3 vector2 = a3 / magnitude2;
			Vector3 rhs = Vector3.Cross(vector, vector2);
			float sqrMagnitude = rhs.sqrMagnitude;
			bool flag = Mathf.Approximately(sqrMagnitude, 0f);
			ValueTuple<Vector3, Vector3, float> result;
			if (flag)
			{
				float num = Vector3.Dot(vector, b0 - a0);
				bool flag2 = clampA0 || clampA1 || clampB0 || clampB1;
				if (flag2)
				{
					float num2 = Vector3.Dot(vector, b1 - a0);
					bool flag3 = num <= 0f && num2 >= 0f;
					if (flag3)
					{
						bool flag4 = clampA0 && clampB1;
						if (flag4)
						{
							bool flag5 = Mathf.Abs(num) < Mathf.Abs(num2);
							if (flag5)
							{
								return new ValueTuple<Vector3, Vector3, float>(a0, b0, (a0 - b0).magnitude);
							}
							return new ValueTuple<Vector3, Vector3, float>(a0, b1, (a0 - b1).magnitude);
						}
					}
					else
					{
						bool flag6 = num >= magnitude && num2 <= magnitude;
						if (flag6)
						{
							bool flag7 = clampA1 && clampB0;
							if (flag7)
							{
								bool flag8 = Mathf.Abs(num) < Mathf.Abs(num2);
								if (flag8)
								{
									return new ValueTuple<Vector3, Vector3, float>(a1, b0, (a1 - b0).magnitude);
								}
								return new ValueTuple<Vector3, Vector3, float>(a1, b1, (a1 - b1).magnitude);
							}
						}
					}
				}
				result = new ValueTuple<Vector3, Vector3, float>(Vector3.zero, Vector3.zero, (num * vector + a0 - b0).magnitude);
			}
			else
			{
				Vector3 lhs = b0 - a0;
				float num3 = Vector3.Dot(Vector3.Cross(lhs, vector2), rhs);
				float num4 = Vector3.Dot(Vector3.Cross(lhs, vector), rhs);
				float num5 = num3 / sqrMagnitude;
				float num6 = num4 / sqrMagnitude;
				Vector3 vector3 = a0 + vector * num5;
				Vector3 vector4 = b0 + vector2 * num6;
				bool flag9 = clampA0 || clampA1 || clampB0 || clampB1;
				if (flag9)
				{
					bool flag10 = clampA0 && num5 < 0f;
					if (flag10)
					{
						vector3 = a0;
					}
					else
					{
						bool flag11 = clampA1 && num5 > magnitude;
						if (flag11)
						{
							vector3 = a1;
						}
					}
					bool flag12 = clampB0 && num6 < 0f;
					if (flag12)
					{
						vector4 = b0;
					}
					else
					{
						bool flag13 = clampB1 && num6 > magnitude2;
						if (flag13)
						{
							vector4 = b1;
						}
					}
					bool flag14 = (clampA0 && num5 < 0f) || (clampA1 && num5 > magnitude);
					if (flag14)
					{
						float num7 = Vector3.Dot(vector2, vector3 - b0);
						bool flag15 = clampB0 && num7 < 0f;
						if (flag15)
						{
							num7 = 0f;
						}
						else
						{
							bool flag16 = clampB1 && num7 > magnitude2;
							if (flag16)
							{
								num7 = magnitude2;
							}
						}
						vector4 = b0 + vector2 * num7;
					}
					bool flag17 = (clampB0 && num6 < 0f) || (clampB1 && num6 > magnitude2);
					if (flag17)
					{
						float num8 = Vector3.Dot(vector, vector4 - a0);
						bool flag18 = clampA0 && num8 < 0f;
						if (flag18)
						{
							num8 = 0f;
						}
						else
						{
							bool flag19 = clampA1 && num8 > magnitude;
							if (flag19)
							{
								num8 = magnitude;
							}
						}
						vector3 = a0 + vector * num8;
					}
				}
				result = new ValueTuple<Vector3, Vector3, float>(vector3, vector4, (vector3 - vector4).magnitude);
			}
			return result;
		}

		internal static float RayCapsuleIntersect(Vector3 rayOrigin, Vector3 rayDir, Vector3 capsulePointA, Vector3 capsulePointB, float capsuleRadius)
		{
			Vector3 vector = capsulePointB - capsulePointA;
			Vector3 vector2 = rayOrigin - capsulePointA;
			float num = Vector3.Dot(vector, vector);
			float num2 = Vector3.Dot(vector, rayDir);
			float num3 = Vector3.Dot(vector, vector2);
			float num4 = Vector3.Dot(rayDir, vector2);
			float num5 = Vector3.Dot(vector2, vector2);
			float num6 = num - num2 * num2;
			float num7 = num * num4 - num3 * num2;
			float num8 = num * num5 - num3 * num3 - capsuleRadius * capsuleRadius * num;
			float num9 = num7 * num7 - num6 * num8;
			bool flag = (double)num9 >= 0.0;
			if (flag)
			{
				float num10 = (-num7 - Mathf.Sqrt(num9)) / num6;
				float num11 = num3 + num10 * num2;
				bool flag2 = (double)num11 > 0.0 && num11 < num;
				if (flag2)
				{
					return num10;
				}
				Vector3 vector3 = ((double)num11 <= 0.0) ? vector2 : (rayOrigin - capsulePointB);
				num7 = Vector3.Dot(rayDir, vector3);
				num8 = Vector3.Dot(vector3, vector3) - capsuleRadius * capsuleRadius;
				num9 = num7 * num7 - num8;
				bool flag3 = (double)num9 > 0.0;
				if (flag3)
				{
					return -num7 - Mathf.Sqrt(num9);
				}
			}
			return -1f;
		}

		internal static Vector3 ClosestPtPointSegment(Vector3 point, Vector3 a, Vector3 b)
		{
			Vector3 vector = b - a;
			float value = Vector3.Dot(point - a, vector) / Vector3.Dot(vector, vector);
			return Vector3.Lerp(a, b, Mathf.Clamp01(value));
		}

		internal static bool SphereSphere(Vector3 centerA, float radiusA, Vector3 centerB, float radiusB, out Vector3 intersection, out Vector3 normal)
		{
			intersection = default(Vector3);
			normal = centerA - centerB;
			float sqrMagnitude = normal.sqrMagnitude;
			float num = radiusA + radiusB;
			bool flag = sqrMagnitude >= num * num;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				float num2 = Mathf.Sqrt(sqrMagnitude);
				bool flag2 = num2 > float.Epsilon;
				if (flag2)
				{
					normal /= num2;
					intersection = centerB + normal * radiusB;
				}
				else
				{
					normal = Vector3.right;
					intersection = centerB;
				}
				result = true;
			}
			return result;
		}

		internal static bool RayAABB(ref Vector3 minB, ref Vector3 maxB, ref Vector3 origin, ref Vector3 dir, float sqrMaxdistance, out Vector3 point, out Vector3 normal, out float distance)
		{
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			Vector3 vector = default(Vector3);
			Vector3 vector2 = default(Vector3);
			point = default(Vector3);
			normal = default(Vector3);
			distance = 0f;
			bool flag5 = origin.x < minB.x;
			if (flag5)
			{
				vector2.x = minB.x;
				flag = false;
			}
			else
			{
				bool flag6 = origin.x > maxB.x;
				if (flag6)
				{
					vector2.x = maxB.x;
					flag = false;
				}
				else
				{
					flag2 = true;
				}
			}
			bool flag7 = origin.y < minB.y;
			if (flag7)
			{
				vector2.y = minB.y;
				flag = false;
			}
			else
			{
				bool flag8 = origin.y > maxB.y;
				if (flag8)
				{
					vector2.y = maxB.y;
					flag = false;
				}
				else
				{
					flag3 = true;
				}
			}
			bool flag9 = origin.z < minB.z;
			if (flag9)
			{
				vector2.z = minB.z;
				flag = false;
			}
			else
			{
				bool flag10 = origin.z > maxB.z;
				if (flag10)
				{
					vector2.z = maxB.z;
					flag = false;
				}
				else
				{
					flag4 = true;
				}
			}
			bool flag11 = flag;
			bool result;
			if (flag11)
			{
				point = origin;
				result = false;
			}
			else
			{
				bool flag12 = dir.x != 0f && !flag2;
				if (flag12)
				{
					vector.x = (vector2.x - origin.x) / dir.x;
				}
				else
				{
					vector.x = -1f;
				}
				bool flag13 = dir.y != 0f && !flag3;
				if (flag13)
				{
					vector.y = (vector2.y - origin.y) / dir.y;
				}
				else
				{
					vector.y = -1f;
				}
				bool flag14 = dir.z != 0f && !flag4;
				if (flag14)
				{
					vector.z = (vector2.z - origin.z) / dir.z;
				}
				else
				{
					vector.z = -1f;
				}
				int num = 0;
				float num2 = vector.x;
				bool flag15 = num2 < vector.y;
				if (flag15)
				{
					num = 1;
					num2 = vector.y;
				}
				bool flag16 = num2 < vector.z;
				if (flag16)
				{
					num = 2;
					num2 = vector.z;
				}
				bool flag17 = num2 < 0f;
				if (flag17)
				{
					result = false;
				}
				else
				{
					bool flag18 = num != 0;
					if (flag18)
					{
						point.x = origin.x + num2 * dir.x;
						bool flag19 = point.x < minB.x || point.x > maxB.x;
						if (flag19)
						{
							return false;
						}
					}
					else
					{
						point.x = vector2.x;
					}
					bool flag20 = num != 1;
					if (flag20)
					{
						point.y = origin.y + num2 * dir.y;
						bool flag21 = point.y < minB.y || point.y > maxB.y;
						if (flag21)
						{
							return false;
						}
					}
					else
					{
						point.y = vector2.y;
					}
					bool flag22 = num != 2;
					if (flag22)
					{
						point.z = origin.z + num2 * dir.z;
						bool flag23 = point.z < minB.z || point.z > maxB.z;
						if (flag23)
						{
							return false;
						}
					}
					else
					{
						point.z = vector2.z;
					}
					float sqrMagnitude = (origin - point).sqrMagnitude;
					bool flag24 = sqrMagnitude <= sqrMaxdistance;
					if (flag24)
					{
						switch (num)
						{
						case 0:
							normal = ((origin.x > point.x) ? Vector3.right : Vector3.left);
							break;
						case 1:
							normal = ((origin.y > point.y) ? Vector3.up : Vector3.down);
							break;
						case 2:
							normal = ((origin.z > point.z) ? Vector3.forward : Vector3.back);
							break;
						}
						distance = Mathf.Sqrt(sqrMagnitude);
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		internal static bool RaySphereIntersection(Vector3 p1, Vector3 dir, float length, Vector3 center, float radius, out Vector3 hitPoint, out Vector3 normal, out float distance)
		{
			float num = radius * radius;
			Vector3 lhs = p1 - center;
			float sqrMagnitude = lhs.sqrMagnitude;
			bool flag = sqrMagnitude < num;
			bool result;
			if (flag)
			{
				hitPoint = default(Vector3);
				normal = default(Vector3);
				distance = 0f;
				result = false;
			}
			else
			{
				bool flag2 = length < float.Epsilon;
				if (flag2)
				{
					hitPoint = default(Vector3);
					normal = default(Vector3);
					distance = 0f;
					result = false;
				}
				else
				{
					float num2 = Vector3.Dot(lhs, -dir);
					bool flag3 = num2 < 0f;
					if (flag3)
					{
						hitPoint = default(Vector3);
						normal = default(Vector3);
						distance = 0f;
						result = false;
					}
					else
					{
						Vector3 vector = p1 + dir * num2;
						float sqrMagnitude2 = (center - vector).sqrMagnitude;
						bool flag4 = sqrMagnitude2 > num;
						if (flag4)
						{
							hitPoint = default(Vector3);
							normal = default(Vector3);
							distance = 0f;
							result = false;
						}
						else
						{
							float num3 = Mathf.Sqrt(num - sqrMagnitude2);
							hitPoint = vector - dir * num3;
							distance = num2 - num3;
							bool flag5 = length < distance;
							if (flag5)
							{
								normal = default(Vector3);
								distance = 0f;
								result = false;
							}
							else
							{
								normal = (hitPoint - center).normalized;
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		private const float ALLOWED_DOT_DIFF = 0.975f;

		private const float EXTENTS_EXPANSION_MULTIPLIER = 1.025f;

		private const float MIN_CROSS_THRESHOLD = 0.0001f;

		internal struct CustomPlanesBox
		{
			public LagCompensationUtils.CustomPlane P0;

			public LagCompensationUtils.CustomPlane P1;

			public LagCompensationUtils.CustomPlane P2;

			public LagCompensationUtils.CustomPlane P3;

			public LagCompensationUtils.CustomPlane P4;

			public LagCompensationUtils.CustomPlane P5;
		}

		internal struct CustomPlane
		{
			public CustomPlane(Vector3 normal, Vector3 pointOnPlane)
			{
				this.Normal = normal;
				this.PointOnPlane = pointOnPlane;
			}

			public Vector3 Normal;

			public Vector3 PointOnPlane;
		}

		internal struct CustomEdgesBox
		{
			public LagCompensationUtils.CustomLine E00
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return new LagCompensationUtils.CustomLine(this.P0, this.P1);
				}
			}

			public LagCompensationUtils.CustomLine E01
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return new LagCompensationUtils.CustomLine(this.P1, this.P2);
				}
			}

			public LagCompensationUtils.CustomLine E02
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return new LagCompensationUtils.CustomLine(this.P2, this.P3);
				}
			}

			public LagCompensationUtils.CustomLine E03
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return new LagCompensationUtils.CustomLine(this.P3, this.P0);
				}
			}

			public LagCompensationUtils.CustomLine E04
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return new LagCompensationUtils.CustomLine(this.P4, this.P5);
				}
			}

			public LagCompensationUtils.CustomLine E05
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return new LagCompensationUtils.CustomLine(this.P5, this.P6);
				}
			}

			public LagCompensationUtils.CustomLine E06
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return new LagCompensationUtils.CustomLine(this.P6, this.P7);
				}
			}

			public LagCompensationUtils.CustomLine E07
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return new LagCompensationUtils.CustomLine(this.P7, this.P4);
				}
			}

			public LagCompensationUtils.CustomLine E08
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return new LagCompensationUtils.CustomLine(this.P4, this.P0);
				}
			}

			public LagCompensationUtils.CustomLine E09
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return new LagCompensationUtils.CustomLine(this.P5, this.P1);
				}
			}

			public LagCompensationUtils.CustomLine E10
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return new LagCompensationUtils.CustomLine(this.P6, this.P2);
				}
			}

			public LagCompensationUtils.CustomLine E11
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return new LagCompensationUtils.CustomLine(this.P7, this.P3);
				}
			}

			public Vector3 P0;

			public Vector3 P1;

			public Vector3 P2;

			public Vector3 P3;

			public Vector3 P4;

			public Vector3 P5;

			public Vector3 P6;

			public Vector3 P7;
		}

		internal struct CustomLine
		{
			public CustomLine(Vector3 start, Vector3 end)
			{
				this.Start = start;
				this.End = end;
			}

			public Vector3 Start;

			public Vector3 End;
		}

		private struct RotationMatrix
		{
			public float M00;

			public float M01;

			public float M02;

			public float M10;

			public float M11;

			public float M12;

			public float M20;

			public float M21;

			public float M22;
		}

		internal struct BoxNarrowData
		{
			public BoxNarrowData(Vector3 pos, Quaternion rot, Vector3 extents)
			{
				this.Position = pos;
				this.Extents = extents;
				this.RotatedRight = rot * Vector3.right;
				this.RotatedUp = rot * Vector3.up;
				this.RotatedForward = rot * Vector3.forward;
				this.BoxEdgesRotated.P0 = rot * new Vector3(-extents.x, extents.y, extents.z);
				this.BoxEdgesRotated.P1 = rot * new Vector3(extents.x, extents.y, extents.z);
				this.BoxEdgesRotated.P2 = rot * new Vector3(extents.x, extents.y, -extents.z);
				this.BoxEdgesRotated.P3 = rot * new Vector3(-extents.x, extents.y, -extents.z);
				this.BoxEdgesRotated.P4 = rot * new Vector3(-extents.x, -extents.y, extents.z);
				this.BoxEdgesRotated.P5 = rot * new Vector3(extents.x, -extents.y, extents.z);
				this.BoxEdgesRotated.P6 = rot * new Vector3(extents.x, -extents.y, -extents.z);
				this.BoxEdgesRotated.P7 = rot * new Vector3(-extents.x, -extents.y, -extents.z);
				this.BoxPlanesRotated.P0.Normal = this.RotatedRight;
				this.BoxPlanesRotated.P0.PointOnPlane.x = this.RotatedRight.x * extents.x;
				this.BoxPlanesRotated.P0.PointOnPlane.y = this.RotatedRight.y * extents.x;
				this.BoxPlanesRotated.P0.PointOnPlane.z = this.RotatedRight.z * extents.x;
				this.BoxPlanesRotated.P1.Normal.x = -this.RotatedRight.x;
				this.BoxPlanesRotated.P1.Normal.y = -this.RotatedRight.y;
				this.BoxPlanesRotated.P1.Normal.z = -this.RotatedRight.z;
				this.BoxPlanesRotated.P1.PointOnPlane.x = -this.BoxPlanesRotated.P0.PointOnPlane.x;
				this.BoxPlanesRotated.P1.PointOnPlane.y = -this.BoxPlanesRotated.P0.PointOnPlane.y;
				this.BoxPlanesRotated.P1.PointOnPlane.z = -this.BoxPlanesRotated.P0.PointOnPlane.z;
				this.BoxPlanesRotated.P2.Normal = this.RotatedUp;
				this.BoxPlanesRotated.P2.PointOnPlane.x = this.RotatedUp.x * extents.y;
				this.BoxPlanesRotated.P2.PointOnPlane.y = this.RotatedUp.y * extents.y;
				this.BoxPlanesRotated.P2.PointOnPlane.z = this.RotatedUp.z * extents.y;
				this.BoxPlanesRotated.P3.Normal.x = -this.RotatedUp.x;
				this.BoxPlanesRotated.P3.Normal.y = -this.RotatedUp.y;
				this.BoxPlanesRotated.P3.Normal.z = -this.RotatedUp.z;
				this.BoxPlanesRotated.P3.PointOnPlane.x = -this.BoxPlanesRotated.P2.PointOnPlane.x;
				this.BoxPlanesRotated.P3.PointOnPlane.y = -this.BoxPlanesRotated.P2.PointOnPlane.y;
				this.BoxPlanesRotated.P3.PointOnPlane.z = -this.BoxPlanesRotated.P2.PointOnPlane.z;
				this.BoxPlanesRotated.P4.Normal = this.RotatedForward;
				this.BoxPlanesRotated.P4.PointOnPlane.x = this.RotatedForward.x * extents.z;
				this.BoxPlanesRotated.P4.PointOnPlane.y = this.RotatedForward.y * extents.z;
				this.BoxPlanesRotated.P4.PointOnPlane.z = this.RotatedForward.z * extents.z;
				this.BoxPlanesRotated.P5.Normal.x = -this.RotatedForward.x;
				this.BoxPlanesRotated.P5.Normal.y = -this.RotatedForward.y;
				this.BoxPlanesRotated.P5.Normal.z = -this.RotatedForward.z;
				this.BoxPlanesRotated.P5.PointOnPlane.x = -this.BoxPlanesRotated.P4.PointOnPlane.x;
				this.BoxPlanesRotated.P5.PointOnPlane.y = -this.BoxPlanesRotated.P4.PointOnPlane.y;
				this.BoxPlanesRotated.P5.PointOnPlane.z = -this.BoxPlanesRotated.P4.PointOnPlane.z;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Vector3 LocalToWorldPoint(Vector3 point)
			{
				return this.LocalToWorldVector(point) + this.Position;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Vector3 WorldToLocalPoint(Vector3 point)
			{
				return this.WorldToLocalVector(point - this.Position);
			}

			public Vector3 LocalToWorldVector(Vector3 vec)
			{
				Vector3 result;
				result.x = this.RotatedRight.x * vec.x + this.RotatedUp.x * vec.y + this.RotatedForward.x * vec.z;
				result.y = this.RotatedRight.y * vec.x + this.RotatedUp.y * vec.y + this.RotatedForward.y * vec.z;
				result.z = this.RotatedRight.z * vec.x + this.RotatedUp.z * vec.y + this.RotatedForward.z * vec.z;
				return result;
			}

			public Vector3 WorldToLocalVector(Vector3 vec)
			{
				Vector3 result;
				result.x = this.RotatedRight.x * vec.x + this.RotatedRight.y * vec.y + this.RotatedRight.z * vec.z;
				result.y = this.RotatedUp.x * vec.x + this.RotatedUp.y * vec.y + this.RotatedUp.z * vec.z;
				result.z = this.RotatedForward.x * vec.x + this.RotatedForward.y * vec.y + this.RotatedForward.z * vec.z;
				return result;
			}

			public Vector3 Position;

			public Vector3 Extents;

			public Vector3 RotatedRight;

			public Vector3 RotatedUp;

			public Vector3 RotatedForward;

			public LagCompensationUtils.CustomPlanesBox BoxPlanesRotated;

			public LagCompensationUtils.CustomEdgesBox BoxEdgesRotated;
		}

		public struct ContactData
		{
			public Vector3 Point;

			public Vector3 Normal;

			public float Penetration;
		}
	}
}
