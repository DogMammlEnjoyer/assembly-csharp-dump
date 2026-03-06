using System;
using System.Collections.Specialized;
using System.Configuration.Internal;
using System.IO;
using System.Reflection;
using System.Text;
using Unity;

namespace System.Configuration
{
	/// <summary>Provides access to configuration files for client applications. This class cannot be inherited.</summary>
	public static class ConfigurationManager
	{
		[MonoTODO("Evidence and version still needs work")]
		private static string GetAssemblyInfo(Assembly a)
		{
			object[] customAttributes = a.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
			string arg;
			if (customAttributes != null && customAttributes.Length != 0)
			{
				arg = ((AssemblyProductAttribute)customAttributes[0]).Product;
			}
			else
			{
				arg = AppDomain.CurrentDomain.FriendlyName;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("evidencehere");
			string arg2 = stringBuilder.ToString();
			customAttributes = a.GetCustomAttributes(typeof(AssemblyVersionAttribute), false);
			string path;
			if (customAttributes != null && customAttributes.Length != 0)
			{
				path = ((AssemblyVersionAttribute)customAttributes[0]).Version;
			}
			else
			{
				path = "1.0.0.0";
			}
			return Path.Combine(string.Format("{0}_{1}", arg, arg2), path);
		}

		internal static Configuration OpenExeConfigurationInternal(ConfigurationUserLevel userLevel, Assembly calling_assembly, string exePath)
		{
			ExeConfigurationFileMap exeConfigurationFileMap = new ExeConfigurationFileMap();
			if (userLevel != ConfigurationUserLevel.None)
			{
				if (userLevel != ConfigurationUserLevel.PerUserRoaming)
				{
					if (userLevel != ConfigurationUserLevel.PerUserRoamingAndLocal)
					{
						goto IL_EA;
					}
					exeConfigurationFileMap.LocalUserConfigFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ConfigurationManager.GetAssemblyInfo(calling_assembly));
					exeConfigurationFileMap.LocalUserConfigFilename = Path.Combine(exeConfigurationFileMap.LocalUserConfigFilename, "user.config");
				}
				exeConfigurationFileMap.RoamingUserConfigFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ConfigurationManager.GetAssemblyInfo(calling_assembly));
				exeConfigurationFileMap.RoamingUserConfigFilename = Path.Combine(exeConfigurationFileMap.RoamingUserConfigFilename, "user.config");
			}
			if (exePath == null || exePath.Length == 0)
			{
				exeConfigurationFileMap.ExeConfigFilename = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
			}
			else
			{
				if (!Path.IsPathRooted(exePath))
				{
					exePath = Path.GetFullPath(exePath);
				}
				if (!File.Exists(exePath))
				{
					Exception inner = new ArgumentException("The specified path does not exist.", "exePath");
					throw new ConfigurationErrorsException("Error Initializing the configuration system:", inner);
				}
				exeConfigurationFileMap.ExeConfigFilename = exePath + ".config";
			}
			IL_EA:
			return ConfigurationManager.ConfigurationFactory.Create(typeof(ExeConfigurationHost), new object[]
			{
				exeConfigurationFileMap,
				userLevel
			});
		}

		/// <summary>Opens the configuration file for the current application as a <see cref="T:System.Configuration.Configuration" /> object.</summary>
		/// <param name="userLevel">The <see cref="T:System.Configuration.ConfigurationUserLevel" /> for which you are opening the configuration.</param>
		/// <returns>A <see cref="T:System.Configuration.Configuration" /> object.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">A configuration file could not be loaded.</exception>
		public static Configuration OpenExeConfiguration(ConfigurationUserLevel userLevel)
		{
			return ConfigurationManager.OpenExeConfigurationInternal(userLevel, Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly(), null);
		}

		/// <summary>Opens the specified client configuration file as a <see cref="T:System.Configuration.Configuration" /> object.</summary>
		/// <param name="exePath">The path of the executable (exe) file.</param>
		/// <returns>A <see cref="T:System.Configuration.Configuration" /> object.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">A configuration file could not be loaded.</exception>
		public static Configuration OpenExeConfiguration(string exePath)
		{
			return ConfigurationManager.OpenExeConfigurationInternal(ConfigurationUserLevel.None, Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly(), exePath);
		}

		/// <summary>Opens the specified client configuration file as a <see cref="T:System.Configuration.Configuration" /> object that uses the specified file mapping and user level.</summary>
		/// <param name="fileMap">An <see cref="T:System.Configuration.ExeConfigurationFileMap" /> object that references configuration file to use instead of the application default configuration file.</param>
		/// <param name="userLevel">The <see cref="T:System.Configuration.ConfigurationUserLevel" /> object for which you are opening the configuration.</param>
		/// <returns>The configuration object.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">A configuration file could not be loaded.</exception>
		[MonoLimitation("ConfigurationUserLevel parameter is not supported.")]
		public static Configuration OpenMappedExeConfiguration(ExeConfigurationFileMap fileMap, ConfigurationUserLevel userLevel)
		{
			return ConfigurationManager.ConfigurationFactory.Create(typeof(ExeConfigurationHost), new object[]
			{
				fileMap,
				userLevel
			});
		}

