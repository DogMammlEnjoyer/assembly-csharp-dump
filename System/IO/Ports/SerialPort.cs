using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Microsoft.Win32;

namespace System.IO.Ports
{
	/// <summary>Represents a serial port resource.</summary>
	[MonitoringDescription("")]
	public class SerialPort : Component
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.IO.Ports.SerialPort" /> class.</summary>
		public SerialPort() : this(SerialPort.GetDefaultPortName(), 9600, Parity.None, 8, StopBits.One)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.Ports.SerialPort" /> class using the specified <see cref="T:System.ComponentModel.IContainer" /> object.</summary>
		/// <param name="container">An interface to a container.</param>
		/// <exception cref="T:System.IO.IOException">The specified port could not be found or opened.</exception>
		public SerialPort(IContainer container) : this()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.Ports.SerialPort" /> class using the specified port name.</summary>
		/// <param name="portName">The port to use (for example, COM1).</param>
		/// <exception cref="T:System.IO.IOException">The specified port could not be found or opened.</exception>
		public SerialPort(string portName) : this(portName, 9600, Parity.None, 8, StopBits.One)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.Ports.SerialPort" /> class using the specified port name and baud rate.</summary>
		/// <param name="portName">The port to use (for example, COM1).</param>
		/// <param name="baudRate">The baud rate.</param>
		/// <exception cref="T:System.IO.IOException">The specified port could not be found or opened.</exception>
		public SerialPort(string portName, int baudRate) : this(portName, baudRate, Parity.None, 8, StopBits.One)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.Ports.SerialPort" /> class using the specified port name, baud rate, and parity bit.</summary>
		/// <param name="portName">The port to use (for example, COM1).</param>
		/// <param name="baudRate">The baud rate.</param>
		/// <param name="parity">One of the <see cref="P:System.IO.Ports.SerialPort.Parity" /> values.</param>
		/// <exception cref="T:System.IO.IOException">The specified port could not be found or opened.</exception>
		public SerialPort(string portName, int baudRate, Parity parity) : this(portName, baudRate, parity, 8, StopBits.One)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.Ports.SerialPort" /> class using the specified port name, baud rate, parity bit, and data bits.</summary>
		/// <param name="portName">The port to use (for example, COM1).</param>
		/// <param name="baudRate">The baud rate.</param>
		/// <param name="parity">One of the <see cref="P:System.IO.Ports.SerialPort.Parity" /> values.</param>
		/// <param name="dataBits">The data bits value.</param>
		/// <exception cref="T:System.IO.IOException">The specified port could not be found or opened.</exception>
		public SerialPort(string portName, int baudRate, Parity parity, int dataBits) : this(portName, baudRate, parity, dataBits, StopBits.One)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.Ports.SerialPort" /> class using the specified port name, baud rate, parity bit, data bits, and stop bit.</summary>
		/// <param name="portName">The port to use (for example, COM1).</param>
		/// <param name="baudRate">The baud rate.</param>
		/// <param name="parity">One of the <see cref="P:System.IO.Ports.SerialPort.Parity" /> values.</param>
		/// <param name="dataBits">The data bits value.</param>
		/// <param name="stopBits">One of the <see cref="P:System.IO.Ports.SerialPort.StopBits" /> values.</param>
		/// <exception cref="T:System.IO.IOException">The specified port could not be found or opened.</exception>
		public SerialPort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
		{
			this.port_name = portName;
			this.baud_rate = baudRate;
			this.data_bits = dataBits;
			this.stop_bits = stopBits;
			this.parity = parity;
		}

		private static string GetDefaultPortName()
		{
			string[] portNames = SerialPort.GetPortNames();
			if (portNames.Length != 0)
			{
				return portNames[0];
			}
			int platform = (int)Environment.OSVersion.Platform;
			if (platform == 4 || platform == 128 || platform == 6)
			{
				return "ttyS0";
			}
			return "COM1";
		}

		/// <summary>Gets the underlying <see cref="T:System.IO.Stream" /> object for a <see cref="T:System.IO.Ports.SerialPort" /> object.</summary>
		/// <returns>A <see cref="T:System.IO.Stream" /> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">The stream is closed. This can occur because the <see cref="M:System.IO.Ports.SerialPort.Open" /> method has not been called or the <see cref="M:System.IO.Ports.SerialPort.Close" /> method has been called.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream is in a .NET Compact Framework application and one of the following methods was called:  
		///  <see cref="M:System.IO.Stream.BeginRead(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)" /><see cref="M:System.IO.Stream.BeginWrite(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)" /><see cref="M:System.IO.Stream.EndRead(System.IAsyncResult)" /><see cref="M:System.IO.Stream.EndWrite(System.IAsyncResult)" />  
		///
		///  The .NET Compact Framework does not support the asynchronous model with base streams.</exception>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Stream BaseStream
		{
			get
			{
				this.CheckOpen();
				return (Stream)this.stream;
			}
		}

