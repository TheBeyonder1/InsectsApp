using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class ImagePicker : MonoBehaviour
{
    public RawImage imageDisplay, imageDisplay2, imagePost;
    public GameObject imageDisplayGameObject;

    public Button pickImageButton;

    private Texture2D selectedImage;

    public string nombreEscena = "Feed"; 
    // Llamar desde un botón
    public void PickImage()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                StartCoroutine(LoadImage(path));
            }
        }, "Selecciona una imagen");

        Debug.Log("Permiso: " + permission);
    }

    // Llamar desde otro botón
    public void TakePhoto()
    {
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            if (path != null)
            {
                StartCoroutine(LoadImage(path));
            }
        }, maxSize: 1024);

        Debug.Log("Permiso cámara: " + permission);
    }

    IEnumerator LoadImage(string path)
    {
        WWW www = new WWW("file://" + path);
        yield return www;

        selectedImage = www.texture;
        imageDisplayGameObject.SetActive(true);
        pickImageButton.interactable = true; // Desactiva el botón después de seleccionar la imagen
        imageDisplay.texture = selectedImage;
        imageDisplay2.texture = selectedImage; // Asigna la imagen a la segunda RawImage
        imagePost.texture = selectedImage;
    }

    public Texture2D GetSelectedImage()
    {
        return selectedImage;
    }

    public void CambiarEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }


}
