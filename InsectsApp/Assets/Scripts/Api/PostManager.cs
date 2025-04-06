using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;


public class PostManager : MonoBehaviour
{
    public TMP_InputField descriptionInputField; // Campo de descripción
    public string urlInputField; // URL de la imagen
    public ApiClient ApiClient; // Referencia a ApiClient

    public Transform postContainer; // Contenedor en la UI donde se mostrarán los posts
    public GameObject postPrefab; // Prefab del post (con imagen y descripción)


    public void Start()
    {
       ApiClient=  FindObjectOfType<ApiClient>();
       
     
    }
    IEnumerator CargarImagen(string url, RawImage image)
    {
        using (UnityEngine.Networking.UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                image.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            }
            else
            {
                Debug.LogError("Error al cargar imagen: " + request.error);
            }
        }
    }
    public void OnCreatePostButtonClicked()
    {

        int userId = PlayerPrefs.GetInt("user_id", 0); // Obtiene el ID del usuario guardado       
        string description = descriptionInputField.text;

        if (string.IsNullOrEmpty(urlInputField) || string.IsNullOrEmpty(description))
        {
            Debug.LogWarning("Faltan datos para crear el post.");
            return;
        }

        StartCoroutine(ApiClient.CrearPost(userId, urlInputField, description, OnPostCreated));
    }

    void OnPostCreated(string response)
    {
        Debug.Log("Post creado con éxito: " + response);
    }
}
