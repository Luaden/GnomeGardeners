using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Debug variables
    [SerializeField]
    private bool debugMenu;
    [SerializeField]
    private string sceneToLoad;
    public bool loadTestingScenes;

    //Cached references
    private static GameManager gameManager;
    private GameTime gameTime;
    private PlayerConfigManager playerConfigManager;
    private IInteractionController interactionController;
    private GridManager gridManager;
    private SceneController sceneController;
    private AudioManager audioManager;
    private LevelManager levelManager;
    private HazardController hazardController;
    private ConfigController configController;

    // Game variables
    public bool playersReady = false;
    bool werePlayersReady = false;



    public static GameManager Instance { get => gameManager; }
    public GameTime Time { get => gameTime; set => gameTime = value; }
    public PlayerConfigManager PlayerConfigManager { get => playerConfigManager; set => playerConfigManager = value; }
    public IInteractionController InteractionController { get => interactionController; set => interactionController = value; }
    public GridManager GridManager { get => gridManager; set => gridManager = value; }
    public SceneController SceneController { get => sceneController; set => sceneController = value; }
    public AudioManager AudioManager { get => audioManager; set => audioManager = value; }
    public LevelManager LevelManager { get => levelManager; set => levelManager = value; }
    public HazardController HazardController { get => hazardController; set => hazardController = value; }
    public ConfigController Config { get => configController; set => configController = value; }

    public bool DebugMenu { get => debugMenu; }
    public string SceneToLoad { get => sceneToLoad; }

    #region Unity Methods

    private void Awake()
    {
        #region Singleton
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            gameManager = this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion
    }

    private void Update()
    {
        if(werePlayersReady != playersReady)
        {
        }
        werePlayersReady = playersReady;
    }

    #endregion

    #region Private Methods

    [ContextMenu("Announce Times")]
    private void AnnounceTimes()
    {
        Debug.Log("Game elapsed time: " + gameTime.ElapsedTime);
    }

    #endregion
}