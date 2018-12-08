using System;

namespace ICMP.Network {
	public class PingRequestInfo {
		public int TTL {get;set;}
		public string IPStartPoint {get;set;}
		public string IPEndPoint {get;set;}
		public TimeSpan Duration {get;set;}
	}
}