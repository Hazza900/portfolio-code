public class NetworkManager : TCPServer
{
    //Singleton Instance
    public static NetworkManager instance;
    InteractionManager interactionManager;
    UIManager UIManager;

    void Awake()
    {
        #region singleton
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion
    }

    void Start()
    {
        //Get reference to UI Manager script
        UIManager = GameObject.FindGameObjectWithTag("UIManager").gameObject.GetComponent<UIManager>();
        ipAddress = IPManager.GetIP(ADDRESSFAM.IPv4);
    }

    public override void ClientConnected(IAsyncResult result)
    {
        base.ClientConnected(result);

        Debug.Log("Called");

        //Enable server start button
        UIManager.startServer.interactable = true;
        UIManager.connectionText.text = "Client Connected";
    }

    public override void ProcessMessage(string s)
    {
        base.ProcessMessage(s);

        switch (s)
        {
            #region companion interactions

            case "GMove":

                if (interactionManager == null)
                    interactionManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<InteractionManager>();

                StartCoroutine(interactionManager.BuffMovement());
                break;

            case "GGun":

                if (interactionManager == null)
                    interactionManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<InteractionManager>();

                StartCoroutine(interactionManager.EnhancedWeapon());
                break;

            case "GHealth":

                if (interactionManager == null)
                    interactionManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<InteractionManager>();

                interactionManager.SpawnHealth();

                break;

            case "GEnemy":

                if (interactionManager == null)
                    interactionManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<InteractionManager>();

                interactionManager.DisarmEnemy();

                break;

            case "BMove":

                if (interactionManager == null)
                    interactionManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<InteractionManager>();

                StartCoroutine(interactionManager.SlowPlayer());

                break;

            case "BControls":

                if (interactionManager == null)
                    interactionManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<InteractionManager>();

                StartCoroutine(interactionManager.ScrambleControls());

                break;

            case "BHeal":

                if (interactionManager == null)
                    interactionManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<InteractionManager>();

                interactionManager.HealEnemies();

                break;

            case "BBlind":

                if (interactionManager == null)
                    interactionManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<InteractionManager>();

                StartCoroutine(interactionManager.BlindPlayer());

                break;

                #endregion
        }
    }

    public void OnApplicationQuit()
    {
        if (tcpClient != null)
        {
            ServerSendMessage("Quit");
        }
    }

    public void StartGame()
    {
        ServerSendMessage("Ready");
    }
}
