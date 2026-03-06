using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OVR.OpenVR
{
	public class CVRSettings
	{
		internal CVRSettings(IntPtr pInterface)
		{
			this.FnTable = (IVRSettings)Marshal.PtrToStructure(pInterface, typeof(IVRSettings));
		}

		public string GetSettingsErrorNameFromEnum(EVRSettingsError eError)
		{
			return Marshal.PtrToStringAnsi(this.FnTable.GetSettingsErrorNameFromEnum(eError));
		}

		public bool Sync(bool bForce, ref EVRSettingsError peError)
		{
			return this.FnTable.Sync(bForce, ref peError);
		}

		public void SetBool(string pchSection, string pchSettingsKey, bool bValue, ref EVRSettingsError peError)
		{
			this.FnTable.SetBool(pchSection, pchSettingsKey, bValue, ref peError);
		}

		public void SetInt32(string pchSection, string pchSettingsKey, int nValue, ref EVRSettingsError peError)
		{
			this.FnTable.SetInt32(pchSection, pchSettingsKey, nValue, ref peError);
		}

		public void SetFloat(string pchSection, string pchSettingsKey, float flValue, ref EVRSettingsError peError)
		{
			this.FnTable.SetFloat(pchSection, pchSettingsKey, flValue, ref peError);
		}

		public void SetString(string pchSection, string pchSettingsKey, string pchValue, ref EVRSettingsError peError)
		{
			this.FnTable.SetString(pchSection, pchSettingsKey, pchValue, ref peError);
		}

		public bool GetBool(string pchSection, string pchSettingsKey, ref EVRSettingsError peError)
		{
			return this.FnTable.GetBool(pchSection, pchSettingsKey, ref peError);
		}

		public int GetInt32(string pchSection, string pchSettingsKey, ref EVRSettingsError peError)
		{
			return this.FnTable.GetInt32(pchSection, pchSettingsKey, ref peError);
		}

		public float GetFloat(string pchSection, string pchSettingsKey, ref EVRSettingsError peError)
		{
			return this.FnTable.GetFloat(pchSection, pchSettingsKey, ref peError);
		}

		public void GetString(string pchSection, string pchSettingsKey, StringBuilder pchValue, uint unValueLen, ref EVRSettingsError peError)
		{
			this.FnTable.GetString(pchSection, pchSettingsKey, pchValue, unValueLen, ref peError);
		}

		public void RemoveSection(string pchSection, ref EVRSettingsError peError)
		{
			this.FnTable.RemoveSection(pchSection, ref peError);
		}

		public void RemoveKeyInSection(string pchSection, string pchSettingsKey, ref EVRSettingsError peError)
		{
			this.FnTable.RemoveKeyInSection(pchSection, pchSettingsKey, ref peError);
		}

		private IVRSettings FnTable;
	}
}
