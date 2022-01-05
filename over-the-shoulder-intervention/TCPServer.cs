public class TCPServer : MonoBehaviour
{
    [Header("Server Settings")]
    public string ipAddress;
    public int port = 2481;

    private TcpListener tcpListener;
    public TcpClient tcpClient;
    private NetworkStream netStream;
    private byte[] buffer = new byte[4096];
    private int bytesRecieved = 0;
    private string messageRecieved;
    bool isListening;

    public virtual void StartServer()
    {
        //Initilise and start server at ip address and port
        tcpListener = new TcpListener(IPAddress.Any, port);
        tcpListener.Start();

        //start waiting for async client connection
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(ClientConnected), null);

        Debug.Log("Server Started and listening for clients");
    }

    public virtual void CloseServer()
    {
        if (tcpClient != null)
        {
            ServerSendMessage("Quit");
            tcpClient.Close();
            tcpClient = null;
        }

        tcpListener.Stop();

        Debug.Log("Server closed successfully");
    }

    public virtual void ClientConnected(IAsyncResult result)
    {
        //Once async connection made, set client and stop accepting connections
        tcpClient = tcpListener.EndAcceptTcpClient(result);
        Debug.Log("Client Connected");
    }

    private void Update()
    {
        if (tcpClient != null && !isListening)
        {
            StartCoroutine(ListenForMessages());
        }
    }

    private IEnumerator ListenForMessages()
    {
        Debug.Log("Listening for messages");
        isListening = true;

        netStream = tcpClient.GetStream();

        do
        {
            netStream.BeginRead(buffer, 0, buffer.Length, MessageRecieved, null);

            if (bytesRecieved > 0)
            {
                ProcessMessage(messageRecieved);
                bytesRecieved = 0;
            }

            yield return new WaitForSeconds(2);
        }
        while (tcpClient != null);
    }

    private void MessageRecieved(IAsyncResult result)
    {
        if (tcpClient.Connected && result.IsCompleted)
        {
            bytesRecieved = netStream.EndRead(result);
            messageRecieved = Encoding.ASCII.GetString(buffer, 0, bytesRecieved);
        }
    }

    public virtual void ProcessMessage(string s)
    {
        Debug.Log("Message Recieved: " + s);
    }

    public void ServerSendMessage(string s)
    {
        if (!tcpClient.Connected)
        {
            Debug.Log("Client not connected!");
            return;
        }

        byte[] message = Encoding.ASCII.GetBytes(s);
        netStream.Write(message, 0, message.Length);
    }
}
