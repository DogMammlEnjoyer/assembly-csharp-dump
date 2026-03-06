using System;

namespace g3
{
	public struct Frame3f
	{
		public Frame3f(Frame3f copy)
		{
			this.rotation = copy.rotation;
			this.origin = copy.origin;
		}

		public Frame3f(Vector3f origin)
		{
			this.rotation = Quaternionf.Identity;
			this.origin = origin;
		}

		public Frame3f(Vector3d origin)
		{
			this.rotation = Quaternionf.Identity;
			this.origin = (Vector3f)origin;
		}

		public Frame3f(Vector3f origin, Vector3f setZ)
		{
			this.rotation = Quaternionf.FromTo(Vector3f.AxisZ, setZ);
			this.origin = origin;
		}

		public Frame3f(Vector3d origin, Vector3d setZ)
		{
			this.rotation = Quaternionf.FromTo(Vector3f.AxisZ, (Vector3f)setZ);
			this.origin = (Vector3f)origin;
		}

		public Frame3f(Vector3f origin, Vector3f setAxis, int nAxis)
		{
			if (nAxis == 0)
			{
				this.rotation = Quaternionf.FromTo(Vector3f.AxisX, setAxis);
			}
			else if (nAxis == 1)
			{
				this.rotation = Quaternionf.FromTo(Vector3f.AxisY, setAxis);
			}
			else
			{
				this.rotation = Quaternionf.FromTo(Vector3f.AxisZ, setAxis);
			}
			this.origin = origin;
		}

		public Frame3f(Vector3f origin, Quaternionf orientation)
		{
			this.rotation = orientation;
			this.origin = origin;
		}

		public Frame3f(Vector3f origin, Vector3f x, Vector3f y, Vector3f z)
		{
			this.origin = origin;
			Matrix3f matrix3f = new Matrix3f(x, y, z, false);
			this.rotation = matrix3f.ToQuaternion();
		}

		public Quaternionf Rotation
		{
			get
			{
				return this.rotation;
			}
			set
			{
				this.rotation = value;
			}
		}

		public Vector3f Origin
		{
			get
			{
				return this.origin;
			}
			set
			{
				this.origin = value;
			}
		}

		public Vector3f X
		{
			get
			{
				return this.rotation.AxisX;
			}
		}

		public Vector3f Y
		{
			get
			{
				return this.rotation.AxisY;
			}
		}

		public Vector3f Z
		{
			get
			{
				return this.rotation.AxisZ;
			}
		}

		public Vector3f GetAxis(int nAxis)
		{
			if (nAxis == 0)
			{
				return this.rotation * Vector3f.AxisX;
			}
			if (nAxis == 1)
			{
				return this.rotation * Vector3f.AxisY;
			}
			if (nAxis == 2)
			{
				return this.rotation * Vector3f.AxisZ;
			}
			throw new ArgumentOutOfRangeException("nAxis");
		}

		public void Translate(Vector3f v)
		{
			this.origin += v;
		}

		public Frame3f Translated(Vector3f v)
		{
			return new Frame3f(this.origin + v, this.rotation);
		}

		public Frame3f Translated(float fDistance, int nAxis)
		{
			return new Frame3f(this.origin + fDistance * this.GetAxis(nAxis), this.rotation);
		}

		public void Scale(float f)
		{
			this.origin *= f;
		}

		public void Scale(Vector3f scale)
		{
			this.origin *= scale;
		}

		public Frame3f Scaled(float f)
		{
			return new Frame3f(f * this.origin, this.rotation);
		}

		public Frame3f Scaled(Vector3f scale)
		{
			return new Frame3f(scale * this.origin, this.rotation);
		}

		public void Rotate(Quaternionf q)
		{
			this.rotation = q * this.rotation;
		}

		public Frame3f Rotated(Quaternionf q)
		{
			return new Frame3f(this.origin, q * this.rotation);
		}

		public Frame3f Rotated(float fAngle, int nAxis)
		{
			return this.Rotated(new Quaternionf(this.GetAxis(nAxis), fAngle));
		}

