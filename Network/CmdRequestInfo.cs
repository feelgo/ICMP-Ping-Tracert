namespace ICMP.Network {
	public enum CmdType {
		ping,
		tracert
	}

	public class CmdRequestInfo {
		public int UpdatePeriod { get; set; } = 1000;
		public CmdType Cmd { get; set; } = CmdType.tracert;
		public string Host { get; set; } = "127.0.0.1";
		public int WeightPacket { get; set; } = 32;
		public int NbrEcho { get; set; } = 4;
		public int Timeout { get; set; } = 1000;
		public int MaxTTL { get; set; } = 128;
		public string SessionId { get; set; }

		public CmdRequestInfo(string host, string sessionId, CmdType cmd, int nbrEcho, int weightPacket, int updatePeriod, int timeout, int maxTTL) {
			if (weightPacket < 1) {
				WeightPacket = 1;
			}

			Host = host;
			SessionId = sessionId;
			Cmd = cmd;
			NbrEcho = nbrEcho;
			UpdatePeriod = updatePeriod;
			Timeout = timeout;
			MaxTTL = maxTTL;
		}
	}
}
