using UnityEngine;
using UnityEngine.UI;

public class AlbumUIController : MonoBehaviour
{
    public static AlbumUIController Instance;

    public GameObject albumPanel;
    public PlayerController playerController;

    public AlbumManager albumManager;

    public RawImage fullPhotoDisplay;
    public GameObject fullPhotoPanel;

    private bool isOpen = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void ToggleAlbum()
    {
        if (isOpen)
            CloseAlbum();
        else
            OpenAlbum();
    }

    public void OpenAlbum()
    {
        albumPanel.SetActive(true);
        isOpen = true;

        playerController.SetGameplayActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;

        albumManager.RefreshAlbum();
    }

    public void CloseAlbum()
    {
        albumPanel.SetActive(false);
        isOpen = false;

        playerController.SetGameplayActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
    }

    public void ShowPhotoFullScreen(Texture2D tex, int photoIndex)
    {
        fullPhotoPanel.SetActive(true);
        fullPhotoDisplay.texture = tex;
    }

    public void CloseFullPhoto()
    {
        fullPhotoPanel.SetActive(false);
    }
}
