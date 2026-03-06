using System;
using System.Runtime.CompilerServices;
using Mono;

namespace System.Runtime.InteropServices
{
	/// <summary>Provides information about the .NET runtime installation.</summary>
	public static class RuntimeInformation
	{
		static RuntimeInformation()
		{
			string runtimeArchitecture = RuntimeInformation.GetRuntimeArchitecture();
			string osname = RuntimeInformation.GetOSName();
			if (!(runtimeArchitecture == "arm"))
			{
				if (!(runtimeArchitecture == "armv8"))
				{
					if (!(runtimeArchitecture == "x86"))
					{
						if (!(runtimeArchitecture == "x86-64"))
						{
							if (!(runtimeArchitecture == "wasm"))
							{
							}
							RuntimeInformation._osArchitecture = (Environment.Is64BitOperatingSystem ? Architecture.X64 : Architecture.X86);
							RuntimeInformation._processArchitecture = (Environment.Is64BitProcess ? Architecture.X64 : Architecture.X86);
						}
						else
						{
							RuntimeInformation._osArchitecture = (Environment.Is64BitOperatingSystem ? Architecture.X64 : Architecture.X86);
							RuntimeInformation._processArchitecture = Architecture.X64;
						}
					}
					else
					{
						RuntimeInformation._osArchitecture = (Environment.Is64BitOperatingSystem ? Architecture.X64 : Architecture.X86);
						RuntimeInformation._processArchitecture = Architecture.X86;
					}
				}
				else
				{
					RuntimeInformation._osArchitecture = (Environment.Is64BitOperatingSystem ? Architecture.Arm64 : Architecture.Arm);
					RuntimeInformation._processArchitecture = Architecture.Arm64;
				}
			}
			else
			{
				RuntimeInformation._osArchitecture = (Environment.Is64BitOperatingSystem ? Architecture.Arm64 : Architecture.Arm);
				RuntimeInformation._processArchitecture = Architecture.Arm;
			}
			uint num = <PrivateImplementationDetails>.ComputeStringHash(osname);
			if (num <= 2784415053U)
			{
				if (num <= 758268069U)
				{
					if (num != 311744602U)
					{
						if (num == 758268069U)
						{
							if (osname == "aix")
							{
								RuntimeInformation._osPlatform = OSPlatform.Create("AIX");
								return;
							}
						}
					}
					else if (osname == "solaris")
					{
						RuntimeInformation._osPlatform = OSPlatform.Create("SOLARIS");
						return;
					}
				}
				else if (num != 1846719142U)
				{
					if (num != 1968959064U)
					{
						if (num == 2784415053U)
						{
							if (osname == "wasm")
							{
								RuntimeInformation._osPlatform = OSPlatform.Create("BROWSER");
								return;
							}
						}
					}
					else if (osname == "hpux")
					{
						RuntimeInformation._osPlatform = OSPlatform.Create("HPUX");
						return;
					}
				}
				else if (osname == "openbsd")
				{
					RuntimeInformation._osPlatform = OSPlatform.Create("OPENBSD");
					return;
				}
			}
			else if (num <= 3229321689U)
			{
				if (num != 2876596737U)
				{
					if (num != 3139461053U)
					{
						if (num == 3229321689U)
						{
							if (osname == "netbsd")
							{
								RuntimeInformation._osPlatform = OSPlatform.Create("NETBSD");
								return;
							}
						}
					}
					else if (osname == "osx")
					{
						RuntimeInformation._osPlatform = OSPlatform.OSX;
						return;
					}
				}
				else if (osname == "haiku")
				{
					RuntimeInformation._osPlatform = OSPlatform.Create("HAIKU");
					return;
				}
			}
			else if (num != 3583452906U)
			{
				if (num != 3971716381U)
				{
					if (num == 4059584116U)
					{
						if (osname == "freebsd")
						{
							RuntimeInformation._osPlatform = OSPlatform.Create("FREEBSD");
							return;
						}
					}
				}
				else if (osname == "linux")
				{
					RuntimeInformation._osPlatform = OSPlatform.Linux;
					return;
				}
			}
			else if (osname == "windows")
			{
				RuntimeInformation._osPlatform = OSPlatform.Windows;
				return;
			}
			RuntimeInformation._osPlatform = OSPlatform.Create("UNKNOWN");
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetRuntimeArchitecture();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetOSName();

		/// <summary>Returns a string that indicates the name of the .NET installation on which an app is running.</summary>
		/// <returns>The name of the .NET installation on which the app is running.</returns>
		public static string FrameworkDescription
		{
			get
			{
				return "Mono " + Runtime.GetDisplayName();
			}
		}

		/// <summary>Indicates whether the current application is running on the specified platform.</summary>
		/// <param name="osPlatform">A platform.</param>
		/// <returns>
		///   <see langword="true" /> if the current app is running on the specified platform; otherwise, <see langword="false" />.</returns>
		public static bool IsOSPlatform(OSPlatform osPlatform)
		{
			return RuntimeInformation._osPlatform == osPlatform;
		}

		/// <summary>Gets a string that describes the operating system on which the app is running.</summary>
		/// <returns>The description of the operating system on which the app is running.</returns>
		public static string OSDescription
		{
			get
			{
				return Environment.OSVersion.VersionString;
			}
		}

		/// <summary>Gets the platform architecture on which the current app is running.</summary>
		/// <returns>The platform architecture on which the current app is running.</returns>
		public static Architecture OSArchitecture
		{
			get
			{
				return RuntimeInformation._osArchitecture;
			}
		}

		/// <summary>Gets the process architecture of the currently running app.</summary>
		/// <returns>The process architecture of the currently running app.</returns>
		public static Architecture ProcessArchitecture
		{
			get
			{
				return RuntimeInformation._processArchitecture;
			}
		}

		private static readonly Architecture _osArchitecture;

		private static readonly Architecture _processArchitecture;

		private static readonly OSPlatform _osPlatform;
	}
}