		/// <summary>Gets or sets the serial baud rate.</summary>
		/// <returns>The baud rate.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The baud rate specified is less than or equal to zero, or is greater than the maximum allowable baud rate for the device.</exception>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		[Browsable(true)]
		[MonitoringDescription("")]
		[DefaultValue(9600)]
		public int BaudRate
		{
			get
			{
				return this.baud_rate;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (this.is_open)
				{
					this.stream.SetAttributes(value, this.parity, this.data_bits, this.stop_bits, this.handshake);
				}
				this.baud_rate = value;
			}
		}

		/// <summary>Gets or sets the break signal state.</summary>
		/// <returns>
		///   <see langword="true" /> if the port is in a break state; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		/// <exception cref="T:System.InvalidOperationException">The stream is closed. This can occur because the <see cref="M:System.IO.Ports.SerialPort.Open" /> method has not been called or the <see cref="M:System.IO.Ports.SerialPort.Close" /> method has been called.</exception>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool BreakState
		{
			get
			{
				return this.break_state;
			}
			set
			{
				this.CheckOpen();
				if (value == this.break_state)
				{
					return;
				}
				this.stream.SetBreakState(value);
				this.break_state = value;
			}
		}

		/// <summary>Gets the number of bytes of data in the receive buffer.</summary>
		/// <returns>The number of bytes of data in the receive buffer.</returns>
		/// <exception cref="T:System.InvalidOperationException">The port is not open.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public int BytesToRead
		{
			get
			{
				this.CheckOpen();
				return this.stream.BytesToRead;
			}
		}

		/// <summary>Gets the number of bytes of data in the send buffer.</summary>
		/// <returns>The number of bytes of data in the send buffer.</returns>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.</exception>
		/// <exception cref="T:System.InvalidOperationException">The stream is closed. This can occur because the <see cref="M:System.IO.Ports.SerialPort.Open" /> method has not been called or the <see cref="M:System.IO.Ports.SerialPort.Close" /> method has been called.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public int BytesToWrite
		{
			get
			{
				this.CheckOpen();
				return this.stream.BytesToWrite;
			}
		}

		/// <summary>Gets the state of the Carrier Detect line for the port.</summary>
		/// <returns>
		///   <see langword="true" /> if the carrier is detected; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		/// <exception cref="T:System.InvalidOperationException">The stream is closed. This can occur because the <see cref="M:System.IO.Ports.SerialPort.Open" /> method has not been called or the <see cref="M:System.IO.Ports.SerialPort.Close" /> method has been called.</exception>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CDHolding
		{
			get
			{
				this.CheckOpen();
				return (this.stream.GetSignals() & SerialSignal.Cd) > SerialSignal.None;
			}
		}

		/// <summary>Gets the state of the Clear-to-Send line.</summary>
		/// <returns>
		///   <see langword="true" /> if the Clear-to-Send line is detected; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		/// <exception cref="T:System.InvalidOperationException">The stream is closed. This can occur because the <see cref="M:System.IO.Ports.SerialPort.Open" /> method has not been called or the <see cref="M:System.IO.Ports.SerialPort.Close" /> method has been called.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public bool CtsHolding
		{
			get
			{
				this.CheckOpen();
				return (this.stream.GetSignals() & SerialSignal.Cts) > SerialSignal.None;
			}
		}

