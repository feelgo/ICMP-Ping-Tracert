using System;

namespace ICMP.Messages {
	// Definition of a base ICMP-message
	public abstract class IcmpMessage {
		public byte Type { get; set; } // Type of the message
		public byte Code { get; set; } // The message code
		public ushort CheckSum { get; set; } // The check sum of the message

		// Object serialization into a byte[] -- the message itself
		public virtual byte[] SerializeObject() {
			byte[] msg = new byte[4];
			Array.Copy(BitConverter.GetBytes(Type), 0, msg, 0, 1);
			Array.Copy(BitConverter.GetBytes(Code), 0, msg, 1, 1);
			Array.Copy(BitConverter.GetBytes(CheckSum), 0, msg, 2, 2);

			return msg;
		}

		// Calculation the check sum of the message
		public ushort GetCheckSum() {
			ulong sum = 0;
			byte[] msg = SerializeObject();

			// Sum all words and adding the final byte if size is odd
			int i;

			for (i = 0; i < msg.Length - 1; i += 2) {
				sum += BitConverter.ToUInt16(msg, i);
			}

			if (i != msg.Length) {
				sum += msg[i];
			}

			sum = (sum >> 16) + (sum & 0xffff);
			sum += (sum >> 16);

			return (ushort)(~sum);
		}
	}
}
