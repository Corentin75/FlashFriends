using UnityEngine;
using UnityEngine.UI;


// Controls the Album UI: opens/closes the album panel, shows full-screen photos
// Singleton pattern for easy access from AlbumManager or GameManager for example
public class AlbumUIController : MonoBehaviour
{
    public static AlbumUIController Instance;

    [Header("UI Panels")]
    public GameObject albumPanel;
    public GameObject fullPhotoPanel;

    [Header("Full Photo Display")]
    public RawImage fullPhotoDisplay;

    [Header("References")]
    public AlbumManager albumManager;

    private bool isOpen = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Toggles the album panel on/off
    public void ToggleAlbum()
    {
        if (isOpen) CloseAlbum();
        else OpenAlbum();
    }

    // Opens the album panel and refresh content
    public void OpenAlbum()
    {
        albumPanel.SetActive(true);
        isOpen = true;
        albumManager.RefreshAlbum();
    }

    // Closes the album panel
    public void CloseAlbum()
    {
        albumPanel.SetActive(false);
        isOpen = false;
    }

    // Shows a photo in full-screen view
    public void ShowPhotoFullScreen(Texture2D tex, int photoIndex)
    {
        fullPhotoPanel.SetActive(true);
        fullPhotoDisplay.texture = tex;
    }

    // Close the full-screen photo view
    public void CloseFullPhoto()
    {
        fullPhotoPanel.SetActive(false);
    }
}