		/// <summary>Gets or sets the standard length of data bits per byte.</summary>
		/// <returns>The data bits length.</returns>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The data bits value is less than 5 or more than 8.</exception>
		[Browsable(true)]
		[DefaultValue(8)]
		[MonitoringDescription("")]
		public int DataBits
		{
			get
			{
				return this.data_bits;
			}
			set
			{
				if (value < 5 || value > 8)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (this.is_open)
				{
					this.stream.SetAttributes(this.baud_rate, this.parity, value, this.stop_bits, this.handshake);
				}
				this.data_bits = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether null bytes are ignored when transmitted between the port and the receive buffer.</summary>
		/// <returns>
		///   <see langword="true" /> if null bytes are ignored; otherwise <see langword="false" />. The default is <see langword="false" />.</returns>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		/// <exception cref="T:System.InvalidOperationException">The stream is closed. This can occur because the <see cref="M:System.IO.Ports.SerialPort.Open" /> method has not been called or the <see cref="M:System.IO.Ports.SerialPort.Close" /> method has been called.</exception>
		[MonoTODO("Not implemented")]
		[Browsable(true)]
		[DefaultValue(false)]
		[MonitoringDescription("")]
		public bool DiscardNull
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the state of the Data Set Ready (DSR) signal.</summary>
		/// <returns>
		///   <see langword="true" /> if a Data Set Ready signal has been sent to the port; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		/// <exception cref="T:System.InvalidOperationException">The stream is closed. This can occur because the <see cref="M:System.IO.Ports.SerialPort.Open" /> method has not been called or the <see cref="M:System.IO.Ports.SerialPort.Close" /> method has been called.</exception>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool DsrHolding
		{
			get
			{
				this.CheckOpen();
				return (this.stream.GetSignals() & SerialSignal.Dsr) > SerialSignal.None;
			}
		}

		/// <summary>Gets or sets a value that enables the Data Terminal Ready (DTR) signal during serial communication.</summary>
		/// <returns>
		///   <see langword="true" /> to enable Data Terminal Ready (DTR); otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		[DefaultValue(false)]
		[Browsable(true)]
		[MonitoringDescription("")]
		public bool DtrEnable
		{
			get
			{
				return this.dtr_enable;
			}
			set
			{
				if (value == this.dtr_enable)
				{
					return;
				}
				if (this.is_open)
				{
					this.stream.SetSignal(SerialSignal.Dtr, value);
				}
				this.dtr_enable = value;
			}
		}

		/// <summary>Gets or sets the byte encoding for pre- and post-transmission conversion of text.</summary>
		/// <returns>An <see cref="T:System.Text.Encoding" /> object. The default is <see cref="T:System.Text.ASCIIEncoding" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.IO.Ports.SerialPort.Encoding" /> property was set to <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.IO.Ports.SerialPort.Encoding" /> property was set to an encoding that is not <see cref="T:System.Text.ASCIIEncoding" />, <see cref="T:System.Text.UTF8Encoding" />, <see cref="T:System.Text.UTF32Encoding" />, <see cref="T:System.Text.UnicodeEncoding" />, one of the Windows single byte encodings, or one of the Windows double byte encodings.</exception>
		[Browsable(false)]
		[MonitoringDescription("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Encoding Encoding
		{
			get
			{
				return this.encoding;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.encoding = value;
			}
		}

		/// <summary>Gets or sets the handshaking protocol for serial port transmission of data using a value from <see cref="T:System.IO.Ports.Handshake" />.</summary>
		/// <returns>One of the <see cref="T:System.IO.Ports.Handshake" /> values. The default is <see langword="None" />.</returns>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The value passed is not a valid value in the <see cref="T:System.IO.Ports.Handshake" /> enumeration.</exception>
		[MonitoringDescription("")]
		[Browsable(true)]
		[DefaultValue(Handshake.None)]
		public Handshake Handshake
		{
			get
			{
				return this.handshake;
			}
			set
			{
				if (value < Handshake.None || value > Handshake.RequestToSendXOnXOff)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (this.is_open)
				{
					this.stream.SetAttributes(this.baud_rate, this.parity, this.data_bits, this.stop_bits, value);
				}
				this.handshake = value;
			}
		}

		/// <summary>Gets a value indicating the open or closed status of the <see cref="T:System.IO.Ports.SerialPort" /> object.</summary>
		/// <returns>
		///   <see langword="true" /> if the serial port is open; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.IO.Ports.SerialPort.IsOpen" /> value passed is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.IO.Ports.SerialPort.IsOpen" /> value passed is an empty string ("").</exception>
		[Browsable(false)]
		public bool IsOpen
		{
			get
			{
				return this.is_open;
			}
		}

		/// <summary>Gets or sets the value used to interpret the end of a call to the <see cref="M:System.IO.Ports.SerialPort.ReadLine" /> and <see cref="M:System.IO.Ports.SerialPort.WriteLine(System.String)" /> methods.</summary>
		/// <returns>A value that represents the end of a line. The default is a line feed, <see cref="P:System.Environment.NewLine" />.</returns>
		/// <exception cref="T:System.ArgumentException">The property value is empty.</exception>
		/// <exception cref="T:System.ArgumentNullException">The property value is <see langword="null" />.</exception>
		[Browsable(false)]
		[MonitoringDescription("")]
		[DefaultValue("\n")]
		public string NewLine
		{
			get
			{
				return this.new_line;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length == 0)
				{
					throw new ArgumentException("NewLine cannot be null or empty.", "value");
				}
				this.new_line = value;
			}
		}

		/// <summary>Gets or sets the parity-checking protocol.</summary>
		/// <returns>One of the enumeration values that represents the parity-checking protocol. The default is <see cref="F:System.IO.Ports.Parity.None" />.</returns>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <see cref="P:System.IO.Ports.SerialPort.Parity" /> value passed is not a valid value in the <see cref="T:System.IO.Ports.Parity" /> enumeration.</exception>
		[DefaultValue(Parity.None)]
		[Browsable(true)]
		[MonitoringDescription("")]
		public Parity Parity
		{
			get
			{
				return this.parity;
			}
			set
			{
				if (value < Parity.None || value > Parity.Space)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (this.is_open)
				{
					this.stream.SetAttributes(this.baud_rate, value, this.data_bits, this.stop_bits, this.handshake);
				}
				this.parity = value;
			}
		}

		/// <summary>Gets or sets the byte that replaces invalid bytes in a data stream when a parity error occurs.</summary>
		/// <returns>A byte that replaces invalid bytes.</returns>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		[MonitoringDescription("")]
		[Browsable(true)]
		[MonoTODO("Not implemented")]
		[DefaultValue(63)]
		public byte ParityReplace
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets or sets the port for communications, including but not limited to all available COM ports.</summary>
		/// <returns>The communications port. The default is COM1.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.IO.Ports.SerialPort.PortName" /> property was set to a value with a length of zero.  
		///  -or-  
		///  The <see cref="P:System.IO.Ports.SerialPort.PortName" /> property was set to a value that starts with "\\".  
		///  -or-  
		///  The port name was not valid.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.IO.Ports.SerialPort.PortName" /> property was set to <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The specified port is open.</exception>
		[DefaultValue("COM1")]
		[MonitoringDescription("")]
		[Browsable(true)]
		public string PortName
		{
			get
			{
				return this.port_name;
			}
			set
			{
				if (this.is_open)
				{
					throw new InvalidOperationException("Port name cannot be set while port is open.");
				}
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length == 0 || value.StartsWith("\\\\"))
				{
					throw new ArgumentException("value");
				}
				this.port_name = value;
			}
		}

		/// <summary>Gets or sets the size of the <see cref="T:System.IO.Ports.SerialPort" /> input buffer.</summary>
		/// <returns>The buffer size, in bytes. The default value is 4096; the maximum value is that of a positive int, or 2147483647.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <see cref="P:System.IO.Ports.SerialPort.ReadBufferSize" /> value set is less than or equal to zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.IO.Ports.SerialPort.ReadBufferSize" /> property was set while the stream was open.</exception>
		/// <exception cref="T:System.IO.IOException">The <see cref="P:System.IO.Ports.SerialPort.ReadBufferSize" /> property was set to an odd integer value.</exception>
		[MonitoringDescription("")]
		[Browsable(true)]
		[DefaultValue(4096)]
		public int ReadBufferSize
		{
			get
			{
				return this.readBufferSize;
			}
			set
			{
				if (this.is_open)
				{
					throw new InvalidOperationException();
				}
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (value <= 4096)
				{
					return;
				}
				this.readBufferSize = value;
			}
		}

		/// <summary>Gets or sets the number of milliseconds before a time-out occurs when a read operation does not finish.</summary>
		/// <returns>The number of milliseconds before a time-out occurs when a read operation does not finish.</returns>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The read time-out value is less than zero and not equal to <see cref="F:System.IO.Ports.SerialPort.InfiniteTimeout" />.</exception>
		[DefaultValue(-1)]
		[MonitoringDescription("")]
		[Browsable(true)]
		public int ReadTimeout
		{
			get
			{
				return this.read_timeout;
			}
			set
			{
				if (value < 0 && value != -1)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (this.is_open)
				{
					this.stream.ReadTimeout = value;
				}
				this.read_timeout = value;
			}
		}

		/// <summary>Gets or sets the number of bytes in the internal input buffer before a <see cref="E:System.IO.Ports.SerialPort.DataReceived" /> event occurs.</summary>
		/// <returns>The number of bytes in the internal input buffer before a <see cref="E:System.IO.Ports.SerialPort.DataReceived" /> event is fired. The default is 1.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <see cref="P:System.IO.Ports.SerialPort.ReceivedBytesThreshold" /> value is less than or equal to zero.</exception>
		[MonitoringDescription("")]
		[Browsable(true)]
		[DefaultValue(1)]
		[MonoTODO("Not implemented")]
		public int ReceivedBytesThreshold
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets or sets a value indicating whether the Request to Send (RTS) signal is enabled during serial communication.</summary>
		/// <returns>
		///   <see langword="true" /> to enable Request to Transmit (RTS); otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">The value of the <see cref="P:System.IO.Ports.SerialPort.RtsEnable" /> property was set or retrieved while the <see cref="P:System.IO.Ports.SerialPort.Handshake" /> property is set to the <see cref="F:System.IO.Ports.Handshake.RequestToSend" /> value or the <see cref="F:System.IO.Ports.Handshake.RequestToSendXOnXOff" /> value.</exception>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		[MonitoringDescription("")]
		[Browsable(true)]
		[DefaultValue(false)]
		public bool RtsEnable
		{
			get
			{
				return this.rts_enable;
			}
			set
			{
				if (value == this.rts_enable)
				{
					return;
				}
				if (this.is_open)
				{
					this.stream.SetSignal(SerialSignal.Rts, value);
				}
				this.rts_enable = value;
			}
		}

		/// <summary>Gets or sets the standard number of stopbits per byte.</summary>
		/// <returns>One of the <see cref="T:System.IO.Ports.StopBits" /> values.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <see cref="P:System.IO.Ports.SerialPort.StopBits" /> value is  <see cref="F:System.IO.Ports.StopBits.None" />.</exception>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		[Browsable(true)]
		[DefaultValue(StopBits.One)]
		[MonitoringDescription("")]
		public StopBits StopBits
		{
			get
			{
				return this.stop_bits;
			}
			set
			{
				if (value < StopBits.One || value > StopBits.OnePointFive)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (this.is_open)
				{
					this.stream.SetAttributes(this.baud_rate, this.parity, this.data_bits, value, this.handshake);
				}
				this.stop_bits = value;
			}
		}

		/// <summary>Gets or sets the size of the serial port output buffer.</summary>
		/// <returns>The size of the output buffer. The default is 2048.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <see cref="P:System.IO.Ports.SerialPort.WriteBufferSize" /> value is less than or equal to zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.IO.Ports.SerialPort.WriteBufferSize" /> property was set while the stream was open.</exception>
		/// <exception cref="T:System.IO.IOException">The <see cref="P:System.IO.Ports.SerialPort.WriteBufferSize" /> property was set to an odd integer value.</exception>
		[MonitoringDescription("")]
		[Browsable(true)]
		[DefaultValue(2048)]
		public int WriteBufferSize
		{
			get
			{
				return this.writeBufferSize;
			}
			set
			{
				if (this.is_open)
				{
					throw new InvalidOperationException();
				}
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (value <= 2048)
				{
					return;
				}
				this.writeBufferSize = value;
			}
		}

		/// <summary>Gets or sets the number of milliseconds before a time-out occurs when a write operation does not finish.</summary>
		/// <returns>The number of milliseconds before a time-out occurs. The default is <see cref="F:System.IO.Ports.SerialPort.InfiniteTimeout" />.</returns>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <see cref="P:System.IO.Ports.SerialPort.WriteTimeout" /> value is less than zero and not equal to <see cref="F:System.IO.Ports.SerialPort.InfiniteTimeout" />.</exception>
		[Browsable(true)]
		[DefaultValue(-1)]
		[MonitoringDescription("")]
		public int WriteTimeout
		{
			get
			{
				return this.write_timeout;
			}
			set
			{
				if (value < 0 && value != -1)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (this.is_open)
				{
					this.stream.WriteTimeout = value;
				}
				this.write_timeout = value;
			}
		}

		/// <summary>Closes the port connection, sets the <see cref="P:System.IO.Ports.SerialPort.IsOpen" /> property to <see langword="false" />, and disposes of the internal <see cref="T:System.IO.Stream" /> object.</summary>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		public void Close()
		{
			this.Dispose(true);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.IO.Ports.SerialPort" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		protected override void Dispose(bool disposing)
		{
			if (!this.is_open)
			{
				return;
			}
			this.is_open = false;
			if (disposing)
			{
				this.stream.Close();
			}
			this.stream = null;
		}

		/// <summary>Discards data from the serial driver's receive buffer.</summary>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		/// <exception cref="T:System.InvalidOperationException">The stream is closed. This can occur because the <see cref="M:System.IO.Ports.SerialPort.Open" /> method has not been called or the <see cref="M:System.IO.Ports.SerialPort.Close" /> method has been called.</exception>
		public void DiscardInBuffer()
		{
			this.CheckOpen();
			this.stream.DiscardInBuffer();
		}

		/// <summary>Discards data from the serial driver's transmit buffer.</summary>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		/// <exception cref="T:System.InvalidOperationException">The stream is closed. This can occur because the <see cref="M:System.IO.Ports.SerialPort.Open" /> method has not been called or the <see cref="M:System.IO.Ports.SerialPort.Close" /> method has been called.</exception>
		public void DiscardOutBuffer()
		{
			this.CheckOpen();
			this.stream.DiscardOutBuffer();
		}

		/// <summary>Gets an array of serial port names for the current computer.</summary>
		/// <returns>An array of serial port names for the current computer.</returns>
		/// <exception cref="T:System.ComponentModel.Win32Exception">The serial port names could not be queried.</exception>
		public static string[] GetPortNames()
		{
			int platform = (int)Environment.OSVersion.Platform;
			List<string> list = new List<string>();
			if (platform == 4 || platform == 128 || platform == 6)
			{
				string[] files = Directory.GetFiles("/dev/", "tty*");
				bool flag = false;
				foreach (string text in files)
				{
					if (text.StartsWith("/dev/ttyS") || text.StartsWith("/dev/ttyUSB") || text.StartsWith("/dev/ttyACM"))
					{
						flag = true;
						break;
					}
				}
				foreach (string text2 in files)
				{
					if (flag)
					{
						if (text2.StartsWith("/dev/ttyS") || text2.StartsWith("/dev/ttyUSB") || text2.StartsWith("/dev/ttyACM"))
						{
							list.Add(text2);
						}
					}
					else if (text2 != "/dev/tty" && text2.StartsWith("/dev/tty") && !text2.StartsWith("/dev/ttyC"))
					{
						list.Add(text2);
					}
				}
			}
			else
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("HARDWARE\\DEVICEMAP\\SERIALCOMM"))
				{
					if (registryKey != null)
					{
						foreach (string name in registryKey.GetValueNames())
						{
							string text3 = registryKey.GetValue(name, "").ToString();
							if (text3 != "")
							{
								list.Add(text3);
							}
						}
					}
				}
			}
			return list.ToArray();
		}

		private static bool IsWindows
		{
			get
			{
				PlatformID platform = Environment.OSVersion.Platform;
				return platform == PlatformID.Win32Windows || platform == PlatformID.Win32NT;
			}
		}

		/// <summary>Opens a new serial port connection.</summary>
		/// <exception cref="T:System.UnauthorizedAccessException">Access is denied to the port.  
		/// -or-
		///  The current process, or another process on the system, already has the specified COM port open either by a <see cref="T:System.IO.Ports.SerialPort" /> instance or in unmanaged code.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">One or more of the properties for this instance are invalid. For example, the <see cref="P:System.IO.Ports.SerialPort.Parity" />, <see cref="P:System.IO.Ports.SerialPort.DataBits" />, or <see cref="P:System.IO.Ports.SerialPort.Handshake" /> properties are not valid values; the <see cref="P:System.IO.Ports.SerialPort.BaudRate" /> is less than or equal to zero; the <see cref="P:System.IO.Ports.SerialPort.ReadTimeout" /> or <see cref="P:System.IO.Ports.SerialPort.WriteTimeout" /> property is less than zero and is not <see cref="F:System.IO.Ports.SerialPort.InfiniteTimeout" />.</exception>
		/// <exception cref="T:System.ArgumentException">The port name does not begin with "COM".  
		/// -or-
		///  The file type of the port is not supported.</exception>
		/// <exception cref="T:System.IO.IOException">The port is in an invalid state.  
		/// -or-
		///  An attempt to set the state of the underlying port failed. For example, the parameters passed from this <see cref="T:System.IO.Ports.SerialPort" /> object were invalid.</exception>
		/// <exception cref="T:System.InvalidOperationException">The specified port on the current instance of the <see cref="T:System.IO.Ports.SerialPort" /> is already open.</exception>
		public void Open()
		{
			if (this.is_open)
			{
				throw new InvalidOperationException("Port is already open");
			}
			if (SerialPort.IsWindows)
			{
				this.stream = new WinSerialStream(this.port_name, this.baud_rate, this.data_bits, this.parity, this.stop_bits, this.dtr_enable, this.rts_enable, this.handshake, this.read_timeout, this.write_timeout, this.readBufferSize, this.writeBufferSize);
			}
			else
			{
				this.stream = new SerialPortStream(this.port_name, this.baud_rate, this.data_bits, this.parity, this.stop_bits, this.dtr_enable, this.rts_enable, this.handshake, this.read_timeout, this.write_timeout, this.readBufferSize, this.writeBufferSize);
			}
			this.is_open = true;
		}

		/// <summary>Reads a number of bytes from the <see cref="T:System.IO.Ports.SerialPort" /> input buffer and writes those bytes into a byte array at the specified offset.</summary>
		/// <param name="buffer">The byte array to write the input to.</param>
		/// <param name="offset">The offset in <paramref name="buffer" /> at which to write the bytes.</param>
		/// <param name="count">The maximum number of bytes to read. Fewer bytes are read if <paramref name="count" /> is greater than the number of bytes in the input buffer.</param>
		/// <returns>The number of bytes read.</returns>
		/// <exception cref="T:System.ArgumentNullException">The buffer passed is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The specified port is not open.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="offset" /> or <paramref name="count" /> parameters are outside a valid region of the <paramref name="buffer" /> being passed. Either <paramref name="offset" /> or <paramref name="count" /> is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="offset" /> plus <paramref name="count" /> is greater than the length of the <paramref name="buffer" />.</exception>
		/// <exception cref="T:System.TimeoutException">No bytes were available to read.</exception>
		public int Read(byte[] buffer, int offset, int count)
		{
			this.CheckOpen();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException("offset or count less than zero.");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("offset+count", "The size of the buffer is less than offset + count.");
			}
			return this.stream.Read(buffer, offset, count);
		}

