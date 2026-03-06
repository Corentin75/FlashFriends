using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public enum GameState
{
    StartMenu,
    Playing,
    Paused,
    Album,
    Quest,
    End
}


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState currentState;

    [Header("References")]
    public UIManager ui;
    public PlayerController player;

    [Header("Input")]
    public InputActionReference pauseAction;

    public EndGameUI endGameUI;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Time.timeScale = 0f;

        currentState = GameState.StartMenu;
        player.SetGameplayActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        ui.ShowStartMenu(true);
        ui.ShowHUD(false);
        ui.ShowPauseMenu(false);
        ui.ShowEndMenu(false);
    }

    private void OnEnable()
    {
        pauseAction.action.Enable();
        pauseAction.action.performed += OnPausePressed;
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= OnPausePressed;
        pauseAction.action.Disable();
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (currentState == GameState.Playing)
            PauseGame();
        else if (currentState == GameState.Paused)
            ResumeGame();
    }

    // -------- GAME FLOW --------

    public void StartGame()
    {
        PhotoManager.Instance.DeleteAllPhotos();

        currentState = GameState.Playing;
        player.SetGameplayActive(true);

        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        ui.ShowStartMenu(false);
        ui.ShowHUD(true);
    }

    public void PauseGame()
    {
        currentState = GameState.Paused;
        player.SetGameplayActive(false);

        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        ui.ShowPauseMenu(true);
    }

    public void ResumeGame()
    {
        currentState = GameState.Playing;
        player.SetGameplayActive(true);

        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        ui.ShowPauseMenu(false);
    }

    public void EndGame()
    {
        currentState = GameState.End;

        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        ui.ShowHUD(false);
        ui.ShowEndMenu(true);

        endGameUI.UpdateStats();
    }

    public void OpenAlbum()
    {
        currentState = GameState.Album;

        player.SetGameplayActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0f;

        ui.ShowHUD(false);

        AlbumUIController.Instance.OpenAlbum();
    }

    public void CloseAlbum()
    {
        currentState = GameState.Playing;

        player.SetGameplayActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1f;

        ui.ShowHUD(true);

        AlbumUIController.Instance.CloseAlbum();
    }

    public void OpenQuestPanel()
    {
        currentState = GameState.Quest;

        player.SetGameplayActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0f;
        ui.ShowHUD(false);

        QuestUIController.Instance.OpenQuestPanel();
    }

    public void CloseQuestPanel()
    {
        currentState = GameState.Playing;

        player.SetGameplayActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1f;
        ui.ShowHUD(true);

        QuestUIController.Instance.CloseQuestPanel();
    }

    // -------- BUTTON ACTIONS --------

    public void RestartGame()
    {
        PhotoManager.Instance.DeleteAllPhotos();

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
