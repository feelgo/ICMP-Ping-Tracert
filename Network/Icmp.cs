using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using ICMP.Messages;

namespace ICMP.Network {
	// Implements the ICMP messaging service
	// The implementation only supports the echo message (better known as "ping")
	public class Icmp : IPing {
		private IPAddress Host { get; set; } // Host, specifies the address of the remote host to communicate with
		public Socket ClientSocket { get; set; } // Socket, used to communicate with the remove server
		public DateTime StartTime { get; set; } // The time when the ping begins
		public Timer PingTimeOut { get; set; } // Triggers the PingTimeOut() method
		public bool HasTimeOut { get; set; } // Indicates whether the ping has timed out or not

		public Icmp (IPAddress host) {
			Host = host;
		}

		~Icmp() {
			if (ClientSocket != null) {
				ClientSocket.Close();
			}
		}

		// Callback, called when the ping method times out
		private void OnTimeOut(object state) {
			HasTimeOut = true;
			if (ClientSocket != null) {
				ClientSocket.Close(); // This will result in a throw of a SocketException in Ping() method
			}
		}

		// Generating the echo message to send
		private byte[] GetEchoMsgBuf(int weight) {
			if (weight < 1) {
				weight = 1;
			}

			var msg = new EchoMessage {
				Type = 8,
				Data = new System.Byte[1 * weight]
			};

			for (int i = 0; i < weight; i++) {
				msg.Data[i] = 32; // Sending spaces
			}

			msg.CheckSum = msg.GetCheckSum();
			return msg.SerializeObject();
		}

		// Initialization of an ICMP ping with timeout of 1 sec
		public PingRequestInfo Ping() {
			return Ping(1000);
		}

		public PingRequestInfo Ping(int timeout) {
			// By default TTL == 128
			return Ping(timeout, 128);
		}

		public PingRequestInfo Ping(int timeout, int TTL) {
			return Ping(timeout, TTL, 32);
		}

		public PingRequestInfo Ping(int timeout, int TTL, int weight) {
			TimeSpan time;
			EndPoint remoteEP = new IPEndPoint(Host, 0);
			int returnTTL = 128;
			ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
			byte[] buf = GetEchoMsgBuf(weight);

			// To diplay the ping packet:
			//
			// foreach (byte b in buf) {
			//     Console.Write($"{b}-");
			// }
			//
			// Console.WriteLing();

			HasTimeOut = false;
			PingTimeOut = new Timer(new TimerCallback(OnTimeOut), null, timeout, Timeout.Infinite);
			StartTime = DateTime.Now;

			try {
				// Set the TTL option (usefull for tracert)
				ClientSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, TTL);

				if (ClientSocket.SendTo(buf, remoteEP) <= 0) {
					throw new SocketException();
				}

				buf = new byte[buf.Length + 20];

				if (ClientSocket.ReceiveFrom(buf, ref remoteEP) <= 0) {
					throw new SocketException();
				}

				// Get the TTL from returned packet
				returnTTL = Convert.ToInt32(buf[8].ToString());
			} catch (SocketException e) {
				if (HasTimeOut) {
					return new PingRequestInfo {
						TTL = returnTTL,
						IPStartPoint = Host.ToString(),
						IPEndPoint = remoteEP.ToString().Substring(0, remoteEP.ToString().Length - 2),
						Duration = TimeSpan.MaxValue
					};
				} else {
					throw e;
				}
			} finally {
				// Closing the socket and destroying the timer
				ClientSocket.Close();
				ClientSocket = null;
				PingTimeOut.Change(Timeout.Infinite, Timeout.Infinite);
				PingTimeOut.Dispose();
				PingTimeOut = null;
			}

			time = DateTime.Now.Subtract(StartTime);
			return new PingRequestInfo {
				TTL = returnTTL,
				IPStartPoint = Host.ToString(),
				IPEndPoint = remoteEP.ToString().Substring(0, remoteEP.ToString().Length - 2),
				Duration = time
			};
		}
	}
}
