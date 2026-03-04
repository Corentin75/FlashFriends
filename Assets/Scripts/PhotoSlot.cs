using UnityEngine;
using UnityEngine.UI;

public class PhotoSlot : MonoBehaviour
{
    public RawImage rawImage;
    public Button button;

    private Texture2D photoTexture;

    public void Setup(Texture2D tex, System.Action<Texture2D> onClick)
    {
        photoTexture = tex;
        rawImage.texture = tex;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick(photoTexture));
    }
}
