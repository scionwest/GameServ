# GameServ

An experimental, still under early development, UDP Server for games developed in C#. All work is taking place under the /dev branch.

## Example Server usage:

    var ipAddress = new byte[] { 10, 0, 1, 6 };
    IServer server = new Server(ipAddress);
    server.Start();
    
If you want to react to messages arriving from the client, you can subscribe to the `ClientDatagramReceived` callback.

    server.ClientDatagramReceived += datagram =>
    {

    }
    
To shut the server down, you can call `server.Shutdown()`.

## Creating a datagram to send from the client

To create a datagram, you have to implement the `IServerDatagram` interface.

    public class MessageDatagram : IClientDatagram
    {
        public MessageDatagram(){}
        public MessageDatagram(string message) => this.Message = message;

        public IClientDatagramHeader Header { get; set; }

        public string Message { get; private set; }

        public long TimeStamp => DateTime.Now.Ticks;

        public void Deserialize(BinaryReader deserializer)
        {
            this.Message = deserializer.ReadString();
        }

        public bool IsMessageValid()
        {
            return string.IsNullOrEmpty(this.Message);
        }

        public void PrepareForReuse()
        {
            this.Message = string.Empty;
        }

        public void Serialize(BinaryWriter serializer)
        {
            serializer.Write(this.Message);
        }
    }
    
You can send datagrams from the server to the client.

server.SendMessage(client, datagram);

Creation of the client and the datagram is currently not built. The source supports it, but I have not built in any API to expose that stuff out of the box.

## Design Roadmap

There are several things planned that have not been developed yet.

- Ability to subscribe to specific Datagrams arriving from the client.
- Easier manor to build datagram mappings.
- Provide the server app with the ClientConnection instance upon establishing an initial session.
- Support authenticating users
- Built some default datagrams
- Add client-side implementation.
- Add support for configuring the server.
