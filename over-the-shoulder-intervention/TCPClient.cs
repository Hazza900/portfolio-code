public class TCPClient : MonoBehaviour
{
    public string ipAddress = "127.0.0.1";
    public int port = 59384;

    public GameObject connectionPanel;
    public GameObject interactionPanel;

    private TcpClient tcpClient;
    private NetworkStream netStream;
    private byte[] buffer = new byte[4096];
    private int bytesRecieved = 0;
    private string messageRecieved;

    public void StartClient(Text errorText)
    {
        if (IPAddress.TryParse(ipAddress, out IPAddress validatedIP))
        {
            IPEndPoint ipend = new IPEndPoint(validatedIP, port);

            try
            {
                tcpClient = new TcpClient(AddressFamily.InterNetwork);
                tcpClient.Connect(ipend.Address, port);

                connectionPanel.SetActive(false);
                interactionPanel.SetActive(true);

                StartCoroutine(ListenForMessages());
            }
            catch (SocketException e)
            {
                Debug.Log("SocketException: " + e.Message);
                errorText.text = "An error has occoured: " + e.Message + " Please refer to the ReadMe file for help";
                StopClient();
            }
        }
        else
        {
            //Error message
            errorText.text = "Invalid IP address entered";
        }
    }

    public void StopClient()
    {
        if (tcpClient == null)
            return;

        if (tcpClient.Connected)
        {
            tcpClient.Close();
            StopCoroutine(ListenForMessages());
            tcpClient = null;
        } 
    }

    private IEnumerator ListenForMessages()
    {
        Debug.Log("Connected, listening for messages");

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
        while (tcpClient != null && tcpClient.Connected);
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

    public void ClientSendMessage(string s)
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
