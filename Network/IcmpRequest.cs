using System;
using System.Net;
using System.Threading;

namespace ICMP.Network {
	public class IcmpRequest {
		public IcmpRequest(CmdRequestInfo request) {
			switch (request.Cmd) {
				case CmdType.ping:
					var icmpPing = new Icmp(Dns.GetHostEntry(request.Host).AddressList[0]);
					Console.WriteLine($"Pinging {request.Host} [{Dns.GetHostEntry(request.Host).AddressList[0]}] with {request.WeightPacket} bytes of data:\n");

					while (request.NbrEcho > 0) {
						try {
							var result = icmpPing.Ping(request.Timeout, request.MaxTTL, request.WeightPacket);

							if (result.Duration.Equals(TimeSpan.MaxValue)) {
								Console.WriteLine("Request timed out.");
							} else {
								Console.WriteLine($"Reply from {Dns.GetHostEntry(request.Host).AddressList[0]}: bytes = {request.WeightPacket}, time = {Math.Round(result.Duration.TotalMilliseconds)}ms, TTL = {result.TTL}");
							}

							if (1000 - result.Duration.TotalMilliseconds > 0) {
								Thread.Sleep(request.UpdatePeriod - (int) result.Duration.TotalMilliseconds);
							}
						} catch {
							Console.WriteLine("Network error");
						}

						request.NbrEcho--;
					}

					break;

				case CmdType.tracert:
					var icmpTracert = new Icmp(Dns.GetHostEntry(request.Host).AddressList[0]);
					Console.WriteLine($"Tracing route to {request.Host} [{Dns.GetHostEntry(request.Host).AddressList[0]}]\r\nover a maximum of {request.MaxTTL} hops:\n");

					bool isFind = false;
					bool isTimeout = false;
					int nbrTTL = 1;
					int i = 1;
					int nbrErr = 0;
					string IP = Dns.GetHostEntry(request.Host).AddressList[0].ToString();

					while (!isFind & !isTimeout) {
						try {
							nbrTTL++;

							var result = icmpTracert.Ping(request.Timeout, nbrTTL);

							if (nbrTTL == 2) {
								result = icmpTracert.Ping(request.Timeout, nbrTTL);
							}

							if (result.Duration.Equals(TimeSpan.MaxValue)) {
								throw new Exception("Time out");
							} else {
								string hostName = "";

								try {
									hostName = Dns.GetHostEntry(result.IPEndPoint).HostName.ToString() + " ";
								} catch { }

								Console.WriteLine($"{new string(' ', 3).Substring(0, 3 - i.ToString().Length)} {i}\t {Convert.ToInt32(Math.Max(1, result.Duration.TotalMilliseconds))}ms\t {hostName} [{Dns.GetHostEntry(result.IPEndPoint).AddressList[0]}]");

								nbrErr = 0;
								i++;
							}

							if (result.IPEndPoint == IP) {
								isFind = true;
							}

							if (nbrTTL >= request.MaxTTL) {
								isTimeout = true;
							}
						} catch {
							nbrErr++;

							if (nbrErr > 10) {
								isTimeout = true;
							}
						}
					}

					break;

				default:
					Console.WriteLine("Unknown command");
					break;
			}

			Console.Write("\r\nTrace complete");
		}
	}
}
