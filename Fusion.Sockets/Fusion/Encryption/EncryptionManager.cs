using System;
using System.Collections.Generic;

namespace Fusion.Encryption
{
	internal class EncryptionManager<THandler, TEncryption> : IDisposable where THandler : IEquatable<THandler> where TEncryption : IDataEncryption, new()
	{
		public void Dispose()
		{
			TraceLogStream logTraceEncryption = InternalLogStreams.LogTraceEncryption;
			if (logTraceEncryption != null)
			{
				logTraceEncryption.Log("Disposing EncryptionManager...");
			}
			foreach (TEncryption tencryption in this._cyphers.Values)
			{
				tencryption.Dispose();
			}
			this._cyphers.Clear();
		}

		public void RegisterEncryptionKey(THandler handle, byte[] key)
		{
			bool flag = this.HasEncryptionForHandle(handle);
			if (flag)
			{
				TraceLogStream logTraceEncryption = InternalLogStreams.LogTraceEncryption;
				if (logTraceEncryption != null)
				{
					logTraceEncryption.Warn(string.Format("RegisterEncryptionKey: handle={0} already registered", handle));
				}
			}
			else
			{
				TEncryption value = Activator.CreateInstance<TEncryption>();
				value.Setup(key);
				this._cyphers[handle] = value;
				TraceLogStream logTraceEncryption2 = InternalLogStreams.LogTraceEncryption;
				if (logTraceEncryption2 != null)
				{
					logTraceEncryption2.Log(string.Format("RegisterEncryptionKey: handle={0} key={1}", handle, BinUtils.BytesToHex(key, 16)));
				}
			}
		}

		public void DeleteEncryptionKey(THandler handle)
		{
			TEncryption tencryption;
			bool flag = this._cyphers.TryGetValue(handle, out tencryption);
			if (flag)
			{
				this._cyphers.Remove(handle);
				tencryption.Dispose();
				TraceLogStream logTraceEncryption = InternalLogStreams.LogTraceEncryption;
				if (logTraceEncryption != null)
				{
					logTraceEncryption.Log(string.Format("DeleteEncryptionKey: handle={0}", handle));
				}
			}
		}

		public bool HasEncryptionForHandle(THandler handle)
		{
			return this._cyphers.ContainsKey(handle);
		}

		public unsafe bool Wrap(THandler handle, byte* buffer, ref int length, int capacity)
		{
			EngineProfiler.Begin("Encryption.Wrap");
			bool result = this.Encrypt(handle, buffer, ref length, capacity) && this.ComputeHash(handle, buffer, ref length, capacity);
			EngineProfiler.End();
			return result;
		}

		public unsafe bool Unwrap(THandler handle, byte* buffer, ref int length, int capacity)
		{
			EngineProfiler.Begin("Encryption.Unwrap");
			bool result = this.VerifyHash(handle, buffer, ref length, capacity) && this.Decrypt(handle, buffer, ref length, capacity);
			EngineProfiler.End();
			return result;
		}

		public byte[] GenerateKey()
		{
			TEncryption tencryption = Activator.CreateInstance<TEncryption>();
			return tencryption.GenerateKey();
		}

		public unsafe bool ComputeHash(THandler handle, byte* buffer, ref int length, int capacity)
		{
			TEncryption tencryption;
			bool flag = this._cyphers.TryGetValue(handle, out tencryption);
			bool result;
			if (flag)
			{
				result = tencryption.ComputeHash(buffer, ref length, capacity);
			}
			else
			{
				TraceLogStream logTraceEncryption = InternalLogStreams.LogTraceEncryption;
				if (logTraceEncryption != null)
				{
					logTraceEncryption.Warn(string.Format("ComputeHash: handle={0} not found", handle));
				}
				result = false;
			}
			return result;
		}

		public unsafe bool VerifyHash(THandler handle, byte* buffer, ref int length, int capacity)
		{
			TEncryption tencryption;
			bool flag = this._cyphers.TryGetValue(handle, out tencryption);
			bool result;
			if (flag)
			{
				result = tencryption.VerifyHash(buffer, ref length, capacity);
			}
			else
			{
				TraceLogStream logTraceEncryption = InternalLogStreams.LogTraceEncryption;
				if (logTraceEncryption != null)
				{
					logTraceEncryption.Warn(string.Format("VerifyHash: handle={0} not found", handle));
				}
				result = false;
			}
			return result;
		}

		public unsafe bool Encrypt(THandler handle, byte* buffer, ref int length, int capacity)
		{
			TEncryption tencryption;
			bool flag = this._cyphers.TryGetValue(handle, out tencryption);
			bool result;
			if (flag)
			{
				result = tencryption.EncryptData(buffer, ref length, capacity);
			}
			else
			{
				TraceLogStream logTraceEncryption = InternalLogStreams.LogTraceEncryption;
				if (logTraceEncryption != null)
				{
					logTraceEncryption.Warn(string.Format("Encrypt: handle={0} not found", handle));
				}
				result = false;
			}
			return result;
		}

		public unsafe bool Decrypt(THandler handle, byte* buffer, ref int length, int capacity)
		{
			TEncryption tencryption;
			bool flag = this._cyphers.TryGetValue(handle, out tencryption);
			bool result;
			if (flag)
			{
				result = tencryption.DecryptData(buffer, ref length, capacity);
			}
			else
			{
				TraceLogStream logTraceEncryption = InternalLogStreams.LogTraceEncryption;
				if (logTraceEncryption != null)
				{
					logTraceEncryption.Warn(string.Format("Decrypt: handle={0} not found", handle));
				}
				result = false;
			}
			return result;
		}

		private readonly Dictionary<THandler, TEncryption> _cyphers = new Dictionary<THandler, TEncryption>();
	}
}