		public void RotateAroundAxes(Quaternionf q)
		{
			this.rotation *= q;
		}

		public void RotateAround(Vector3f point, Quaternionf q)
		{
			Vector3f v = q * (this.origin - point);
			this.rotation = q * this.rotation;
			this.origin = point + v;
		}

		public Frame3f RotatedAround(Vector3f point, Quaternionf q)
		{
			Vector3f v = q * (this.origin - point);
			return new Frame3f(point + v, q * this.rotation);
		}

		public void AlignAxis(int nAxis, Vector3f vTo)
		{
			Quaternionf q = Quaternionf.FromTo(this.GetAxis(nAxis), vTo);
			this.Rotate(q);
		}

		public void ConstrainedAlignAxis(int nAxis, Vector3f vTo, Vector3f vAround)
		{
			float angleDeg = MathUtil.PlaneAngleSignedD(this.GetAxis(nAxis), vTo, vAround);
			Quaternionf q = Quaternionf.AxisAngleD(vAround, angleDeg);
			this.Rotate(q);
		}

		public Vector3f ProjectToPlane(Vector3f p, int nNormal)
		{
			Vector3f v = p - this.origin;
			Vector3f axis = this.GetAxis(nNormal);
			return this.origin + (v - v.Dot(axis) * axis);
		}

		public Vector3f FromPlaneUV(Vector2f v, int nPlaneNormalAxis)
		{
			Vector3f v2 = new Vector3f(v[0], v[1], 0f);
			if (nPlaneNormalAxis == 0)
			{
				v2[0] = 0f;
				v2[2] = v[0];
			}
			else if (nPlaneNormalAxis == 1)
			{
				v2[1] = 0f;
				v2[2] = v[1];
			}
			return this.rotation * v2 + this.origin;
		}

		[Obsolete("replaced with FromPlaneUV")]
		public Vector3f FromFrameP(Vector2f v, int nPlaneNormalAxis)
		{
			return this.FromPlaneUV(v, nPlaneNormalAxis);
		}

		public Vector2f ToPlaneUV(Vector3f p, int nNormal)
		{
			int nAxis = 0;
			int nAxis2 = 1;
			if (nNormal == 0)
			{
				nAxis = 2;
			}
			else if (nNormal == 1)
			{
				nAxis2 = 2;
			}
			Vector3f vector3f = p - this.origin;
			float x = vector3f.Dot(this.GetAxis(nAxis));
			float y = vector3f.Dot(this.GetAxis(nAxis2));
			return new Vector2f(x, y);
		}

		[Obsolete("Use explicit ToPlaneUV instead")]
		public Vector2f ToPlaneUV(Vector3f p, int nNormal, int nAxis0 = -1, int nAxis1 = -1)
		{
			if (nAxis0 != -1 || nAxis1 != -1)
			{
				throw new Exception("[RMS] was this being used?");
			}
			return this.ToPlaneUV(p, nNormal);
		}

		public float DistanceToPlane(Vector3f p, int nNormal)
		{
			return Math.Abs((p - this.origin).Dot(this.GetAxis(nNormal)));
		}

		public float DistanceToPlaneSigned(Vector3f p, int nNormal)
		{
			return (p - this.origin).Dot(this.GetAxis(nNormal));
		}

		public Vector3f ToFrameP(Vector3f v)
		{
			v.x -= this.origin.x;
			v.y -= this.origin.y;
			v.z -= this.origin.z;
			return this.rotation.InverseMultiply(ref v);
		}

		public Vector3f ToFrameP(ref Vector3f v)
		{
			Vector3f vector3f = new Vector3f(v.x - this.origin.x, v.y - this.origin.y, v.z - this.origin.z);
			return this.rotation.InverseMultiply(ref vector3f);
		}

		public Vector3d ToFrameP(Vector3d v)
		{
			v.x -= (double)this.origin.x;
			v.y -= (double)this.origin.y;
			v.z -= (double)this.origin.z;
			return this.rotation.InverseMultiply(ref v);
		}

