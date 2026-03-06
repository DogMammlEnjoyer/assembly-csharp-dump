using System;
using System.Collections.Generic;
using System.IO;

namespace g3
{
	public class TransformSequence
	{
		public TransformSequence()
		{
			this.Operations = new List<TransformSequence.XForm>();
		}

		public TransformSequence(TransformSequence copy)
		{
			this.Operations = new List<TransformSequence.XForm>(copy.Operations);
		}

		public void Append(TransformSequence sequence)
		{
			this.Operations.AddRange(sequence.Operations);
		}

		public void AppendTranslation(Vector3d dv)
		{
			this.Operations.Add(new TransformSequence.XForm
			{
				type = TransformSequence.XFormType.Translation,
				data = new Vector3dTuple3(dv, Vector3d.Zero, Vector3d.Zero)
			});
		}

		public void AppendTranslation(double dx, double dy, double dz)
		{
			this.Operations.Add(new TransformSequence.XForm
			{
				type = TransformSequence.XFormType.Translation,
				data = new Vector3dTuple3(new Vector3d(dx, dy, dz), Vector3d.Zero, Vector3d.Zero)
			});
		}

		public void AppendRotation(Quaternionf q)
		{
			this.Operations.Add(new TransformSequence.XForm
			{
				type = TransformSequence.XFormType.QuaterionRotation,
				data = new Vector3dTuple3(new Vector3d((double)q.x, (double)q.y, (double)q.z), new Vector3d((double)q.w, 0.0, 0.0), Vector3d.Zero)
			});
		}

		public void AppendRotation(Quaternionf q, Vector3d aroundPt)
		{
			this.Operations.Add(new TransformSequence.XForm
			{
				type = TransformSequence.XFormType.QuaternionRotateAroundPoint,
				data = new Vector3dTuple3(new Vector3d((double)q.x, (double)q.y, (double)q.z), new Vector3d((double)q.w, 0.0, 0.0), aroundPt)
			});
		}

		public void AppendScale(Vector3d s)
		{
			this.Operations.Add(new TransformSequence.XForm
			{
				type = TransformSequence.XFormType.Scale,
				data = new Vector3dTuple3(s, Vector3d.Zero, Vector3d.Zero)
			});
		}

		public void AppendScale(Vector3d s, Vector3d aroundPt)
		{
			this.Operations.Add(new TransformSequence.XForm
			{
				type = TransformSequence.XFormType.ScaleAroundPoint,
				data = new Vector3dTuple3(s, Vector3d.Zero, aroundPt)
			});
		}

		public void AppendToFrame(Frame3f frame)
		{
			Quaternionf rotation = frame.Rotation;
			this.Operations.Add(new TransformSequence.XForm
			{
				type = TransformSequence.XFormType.ToFrame,
				data = new Vector3dTuple3(new Vector3d((double)rotation.x, (double)rotation.y, (double)rotation.z), new Vector3d((double)rotation.w, 0.0, 0.0), frame.Origin)
			});
		}

		public void AppendFromFrame(Frame3f frame)
		{
			Quaternionf rotation = frame.Rotation;
			this.Operations.Add(new TransformSequence.XForm
			{
				type = TransformSequence.XFormType.FromFrame,
				data = new Vector3dTuple3(new Vector3d((double)rotation.x, (double)rotation.y, (double)rotation.z), new Vector3d((double)rotation.w, 0.0, 0.0), frame.Origin)
			});
		}

		public Vector3d TransformP(Vector3d p)
		{
			int count = this.Operations.Count;
			for (int i = 0; i < count; i++)
			{
				switch (this.Operations[i].type)
				{
				case TransformSequence.XFormType.Translation:
					p += this.Operations[i].Translation;
					break;
				case TransformSequence.XFormType.QuaterionRotation:
					p = this.Operations[i].Quaternion * p;
					break;
				case TransformSequence.XFormType.QuaternionRotateAroundPoint:
					p -= this.Operations[i].RotateOrigin;
					p = this.Operations[i].Quaternion * p;
					p += this.Operations[i].RotateOrigin;
					break;
				case TransformSequence.XFormType.Scale:
					p *= this.Operations[i].Scale;
					break;
				case TransformSequence.XFormType.ScaleAroundPoint:
					p -= this.Operations[i].RotateOrigin;
					p *= this.Operations[i].Scale;
					p += this.Operations[i].RotateOrigin;
					break;
				case TransformSequence.XFormType.ToFrame:
					p = this.Operations[i].Frame.ToFrameP(ref p);
					break;
				case TransformSequence.XFormType.FromFrame:
					p = this.Operations[i].Frame.FromFrameP(ref p);
					break;
				default:
					throw new NotImplementedException("TransformSequence.TransformP: unhandled type!");
				}
			}
			return p;
		}

