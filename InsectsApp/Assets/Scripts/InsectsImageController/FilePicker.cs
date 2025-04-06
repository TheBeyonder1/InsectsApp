using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using UnityEngine.Networking;

public class FilePicker : MonoBehaviour
{
    public Text fileNameText;
    public Button pickFileButton;

    private string selectedFilePath;

    public void PickFile()
    {
        // Esta función puede no existir en tu versión, puedes quitarla si da error
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
        {
            Debug.LogError("File Picker no compatible en esta plataforma.");
            return;
        }

        NativeFilePicker.PickFile((path) =>
        {
            if (path != null)
            {
                selectedFilePath = path;
                fileNameText.text = Path.GetFileName(path);
                Debug.Log("Archivo seleccionado: " + selectedFilePath);

                StartCoroutine(UploadFileToS3(selectedFilePath));
            }
            else
            {
                Debug.Log("Selección cancelada.");
            }
        });
    }

    IEnumerator UploadFileToS3(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, fileName);

        using (UnityWebRequest request = UnityWebRequest.Post("http://TU_API/api/upload", form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Archivo subido: " + request.downloadHandler.text);
                // Aquí puedes guardar la URL que te devuelva tu API para usarla en el post
            }
            else
            {
                Debug.LogError("Error al subir archivo: " + request.error);
            }
        }
    }

    public string GetSelectedFilePath()
    {
        return selectedFilePath;
    }
}
