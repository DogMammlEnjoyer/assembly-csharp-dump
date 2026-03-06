using System;
using System.Collections.Generic;

namespace g3
{
	public class TransformSequence2 : ITransform2
	{
		public TransformSequence2()
		{
			this.Operations = new List<TransformSequence2.XForm>();
		}

		public TransformSequence2 Translation(Vector2d dv)
		{
			this.Operations.Add(new TransformSequence2.XForm
			{
				type = TransformSequence2.XFormType.Translation,
				data = new Vector2dTuple2(dv, Vector2d.Zero)
			});
			return this;
		}

		public TransformSequence2 Translation(double dx, double dy)
		{
			this.Operations.Add(new TransformSequence2.XForm
			{
				type = TransformSequence2.XFormType.Translation,
				data = new Vector2dTuple2(new Vector2d(dx, dy), Vector2d.Zero)
			});
			return this;
		}

		public TransformSequence2 RotationRad(double angle)
		{
			this.Operations.Add(new TransformSequence2.XForm
			{
				type = TransformSequence2.XFormType.Rotation,
				data = new Vector2dTuple2(new Vector2d(angle, 0.0), Vector2d.Zero)
			});
			return this;
		}

		public TransformSequence2 RotationDeg(double angle)
		{
			return this.RotationRad(angle * 0.017453292519943295);
		}

		public TransformSequence2 RotationRad(double angle, Vector2d aroundPt)
		{
			this.Operations.Add(new TransformSequence2.XForm
			{
				type = TransformSequence2.XFormType.RotateAroundPoint,
				data = new Vector2dTuple2(new Vector2d(angle, 0.0), aroundPt)
			});
			return this;
		}

		public TransformSequence2 RotationDeg(double angle, Vector2d aroundPt)
		{
			return this.RotationRad(angle * 0.017453292519943295, aroundPt);
		}

		public TransformSequence2 Scale(Vector2d s)
		{
			this.Operations.Add(new TransformSequence2.XForm
			{
				type = TransformSequence2.XFormType.Scale,
				data = new Vector2dTuple2(s, Vector2d.Zero)
			});
			return this;
		}

		public TransformSequence2 Scale(Vector2d s, Vector2d aroundPt)
		{
			this.Operations.Add(new TransformSequence2.XForm
			{
				type = TransformSequence2.XFormType.ScaleAroundPoint,
				data = new Vector2dTuple2(s, aroundPt)
			});
			return this;
		}

		public TransformSequence2 Append(ITransform2 t2)
		{
			this.Operations.Add(new TransformSequence2.XForm
			{
				type = TransformSequence2.XFormType.NestedITransform2,
				xform = t2
			});
			return this;
		}

		public Vector2d TransformP(Vector2d p)
		{
			int count = this.Operations.Count;
			int i = 0;
			while (i < count)
			{
				switch (this.Operations[i].type)
				{
				case TransformSequence2.XFormType.Translation:
					p += this.Operations[i].Translation;
					break;
				case TransformSequence2.XFormType.Rotation:
					p = this.Operations[i].Rotation * p;
					break;
				case TransformSequence2.XFormType.RotateAroundPoint:
					p -= this.Operations[i].RotateOrigin;
					p = this.Operations[i].Rotation * p;
					p += this.Operations[i].RotateOrigin;
					break;
				case TransformSequence2.XFormType.Scale:
					p *= this.Operations[i].Scale;
					break;
				case TransformSequence2.XFormType.ScaleAroundPoint:
					p -= this.Operations[i].RotateOrigin;
					p *= this.Operations[i].Scale;
					p += this.Operations[i].RotateOrigin;
					break;
				case (TransformSequence2.XFormType)5:
				case (TransformSequence2.XFormType)6:
				case (TransformSequence2.XFormType)7:
				case (TransformSequence2.XFormType)8:
				case (TransformSequence2.XFormType)9:
					goto IL_189;
				case TransformSequence2.XFormType.NestedITransform2:
					p = this.Operations[i].NestedITransform2.TransformP(p);
					break;
				default:
					goto IL_189;
				}
				i++;
				continue;
				IL_189:
				throw new NotImplementedException("TransformSequence.TransformP: unhandled type!");
			}
			return p;
		}

