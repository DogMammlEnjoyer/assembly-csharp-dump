using System;

namespace UnityEngine
{
	[Obsolete("Use SubsystemDescriptorWithProvider<> instead.", false)]
	public class SubsystemDescriptor<TSubsystem> : SubsystemDescriptor where TSubsystem : Subsystem
	{
		internal override ISubsystem CreateImpl()
		{
			return this.Create();
		}

		public TSubsystem Create()
		{
			TSubsystem tsubsystem = SubsystemManager.FindDeprecatedSubsystemByDescriptor(this) as TSubsystem;
			bool flag = tsubsystem != null;
			TSubsystem result;
			if (flag)
			{
				result = tsubsystem;
			}
			else
			{
				tsubsystem = (Activator.CreateInstance(base.subsystemImplementationType) as TSubsystem);
				tsubsystem.m_SubsystemDescriptor = this;
				SubsystemManager.AddDeprecatedSubsystem(tsubsystem);
				result = tsubsystem;
			}
			return result;
		}
	}
}
