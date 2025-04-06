using UnityEngine;

public class VideoLinks : MonoBehaviour
{
    public void AbrirVideo(string url)
    {
        Application.OpenURL(url); // Abre el navegador con el link
    }
}
