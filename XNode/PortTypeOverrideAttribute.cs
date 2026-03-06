using System;

[AttributeUsage(AttributeTargets.Field)]
public class PortTypeOverrideAttribute : Attribute
{
	public PortTypeOverrideAttribute(Type type)
	{
		this.type = type;
	}

	public Type type;
}
