using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class UploadManager : MonoBehaviour
{
    public ImagePicker imagePicker; // Arrástralo desde el inspector
    public TMP_Text texto;
    public GameObject predictPanel,predictedPanel;
    public TMP_InputField inputFieldPost; 

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
        byte[] imageBytes = image.EncodeToJPG();

        WWWForm form = new WWWForm();
       
        form.AddBinaryData("image", imageBytes, "imagen.jpg", "image/jpeg");

        using (UnityWebRequest www = UnityWebRequest.Post("http://3.145.57.195/api/predict", form))
        {
            // 🔹 Añadir token JWT en el header
            string token = PlayerPrefs.GetString("auth_token", ""); // Asegúrate de que el token se guardó al hacer login
            Debug.Log("Token JWT: " + token);

            if (!string.IsNullOrEmpty(token))
            {
                www.SetRequestHeader("Authorization", "Bearer " + token);
            }
            else
            {
                Debug.LogError("No JWT token found, authentication might fail.");
            }

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Respuesta del servidor: " + www.downloadHandler.text);
                PredictionResponse response = JsonUtility.FromJson<PredictionResponse>(www.downloadHandler.text);

               // Concatenar la información en un solo string
                string resultadoFormateado = $"<b>Nombre Científico:</b> {response.nombre_cientifico}\n" +
                                 $"<b>Nombre Común:</b> {response.nombre_comun}\n" +
                                 $"<b>Descripción:</b> {response.descripcion}";
                texto.text= resultadoFormateado;
                inputFieldPost.text = resultadoFormateado;
                predictPanel.SetActive(false);
                predictedPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("Error al subir la imagen: " + www.error + "\nResponse: " + www.downloadHandler.text);

                Debug.LogError("Error al subir la imagen: " + www.error);
            }
        }
    }
}

[System.Serializable]
public class PredictionResponse
{
    public string nombre_cientifico;
    public string nombre_comun;
    public string descripcion;
}
