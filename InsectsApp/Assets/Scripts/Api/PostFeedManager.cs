using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class PostFeedManager : MonoBehaviour
{
    public ApiClient apiClient; // Referencia al script APIClient
    public GameObject postPrefab; // Prefab del post
    public Transform postContainer; // Contenedor donde se agregarán los posts
    public ScrollRect scrollRect; // ScrollRect para el desplazamiento

    void Start()
    {

        apiClient = FindObjectOfType<ApiClient>();
        Debug.Log("inicio post feed manager");
        StartCoroutine(apiClient.ObtenerTodosLosPosts(OnPostsReceived));


    }

    void OnPostsReceived(List<Post> posts)
    {
        if (posts == null)
        {
            Debug.LogError("No se pudieron obtener los posts.");
            return;
        }

        // Limpiar el feed antes de agregar nuevos posts
        foreach (Transform child in postContainer)
        {
            Destroy(child.gameObject);
        }

        // Crear posts dinámicamente
        foreach (Post post in posts)
        {
            GameObject newPost = Instantiate(postPrefab, postContainer);
            newPost.transform.localScale = Vector3.one;

            // Configurar el post con la información de la API
            PostUI postUI = newPost.GetComponent<PostUI>();
            if (postUI != null)
            {
                postUI.SetPostData(post);
            }
        }
        scrollRect.verticalNormalizedPosition = 0f; // Iniciar en la parte superior
    }
}
