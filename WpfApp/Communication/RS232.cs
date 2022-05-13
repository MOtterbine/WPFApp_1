using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp.Communication
{
	public class RS232 : ICommunicationDevice
	{

		public event DeviceEvent CommunicationEvent;

		#region Properties

		public string MessageString { get; set; }
		private SerialPort _SerialPort = null;
		public int Port
		{
			get
			{
				return this._Port;
			}
			set
			{
				if (this._SerialPort.IsOpen == false)
				{
					this._Port = value;
					this._SerialPort.PortName = "Com" + value;
				}
			}
		}
		private int _Port = -1;
		public StopBits StopBits
		{
			get
			{
				return this._SerialPort.StopBits;
			}
			set
			{
				if (this._SerialPort.IsOpen == false)
				{
					this._SerialPort.StopBits = value;
				}
			}
		}
		protected int _StopBits = 0;
		public Parity Parity
		{
			get
			{
				return this._SerialPort.Parity;
			}
			set
			{
				if (this._SerialPort.IsOpen == false)
				{
					this._SerialPort.Parity = value;
				}
			}
		}
		protected int _DataBits = 8;
		public int DataBits
		{
			get
			{
				return this._SerialPort.DataBits;
			}
			set
			{
				if (this._SerialPort.IsOpen == false)
				{
					this._SerialPort.DataBits = value;
				}
			}
		}
		protected int _BaudRate = 9600;
		public int BaudRate
		{
			get
			{
				return this._SerialPort.BaudRate;
			}
			set
			{
				if (this._SerialPort.IsOpen == false)
				{
					this._SerialPort.BaudRate = value;
				}
			}
		}
		public Handshake Handshake
		{
			get
			{
				return this._SerialPort.Handshake;
			}
			set
			{
				if (this._SerialPort.Handshake != value)
				{
					this._SerialPort.Handshake = value;
				}
			}
		}

		#endregion

		#region Methods

		public static string[] GetPortNames()
		{
			return SerialPort.GetPortNames().Distinct().ToArray();
		}

		// Copy Constructor
		public RS232() // Required for serialization
		{
			this.Initialize();
			this._SerialPort = new SerialPort();
		}
		public RS232(string portname)
		{
			this.Initialize();
			this._SerialPort = new SerialPort(portname);
		}
		public RS232(string portname, int baud)
		{
			this.Initialize();
			this._SerialPort = new SerialPort(portname, baud);
		}
		public RS232(string portname, int baud, int dataBits)
		{
			this.Initialize();
			this._SerialPort = new SerialPort(portname, baud, Parity.None, dataBits);
		}
		public RS232(string portname, int baud, int dataBits, StopBits stopBits)
		{
			this.Initialize();
			this._SerialPort = new SerialPort(portname, baud, Parity.None, 8, stopBits);
		}
		public RS232(string portname, int baud, int dataBits, StopBits stopBits, Parity parity)
		{
			this.Initialize();
			this._SerialPort = new SerialPort(portname, baud, Parity.None, dataBits, stopBits);
		}

		public RS232(int port, int baud)
		{
			this.Initialize();
			string portname = "Com" + port.ToString();
			this._SerialPort = new SerialPort(portname, baud);
		}
		public RS232(int port, int baud, int dataBits)
		{
			this.Initialize();
			string portname = "Com" + port.ToString();
			this._SerialPort = new SerialPort(portname, baud, Parity.None, dataBits);
		}
		public RS232(int port, int baud, int dataBits, StopBits stopBits)
		{
			this.Initialize();
			string portname = "Com" + port.ToString();
			this._SerialPort = new SerialPort(portname, baud, Parity.None, dataBits, stopBits);
		}
		public RS232(int port, int baud, int dataBits, StopBits stopBits, Parity parity)
		{
			this.Initialize();
			string portname = "Com" + port.ToString();
			this._SerialPort = new SerialPort(portname, baud, Parity.None, dataBits, stopBits);
		}
		public bool Reset()
		{
			if (this._SerialPort.IsOpen)
			{
				this.Close();
				return this.Open();
			}
			return false;
		}
		protected void FireStatusMessage(ChannelEventArgs channelArgs)
		{
			this.FireCommunicationEvent(channelArgs);
		}

		#endregion


		protected void FireCommunicationEvent(ChannelEventArgs e)
		{
			if (this.CommunicationEvent != null)
			{
				CommunicationEvent(this, e);
			}
		}



		#region Inherited Properties

		public string DeviceName { get; set; }

		public string Description
		{
			get
			{
				return "RS232: " + this._SerialPort.PortName;
			}
			set
			{

			}
		}
		public bool IsConnected
		{
			get
			{
				return this._SerialPort.IsOpen;
			}
		}
		protected bool _IsListening = false;

		#endregion

		#region Inherited Methods

		public bool Initialize()
		{
			
			return true;
		}

		void _SerialPort_PinChanged(object sender, SerialPinChangedEventArgs e)
		{
			//throw new NotImplementedException();
		}
		public bool Open()
		{
			try
			{
				//	this._SerialPort.Handshake = Handshake.RequestToSend;
				this._SerialPort.Open();
				this._SerialPort.ReadTimeout = 300;
				this._SerialPort.WriteTimeout = 300;
				this._SerialPort.DiscardInBuffer();
				this._SerialPort.DiscardOutBuffer();
				this._SerialPort.DataReceived += new SerialDataReceivedEventHandler(_SerialPort_DataReceived);
				this._SerialPort.PinChanged += new SerialPinChangedEventHandler(_SerialPort_PinChanged);
				//			this._SerialPort.ReadChar();
			}
			catch (Exception ex)
			{
				this.MessageString = ex.Message;
				return false;
			}
			using (ChannelEventArgs evt = new ChannelEventArgs())// this._SerialPort))
			{
				evt.Event = CommunicationEvents.ConnectedAsClient;
				evt.Description = "Connected...";
				FireStatusMessage(evt);
			}
			return true;
		}

		void _SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			using (ChannelEventArgs evt = new ChannelEventArgs()) // (this._SerialPort))
			{
				evt.Event = CommunicationEvents.ReceiveEnd;
				evt.Description = this._SerialPort.ReadExisting();
				evt.data = Encoding.ASCII.GetBytes(evt.Description);
				FireStatusMessage(evt);
			}
		}
		/// <summary>
		/// Synchronous
		/// </summary>
		/// <returns> byte[]</returns>
		public byte[] Read()
		{
			byte[] retData = null;
			while (this._SerialPort.BytesToRead > 0)
			{
				retData = new byte[this._SerialPort.BytesToRead];
				//  this._SerialPort.
				this._SerialPort.Read(retData, 0, this._SerialPort.BytesToRead);
			}

			return retData;
		}
		public async Task<bool> Send(string text)
		{
			await this.Send(Encoding.UTF8.GetBytes(text), 0, text.Length);
			return false;
		}
		public async Task<bool> Send(byte[] buffer, int offset, int count)
		{
			return await Task<bool>.Run(() =>
			{
				try
				{
					this._SerialPort.DiscardOutBuffer();
					this._SerialPort.Write(buffer, offset, count);
				}
				catch (Exception ex)
				{
					this.MessageString = ex.Message;
					return false;
				}
				return true;
			});
		}
















		public bool Close()
		{
			this._SerialPort.Close();
			using (ChannelEventArgs evt = new ChannelEventArgs())
			{
				evt.Event = CommunicationEvents.Disconnected;
				FireStatusMessage(evt);
			}
			return true;
		}
		public void RunEditForm()
		{

		}
		#endregion
	}

}
