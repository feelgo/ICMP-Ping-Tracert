# The Ping and Tracert utilities implementation

This repository contains the implementation of the ping and tracert utilities using ICMP.

I've used **.NET Core 2.1 CLR** as runtime environment.

# Using

There's the test of implementation in `Program.cs` is described. If you want to test each utility separatlelly, you just need to change `Program.cs` in some such way:

For ping as an example:
```csharp
static void Main(string[] args) {
	var request = new IcmpRequest(new CmdRequestInfo(args[0], "Your SessionId", CmdType.ping, 4, 32, 1000, 1000, 128));
}
```

Then you need to build the binary by using `dotnet build` and run the binary. Or just run once by `dotnet run "www.microsoft.com"` for example.