		public Vector3d ToFrameP(ref Vector3d v)
		{
			Vector3d vector3d = new Vector3d(v.x - (double)this.origin.x, v.y - (double)this.origin.y, v.z - (double)this.origin.z);
			return this.rotation.InverseMultiply(ref vector3d);
		}

		public Vector3f FromFrameP(Vector3f v)
		{
			return this.rotation * v + this.origin;
		}

		public Vector3f FromFrameP(ref Vector3f v)
		{
			return this.rotation * v + this.origin;
		}

		public Vector3d FromFrameP(Vector3d v)
		{
			return this.rotation * v + this.origin;
		}

		public Vector3d FromFrameP(ref Vector3d v)
		{
			return this.rotation * v + this.origin;
		}

		public Vector3f ToFrameV(Vector3f v)
		{
			return this.rotation.InverseMultiply(ref v);
		}

		public Vector3f ToFrameV(ref Vector3f v)
		{
			return this.rotation.InverseMultiply(ref v);
		}

		public Vector3d ToFrameV(Vector3d v)
		{
			return this.rotation.InverseMultiply(ref v);
		}

		public Vector3d ToFrameV(ref Vector3d v)
		{
			return this.rotation.InverseMultiply(ref v);
		}

		public Vector3f FromFrameV(Vector3f v)
		{
			return this.rotation * v;
		}

		public Vector3f FromFrameV(ref Vector3f v)
		{
			return this.rotation * v;
		}

		public Vector3d FromFrameV(ref Vector3d v)
		{
			return this.rotation * v;
		}

		public Vector3d FromFrameV(Vector3d v)
		{
			return this.rotation * v;
		}

		public Quaternionf ToFrame(Quaternionf q)
		{
			return Quaternionf.Inverse(this.rotation) * q;
		}

		public Quaternionf ToFrame(ref Quaternionf q)
		{
			return Quaternionf.Inverse(this.rotation) * q;
		}

		public Quaternionf FromFrame(Quaternionf q)
		{
			return this.rotation * q;
		}

		public Quaternionf FromFrame(ref Quaternionf q)
		{
			return this.rotation * q;
		}

		public Ray3f ToFrame(Ray3f r)
		{
			return new Ray3f(this.ToFrameP(ref r.Origin), this.ToFrameV(ref r.Direction), false);
		}

		public Ray3f ToFrame(ref Ray3f r)
		{
			return new Ray3f(this.ToFrameP(ref r.Origin), this.ToFrameV(ref r.Direction), false);
		}

		public Ray3f FromFrame(Ray3f r)
		{
			return new Ray3f(this.FromFrameP(ref r.Origin), this.FromFrameV(ref r.Direction), false);
		}

		public Ray3f FromFrame(ref Ray3f r)
		{
			return new Ray3f(this.FromFrameP(ref r.Origin), this.FromFrameV(ref r.Direction), false);
		}

		public Frame3f ToFrame(Frame3f f)
		{
			return new Frame3f(this.ToFrameP(ref f.origin), this.ToFrame(ref f.rotation));
		}

		public Frame3f ToFrame(ref Frame3f f)
		{
			return new Frame3f(this.ToFrameP(ref f.origin), this.ToFrame(ref f.rotation));
		}

		public Frame3f FromFrame(Frame3f f)
		{
			return new Frame3f(this.FromFrameP(ref f.origin), this.FromFrame(ref f.rotation));
		}

		public Frame3f FromFrame(ref Frame3f f)
		{
			return new Frame3f(this.FromFrameP(ref f.origin), this.FromFrame(ref f.rotation));
		}

		public Box3f ToFrame(ref Box3f box)
		{
			box.Center = this.ToFrameP(ref box.Center);
			box.AxisX = this.ToFrameV(ref box.AxisX);
			box.AxisY = this.ToFrameV(ref box.AxisY);
			box.AxisZ = this.ToFrameV(ref box.AxisZ);
			return box;
		}