		public Vector3d TransformV(Vector3d v)
		{
			int count = this.Operations.Count;
			for (int i = 0; i < count; i++)
			{
				switch (this.Operations[i].type)
				{
				case TransformSequence.XFormType.Translation:
					break;
				case TransformSequence.XFormType.QuaterionRotation:
				case TransformSequence.XFormType.QuaternionRotateAroundPoint:
					v = this.Operations[i].Quaternion * v;
					break;
				case TransformSequence.XFormType.Scale:
				case TransformSequence.XFormType.ScaleAroundPoint:
					v *= this.Operations[i].Scale;
					break;
				case TransformSequence.XFormType.ToFrame:
					v = this.Operations[i].Frame.ToFrameV(ref v);
					break;
				case TransformSequence.XFormType.FromFrame:
					v = this.Operations[i].Frame.FromFrameV(ref v);
					break;
				default:
					throw new NotImplementedException("TransformSequence.TransformV: unhandled type!");
				}
			}
			return v;
		}

		public Vector3f TransformP(Vector3f p)
		{
			return (Vector3f)this.TransformP(p);
		}

		public TransformSequence MakeInverse()
		{
			TransformSequence transformSequence = new TransformSequence();
			for (int i = this.Operations.Count - 1; i >= 0; i--)
			{
				switch (this.Operations[i].type)
				{
				case TransformSequence.XFormType.Translation:
					transformSequence.AppendTranslation(-this.Operations[i].Translation);
					break;
				case TransformSequence.XFormType.QuaterionRotation:
					transformSequence.AppendRotation(this.Operations[i].Quaternion.Inverse());
					break;
				case TransformSequence.XFormType.QuaternionRotateAroundPoint:
					transformSequence.AppendRotation(this.Operations[i].Quaternion.Inverse(), this.Operations[i].RotateOrigin);
					break;
				case TransformSequence.XFormType.Scale:
					transformSequence.AppendScale(1.0 / this.Operations[i].Scale);
					break;
				case TransformSequence.XFormType.ScaleAroundPoint:
					transformSequence.AppendScale(1.0 / this.Operations[i].Scale, this.Operations[i].RotateOrigin);
					break;
				case TransformSequence.XFormType.ToFrame:
					transformSequence.AppendFromFrame(this.Operations[i].Frame);
					break;
				case TransformSequence.XFormType.FromFrame:
					transformSequence.AppendToFrame(this.Operations[i].Frame);
					break;
				default:
					throw new NotImplementedException("TransformSequence.MakeInverse: unhandled type!");
				}
			}
			return transformSequence;
		}

		public void Store(BinaryWriter writer)
		{
			writer.Write(1);
			writer.Write(this.Operations.Count);
			for (int i = 0; i < this.Operations.Count; i++)
			{
				writer.Write((int)this.Operations[i].type);
				gSerialization.Store(this.Operations[i].data.V0, writer);
				gSerialization.Store(this.Operations[i].data.V1, writer);
				gSerialization.Store(this.Operations[i].data.V2, writer);
			}
		}

		public void Restore(BinaryReader reader)
		{
			if (reader.ReadInt32() != 1)
			{
				throw new Exception("TransformSequence.Restore: unknown version number!");
			}
			int num = reader.ReadInt32();
			this.Operations = new List<TransformSequence.XForm>();
			for (int i = 0; i < num; i++)
			{
				int type = reader.ReadInt32();
				TransformSequence.XForm item = new TransformSequence.XForm
				{
					type = (TransformSequence.XFormType)type
				};
				gSerialization.Restore(ref item.data.V0, reader);
				gSerialization.Restore(ref item.data.V1, reader);
				gSerialization.Restore(ref item.data.V2, reader);
				this.Operations.Add(item);
			}
		}

		private List<TransformSequence.XForm> Operations;

		private enum XFormType
		{
			Translation,
			QuaterionRotation,
			QuaternionRotateAroundPoint,
			Scale,
			ScaleAroundPoint,
			ToFrame,
			FromFrame
		}

		private struct XForm
		{
			public Vector3d Translation
			{
				get
				{
					return this.data.V0;
				}
			}

			public Vector3d Scale
			{
				get
				{
					return this.data.V0;
				}
			}

			public Quaternionf Quaternion
			{
				get
				{
					return new Quaternionf((float)this.data.V0.x, (float)this.data.V0.y, (float)this.data.V0.z, (float)this.data.V1.x);
				}
			}

			public Vector3d RotateOrigin
			{
				get
				{
					return this.data.V2;
				}
			}

			public Frame3f Frame
			{
				get
				{
					return new Frame3f((Vector3f)this.RotateOrigin, this.Quaternion);
				}
			}

			public TransformSequence.XFormType type;

			public Vector3dTuple3 data;
		}
	}
}
