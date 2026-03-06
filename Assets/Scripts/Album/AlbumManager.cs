using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;


// Handles the photo album: loads PNGs from disk, displays them in pages, handles navigation...
public class AlbumManager : MonoBehaviour
{
    [Header("Left Page Slots")]
    public RawImage pageLeftSlot1;
    public RawImage pageLeftSlot2;

    [Header("Right Page Slots")]
    public RawImage pageRightSlot1;
    public RawImage pageRightSlot2;

    [Header("Navigation Buttons")]
    public Button nextButton;
    public Button prevButton;

    private List<string> photoPaths = new List<string>();
    private int currentPage = 0;
    private const int photosPerSpread = 4;

    private string folderPath;

    private void Start()
    {
        folderPath = Path.Combine(Application.persistentDataPath, "Photos");
        LoadAllPhotoPaths();
        DisplayCurrentPage();
    }

    // Reloads all PNG paths from the Photos folder
    public void RefreshAlbum()
    {
        LoadAllPhotoPaths();
        currentPage = 0;
        DisplayCurrentPage();
    }

    private void LoadAllPhotoPaths()
    {
        if (!Directory.Exists(folderPath))
            return;

        photoPaths = new List<string>(Directory.GetFiles(folderPath, "*.png"));
        photoPaths.Sort(); // sort by filename (= timestamp)
    }

    public void NextPage()
    {
        if ((currentPage + 1) * photosPerSpread < photoPaths.Count)
        {
            currentPage++;
            DisplayCurrentPage();
        }
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            DisplayCurrentPage();
        }
    }

    private void DisplayCurrentPage()
    {
        int startIndex = currentPage * photosPerSpread;

        PhotoSlot[] slots = new PhotoSlot[]
        {
            pageLeftSlot1.GetComponent<PhotoSlot>(),
            pageLeftSlot2.GetComponent<PhotoSlot>(),
            pageRightSlot1.GetComponent<PhotoSlot>(),
            pageRightSlot2.GetComponent<PhotoSlot>()
        };

        for (int i = 0; i < slots.Length; i++)
        {
            int photoIndex = startIndex + i;
            if (photoIndex < photoPaths.Count)
            {
                Texture2D tex = LoadTexture(photoPaths[photoIndex]);

                slots[i].Setup(tex, (clickedTex) =>
                {
                    AlbumUIController.Instance.ShowPhotoFullScreen(clickedTex, photoIndex);
                });

                slots[i].gameObject.SetActive(true);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }

        UpdateButtons();
    }

    // Loads a PNG from the disk into a Texture2D
    private Texture2D LoadTexture(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);
        return tex;
    }

    private void UpdateButtons()
    {
        prevButton.interactable = currentPage > 0;
        nextButton.interactable = (currentPage + 1) * photosPerSpread < photoPaths.Count;
    }
}