		public Box3f FromFrame(ref Box3f box)
		{
			box.Center = this.FromFrameP(ref box.Center);
			box.AxisX = this.FromFrameV(ref box.AxisX);
			box.AxisY = this.FromFrameV(ref box.AxisY);
			box.AxisZ = this.FromFrameV(ref box.AxisZ);
			return box;
		}

		public Box3d ToFrame(ref Box3d box)
		{
			box.Center = this.ToFrameP(ref box.Center);
			box.AxisX = this.ToFrameV(ref box.AxisX);
			box.AxisY = this.ToFrameV(ref box.AxisY);
			box.AxisZ = this.ToFrameV(ref box.AxisZ);
			return box;
		}

		public Box3d FromFrame(ref Box3d box)
		{
			box.Center = this.FromFrameP(ref box.Center);
			box.AxisX = this.FromFrameV(ref box.AxisX);
			box.AxisY = this.FromFrameV(ref box.AxisY);
			box.AxisZ = this.FromFrameV(ref box.AxisZ);
			return box;
		}

		public Vector3f RayPlaneIntersection(Vector3f ray_origin, Vector3f ray_direction, int nAxisAsNormal)
		{
			Vector3f axis = this.GetAxis(nAxisAsNormal);
			float num = -Vector3f.Dot(this.Origin, axis);
			float num2 = Vector3f.Dot(ray_direction, axis);
			if (MathUtil.EpsilonEqual(num2, 0f, 1E-06f))
			{
				return Vector3f.Invalid;
			}
			float f = -(Vector3f.Dot(ray_origin, axis) + num) / num2;
			return ray_origin + f * ray_direction;
		}

		public static Frame3f Interpolate(Frame3f f1, Frame3f f2, float t)
		{
			return new Frame3f(Vector3f.Lerp(f1.origin, f2.origin, t), Quaternionf.Slerp(f1.rotation, f2.rotation, t));
		}

		public bool EpsilonEqual(Frame3f f2, float epsilon)
		{
			return this.origin.EpsilonEqual(f2.origin, epsilon) && this.rotation.EpsilonEqual(f2.rotation, epsilon);
		}

		public override string ToString()
		{
			return this.ToString("F4");
		}

		public string ToString(string fmt)
		{
			return string.Format("[Frame3f: Origin={0}, X={1}, Y={2}, Z={3}]", new object[]
			{
				this.Origin.ToString(fmt),
				this.X.ToString(fmt),
				this.Y.ToString(fmt),
				this.Z.ToString(fmt)
			});
		}

		public static Frame3f SolveMinRotation(Frame3f source, Frame3f target)
		{
			int num = -1;
			int num2 = -1;
			double num3 = 0.0;
			double num4 = 0.0;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					double value = (double)source.GetAxis(i).Dot(target.GetAxis(j));
					double num5 = Math.Abs(value);
					if (num5 > num3)
					{
						num3 = num5;
						num4 = (double)Math.Sign(value);
						num = i;
						num2 = j;
					}
				}
			}
			Frame3f result = source.Rotated(Quaternionf.FromTo(source.GetAxis(num), (float)num4 * target.GetAxis(num2)));
			Vector3f axis = result.GetAxis(num);
			int nAxis = -1;
			int nAxis2 = -1;
			double num6 = 0.0;
			double num7 = 0.0;
			for (int k = 0; k < 3; k++)
			{
				if (k != num)
				{
					for (int l = 0; l < 3; l++)
					{
						if (l != num2)
						{
							double value2 = (double)result.GetAxis(k).Dot(target.GetAxis(l));
							double num8 = Math.Abs(value2);
							if (num8 > num6)
							{
								num6 = num8;
								num7 = (double)Math.Sign(value2);
								nAxis = k;
								nAxis2 = l;
							}
						}
					}
				}
			}
			result.ConstrainedAlignAxis(nAxis, (float)num7 * target.GetAxis(nAxis2), axis);
			return result;
		}

		private Quaternionf rotation;

		private Vector3f origin;

		public static readonly Frame3f Identity = new Frame3f(Vector3f.Zero, Quaternionf.Identity);
	}
}
