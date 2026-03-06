using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    public GameObject gameHUD;

    [Header("Menus")]
    public GameObject startMenu;
    public GameObject pauseMenu;
    public GameObject endMenu;

    public void ShowStartMenu(bool show) => startMenu.SetActive(show);
    public void ShowPauseMenu(bool show) => pauseMenu.SetActive(show);
    public void ShowHUD(bool show) => gameHUD.SetActive(show);
    public void ShowEndMenu(bool show) => endMenu.SetActive(show);
}
