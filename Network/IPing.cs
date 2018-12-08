using ICMP.Network;

// Interface that declares Ad-Hoc polymorph methods for ping operation
public interface IPing
{
	PingRequestInfo Ping();
	PingRequestInfo Ping(int timeout);
	PingRequestInfo Ping(int timeout, int TTL);
	PingRequestInfo Ping(int timeout, int TTL, int weight);
}