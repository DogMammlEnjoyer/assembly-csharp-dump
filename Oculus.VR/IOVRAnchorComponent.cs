using System;

public interface IOVRAnchorComponent<T>
{
	bool IsNull { get; }

	bool IsEnabled { get; }

	OVRTask<bool> SetEnabledAsync(bool enable, double timeout = 0.0);

	OVRPlugin.SpaceComponentType Type { get; }

	ulong Handle { get; }

	T FromAnchor(OVRAnchor anchor);
}