		/// <summary>Opens the machine configuration file on the current computer as a <see cref="T:System.Configuration.Configuration" /> object.</summary>
		/// <returns>A <see cref="T:System.Configuration.Configuration" /> object.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">A configuration file could not be loaded.</exception>
		public static Configuration OpenMachineConfiguration()
		{
			ConfigurationFileMap configurationFileMap = new ConfigurationFileMap();
			return ConfigurationManager.ConfigurationFactory.Create(typeof(MachineConfigurationHost), new object[]
			{
				configurationFileMap
			});
		}

		/// <summary>Opens the machine configuration file as a <see cref="T:System.Configuration.Configuration" /> object that uses the specified file mapping.</summary>
		/// <param name="fileMap">An <see cref="T:System.Configuration.ExeConfigurationFileMap" /> object that references configuration file to use instead of the application default configuration file.</param>
		/// <returns>A <see cref="T:System.Configuration.Configuration" /> object.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">A configuration file could not be loaded.</exception>
		public static Configuration OpenMappedMachineConfiguration(ConfigurationFileMap fileMap)
		{
			return ConfigurationManager.ConfigurationFactory.Create(typeof(MachineConfigurationHost), new object[]
			{
				fileMap
			});
		}

		internal static IInternalConfigConfigurationFactory ConfigurationFactory
		{
			get
			{
				return ConfigurationManager.configFactory;
			}
		}

		internal static IInternalConfigSystem ConfigurationSystem
		{
			get
			{
				return ConfigurationManager.configSystem;
			}
		}

		/// <summary>Retrieves a specified configuration section for the current application's default configuration.</summary>
		/// <param name="sectionName">The configuration section path and name. Node names are separated by forward slashes, for example "system.net/mailSettings/smtp".</param>
		/// <returns>The specified <see cref="T:System.Configuration.ConfigurationSection" /> object, or <see langword="null" /> if the section does not exist.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">A configuration file could not be loaded.</exception>
		public static object GetSection(string sectionName)
		{
			object section = ConfigurationManager.ConfigurationSystem.GetSection(sectionName);
			if (section is ConfigurationSection)
			{
				return ((ConfigurationSection)section).GetRuntimeObject();
			}
			return section;
		}

		/// <summary>Refreshes the named section so the next time that it is retrieved it will be re-read from disk.</summary>
		/// <param name="sectionName">The configuration section name or the configuration path and section name of the section to refresh.</param>
		public static void RefreshSection(string sectionName)
		{
			ConfigurationManager.ConfigurationSystem.RefreshConfig(sectionName);
		}

		/// <summary>Gets the <see cref="T:System.Configuration.AppSettingsSection" /> data for the current application's default configuration.</summary>
		/// <returns>A <see cref="T:System.Collections.Specialized.NameValueCollection" /> object that contains the contents of the <see cref="T:System.Configuration.AppSettingsSection" /> object for the current application's default configuration.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">Could not retrieve a <see cref="T:System.Collections.Specialized.NameValueCollection" /> object with the application settings data.</exception>
		public static NameValueCollection AppSettings
		{
			get
			{
				return (NameValueCollection)ConfigurationManager.GetSection("appSettings");
			}
		}

		/// <summary>Gets the <see cref="T:System.Configuration.ConnectionStringsSection" /> data for the current application's default configuration.</summary>
		/// <returns>A <see cref="T:System.Configuration.ConnectionStringSettingsCollection" /> object that contains the contents of the <see cref="T:System.Configuration.ConnectionStringsSection" /> object for the current application's default configuration.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">Could not retrieve a <see cref="T:System.Configuration.ConnectionStringSettingsCollection" /> object.</exception>
		public static ConnectionStringSettingsCollection ConnectionStrings
		{
			get
			{
				return ((ConnectionStringsSection)ConfigurationManager.GetSection("connectionStrings")).ConnectionStrings;
			}
		}

		internal static IInternalConfigSystem ChangeConfigurationSystem(IInternalConfigSystem newSystem)
		{
			if (newSystem == null)
			{
				throw new ArgumentNullException("newSystem");
			}
			object obj = ConfigurationManager.lockobj;
			IInternalConfigSystem result;
			lock (obj)
			{
				IInternalConfigSystem internalConfigSystem = ConfigurationManager.configSystem;
				ConfigurationManager.configSystem = newSystem;
				result = internalConfigSystem;
			}
			return result;
		}

		/// <summary>Opens the specified client configuration file as a <see cref="T:System.Configuration.Configuration" /> object that uses the specified file mapping, user level, and preload option.</summary>
		/// <param name="fileMap">An <see cref="T:System.Configuration.ExeConfigurationFileMap" /> object that references the configuration file to use instead of the default application configuration file.</param>
		/// <param name="userLevel">The <see cref="T:System.Configuration.ConfigurationUserLevel" /> object for which you are opening the configuration.</param>
		/// <param name="preLoad">
		///   <see langword="true" /> to preload all section groups and sections; otherwise, <see langword="false" />.</param>
		/// <returns>The configuration object.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">A configuration file could not be loaded.</exception>
		public static Configuration OpenMappedExeConfiguration(ExeConfigurationFileMap fileMap, ConfigurationUserLevel userLevel, bool preLoad)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		private static InternalConfigurationFactory configFactory = new InternalConfigurationFactory();

		private static IInternalConfigSystem configSystem = new ClientConfigurationSystem();

		private static object lockobj = new object();
	}
}
