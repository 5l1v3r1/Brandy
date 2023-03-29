# Brandy Network Communications Framework
Brandy was designed to make network communications easy in C# .NET Framework using TCP technology. It was created with simplicity in mind for small scale projects.

# Features
* Can handle multiple client connections
* Can send and receive a byte[] array format of data that is easy to manipulate
* Can listen on multiple ports

# Usage
## Clientside: 
```c#
static BrandyClient brandyClient = new BrandyClient();

static void Main(string[] args)
{
	brandyClient.Connected += Client_Connected; // lets you know that you have connected
	brandyClient.DataReceived += Client_DataReceived; // provides a byte array as data received
	brandyClient.Disconnected += Client_Disconnected; // lets you know that you have disconnected
	brandyClient.Connect("127.0.0.1", 80);
}
```

## Serverside: 
```c#
static BrandyServer brandyServer = new BrandyServer();

static void Main(string[] args)
{
	brandyServer.BrandyClientConnected += Server_BrandyClientConnected; // lets you know that a client has connected
	brandyServer.BrandyClientDisconnected += Server_BrandyClientDisconnected; // lets you know that a client has disconnected
	brandyServer.Listen(80, 81, 82); // listen to multiple ports
}
```