using System;

public struct OVRSpaceUser : IDisposable
{
	public static bool TryCreate(ulong platformUserId, out OVRSpaceUser spaceUser)
	{
		spaceUser = default(OVRSpaceUser);
		return OVRPlugin.CreateSpaceUser(platformUserId, out spaceUser._handle);
	}

	public static bool TryCreate(string platformUserId, out OVRSpaceUser spaceUser)
	{
		ulong platformUserId2;
		if (ulong.TryParse(platformUserId, out platformUserId2))
		{
			return OVRSpaceUser.TryCreate(platformUserId2, out spaceUser);
		}
		spaceUser = default(OVRSpaceUser);
		return false;
	}

	public bool Valid
	{
		get
		{
			return this._handle != 0UL && this.Id > 0UL;
		}
	}

	[Obsolete("Constructor ignores validation. Use TryCreate(*) methods instead.", false)]
	public OVRSpaceUser(ulong spaceUserId)
	{
		OVRPlugin.CreateSpaceUser(spaceUserId, out this._handle);
	}

	public ulong Id
	{
		get
		{
			if (this._handle == 0UL)
			{
				return 0UL;
			}
			ulong result;
			if (!OVRPlugin.GetSpaceUserId(this._handle, out result))
			{
				return 0UL;
			}
			return result;
		}
	}

	public void Dispose()
	{
		if (this._handle == 0UL)
		{
			return;
		}
		OVRPlugin.DestroySpaceUser(this._handle);
		this._handle = 0UL;
	}

	internal ulong _handle;
}