		public Vector2d TransformN(Vector2d n)
		{
			int count = this.Operations.Count;
			int i = 0;
			while (i < count)
			{
				switch (this.Operations[i].type)
				{
				case TransformSequence2.XFormType.Translation:
					break;
				case TransformSequence2.XFormType.Rotation:
					n = this.Operations[i].Rotation * n;
					break;
				case TransformSequence2.XFormType.RotateAroundPoint:
					n = this.Operations[i].Rotation * n;
					break;
				case TransformSequence2.XFormType.Scale:
					n *= this.Operations[i].Scale;
					break;
				case TransformSequence2.XFormType.ScaleAroundPoint:
					n *= this.Operations[i].Scale;
					break;
				case (TransformSequence2.XFormType)5:
				case (TransformSequence2.XFormType)6:
				case (TransformSequence2.XFormType)7:
				case (TransformSequence2.XFormType)8:
				case (TransformSequence2.XFormType)9:
					goto IL_F5;
				case TransformSequence2.XFormType.NestedITransform2:
					n = this.Operations[i].NestedITransform2.TransformN(n);
					break;
				default:
					goto IL_F5;
				}
				i++;
				continue;
				IL_F5:
				throw new NotImplementedException("TransformSequence.TransformN: unhandled type!");
			}
			return n;
		}

		public double TransformScalar(double s)
		{
			int count = this.Operations.Count;
			int i = 0;
			while (i < count)
			{
				switch (this.Operations[i].type)
				{
				case TransformSequence2.XFormType.Translation:
				case TransformSequence2.XFormType.Rotation:
				case TransformSequence2.XFormType.RotateAroundPoint:
					break;
				case TransformSequence2.XFormType.Scale:
					s *= this.Operations[i].Scale.x;
					break;
				case TransformSequence2.XFormType.ScaleAroundPoint:
					s *= this.Operations[i].Scale.x;
					break;
				case (TransformSequence2.XFormType)5:
				case (TransformSequence2.XFormType)6:
				case (TransformSequence2.XFormType)7:
				case (TransformSequence2.XFormType)8:
				case (TransformSequence2.XFormType)9:
					goto IL_B5;
				case TransformSequence2.XFormType.NestedITransform2:
					s = this.Operations[i].NestedITransform2.TransformScalar(s);
					break;
				default:
					goto IL_B5;
				}
				i++;
				continue;
				IL_B5:
				throw new NotImplementedException("TransformSequence.TransformScalar: unhandled type!");
			}
			return s;
		}

		private List<TransformSequence2.XForm> Operations;

		private enum XFormType
		{
			Translation,
			Rotation,
			RotateAroundPoint,
			Scale,
			ScaleAroundPoint,
			NestedITransform2 = 10
		}

		private struct XForm
		{
			public Vector2d Translation
			{
				get
				{
					return this.data.V0;
				}
			}

			public Vector2d Scale
			{
				get
				{
					return this.data.V0;
				}
			}

			public Matrix2d Rotation
			{
				get
				{
					return new Matrix2d(this.data.V0.x, false);
				}
			}

			public Vector2d RotateOrigin
			{
				get
				{
					return this.data.V1;
				}
			}

			public bool ScaleIsUniform
			{
				get
				{
					return this.data.V0.EpsilonEqual(this.data.V1, 1.1920928955078125E-07);
				}
			}

			public ITransform2 NestedITransform2
			{
				get
				{
					return this.xform as ITransform2;
				}
			}

			public TransformSequence2.XFormType type;

			public Vector2dTuple2 data;

			public object xform;
		}
	}
}
