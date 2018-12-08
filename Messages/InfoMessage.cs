using System;

namespace ICMP.Messages {
	// Definition of a ICMP-message with an ID and sequance number
	public class InfoMessage : IcmpMessage {
		public ushort Id { get; set; } // An ID
		public ushort SeqNum { get; set; } // Sequence number

		// Object serialization into a byte[] -- the message itself
		public override byte[] SerializeObject() {
			byte[] msg = new byte[8];
			Array.Copy(base.SerializeObject(), 0, msg, 0, 4);
			Array.Copy(BitConverter.GetBytes(Id), 0, msg, 4, 2);
			Array.Copy(BitConverter.GetBytes(SeqNum), 0, msg, 6, 2);

			return msg;
		}
	}
}
