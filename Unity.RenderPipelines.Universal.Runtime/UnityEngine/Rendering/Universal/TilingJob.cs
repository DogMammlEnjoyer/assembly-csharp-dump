using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal
{
	[BurstCompile(FloatMode = FloatMode.Default, DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal struct TilingJob : IJobFor
	{
		public void Execute(int jobIndex)
		{
			int num = jobIndex % this.itemsPerTile;
			this.m_ViewIndex = jobIndex / this.itemsPerTile;
			this.m_Offset = jobIndex * this.rangesPerItem;
			this.m_TileYRange = new InclusiveRange(short.MaxValue, short.MinValue);
			for (int i = 0; i < this.rangesPerItem; i++)
			{
				this.tileRanges[this.m_Offset + i] = new InclusiveRange(short.MaxValue, short.MinValue);
			}
			if (num >= this.lights.Length)
			{
				this.TileReflectionProbe(num);
				return;
			}
			if (this.isOrthographic)
			{
				this.TileLightOrthographic(num);
				return;
			}
			this.TileLight(num);
		}

		private void TileLight(int lightIndex)
		{
			TilingJob.<>c__DisplayClass19_0 CS$<>8__locals1;
			CS$<>8__locals1.light = this.lights[lightIndex];
			if (CS$<>8__locals1.light.lightType != LightType.Point && CS$<>8__locals1.light.lightType != LightType.Spot)
			{
				return;
			}
			float4x4 float4x = CS$<>8__locals1.light.localToWorldMatrix;
			CS$<>8__locals1.lightPositionVS = math.mul(this.worldToViews[this.m_ViewIndex], math.float4(float4x.c3.xyz, 1f)).xyz;
			CS$<>8__locals1.lightPositionVS.z = CS$<>8__locals1.lightPositionVS.z * -1f;
			if (CS$<>8__locals1.lightPositionVS.z >= this.near)
			{
				this.ExpandY(CS$<>8__locals1.lightPositionVS);
			}
			CS$<>8__locals1.lightDirectionVS = math.normalize(math.mul(this.worldToViews[this.m_ViewIndex], math.float4(float4x.c2.xyz, 0f)).xyz);
			CS$<>8__locals1.lightDirectionVS.z = CS$<>8__locals1.lightDirectionVS.z * -1f;
			float x = math.radians(CS$<>8__locals1.light.spotAngle * 0.5f);
			float range = CS$<>8__locals1.light.range;
			float num = TilingJob.square(range);
			CS$<>8__locals1.cosHalfAngle = math.cos(x);
			CS$<>8__locals1.coneHeight = CS$<>8__locals1.cosHalfAngle * range;
			float clipRadius = math.sqrt(num - TilingJob.square(this.near - CS$<>8__locals1.lightPositionVS.z));
			float2 yz;
			float2 yz2;
			TilingJob.GetSphereHorizon(CS$<>8__locals1.lightPositionVS.yz, range, this.near, clipRadius, out yz, out yz2);
			float3 @float = math.float3(CS$<>8__locals1.lightPositionVS.x, yz);
			float3 float2 = math.float3(CS$<>8__locals1.lightPositionVS.x, yz2);
			if (TilingJob.<TileLight>g__SpherePointIsValid|19_0(@float, ref CS$<>8__locals1))
			{
				this.ExpandY(@float);
			}
			if (TilingJob.<TileLight>g__SpherePointIsValid|19_0(float2, ref CS$<>8__locals1))
			{
				this.ExpandY(float2);
			}
			float2 float3;
			float2 float4;
			TilingJob.GetSphereHorizon(CS$<>8__locals1.lightPositionVS.xz, range, this.near, clipRadius, out float3, out float4);
			float3 float5 = math.float3(float3.x, CS$<>8__locals1.lightPositionVS.y, float3.y);
			float3 float6 = math.float3(float4.x, CS$<>8__locals1.lightPositionVS.y, float4.y);
			if (TilingJob.<TileLight>g__SpherePointIsValid|19_0(float5, ref CS$<>8__locals1))
			{
				this.ExpandY(float5);
			}
			if (TilingJob.<TileLight>g__SpherePointIsValid|19_0(float6, ref CS$<>8__locals1))
			{
				this.ExpandY(float6);
			}
			if (CS$<>8__locals1.light.lightType == LightType.Spot)
			{
				float num2 = math.sqrt(range * range - CS$<>8__locals1.coneHeight * CS$<>8__locals1.coneHeight);
				float3 float7 = CS$<>8__locals1.lightPositionVS + CS$<>8__locals1.lightDirectionVS * CS$<>8__locals1.coneHeight;
				float3 float8 = (math.abs(math.abs(CS$<>8__locals1.lightDirectionVS.x) - 1f) < 1E-06f) ? math.float3(0f, 1f, 0f) : math.normalize(math.cross(CS$<>8__locals1.lightDirectionVS, math.float3(1f, 0f, 0f)));
				float3 float9 = math.cross(CS$<>8__locals1.lightDirectionVS, float8);
				float2 float10;
				float2 float11;
				TilingJob.GetProjectedCircleHorizon(float7.yz, num2, float8.yz, float9.yz, out float10, out float11);
				float3 float12 = float7 + float10.x * float8 + float10.y * float9;
				float3 float13 = float7 + float11.x * float8 + float11.y * float9;
				if (float12.z >= this.near)
				{
					this.ExpandY(float12);
				}
				if (float13.z >= this.near)
				{
					this.ExpandY(float13);
				}
				float3 float14 = (math.abs(math.abs(CS$<>8__locals1.lightDirectionVS.y) - 1f) < 1E-06f) ? math.float3(1f, 0f, 0f) : math.normalize(math.cross(CS$<>8__locals1.lightDirectionVS, math.float3(0f, 1f, 0f)));
				float3 rhs = math.cross(CS$<>8__locals1.lightDirectionVS, float14);
				float2 float15;
				float2 float16;
				TilingJob.GetProjectedCircleHorizon(float7.xz, num2, float14.xz, rhs.xz, out float15, out float16);
				float3 float17 = float7 + float15.x * float14 + float15.y * rhs;
				float3 float18 = float7 + float16.x * float14 + float16.y * rhs;
				if (float17.z >= this.near)
				{
					this.ExpandY(float17);
				}
				if (float18.z >= this.near)
				{
					this.ExpandY(float18);
				}
				float3 positionVS;
				float3 positionVS2;
				if (TilingJob.GetCircleClipPoints(float7, CS$<>8__locals1.lightDirectionVS, num2, this.near, out positionVS, out positionVS2))
				{
					this.ExpandY(positionVS);
					this.ExpandY(positionVS2);
				}
				float num3 = num2 * math.sqrt(1f - TilingJob.square(CS$<>8__locals1.lightDirectionVS.z));
				bool flag = this.near >= math.min(float7.z - num3, CS$<>8__locals1.lightPositionVS.z) && this.near <= math.max(float7.z + num3, CS$<>8__locals1.lightPositionVS.z);
				float3 float19 = math.cross(CS$<>8__locals1.lightDirectionVS, CS$<>8__locals1.lightPositionVS);
				float19 = ((math.csum(float19) != 0f) ? math.normalize(float19) : math.float3(1f, 0f, 0f));
				float3 float20 = math.cross(CS$<>8__locals1.lightDirectionVS, float19);
				if (flag)
				{
					float r = num2 / CS$<>8__locals1.coneHeight;
					float2 float21 = TilingJob.FindNearConicTangentTheta(CS$<>8__locals1.lightPositionVS.yz, CS$<>8__locals1.lightDirectionVS.yz, r, float19.yz, float20.yz);
					float3 float22 = TilingJob.EvaluateNearConic(this.near, CS$<>8__locals1.lightPositionVS, CS$<>8__locals1.lightDirectionVS, r, float19, float20, float21.x);
					float3 float23 = TilingJob.EvaluateNearConic(this.near, CS$<>8__locals1.lightPositionVS, CS$<>8__locals1.lightDirectionVS, r, float19, float20, float21.y);
					if (TilingJob.<TileLight>g__ConicPointIsValid|19_1(float22, ref CS$<>8__locals1))
					{
						this.ExpandY(float22);
					}
					if (TilingJob.<TileLight>g__ConicPointIsValid|19_1(float23, ref CS$<>8__locals1))
					{
						this.ExpandY(float23);
					}
					float2 float24 = TilingJob.FindNearConicTangentTheta(CS$<>8__locals1.lightPositionVS.xz, CS$<>8__locals1.lightDirectionVS.xz, r, float19.xz, float20.xz);
					float3 float25 = TilingJob.EvaluateNearConic(this.near, CS$<>8__locals1.lightPositionVS, CS$<>8__locals1.lightDirectionVS, r, float19, float20, float24.x);
					float3 float26 = TilingJob.EvaluateNearConic(this.near, CS$<>8__locals1.lightPositionVS, CS$<>8__locals1.lightDirectionVS, r, float19, float20, float24.y);
					if (TilingJob.<TileLight>g__ConicPointIsValid|19_1(float25, ref CS$<>8__locals1))
					{
						this.ExpandY(float25);
					}
					if (TilingJob.<TileLight>g__ConicPointIsValid|19_1(float26, ref CS$<>8__locals1))
					{
						this.ExpandY(float26);
					}
				}
				float3 float27;
				float3 float28;
				TilingJob.GetConeSideTangentPoints(CS$<>8__locals1.lightPositionVS, CS$<>8__locals1.lightDirectionVS, CS$<>8__locals1.cosHalfAngle, num2, CS$<>8__locals1.coneHeight, range, float19, float20, out float27, out float28);
				float3 y = math.float3(0f, 1f, this.viewPlaneBottoms[this.m_ViewIndex]);
				float num4 = math.dot(-CS$<>8__locals1.lightPositionVS, y) / math.dot(float27, y);
				float3 float29 = CS$<>8__locals1.lightPositionVS + float27 * num4;
				if (num4 >= 0f && num4 <= 1f && float29.z >= this.near)
				{
					this.ExpandY(float29);
				}
				float3 y2 = math.float3(0f, 1f, this.viewPlaneTops[this.m_ViewIndex]);
				float num5 = math.dot(-CS$<>8__locals1.lightPositionVS, y2) / math.dot(float27, y2);
				float3 float30 = CS$<>8__locals1.lightPositionVS + float27 * num5;
				if (num5 >= 0f && num5 <= 1f && float30.z >= this.near)
				{
					this.ExpandY(float30);
				}
				this.m_TileYRange.Clamp(0, (short)(this.tileCount.y - 1));
				for (int i = (int)(this.m_TileYRange.start + 1); i <= (int)this.m_TileYRange.end; i++)
				{
					InclusiveRange empty = InclusiveRange.empty;
					float num6 = math.lerp(this.viewPlaneBottoms[this.m_ViewIndex], this.viewPlaneTops[this.m_ViewIndex], (float)i * this.tileScaleInv.y);
					float3 y3 = math.float3(0f, 1f, -num6);
					float num7 = math.dot(-CS$<>8__locals1.lightPositionVS, y3) / math.dot(float27, y3);
					float3 float31 = CS$<>8__locals1.lightPositionVS + float27 * num7;
					if (num7 >= 0f && num7 <= 1f && float31.z >= this.near)
					{
						empty.Expand((short)math.clamp(this.ViewToTileSpace(float31).x, 0f, (float)(this.tileCount.x - 1)));
					}
					float num8 = math.dot(-CS$<>8__locals1.lightPositionVS, y3) / math.dot(float28, y3);
					float3 float32 = CS$<>8__locals1.lightPositionVS + float28 * num8;
					if (num8 >= 0f && num8 <= 1f && float32.z >= this.near)
					{
						empty.Expand((short)math.clamp(this.ViewToTileSpace(float32).x, 0f, (float)(this.tileCount.x - 1)));
					}
					float3 float33;
					float3 float34;
					if (TilingJob.IntersectCircleYPlane(num6, float7, CS$<>8__locals1.lightDirectionVS, float8, float9, num2, out float33, out float34))
					{
						if (float33.z >= this.near)
						{
							empty.Expand((short)math.clamp(this.ViewToTileSpace(float33).x, 0f, (float)(this.tileCount.x - 1)));
						}
						if (float34.z >= this.near)
						{
							empty.Expand((short)math.clamp(this.ViewToTileSpace(float34).x, 0f, (float)(this.tileCount.x - 1)));
						}
					}
					if (flag)
					{
						float y4 = num6 * this.near;
						float r2 = num2 / CS$<>8__locals1.coneHeight;
						float2 float35 = TilingJob.FindNearConicYTheta(this.near, CS$<>8__locals1.lightPositionVS, CS$<>8__locals1.lightDirectionVS, r2, float19, float20, y4);
						float3 float36 = math.float3(TilingJob.EvaluateNearConic(this.near, CS$<>8__locals1.lightPositionVS, CS$<>8__locals1.lightDirectionVS, r2, float19, float20, float35.x).x, y4, this.near);
						float3 float37 = math.float3(TilingJob.EvaluateNearConic(this.near, CS$<>8__locals1.lightPositionVS, CS$<>8__locals1.lightDirectionVS, r2, float19, float20, float35.y).x, y4, this.near);
						if (TilingJob.<TileLight>g__ConicPointIsValid|19_1(float36, ref CS$<>8__locals1))
						{
							empty.Expand((short)math.clamp(this.ViewToTileSpace(float36).x, 0f, (float)(this.tileCount.x - 1)));
						}
						if (TilingJob.<TileLight>g__ConicPointIsValid|19_1(float37, ref CS$<>8__locals1))
						{
							empty.Expand((short)math.clamp(this.ViewToTileSpace(float37).x, 0f, (float)(this.tileCount.x - 1)));
						}
					}
					int num9 = this.m_Offset + 1 + i;
					this.tileRanges[num9] = InclusiveRange.Merge(this.tileRanges[num9], empty);
					this.tileRanges[num9 - 1] = InclusiveRange.Merge(this.tileRanges[num9 - 1], empty);
				}
			}
			this.m_TileYRange.Clamp(0, (short)(this.tileCount.y - 1));
			for (int j = (int)(this.m_TileYRange.start + 1); j <= (int)this.m_TileYRange.end; j++)
			{
				InclusiveRange empty2 = InclusiveRange.empty;
				float y5 = math.lerp(this.viewPlaneBottoms[this.m_ViewIndex], this.viewPlaneTops[this.m_ViewIndex], (float)j * this.tileScaleInv.y);
				float3 float38;
				float3 float39;
				TilingJob.GetSphereYPlaneHorizon(CS$<>8__locals1.lightPositionVS, range, this.near, clipRadius, y5, out float38, out float39);
				if (TilingJob.<TileLight>g__SpherePointIsValid|19_0(float38, ref CS$<>8__locals1))
				{
					empty2.Expand((short)math.clamp(this.ViewToTileSpace(float38).x, 0f, (float)(this.tileCount.x - 1)));
				}
				if (TilingJob.<TileLight>g__SpherePointIsValid|19_0(float39, ref CS$<>8__locals1))
				{
					empty2.Expand((short)math.clamp(this.ViewToTileSpace(float39).x, 0f, (float)(this.tileCount.x - 1)));
				}
				int num10 = this.m_Offset + 1 + j;
				this.tileRanges[num10] = InclusiveRange.Merge(this.tileRanges[num10], empty2);
				this.tileRanges[num10 - 1] = InclusiveRange.Merge(this.tileRanges[num10 - 1], empty2);
			}
			this.tileRanges[this.m_Offset] = this.m_TileYRange;
		}

		private void TileLightOrthographic(int lightIndex)
		{
			TilingJob.<>c__DisplayClass20_0 CS$<>8__locals1;
			CS$<>8__locals1.light = this.lights[lightIndex];
			float4x4 float4x = CS$<>8__locals1.light.localToWorldMatrix;
			CS$<>8__locals1.lightPosVS = math.mul(this.worldToViews[this.m_ViewIndex], math.float4(float4x.c3.xyz, 1f)).xyz;
			CS$<>8__locals1.lightPosVS.z = CS$<>8__locals1.lightPosVS.z * -1f;
			this.ExpandOrthographic(CS$<>8__locals1.lightPosVS);
			CS$<>8__locals1.lightDirVS = math.mul(this.worldToViews[this.m_ViewIndex], math.float4(float4x.c2.xyz, 0f)).xyz;
			CS$<>8__locals1.lightDirVS.z = CS$<>8__locals1.lightDirVS.z * -1f;
			CS$<>8__locals1.lightDirVS = math.normalize(CS$<>8__locals1.lightDirVS);
			float x = math.radians(CS$<>8__locals1.light.spotAngle * 0.5f);
			float range = CS$<>8__locals1.light.range;
			float num = TilingJob.square(range);
			CS$<>8__locals1.cosHalfAngle = math.cos(x);
			float num2 = CS$<>8__locals1.cosHalfAngle * range;
			float num3 = TilingJob.square(num2);
			float num4 = 1f / num2;
			float num5 = TilingJob.square(num4);
			float3 @float = CS$<>8__locals1.lightPosVS - math.float3(0f, range, 0f);
			float3 float2 = CS$<>8__locals1.lightPosVS + math.float3(0f, range, 0f);
			float3 float3 = CS$<>8__locals1.lightPosVS - math.float3(range, 0f, 0f);
			float3 float4 = CS$<>8__locals1.lightPosVS + math.float3(range, 0f, 0f);
			if (TilingJob.<TileLightOrthographic>g__SpherePointIsValid|20_0(@float, ref CS$<>8__locals1))
			{
				this.ExpandOrthographic(@float);
			}
			if (TilingJob.<TileLightOrthographic>g__SpherePointIsValid|20_0(float2, ref CS$<>8__locals1))
			{
				this.ExpandOrthographic(float2);
			}
			if (TilingJob.<TileLightOrthographic>g__SpherePointIsValid|20_0(float3, ref CS$<>8__locals1))
			{
				this.ExpandOrthographic(float3);
			}
			if (TilingJob.<TileLightOrthographic>g__SpherePointIsValid|20_0(float4, ref CS$<>8__locals1))
			{
				this.ExpandOrthographic(float4);
			}
			float3 float5 = CS$<>8__locals1.lightPosVS + CS$<>8__locals1.lightDirVS * num2;
			float num6 = math.sqrt(num - num3);
			float num7 = TilingJob.square(num6);
			float3 float6 = math.normalize(math.float3(0f, 1f, 0f) - CS$<>8__locals1.lightDirVS * CS$<>8__locals1.lightDirVS.y);
			float3 lhs = math.normalize(math.float3(1f, 0f, 0f) - CS$<>8__locals1.lightDirVS * CS$<>8__locals1.lightDirVS.x);
			float3 float7 = float5 - float6 * num6;
			float3 float8 = float5 + float6 * num6;
			if (CS$<>8__locals1.light.lightType == LightType.Spot)
			{
				float3 positionVS = float5 - lhs * num6;
				float3 positionVS2 = float5 + lhs * num6;
				this.ExpandOrthographic(float7);
				this.ExpandOrthographic(float8);
				this.ExpandOrthographic(positionVS);
				this.ExpandOrthographic(positionVS2);
			}
			this.m_TileYRange.Clamp(0, (short)(this.tileCount.y - 1));
			float num8 = 0f;
			float num9 = 0f;
			float num10 = 0f;
			float num11 = 0f;
			if (CS$<>8__locals1.light.lightType == LightType.Spot)
			{
				float lhs2 = num2 + num7 * num4;
				float x2 = math.sqrt(TilingJob.square(num7) * num5 + num7);
				float num12 = math.rcp(math.lengthsq(CS$<>8__locals1.lightDirVS.xy));
				float2 float9 = -num7 * num4 * num12 * CS$<>8__locals1.lightDirVS.xy;
				float2 rhs = math.sqrt((TilingJob.square(x2) - math.lengthsq(float9)) * num12) * math.float2(CS$<>8__locals1.lightDirVS.y, -CS$<>8__locals1.lightDirVS.x);
				float2 lhs3 = CS$<>8__locals1.lightPosVS.xy + lhs2 * CS$<>8__locals1.lightDirVS.xy + float9;
				float2 float10 = lhs3 - rhs;
				float2 float11 = lhs3 + rhs;
				num8 = float10.x - CS$<>8__locals1.lightPosVS.x;
				num9 = math.rcp(float10.y - CS$<>8__locals1.lightPosVS.y);
				num10 = float11.x - CS$<>8__locals1.lightPosVS.x;
				num11 = math.rcp(float11.y - CS$<>8__locals1.lightPosVS.y);
			}
			for (int i = (int)(this.m_TileYRange.start + 1); i <= (int)this.m_TileYRange.end; i++)
			{
				InclusiveRange empty = InclusiveRange.empty;
				float num13 = math.lerp(this.viewPlaneBottoms[this.m_ViewIndex], this.viewPlaneTops[this.m_ViewIndex], (float)i * this.tileScaleInv.y);
				float num14 = math.sqrt(num - TilingJob.square(num13 - CS$<>8__locals1.lightPosVS.y));
				float3 float12 = math.float3(CS$<>8__locals1.lightPosVS.x - num14, num13, CS$<>8__locals1.lightPosVS.z);
				float3 float13 = math.float3(CS$<>8__locals1.lightPosVS.x + num14, num13, CS$<>8__locals1.lightPosVS.z);
				if (TilingJob.<TileLightOrthographic>g__SpherePointIsValid|20_0(float12, ref CS$<>8__locals1))
				{
					this.ExpandRangeOrthographic(ref empty, float12.x);
				}
				if (TilingJob.<TileLightOrthographic>g__SpherePointIsValid|20_0(float13, ref CS$<>8__locals1))
				{
					this.ExpandRangeOrthographic(ref empty, float13.x);
				}
				if (CS$<>8__locals1.light.lightType == LightType.Spot)
				{
					if (num13 >= float7.y && num13 <= float8.y)
					{
						float num15 = (num13 - float5.y) / float6.y;
						float num16 = float5.x + num15 * float6.x;
						float num17 = -CS$<>8__locals1.lightDirVS.z / math.length(math.float3(-CS$<>8__locals1.lightDirVS.z, 0f, CS$<>8__locals1.lightDirVS.x));
						float num18 = math.sqrt(TilingJob.square(num6) - TilingJob.square(num15));
						float xVS = num16 - num18 * num17;
						float xVS2 = num16 + num18 * num17;
						this.ExpandRangeOrthographic(ref empty, xVS);
						this.ExpandRangeOrthographic(ref empty, xVS2);
					}
					float num19 = num13 - CS$<>8__locals1.lightPosVS.y;
					float num20 = num19 * num9;
					float num21 = num19 * num11;
					if (num20 >= 0f && num20 <= 1f)
					{
						this.ExpandRangeOrthographic(ref empty, CS$<>8__locals1.lightPosVS.x + num20 * num8);
					}
					if (num21 >= 0f && num21 <= 1f)
					{
						this.ExpandRangeOrthographic(ref empty, CS$<>8__locals1.lightPosVS.x + num21 * num10);
					}
				}
				int num22 = this.m_Offset + 1 + i;
				this.tileRanges[num22] = InclusiveRange.Merge(this.tileRanges[num22], empty);
				this.tileRanges[num22 - 1] = InclusiveRange.Merge(this.tileRanges[num22 - 1], empty);
			}
			this.tileRanges[this.m_Offset] = this.m_TileYRange;
		}

		private void TileReflectionProbe(int index)
		{
			VisibleReflectionProbe visibleReflectionProbe = this.reflectionProbes[index - this.lights.Length];
			float3 lhs = visibleReflectionProbe.bounds.center;
			float3 lhs2 = visibleReflectionProbe.bounds.extents;
			NativeArray<float3> nativeArray = new NativeArray<float3>(TilingJob.k_CubePoints.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
			NativeArray<float2> nativeArray2 = new NativeArray<float2>(TilingJob.k_CubePoints.Length + TilingJob.k_CubeLineIndices.Length * 3, Allocator.Temp, NativeArrayOptions.ClearMemory);
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < TilingJob.k_CubePoints.Length; i++)
			{
				float3 xyz = math.mul(this.worldToViews[this.m_ViewIndex], math.float4(lhs + lhs2 * TilingJob.k_CubePoints[i], 1f)).xyz;
				xyz.z *= -1f;
				nativeArray[i] = xyz;
				if (xyz.z >= this.near)
				{
					float2 @float = this.isOrthographic ? xyz.xy : (xyz.xy / xyz.z);
					int num3 = num++;
					nativeArray2[num3] = @float;
					if (@float.x < nativeArray2[num2].x)
					{
						num2 = num3;
					}
				}
			}
			for (int j = 0; j < TilingJob.k_CubeLineIndices.Length; j++)
			{
				int4 @int = TilingJob.k_CubeLineIndices[j];
				float3 float2 = nativeArray[@int.x];
				for (int k = 0; k < 3; k++)
				{
					float3 float3 = nativeArray[@int[k + 1]];
					if ((float2.z >= this.near || float3.z >= this.near) && (float2.z < this.near || float3.z < this.near))
					{
						float t = (this.near - float2.z) / (float3.z - float2.z);
						float3 float4 = math.lerp(float2, float3, t);
						float2 float5 = this.isOrthographic ? float4.xy : (float4.xy / float4.z);
						int num4 = num++;
						nativeArray2[num4] = float5;
						if (float5.x < nativeArray2[num2].x)
						{
							num2 = num4;
						}
					}
				}
			}
			NativeArray<float2> nativeArray3 = new NativeArray<float2>(num, Allocator.Temp, NativeArrayOptions.ClearMemory);
			int num5 = 0;
			if (num > 0)
			{
				int num6 = num2;
				do
				{
					float2 float6 = nativeArray2[num6];
					this.ExpandY(math.float3(float6, 1f));
					nativeArray3[num5++] = float6;
					int num7 = 0;
					float2 float7 = nativeArray2[num7] - float6;
					for (int l = 0; l < num; l++)
					{
						float2 float8 = nativeArray2[l] - float6;
						float num8 = math.determinant(math.float2x2(float7, float8));
						if (num7 == num6 || num8 > 0f || (num8 == 0f && math.lengthsq(float8) > math.lengthsq(float7)))
						{
							num7 = l;
							float7 = float8;
						}
					}
					num6 = num7;
				}
				while (num6 != num2 && num5 < num);
				this.m_TileYRange.Clamp(0, (short)(this.tileCount.y - 1));
				for (int m = (int)(this.m_TileYRange.start + 1); m <= (int)this.m_TileYRange.end; m++)
				{
					InclusiveRange empty = InclusiveRange.empty;
					float num9 = math.lerp(this.viewPlaneBottoms[this.m_ViewIndex], this.viewPlaneTops[this.m_ViewIndex], (float)m * this.tileScaleInv.y);
					for (int n = 0; n < num5; n++)
					{
						float2 float9 = nativeArray3[n];
						float2 float10 = nativeArray3[(n + 1) % num5];
						float num10 = (num9 - float9.y) / (float10.y - float9.y);
						if (num10 >= 0f && num10 <= 1f)
						{
							float3 positionVS = math.float3(math.lerp(float9.x, float10.x, num10), num9, 1f);
							float2 float11 = this.isOrthographic ? this.ViewToTileSpaceOrthographic(positionVS) : this.ViewToTileSpace(positionVS);
							empty.Expand((short)math.clamp(float11.x, 0f, (float)(this.tileCount.x - 1)));
						}
					}
					int num11 = this.m_Offset + 1 + m;
					this.tileRanges[num11] = InclusiveRange.Merge(this.tileRanges[num11], empty);
					this.tileRanges[num11 - 1] = InclusiveRange.Merge(this.tileRanges[num11 - 1], empty);
				}
				this.tileRanges[this.m_Offset] = this.m_TileYRange;
			}
			nativeArray3.Dispose();
			nativeArray2.Dispose();
			nativeArray.Dispose();
		}

		private float2 ViewToTileSpace(float3 positionVS)
		{
			return (positionVS.xy / positionVS.z * this.viewToViewportScaleBiases[this.m_ViewIndex].xy + this.viewToViewportScaleBiases[this.m_ViewIndex].zw) * this.tileScale;
		}

		private float2 ViewToTileSpaceOrthographic(float3 positionVS)
		{
			return (positionVS.xy * this.viewToViewportScaleBiases[this.m_ViewIndex].xy + this.viewToViewportScaleBiases[this.m_ViewIndex].zw) * this.tileScale;
		}

		private void ExpandY(float3 positionVS)
		{
			float2 @float = this.ViewToTileSpace(positionVS);
			int num = (int)@float.y;
			int num2 = (int)@float.x;
			this.m_TileYRange.Expand((short)math.clamp(num, 0, this.tileCount.y - 1));
			if (num >= 0 && num < this.tileCount.y && num2 >= 0 && num2 < this.tileCount.x)
			{
				InclusiveRange value = this.tileRanges[this.m_Offset + 1 + num];
				value.Expand((short)num2);
				this.tileRanges[this.m_Offset + 1 + num] = value;
			}
		}

		private void ExpandOrthographic(float3 positionVS)
		{
			float2 @float = this.ViewToTileSpaceOrthographic(positionVS);
			int num = (int)@float.y;
			int num2 = (int)@float.x;
			this.m_TileYRange.Expand((short)math.clamp(num, 0, this.tileCount.y - 1));
			if (num >= 0 && num < this.tileCount.y && num2 >= 0 && num2 < this.tileCount.x)
			{
				InclusiveRange value = this.tileRanges[this.m_Offset + 1 + num];
				value.Expand((short)num2);
				this.tileRanges[this.m_Offset + 1 + num] = value;
			}
		}

		private void ExpandRangeOrthographic(ref InclusiveRange range, float xVS)
		{
			range.Expand((short)math.clamp(this.ViewToTileSpaceOrthographic(xVS).x, 0f, (float)(this.tileCount.x - 1)));
		}

		private static float square(float x)
		{
			return x * x;
		}

		private static void GetSphereHorizon(float2 center, float radius, float near, float clipRadius, out float2 p0, out float2 p1)
		{
			float2 @float = math.normalize(center);
			float num = math.length(center);
			float num2 = math.sqrt(num * num - radius * radius);
			float num3 = num2 * radius / num;
			float2 lhs = @float * (num2 * num3 / radius);
			p0 = math.float2(float.MinValue, 1f);
			p1 = math.float2(float.MaxValue, 1f);
			if (center.y - radius < near)
			{
				p0 = math.float2(center.x + clipRadius, near);
				p1 = math.float2(center.x - clipRadius, near);
			}
			float2 float2 = lhs + math.float2(-@float.y, @float.x) * num3;
			if (TilingJob.square(num) >= TilingJob.square(radius) && float2.y >= near)
			{
				if (float2.x > p0.x)
				{
					p0 = float2;
				}
				if (float2.x < p1.x)
				{
					p1 = float2;
				}
			}
			float2 float3 = lhs + math.float2(@float.y, -@float.x) * num3;
			if (TilingJob.square(num) >= TilingJob.square(radius) && float3.y >= near)
			{
				if (float3.x > p0.x)
				{
					p0 = float3;
				}
				if (float3.x < p1.x)
				{
					p1 = float3;
				}
			}
		}

		private static void GetSphereYPlaneHorizon(float3 center, float sphereRadius, float near, float clipRadius, float y, out float3 left, out float3 right)
		{
			float num = y * near;
			float num2 = math.sqrt(TilingJob.square(clipRadius) - TilingJob.square(num - center.y));
			left = math.float3(center.x - num2, num, near);
			right = math.float3(center.x + num2, num, near);
			float3 @float = math.normalize(math.float3(0f, y, 1f));
			float3 float2 = math.float3(1f, 0f, 0f);
			float x = math.abs(math.dot(math.normalize(math.float3(0f, 1f, -y)), center));
			float2 float3 = math.float2(math.dot(center, @float), math.dot(center, float2));
			float num3 = math.length(float3);
			float2 float4 = float3 / num3;
			float num4 = math.sqrt(TilingJob.square(sphereRadius) - TilingJob.square(x));
			if (TilingJob.square(x) <= TilingJob.square(sphereRadius) && TilingJob.square(num4) <= TilingJob.square(num3))
			{
				float num5 = math.sqrt(TilingJob.square(num3) - TilingJob.square(num4));
				float num6 = num5 * num4 / num3;
				float2 lhs = float4 * (num5 * num6 / num4);
				float2 float5 = lhs + math.float2(float4.y, -float4.x) * num6;
				float2 float6 = lhs + math.float2(-float4.y, float4.x) * num6;
				float3 float7 = float5.x * @float + float5.y * float2;
				if (float7.z >= near)
				{
					left = float7;
				}
				float3 float8 = float6.x * @float + float6.y * float2;
				if (float8.z >= near)
				{
					right = float8;
				}
			}
		}

		private static bool GetCircleClipPoints(float3 circleCenter, float3 circleNormal, float circleRadius, float near, out float3 p0, out float3 p1)
		{
			float3 @float = math.normalize(math.cross(circleNormal, math.float3(0f, 0f, 1f)));
			float3 float2 = math.cross(@float, circleNormal);
			float num = (near - circleCenter.z) / float2.z;
			float3 lhs = circleCenter + float2 * num;
			float rhs = math.sqrt(TilingJob.square(circleRadius) - TilingJob.square(num));
			p0 = lhs + @float * rhs;
			p1 = lhs - @float * rhs;
			return math.abs(num) <= circleRadius;
		}

		private static ValueTuple<float, float> IntersectEllipseLine(float a, float b, float3 line)
		{
			float num = math.rcp(TilingJob.square(line.y) * TilingJob.square(b));
			float num2 = 1f / TilingJob.square(a) + TilingJob.square(line.x) * num;
			float num3 = 2f * line.x * line.z * num;
			float num4 = TilingJob.square(line.z) * num - 1f;
			float num5 = math.sqrt(num3 * num3 - 4f * num2 * num4);
			float item = (-num3 + num5) / (2f * num2);
			float item2 = (-num3 - num5) / (2f * num2);
			return new ValueTuple<float, float>(item, item2);
		}

		private static void GetProjectedCircleHorizon(float2 center, float radius, float2 U, float2 V, out float2 uv1, out float2 uv2)
		{
			float num = math.length(V);
			if (num < 1E-06f)
			{
				uv1 = math.float2(radius, 0f);
				uv2 = math.float2(-radius, 0f);
				return;
			}
			float num2 = math.length(U);
			float num3 = math.rcp(num2);
			float num4 = math.rcp(num);
			float2 y = U * num3;
			float2 y2 = V * num4;
			float num5 = num2 * radius;
			float num6 = num * radius;
			float2 @float = math.float2(math.dot(-center, y), math.dot(-center, y2));
			float3 float2 = math.float3(@float.x / TilingJob.square(num5), @float.y / TilingJob.square(num6), -1f);
			ValueTuple<float, float> valueTuple = TilingJob.IntersectEllipseLine(num5, num6, float2);
			float item = valueTuple.Item1;
			float item2 = valueTuple.Item2;
			uv1 = math.float2(item * num3, (-float2.x / float2.y * item - float2.z / float2.y) * num4);
			uv2 = math.float2(item2 * num3, (-float2.x / float2.y * item2 - float2.z / float2.y) * num4);
		}

		private static bool IntersectCircleYPlane(float y, float3 circleCenter, float3 circleNormal, float3 circleU, float3 circleV, float circleRadius, out float3 p1, out float3 p2)
		{
			p1 = (p2 = 0);
			float rhs = math.dot(circleCenter, circleNormal);
			float3 x = math.float3(1f, y, 1f) * rhs / math.dot(math.float3(1f, y, 1f), circleNormal) - circleCenter;
			float2 @float = math.float2(math.dot(x, circleU), math.dot(x, circleV));
			float3 x2 = math.float3(-1f, y, 1f) * rhs / math.dot(math.float3(-1f, y, 1f), circleNormal) - circleCenter;
			float2 float2 = math.normalize(math.float2(math.dot(x2, circleU), math.dot(x2, circleV)) - @float);
			float2 float3 = math.float2(float2.y, -float2.x);
			float num = math.dot(@float, float3);
			float2 lhs = float3 * num;
			if (num > circleRadius)
			{
				return false;
			}
			float lhs2 = math.sqrt(circleRadius * circleRadius - num * num);
			float2 float4 = lhs + lhs2 * float2;
			float2 float5 = lhs - lhs2 * float2;
			p1 = circleCenter + float4.x * circleU + float4.y * circleV;
			p2 = circleCenter + float5.x * circleU + float5.y * circleV;
			return true;
		}

		private static void GetConeSideTangentPoints(float3 vertex, float3 axis, float cosHalfAngle, float circleRadius, float coneHeight, float range, float3 circleU, float3 circleV, out float3 l1, out float3 l2)
		{
			l1 = (l2 = 0);
			if (math.dot(math.normalize(-vertex), axis) >= cosHalfAngle)
			{
				return;
			}
			float num = -math.dot(vertex, axis);
			if (num == 0f)
			{
				num = 1E-06f;
			}
			float rhs = (num < 0f) ? -1f : 1f;
			float3 @float = vertex + axis * num;
			float num2 = math.abs(num) * circleRadius / coneHeight;
			float3 float2 = math.float3(math.float2(math.dot(circleU, -@float), math.dot(circleV, -@float)), -TilingJob.square(num2));
			float2 float3 = math.float2(-1f, -float2.x / float2.y * -1f - float2.z / float2.y);
			float2 float4 = math.normalize(math.float2(1f, -float2.x / float2.y * 1f - float2.z / float2.y) - float3);
			float2 float5 = math.float2(float4.y, -float4.x);
			float num3 = math.dot(float3, float5);
			float2 lhs = float5 * num3;
			float lhs2 = math.sqrt(num2 * num2 - num3 * num3);
			float2 float6 = lhs + lhs2 * float4;
			float2 float7 = lhs - lhs2 * float4;
			float3 lhs3 = math.normalize(@float + float6.x * circleU + float6.y * circleV - vertex) * rhs;
			float3 lhs4 = math.normalize(@float + float7.x * circleU + float7.y * circleV - vertex) * rhs;
			l1 = lhs3 * range;
			l2 = lhs4 * range;
		}

		private static float3 EvaluateNearConic(float near, float3 o, float3 d, float r, float3 u, float3 v, float theta)
		{
			float lhs = (near - o.z) / (d.z + r * u.z * math.cos(theta) + r * v.z * math.sin(theta));
			return math.float3(o.xy + lhs * (d.xy + r * u.xy * math.cos(theta) + r * v.xy * math.sin(theta)), near);
		}

		private static float2 FindNearConicTangentTheta(float2 o, float2 d, float r, float2 u, float2 v)
		{
			float rhs = math.sqrt(TilingJob.square(d.x) * TilingJob.square(u.y) + TilingJob.square(d.x) * TilingJob.square(v.y) - 2f * d.x * d.y * u.x * u.y - 2f * d.x * d.y * v.x * v.y + TilingJob.square(d.y) * TilingJob.square(u.x) + TilingJob.square(d.y) * TilingJob.square(v.x) - TilingJob.square(r) * TilingJob.square(u.x) * TilingJob.square(v.y) + 2f * TilingJob.square(r) * u.x * u.y * v.x * v.y - TilingJob.square(r) * TilingJob.square(u.y) * TilingJob.square(v.x));
			float rhs2 = d.x * v.y - d.y * v.x - r * u.x * v.y + r * u.y * v.x;
			return 2f * math.atan((-d.x * u.y + d.y * u.x + math.float2(1f, -1f) * rhs) / rhs2);
		}

		private static float2 FindNearConicYTheta(float near, float3 o, float3 d, float r, float3 u, float3 v, float y)
		{
			float rhs = math.sqrt(-TilingJob.square(d.y) * TilingJob.square(o.z) + 2f * TilingJob.square(d.y) * o.z * near - TilingJob.square(d.y) * TilingJob.square(near) + 2f * d.y * d.z * o.y * o.z - 2f * d.y * d.z * o.y * near - 2f * d.y * d.z * o.z * y + 2f * d.y * d.z * y * near - TilingJob.square(d.z) * TilingJob.square(o.y) + 2f * TilingJob.square(d.z) * o.y * y - TilingJob.square(d.z) * TilingJob.square(y) + TilingJob.square(o.y) * TilingJob.square(r) * TilingJob.square(u.z) + TilingJob.square(o.y) * TilingJob.square(r) * TilingJob.square(v.z) - 2f * o.y * o.z * TilingJob.square(r) * u.y * u.z - 2f * o.y * o.z * TilingJob.square(r) * v.y * v.z - 2f * o.y * y * TilingJob.square(r) * TilingJob.square(u.z) - 2f * o.y * y * TilingJob.square(r) * TilingJob.square(v.z) + 2f * o.y * TilingJob.square(r) * u.y * u.z * near + 2f * o.y * TilingJob.square(r) * v.y * v.z * near + TilingJob.square(o.z) * TilingJob.square(r) * TilingJob.square(u.y) + TilingJob.square(o.z) * TilingJob.square(r) * TilingJob.square(v.y) + 2f * o.z * y * TilingJob.square(r) * u.y * u.z + 2f * o.z * y * TilingJob.square(r) * v.y * v.z - 2f * o.z * TilingJob.square(r) * TilingJob.square(u.y) * near - 2f * o.z * TilingJob.square(r) * TilingJob.square(v.y) * near + TilingJob.square(y) * TilingJob.square(r) * TilingJob.square(u.z) + TilingJob.square(y) * TilingJob.square(r) * TilingJob.square(v.z) - 2f * y * TilingJob.square(r) * u.y * u.z * near - 2f * y * TilingJob.square(r) * v.y * v.z * near + TilingJob.square(r) * TilingJob.square(u.y) * TilingJob.square(near) + TilingJob.square(r) * TilingJob.square(v.y) * TilingJob.square(near));
			float rhs2 = d.y * o.z - d.y * near - d.z * o.y + d.z * y + o.y * r * u.z - o.z * r * u.y - y * r * u.z + r * u.y * near;
			return 2f * math.atan((r * (o.y * v.z - o.z * v.y - y * v.z + v.y * near) + math.float2(1f, -1f) * rhs) / rhs2);
		}

		[CompilerGenerated]
		internal static bool <TileLight>g__SpherePointIsValid|19_0(float3 p, ref TilingJob.<>c__DisplayClass19_0 A_1)
		{
			return A_1.light.lightType == LightType.Point || math.dot(math.normalize(p - A_1.lightPositionVS), A_1.lightDirectionVS) >= A_1.cosHalfAngle;
		}

		[CompilerGenerated]
		internal static bool <TileLight>g__ConicPointIsValid|19_1(float3 p, ref TilingJob.<>c__DisplayClass19_0 A_1)
		{
			return math.dot(math.normalize(p - A_1.lightPositionVS), A_1.lightDirectionVS) >= 0f && math.dot(p - A_1.lightPositionVS, A_1.lightDirectionVS) <= A_1.coneHeight;
		}

		[CompilerGenerated]
		internal static bool <TileLightOrthographic>g__SpherePointIsValid|20_0(float3 p, ref TilingJob.<>c__DisplayClass20_0 A_1)
		{
			return A_1.light.lightType == LightType.Point || math.dot(math.normalize(p - A_1.lightPosVS), A_1.lightDirVS) >= A_1.cosHalfAngle;
		}

		[ReadOnly]
		public NativeArray<VisibleLight> lights;

		[ReadOnly]
		public NativeArray<VisibleReflectionProbe> reflectionProbes;

		[NativeDisableParallelForRestriction]
		public NativeArray<InclusiveRange> tileRanges;

		public int itemsPerTile;

		public int rangesPerItem;

		public Fixed2<float4x4> worldToViews;

		public float2 tileScale;

		public float2 tileScaleInv;

		public Fixed2<float> viewPlaneBottoms;

		public Fixed2<float> viewPlaneTops;

		public Fixed2<float4> viewToViewportScaleBiases;

		public int2 tileCount;

		public float near;

		public bool isOrthographic;

		private InclusiveRange m_TileYRange;

		private int m_Offset;

		private int m_ViewIndex;

		private float2 m_CenterOffset;

		private static readonly float3[] k_CubePoints = new float3[]
		{
			new float3(-1f, -1f, -1f),
			new float3(-1f, -1f, 1f),
			new float3(-1f, 1f, -1f),
			new float3(-1f, 1f, 1f),
			new float3(1f, -1f, -1f),
			new float3(1f, -1f, 1f),
			new float3(1f, 1f, -1f),
			new float3(1f, 1f, 1f)
		};

		private static readonly int4[] k_CubeLineIndices = new int4[]
		{
			new int4(0, 4, 2, 1),
			new int4(3, 7, 1, 2),
			new int4(5, 1, 7, 4),
			new int4(6, 2, 4, 7)
		};
	}
}
