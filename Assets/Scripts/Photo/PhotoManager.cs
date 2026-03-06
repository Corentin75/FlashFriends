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
        // Singleton pattern
        if (Instance == null) Instance = this;

        // Ensures the Photos folder exists
        photoFolderPath = Path.Combine(Application.persistentDataPath, "Photos");
        if (!Directory.Exists(photoFolderPath))
            Directory.CreateDirectory(photoFolderPath);
    }

    // Captures a photo and saves it to disk
    public void CapturePhoto(int score = 0)
    {
        totalPhotosTaken++;

        // Renders the camera to the RenderTexture
        photoCamera.targetTexture = photoRenderTexture;
        photoCamera.Render();

        // Copies RenderTexture to Texture2D
        RenderTexture.active = photoRenderTexture;
        Texture2D photo = new Texture2D(photoRenderTexture.width, photoRenderTexture.height, TextureFormat.RGB24, false);
        photo.ReadPixels(new Rect(0, 0, photoRenderTexture.width, photoRenderTexture.height), 0, 0);
        photo.Apply();

        RenderTexture.active = null;
        photoCamera.targetTexture = null;

        // Encodes to PNG
        byte[] pngData = photo.EncodeToPNG();

        // Saves with timestamped filename
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = "Photo_" + timestamp + ".png";
        string filePath = Path.Combine(photoFolderPath, fileName);
        File.WriteAllBytes(filePath, pngData);

        // Saves metadata (score, date)
        SaveMetadata(fileName, score);

        Debug.Log("Photo saved: " + filePath);

        Destroy(photo);
    }

    // Saves photo metadata as JSON
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

    // Deletes all photos and recreates the folder
    public void DeleteAllPhotos()
    {
        if (Directory.Exists(photoFolderPath))
            Directory.Delete(photoFolderPath, true);

        Directory.CreateDirectory(photoFolderPath);

        Debug.Log("All photos deleted.");
    }
}

// Metadata structure for each photo
[Serializable]
public class PhotoMetadata
{
    public string fileName;
    public int score;
    public string date;
}
