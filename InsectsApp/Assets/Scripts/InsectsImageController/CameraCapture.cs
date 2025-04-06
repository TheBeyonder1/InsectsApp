using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraCapture : MonoBehaviour
{
    public RawImage preview;
    private WebCamTexture camTexture;

    public void StartCamera()
    {
        camTexture = new WebCamTexture();
        preview.texture = camTexture;
        preview.material.mainTexture = camTexture;
        camTexture.Play();
    }

    public Texture2D CaptureImage()
    {
        Texture2D photo = new Texture2D(camTexture.width, camTexture.height);
        photo.SetPixels(camTexture.GetPixels());
        photo.Apply();
        return photo;
    }

    public void StopCamera()
    {
        if (camTexture != null) camTexture.Stop();
    }
}
