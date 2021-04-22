using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool debugMenu;
    [SerializeField]
    private string sceneToLoad;

    private static GameManager gameManager;
    private GameTime gameTime;
    private LevelManager levelManager;
    private ObjectManager objectManager;
    private PlayerConfigManager playerConfigManager;
    private IInteractionController interactionController;
    private GridManager gridManager;
    private SceneController sceneController;
    private AudioManager audioManager;

    public static GameManager Instance { get => gameManager; }
    public GameTime Time { get => gameTime; set => gameTime = value; }
    public LevelManager LevelManager { get => levelManager; set => levelManager = value; }
    public ObjectManager ObjectManager { get => objectManager; set => objectManager = value; }
    public PlayerConfigManager PlayerConfigManager { get => playerConfigManager; set => playerConfigManager = value; }
    public IInteractionController InteractionController { get => interactionController; set => interactionController = value; }
    public GridManager GridManager { get => gridManager; set => gridManager = value; }
    public SceneController SceneController { get => sceneController; set => sceneController = value; }
    public AudioManager AudioManager { get => audioManager; set => audioManager = value; }

    public bool DebugMenu { get => debugMenu; }

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

    #endregion

    #region Private Methods

    [ContextMenu("Start Level")]
    private void StartLevel()
    {
        Instance.LevelManager.SetLevelActive(0);
    }

    [ContextMenu("Announce Times")]
    private void AnnounceTimes()
    {
        Debug.Log("Game elapsed time: " + gameTime.ElapsedTime);
    }

    #endregion
}
