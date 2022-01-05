public class NetworkManager : TCPClient
{
    UIController uiController;

    public static NetworkManager instance;
    public bool isGameOver;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        uiController = GetComponent<UIController>();
    }

    public override void ProcessMessage(string s)
    {
        base.ProcessMessage(s);

        switch (s)
        {
            case "Ready":

                uiController.statusText.text = "Interact with the main game using the buttons below";
                uiController.EnableAllButtons();
                break;

            case "Complete":

                uiController.statusText.text = "All test chambers complete!";
                uiController.DisableAllButtons();
                isGameOver = true;
                break;

            case "Quit":

                Debug.Log("Quit recieved");
                uiController.DisconnectButton();
                break;
        }
    }
}
