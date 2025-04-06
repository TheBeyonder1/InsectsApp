using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class UploadToS3 : MonoBehaviour
{
    public ImagePicker imagePicker; // Arrástralo desde el inspector
    public string urlImage; // Para mostrar la URL obtenida
    public PostManager postmanager;

    public void UploadImage()
    {
        Texture2D image = imagePicker.GetSelectedImage();
        if (image != null)
        {
            StartCoroutine(Upload(image));
        }
        else
        {
            Debug.LogWarning("No image selected.");
        }
    }

    IEnumerator Upload(Texture2D image)
    {
        Debug.Log("upload image");
        byte[] imageBytes = image.EncodeToJPG();
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageBytes, "imagen.jpg", "image/jpeg");

        using (UnityWebRequest www = UnityWebRequest.Post("http://3.145.57.195/api/upload", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Respuesta del servidor cargando img: " + www.downloadHandler.text);
                UploadResponse response = JsonUtility.FromJson<UploadResponse>(www.downloadHandler.text);

                // Asignar la URL al TMP_InputField
                urlImage = response.url;
                postmanager.urlInputField=response.url;
            }
            else
            {
                Debug.LogError("Error al subir la imagen: " + www.error);
            }
        }
    }
}

[System.Serializable]
public class UploadResponse
{
    public string message;
    public string url;
}
