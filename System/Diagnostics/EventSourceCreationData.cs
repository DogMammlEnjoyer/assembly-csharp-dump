using System;

namespace System.Diagnostics
{
	/// <summary>Represents the configuration settings used to create an event log source on the local computer or a remote computer.</summary>
	public class EventSourceCreationData
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.EventSourceCreationData" /> class with a specified event source and event log name.</summary>
		/// <param name="source">The name to register with the event log as a source of entries.</param>
		/// <param name="logName">The name of the log to which entries from the source are written.</param>
		public EventSourceCreationData(string source, string logName)
		{
			this._source = source;
			this._logName = logName;
			this._machineName = ".";
		}

		internal EventSourceCreationData(string source, string logName, string machineName)
		{
			this._source = source;
			if (logName == null || logName.Length == 0)
			{
				this._logName = "Application";
			}
			else
			{
				this._logName = logName;
			}
			this._machineName = machineName;
		}

		/// <summary>Gets or sets the number of categories in the category resource file.</summary>
		/// <returns>The number of categories in the category resource file. The default value is zero.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The property is set to a negative value or to a value larger than <see cref="F:System.UInt16.MaxValue" />.</exception>
		public int CategoryCount
		{
			get
			{
				return this._categoryCount;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._categoryCount = value;
			}
		}

		/// <summary>Gets or sets the path of the resource file that contains category strings for the source.</summary>
		/// <returns>The path of the category resource file. The default is an empty string ("").</returns>
		public string CategoryResourceFile
		{
			get
			{
				return this._categoryResourceFile;
			}
			set
			{
				this._categoryResourceFile = value;
			}
		}

		/// <summary>Gets or sets the name of the event log to which the source writes entries.</summary>
		/// <returns>The name of the event log. This can be Application, System, or a custom log name. The default value is "Application."</returns>
		public string LogName
		{
			get
			{
				return this._logName;
			}
			set
			{
				this._logName = value;
			}
		}

		/// <summary>Gets or sets the name of the computer on which to register the event source.</summary>
		/// <returns>The name of the system on which to register the event source. The default is the local computer (".").</returns>
		/// <exception cref="T:System.ArgumentException">The computer name is invalid.</exception>
		public string MachineName
		{
			get
			{
				return this._machineName;
			}
			set
			{
				this._machineName = value;
			}
		}

		/// <summary>Gets or sets the path of the message resource file that contains message formatting strings for the source.</summary>
		/// <returns>The path of the message resource file. The default is an empty string ("").</returns>
		public string MessageResourceFile
		{
			get
			{
				return this._messageResourceFile;
			}
			set
			{
				this._messageResourceFile = value;
			}
		}

		/// <summary>Gets or sets the path of the resource file that contains message parameter strings for the source.</summary>
		/// <returns>The path of the parameter resource file. The default is an empty string ("").</returns>
		public string ParameterResourceFile
		{
			get
			{
				return this._parameterResourceFile;
			}
			set
			{
				this._parameterResourceFile = value;
			}
		}

		/// <summary>Gets or sets the name to register with the event log as an event source.</summary>
		/// <returns>The name to register with the event log as a source of entries. The default is an empty string ("").</returns>
		public string Source
		{
			get
			{
				return this._source;
			}
			set
			{
				this._source = value;
			}
		}

		private string _source;

		private string _logName;

		private string _machineName;

		private string _messageResourceFile;

		private string _parameterResourceFile;

		private string _categoryResourceFile;

		private int _categoryCount;
	}
}
