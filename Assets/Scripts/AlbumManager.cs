using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class AlbumManager : MonoBehaviour
{
    [Header("Page Left")]
    public RawImage pageLeftSlot1;
    public RawImage pageLeftSlot2;

    [Header("Page Right")]
    public RawImage pageRightSlot1;
    public RawImage pageRightSlot2;

    [Header("Navigation Buttons")]
    public Button nextButton;
    public Button prevButton;

    private List<string> photoPaths = new List<string>();

    private int currentPage = 0;
    private int photosPerSpread = 4;

    private string folderPath;

    private void Start()
    {
        folderPath = Application.persistentDataPath + "/Photos";
        LoadAllPhotoPaths();
        DisplayCurrentPage();
    }

    public void RefreshAlbum()
    {
        LoadAllPhotoPaths();
        currentPage = 0;
        DisplayCurrentPage();
    }

    void LoadAllPhotoPaths()
    {
        if (!Directory.Exists(folderPath))
            return;

        string[] files = Directory.GetFiles(folderPath, "*.png");

        photoPaths = new List<string>(files);

        photoPaths.Sort(); // tri par nom (timestamp)
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

    void DisplayCurrentPage()
    {
        int startIndex = currentPage * photosPerSpread;

        PhotoSlot[] slots = new PhotoSlot[] {
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
                byte[] fileData = File.ReadAllBytes(photoPaths[photoIndex]);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);

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

    void UpdateButtons()
    {
        prevButton.interactable = currentPage > 0;
        nextButton.interactable =
            (currentPage + 1) * photosPerSpread < photoPaths.Count;
    }
}