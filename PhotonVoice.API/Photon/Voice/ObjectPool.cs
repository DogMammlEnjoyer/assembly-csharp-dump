using System;

namespace Photon.Voice
{
	public abstract class ObjectPool<TType, TInfo> : IDisposable
	{
		protected abstract TType createObject(TInfo info);

		protected abstract void destroyObject(TType obj);

		protected abstract bool infosMatch(TInfo i0, TInfo i1);

		internal string LogPrefix
		{
			get
			{
				return "[ObjectPool] [" + this.name + "]";
			}
		}

		public ObjectPool(int capacity, string name)
		{
			this.capacity = capacity;
			this.name = name;
		}

		public ObjectPool(int capacity, string name, TInfo info)
		{
			this.capacity = capacity;
			this.name = name;
			this.Init(info);
		}

		public void Init(TInfo info)
		{
			lock (this)
			{
				while (this.pos > 0)
				{
					TType[] array = this.freeObj;
					int num = this.pos - 1;
					this.pos = num;
					this.destroyObject(array[num]);
				}
				this.info = info;
				this.freeObj = new TType[this.capacity];
				this.inited = true;
			}
		}

		public TInfo Info
		{
			get
			{
				return this.info;
			}
		}

		public TType AcquireOrCreate()
		{
			lock (this)
			{
				if (this.pos > 0)
				{
					TType[] array = this.freeObj;
					int num = this.pos - 1;
					this.pos = num;
					return array[num];
				}
				if (!this.inited)
				{
					throw new Exception(this.LogPrefix + " not initialized");
				}
			}
			return this.createObject(this.info);
		}

		public TType AcquireOrCreate(TInfo info)
		{
			if (!this.infosMatch(this.info, info))
			{
				this.Init(info);
			}
			return this.AcquireOrCreate();
		}

		public virtual bool Release(TType obj, TInfo objInfo)
		{
			if (this.infosMatch(this.info, objInfo))
			{
				lock (this)
				{
					if (this.pos < this.freeObj.Length)
					{
						TType[] array = this.freeObj;
						int num = this.pos;
						this.pos = num + 1;
						array[num] = obj;
						return true;
					}
				}
			}
			this.destroyObject(obj);
			return false;
		}

		public virtual bool Release(TType obj)
		{
			lock (this)
			{
				if (this.pos < this.freeObj.Length)
				{
					TType[] array = this.freeObj;
					int num = this.pos;
					this.pos = num + 1;
					array[num] = obj;
					return true;
				}
			}
			this.destroyObject(obj);
			return false;
		}

		public void Dispose()
		{
			lock (this)
			{
				while (this.pos > 0)
				{
					TType[] array = this.freeObj;
					int num = this.pos - 1;
					this.pos = num;
					this.destroyObject(array[num]);
				}
				this.freeObj = new TType[0];
			}
		}

		protected int capacity;

		protected TInfo info;

		private TType[] freeObj = new TType[0];

		protected int pos;

		protected string name;

		private bool inited;
	}
}
