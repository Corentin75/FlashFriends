using UnityEngine;
using System.IO;
using System;

public class PhotoManager : MonoBehaviour
{
    public static PhotoManager Instance;

    [Header("Photo Settings")]
    public Camera photoCamera;
    public RenderTexture photoRenderTexture;

    private string photoFolderPath;

    public int totalPhotosTaken = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        photoFolderPath = Path.Combine(Application.persistentDataPath, "Photos");

        if (!Directory.Exists(photoFolderPath))
            Directory.CreateDirectory(photoFolderPath);
    }

    public void CapturePhoto(int score = 0)
    {
        totalPhotosTaken++;

        // Render the camera in the RenderTexture
        photoCamera.targetTexture = photoRenderTexture;
        photoCamera.Render();

        // Copy RenderTexture to Texture2D
        RenderTexture.active = photoRenderTexture;

        Texture2D photo = new Texture2D(
            photoRenderTexture.width,
            photoRenderTexture.height,
            TextureFormat.RGB24,
            false
        );

        photo.ReadPixels(
            new Rect(0, 0, photoRenderTexture.width, photoRenderTexture.height),
            0,
            0
        );

        photo.Apply();

        RenderTexture.active = null;
        photoCamera.targetTexture = null;

        // Encode to PNG
        byte[] pngData = photo.EncodeToPNG();

        // Unique name with date
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = "Photo_" + timestamp + ".png";
        string filePath = Path.Combine(photoFolderPath, fileName);

        File.WriteAllBytes(filePath, pngData);

        // Save metadata to JSON
        SaveMetadata(fileName, score);

        Debug.Log("Photo saved: " + filePath);

        Destroy(photo);
    }

    private void SaveMetadata(string fileName, int score)
    {
        PhotoMetadata metadata = new PhotoMetadata
        {
            fileName = fileName,
            score = score,
            date = DateTime.Now.ToString()
        };

        string json = JsonUtility.ToJson(metadata, true);

        string jsonPath = Path.Combine(photoFolderPath, fileName.Replace(".png", ".json"));

        File.WriteAllText(jsonPath, json);
    }

    public void DeleteAllPhotos()
    {
        if (Directory.Exists(photoFolderPath))
        {
            Directory.Delete(photoFolderPath, true);
        }

        Directory.CreateDirectory(photoFolderPath);

        Debug.Log("All photos deleted.");
    }
}

[Serializable]
public class PhotoMetadata
{
    public string fileName;
    public int score;
    public string date;
}
