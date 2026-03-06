using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class ProcessPort
{
	public override string ToString()
	{
		return string.Format("{0}({1}) ({2} port {3})", new object[]
		{
			this.processName,
			this.processId,
			this.protocol,
			this.portNumber
		});
	}

	public string processName { get; set; }

	public int processId { get; set; }

	public string portNumber { get; set; }

	public string protocol { get; set; }

	private static string LookupProcess(int pid)
	{
		string result;
		try
		{
			result = Process.GetProcessById(pid).ProcessName;
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			result = "-";
		}
		return result;
	}

	public static List<ProcessPort> GetProcessesByPort(string targetPort)
	{
		List<ProcessPort> list = new List<ProcessPort>();
		try
		{
			using (Process process = new Process())
			{
				process.StartInfo = new ProcessStartInfo
				{
					Arguments = "-a -n -o",
					FileName = "netstat.exe",
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				};
				process.Start();
				TextReader standardOutput = process.StandardOutput;
				StreamReader standardError = process.StandardError;
				string input = standardOutput.ReadToEnd() + standardError.ReadToEnd();
				if (process.ExitCode != 0)
				{
					Debug.LogError("netstat call failed");
					return list;
				}
				Regex regex = new Regex("\r\n");
				Regex regex2 = new Regex("\\s+");
				Regex regex3 = new Regex("\\[(.*?)\\]");
				foreach (string input2 in regex.Split(input))
				{
					string[] array2 = regex2.Split(input2);
					if (array2.Length > 4 && (array2[1].Equals("UDP") || array2[1].Equals("TCP")))
					{
						string text = regex3.Replace(array2[2], "1.1.1.1");
						string text2 = text.Split(':', StringSplitOptions.None)[1];
						if (!(targetPort != text2))
						{
							int num = 0;
							try
							{
								num = (array2[1].Equals("UDP") ? Convert.ToInt32(array2[4]) : Convert.ToInt32(array2[5]));
							}
							catch (Exception ex)
							{
								Debug.LogError(string.Concat(new string[]
								{
									array2[1],
									" ",
									array2[4],
									" ",
									array2[5]
								}));
								throw ex;
							}
							list.Add(new ProcessPort
							{
								protocol = (text.Contains("1.1.1.1") ? string.Format("{0}v6", array2[1]) : string.Format("{0}v4", array2[1])),
								portNumber = text2,
								processName = ProcessPort.LookupProcess(num),
								processId = num
							});
						}
					}
				}
			}
		}
		catch (Exception ex2)
		{
			Debug.LogError(ex2.Message);
		}
		return list;
	}
}