		/// <summary>Reads a number of characters from the <see cref="T:System.IO.Ports.SerialPort" /> input buffer and writes them into an array of characters at a given offset.</summary>
		/// <param name="buffer">The character array to write the input to.</param>
		/// <param name="offset">The offset in <paramref name="buffer" /> at which to write the characters.</param>
		/// <param name="count">The maximum number of characters to read. Fewer characters are read if <paramref name="count" /> is greater than the number of characters in the input buffer.</param>
		/// <returns>The number of characters read.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="offset" /> plus <paramref name="count" /> is greater than the length of the buffer.  
		/// -or-
		///  <paramref name="count" /> is 1 and there is a surrogate character in the buffer.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> passed is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="offset" /> or <paramref name="count" /> parameters are outside a valid region of the <paramref name="buffer" /> being passed. Either <paramref name="offset" /> or <paramref name="count" /> is less than zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">The specified port is not open.</exception>
		/// <exception cref="T:System.TimeoutException">No characters were available to read.</exception>
		public int Read(char[] buffer, int offset, int count)
		{
			this.CheckOpen();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException("offset or count less than zero.");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("offset+count", "The size of the buffer is less than offset + count.");
			}
			int num = 0;
			int num2;
			while (num < count && (num2 = this.ReadChar()) != -1)
			{
				buffer[offset + num] = (char)num2;
				num++;
			}
			return num;
		}

		internal int read_byte()
		{
			byte[] array = new byte[1];
			if (this.stream.Read(array, 0, 1) > 0)
			{
				return (int)array[0];
			}
			return -1;
		}

		/// <summary>Synchronously reads one byte from the <see cref="T:System.IO.Ports.SerialPort" /> input buffer.</summary>
		/// <returns>The byte, cast to an <see cref="T:System.Int32" />, or -1 if the end of the stream has been read.</returns>
		/// <exception cref="T:System.InvalidOperationException">The specified port is not open.</exception>
		/// <exception cref="T:System.ServiceProcess.TimeoutException">The operation did not complete before the time-out period ended.  
		/// -or-
		///  No byte was read.</exception>
		public int ReadByte()
		{
			this.CheckOpen();
			return this.read_byte();
		}

		/// <summary>Synchronously reads one character from the <see cref="T:System.IO.Ports.SerialPort" /> input buffer.</summary>
		/// <returns>The character that was read.</returns>
		/// <exception cref="T:System.InvalidOperationException">The specified port is not open.</exception>
		/// <exception cref="T:System.ServiceProcess.TimeoutException">The operation did not complete before the time-out period ended.  
		/// -or-
		///  No character was available in the allotted time-out period.</exception>
		public int ReadChar()
		{
			this.CheckOpen();
			byte[] array = new byte[16];
			int num = 0;
			char[] chars;
			for (;;)
			{
				int num2 = this.read_byte();
				if (num2 == -1)
				{
					break;
				}
				array[num++] = (byte)num2;
				chars = this.encoding.GetChars(array, 0, 1);
				if (chars.Length != 0)
				{
					goto Block_2;
				}
				if (num >= array.Length)
				{
					return -1;
				}
			}
			return -1;
			Block_2:
			return (int)chars[0];
		}

		/// <summary>Reads all immediately available bytes, based on the encoding, in both the stream and the input buffer of the <see cref="T:System.IO.Ports.SerialPort" /> object.</summary>
		/// <returns>The contents of the stream and the input buffer of the <see cref="T:System.IO.Ports.SerialPort" /> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">The specified port is not open.</exception>
		public string ReadExisting()
		{
			this.CheckOpen();
			int bytesToRead = this.BytesToRead;
			byte[] array = new byte[bytesToRead];
			int count = this.stream.Read(array, 0, bytesToRead);
			return new string(this.encoding.GetChars(array, 0, count));
		}

		/// <summary>Reads up to the <see cref="P:System.IO.Ports.SerialPort.NewLine" /> value in the input buffer.</summary>
		/// <returns>The contents of the input buffer up to the first occurrence of a <see cref="P:System.IO.Ports.SerialPort.NewLine" /> value.</returns>
		/// <exception cref="T:System.InvalidOperationException">The specified port is not open.</exception>
		/// <exception cref="T:System.TimeoutException">The operation did not complete before the time-out period ended.  
		/// -or-
		///  No bytes were read.</exception>
		public string ReadLine()
		{
			return this.ReadTo(this.new_line);
		}

		/// <summary>Reads a string up to the specified <paramref name="value" /> in the input buffer.</summary>
		/// <param name="value">A value that indicates where the read operation stops.</param>
		/// <returns>The contents of the input buffer up to the specified <paramref name="value" />.</returns>
		/// <exception cref="T:System.ArgumentException">The length of the <paramref name="value" /> parameter is 0.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The specified port is not open.</exception>
		/// <exception cref="T:System.TimeoutException">The operation did not complete before the time-out period ended.</exception>
		public string ReadTo(string value)
		{
			this.CheckOpen();
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Length == 0)
			{
				throw new ArgumentException("value");
			}
			byte[] bytes = this.encoding.GetBytes(value);
			int num = 0;
			List<byte> list = new List<byte>();
			for (;;)
			{
				int num2 = this.read_byte();
				if (num2 == -1)
				{
					goto IL_89;
				}
				list.Add((byte)num2);
				if (num2 == (int)bytes[num])
				{
					num++;
					if (num == bytes.Length)
					{
						break;
					}
				}
				else
				{
					num = (((int)bytes[0] == num2) ? 1 : 0);
				}
			}
			return this.encoding.GetString(list.ToArray(), 0, list.Count - bytes.Length);
			IL_89:
			return this.encoding.GetString(list.ToArray());
		}

		/// <summary>Writes the specified string to the serial port.</summary>
		/// <param name="text">The string for output.</param>
		/// <exception cref="T:System.InvalidOperationException">The specified port is not open.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="text" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ServiceProcess.TimeoutException">The operation did not complete before the time-out period ended.</exception>
		public void Write(string text)
		{
			this.CheckOpen();
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			byte[] bytes = this.encoding.GetBytes(text);
			this.Write(bytes, 0, bytes.Length);
		}

		/// <summary>Writes a specified number of bytes to the serial port using data from a buffer.</summary>
		/// <param name="buffer">The byte array that contains the data to write to the port.</param>
		/// <param name="offset">The zero-based byte offset in the <paramref name="buffer" /> parameter at which to begin copying bytes to the port.</param>
		/// <param name="count">The number of bytes to write.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> passed is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The specified port is not open.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="offset" /> or <paramref name="count" /> parameters are outside a valid region of the <paramref name="buffer" /> being passed. Either <paramref name="offset" /> or <paramref name="count" /> is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="offset" /> plus <paramref name="count" /> is greater than the length of the <paramref name="buffer" />.</exception>
		/// <exception cref="T:System.ServiceProcess.TimeoutException">The operation did not complete before the time-out period ended.</exception>
		public void Write(byte[] buffer, int offset, int count)
		{
			this.CheckOpen();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("offset+count", "The size of the buffer is less than offset + count.");
			}
			this.stream.Write(buffer, offset, count);
		}

		/// <summary>Writes a specified number of characters to the serial port using data from a buffer.</summary>
		/// <param name="buffer">The character array that contains the data to write to the port.</param>
		/// <param name="offset">The zero-based byte offset in the <paramref name="buffer" /> parameter at which to begin copying bytes to the port.</param>
		/// <param name="count">The number of characters to write.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> passed is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The specified port is not open.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="offset" /> or <paramref name="count" /> parameters are outside a valid region of the <paramref name="buffer" /> being passed. Either <paramref name="offset" /> or <paramref name="count" /> is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="offset" /> plus <paramref name="count" /> is greater than the length of the <paramref name="buffer" />.</exception>
		/// <exception cref="T:System.ServiceProcess.TimeoutException">The operation did not complete before the time-out period ended.</exception>
		public void Write(char[] buffer, int offset, int count)
		{
			this.CheckOpen();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("offset+count", "The size of the buffer is less than offset + count.");
			}
			byte[] bytes = this.encoding.GetBytes(buffer, offset, count);
			this.stream.Write(bytes, 0, bytes.Length);
		}

		/// <summary>Writes the specified string and the <see cref="P:System.IO.Ports.SerialPort.NewLine" /> value to the output buffer.</summary>
		/// <param name="text">The string to write to the output buffer.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="text" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The specified port is not open.</exception>
		/// <exception cref="T:System.TimeoutException">The <see cref="M:System.IO.Ports.SerialPort.WriteLine(System.String)" /> method could not write to the stream.</exception>
		public void WriteLine(string text)
		{
			this.Write(text + this.new_line);
		}

		private void CheckOpen()
		{
			if (!this.is_open)
			{
				throw new InvalidOperationException("Specified port is not open.");
			}
		}

		internal void OnErrorReceived(SerialErrorReceivedEventArgs args)
		{
			SerialErrorReceivedEventHandler serialErrorReceivedEventHandler = (SerialErrorReceivedEventHandler)base.Events[this.error_received];
			if (serialErrorReceivedEventHandler != null)
			{
				serialErrorReceivedEventHandler(this, args);
			}
		}

		internal void OnDataReceived(SerialDataReceivedEventArgs args)
		{
			SerialDataReceivedEventHandler serialDataReceivedEventHandler = (SerialDataReceivedEventHandler)base.Events[this.data_received];
			if (serialDataReceivedEventHandler != null)
			{
				serialDataReceivedEventHandler(this, args);
			}
		}

		internal void OnDataReceived(SerialPinChangedEventArgs args)
		{
			SerialPinChangedEventHandler serialPinChangedEventHandler = (SerialPinChangedEventHandler)base.Events[this.pin_changed];
			if (serialPinChangedEventHandler != null)
			{
				serialPinChangedEventHandler(this, args);
			}
		}

		/// <summary>Indicates that an error has occurred with a port represented by a <see cref="T:System.IO.Ports.SerialPort" /> object.</summary>
		[MonitoringDescription("")]
		public event SerialErrorReceivedEventHandler ErrorReceived
		{
			add
			{
				base.Events.AddHandler(this.error_received, value);
			}
			remove
			{
				base.Events.RemoveHandler(this.error_received, value);
			}
		}

		/// <summary>Indicates that a non-data signal event has occurred on the port represented by the <see cref="T:System.IO.Ports.SerialPort" /> object.</summary>
		[MonitoringDescription("")]
		public event SerialPinChangedEventHandler PinChanged
		{
			add
			{
				base.Events.AddHandler(this.pin_changed, value);
			}
			remove
			{
				base.Events.RemoveHandler(this.pin_changed, value);
			}
		}

		/// <summary>Indicates that data has been received through a port represented by the <see cref="T:System.IO.Ports.SerialPort" /> object.</summary>
		[MonitoringDescription("")]
		public event SerialDataReceivedEventHandler DataReceived
		{
			add
			{
				base.Events.AddHandler(this.data_received, value);
			}
			remove
			{
				base.Events.RemoveHandler(this.data_received, value);
			}
		}

		/// <summary>Indicates that no time-out should occur.</summary>
		public const int InfiniteTimeout = -1;

		private const int DefaultReadBufferSize = 4096;

		private const int DefaultWriteBufferSize = 2048;

		private const int DefaultBaudRate = 9600;

		private const int DefaultDataBits = 8;

		private const Parity DefaultParity = Parity.None;

		private const StopBits DefaultStopBits = StopBits.One;

		private bool is_open;

		private int baud_rate;

		private Parity parity;

		private StopBits stop_bits;

		private Handshake handshake;

		private int data_bits;

		private bool break_state;

		private bool dtr_enable;

		private bool rts_enable;

		private ISerialStream stream;

		private Encoding encoding = Encoding.ASCII;

		private string new_line = Environment.NewLine;

		private string port_name;

		private int read_timeout = -1;

		private int write_timeout = -1;

		private int readBufferSize = 4096;

		private int writeBufferSize = 2048;

		private object error_received = new object();

		private object data_received = new object();

		private object pin_changed = new object();
	}
}
