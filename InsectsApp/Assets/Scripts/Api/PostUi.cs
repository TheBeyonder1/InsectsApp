using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.Events;

public class PostUI : MonoBehaviour
{
    public Image postImage;
    public TMP_Text descriptionText;
    public TMP_Text ubicacion;
    public TMP_Text usernameText;     // 👤 Nuevo: Username
    public TMP_Text dateText, likesText, commentsCountText, commentsText;         // 🕓 Nuevo: Fecha
    public Button linkmapa;

    private string imageUrl;
    private float? latitude;
    private float? longitude;

    public Button comentarButton;
    public Button likeButton;
    public Button verComentariosButton;

    public TMP_InputField comentarioInputField; // Campo de texto para el comentario



    // Opcional: Para alternar like/unlike
    private bool yaLeDioLike = false;
    public ApiClient apiClient; // ← te permitirá arrastrarlo en el editor


    // Info del post
    public int postId;
    public int userId;
    public GameObject PanelCommentar; // Panel de comentarios
    private void Start()
    {
        apiClient = FindObjectOfType<ApiClient>();
        verComentariosButton.onClick.AddListener(() => VerComentarios());
        comentarButton.onClick.AddListener(() => Comentar(comentarioInputField.text)); // Enviar el comentario

        likeButton.onClick.AddListener(() => ToggleLike());

    }



    public void SetPostData(Post post)
    {
        descriptionText.text = post.description;
        imageUrl = post.image_url;
        latitude = post.latitude;
        longitude = post.longitude;
        likesText.text = post.total_likes.ToString(); // Mostrar likes
        userId = post.user_id; // Guardar el userId del post
        postId = post.id; // Guardar el postId del post 
        commentsCountText.text = post.total_comments.ToString(); // Mostrar total de comentarios

        Debug.Log(post.latitude);
        Debug.Log(post.longitude);


        // 👤 Mostrar nombre de usuario
        usernameText.text = $"Publicado por: {post.username}";

        // 🕓 Mostrar fecha
        if (!string.IsNullOrEmpty(post.created_at))
        {
            System.DateTime parsedDate;
            if (System.DateTime.TryParse(post.created_at, out parsedDate))
            {
                dateText.text = parsedDate.ToString("dd MMM yyyy, HH:mm"); // Ej: 04 Abr 2025, 16:30
            }
            else
            {
                dateText.text = post.created_at;
            }
        }

        // 🌍 Mostrar ubicación
        if (post.latitude != 0f || post.longitude != 0f)
        {
            ubicacion.text = $"Ubicación: Lat {post.latitude:F4}, Lon {post.longitude:F4}";

            string mapsUrl = $"https://www.google.com/maps?q={post.latitude},{post.longitude}";
            linkmapa.onClick.RemoveAllListeners();
            linkmapa.onClick.AddListener(() =>
            {
                Application.OpenURL(mapsUrl);
            });
        }
        else
        {
            ubicacion.text = "Ubicación no disponible";
            linkmapa.interactable = false;
        }


        StartCoroutine(LoadImage(imageUrl));
        yaLeDioLike = post.likes == 1;

        likeButton.GetComponentInChildren<TMP_Text>().text = yaLeDioLike ? "Ya no me Gusta" : "Me gusta";


    }

    private IEnumerator LoadImage(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogWarning("URL de imagen vacía");
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                postImage.sprite = SpriteFromTexture(texture);
            }
            else
            {
                Debug.LogError("Error al cargar imagen: " + request.error);
            }
        }
    }

    private Sprite SpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public void Comentar(string comentario)
    {
        comentario = comentarioInputField.text; // Obtener el texto del campo de entrada
        int userIdNuevo = PlayerPrefs.GetInt("user_id", 0); // Obtener el userId del jugador
        Debug.Log("Comentario: " + comentario);
        Debug.Log("userIdNuevo: comentario " + userIdNuevo);
        Debug.Log("postId comentario: " + postId); // Asegúrate de que postId esté definido correctamente
        StartCoroutine(ApiClient.Instance.AgregarComentario( userIdNuevo,postId, comentario, (respuesta) =>
        {
            Debug.Log("Comentario enviado: " + respuesta);
            commentsCountText.text = (int.Parse(commentsCountText.text) + 1).ToString(); // Incrementar el contador de comentarios
            PanelCommentar.SetActive(false); // Cerrar el panel de comentarios
        }));
    }


    public void VerComentarios()
    {
        StartCoroutine(ApiClient.Instance.ObtenerComentarios(postId, (comentarios) =>
        {
            Debug.Log("Comentarios obtenidos: " + comentarios);
            Debug.Log("Comentarios obtenidos postid: " + postId);
            if (comentarios != null)
            {
                commentsText.text = ""; // Limpiar antes de mostrar nuevos comentarios
                foreach (var c in comentarios)
                {
                    commentsText.text += $"{c.username}: {c.text}\n";
                    Debug.Log($"[{c.username}] {c.text}");
                }
            }
            else
            {
                Debug.Log("No se pudieron cargar comentarios.");
            }
        }));
    }


    public void ToggleLike()
    {
        int userIdNuevo = PlayerPrefs.GetInt("user_id", 0); // Obtener el userId del jugador
        if (yaLeDioLike)
        {

            StartCoroutine(ApiClient.Instance.QuitarLike(userIdNuevo, postId, (respuesta) =>
            {
                Debug.Log("Like quitado: " + respuesta);
                yaLeDioLike = false;
                likesText.text = (int.Parse(likesText.text) - 1).ToString(); // Decrementar el contador de likes
                likeButton.GetComponentInChildren<TMP_Text>().text = "Me gusta"; // Cambiar texto a "Me gusta"
            }));
        }
        else
        {
            StartCoroutine(ApiClient.Instance.DarLike(userIdNuevo, postId, (respuesta) =>
            {
                Debug.Log("Like dado: " + respuesta);
                yaLeDioLike = true;
                likesText.text = (int.Parse(likesText.text) + 1).ToString(); // Incrementar el contador de likes
                likeButton.GetComponentInChildren<TMP_Text>().text = "Ya no me Gusta"; // Cambiar texto a "Ya no me gusta"

            }));
        }
    }

    public void EliminarComentario(int commentId)
    {
        StartCoroutine(ApiClient.Instance.EliminarComentario(commentId, (respuesta) =>
        {
            Debug.Log("Comentario eliminado: " + respuesta);
            VerComentarios(); // refrescar
        }));
    }



}
