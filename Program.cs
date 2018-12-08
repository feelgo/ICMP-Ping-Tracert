using System;

using ICMP.Network;

namespace ICMP {
	class Program {
		[STAThread]
		static void Main(string[] args) {
			// Testing
			Console.WriteLine($"{new string('-', 10)} Test ping {new string('-', 10)}\r\n");
			var request = new IcmpRequest(new CmdRequestInfo("www.github.com", "Hello", CmdType.ping, 4, 32, 1000, 1000, 128));

			Console.WriteLine($"\r\n\r\n{new string('-', 10)} Test tracert {new string('-', 10)}\r\n");
			request = new IcmpRequest(new CmdRequestInfo("www.bmstu.ru", "Hello", CmdType.tracert, 4, 32, 1000, 1000, 128));

			Console.WriteLine("\n\nPress any key to exit...");
			Console.ReadLine();
		}
	}
}
