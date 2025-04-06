using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ApiClient : MonoBehaviour
{

    private string apiUrl = "http://3.145.57.195/api"; // Reemplaza con la IP de tu servidor
    public static ApiClient Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional, si quieres que persista
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void cambioEscena(string nombreEscena)
    {
        
        Debug.Log("Cambiando a la escena: " + nombreEscena);
    }
    // 🔹 Crear un Post
    public IEnumerator CrearPost(int userId, string imageUrl, string description, Action<string> callback)
    {
        float latitude = 0f;
        float longitude = 0f;

        // Verificar si el usuario tiene la ubicación activada
        if (Input.location.isEnabledByUser)
        {
            Input.location.Start();

            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            if (Input.location.status == LocationServiceStatus.Running)
            {
                latitude = Input.location.lastData.latitude;
                longitude = Input.location.lastData.longitude;
                Debug.Log($"Ubicación obtenida: lat={latitude}, lon={longitude}");
            }
            else
            {
                Debug.LogWarning("No se pudo obtener la ubicación, se usarán valores por defecto.");
            }

            Input.location.Stop();
        }
        else
        {
            Debug.LogWarning("Ubicación desactivada por el usuario.");
        }

        string url = apiUrl + "/posts";
        PostData data = new PostData
        {
            user_id = userId,
            image_url = imageUrl,
            description = description,
            latitude = latitude,
            longitude = longitude
        };

        string jsonData = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                callback(request.downloadHandler.text);
            else
                Debug.LogError("Error al crear post: " + request.error);
        }
    }



    // 🔹 Editar un Post
    public IEnumerator EditarPost(int postId, string newImageUrl, string newDescription, Action<string> callback)
    {
        string url = apiUrl + $"/posts/{postId}";
        PostData data = new PostData { image_url = newImageUrl, description = newDescription };
        string jsonData = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                callback(request.downloadHandler.text);
            else
                Debug.LogError("Error al editar post: " + request.error);
        }
    }

    // 🔹 Eliminar un Post
    public IEnumerator EliminarPost(int postId, Action<string> callback)
    {
        string url = apiUrl + $"/posts/{postId}";

        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                callback(request.downloadHandler.text);
            else
                Debug.LogError("Error al eliminar post: " + request.error);
        }
    }

    // 🔹 Agregar un Comentario
    public IEnumerator AgregarComentario(int userId, int postId, string text, Action<string> callback)
    {
        Debug.Log("userId: coment " + userId);
        Debug.Log("postId: coment" + postId); 
        

        string url = apiUrl + $"/posts/{postId}/comments";
        CommentData data = new CommentData { user_id = userId, text = text };
        string jsonData = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                callback(request.downloadHandler.text);
            else
                Debug.LogError("Error al agregar comentario: " + request.error);
        }
    }

    // 🔹 Dar Like a un Post
    public IEnumerator DarLike(int userId, int postId, Action<string> callback)
    {
        string url = apiUrl + $"/posts/{postId}/like";
        Debug.Log("url: " + url);
        Debug.Log("userId: like " + userId);
        Debug.Log("postId: like " + postId);
        LikeData data = new LikeData { user_id = userId };
        string jsonData = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                callback(request.downloadHandler.text);
            else
                Debug.LogError("Error al dar like: " + request.error);
        }
    }

    // 🔹 Quitar Like de un Post
    public IEnumerator QuitarLike(int userId, int postId, Action<string> callback)
    {
        Debug.Log("userId: unlike " + userId);
        Debug.Log("postId: unlike " + postId);
        Debug.Log("url: " + apiUrl + $"/posts/{postId}/like");
        string url = apiUrl + $"/posts/{postId}/like";
        LikeData data = new LikeData { user_id = userId };
        string jsonData = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(url, "DELETE"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                callback(request.downloadHandler.text);
            else
                Debug.LogError("Error al quitar like: " + request.error);
        }
    }

    // 🔹 Eliminar un Comentario
    public IEnumerator EliminarComentario(int commentId, Action<string> callback)
    {
        string url = apiUrl + $"/comments/{commentId}";

        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                callback(request.downloadHandler.text);
            else
                Debug.LogError("Error al eliminar comentario: " + request.error);
        }
    }

    public IEnumerator ObtenerTodosLosPosts(Action<List<Post>> callback)
    {
        string url = apiUrl + "/posts";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                List<Post> postList = JsonConvert.DeserializeObject<List<Post>>(json);
                callback(postList);
                Debug.Log("obtuvo todos los posts");
                Debug.Log(request.downloadHandler.text);
                Debug.Log(postList);

            }
            else
            {
                Debug.LogError("Error al obtener los posts: " + request.error);
                callback(null);
            }
        }
    }
    public IEnumerator ObtenerComentarios(int postId, Action<List<Comment>> callback)
    {
        string url = $"{apiUrl}/posts/{postId}/comments";
        Debug.Log("url: all comms" + url);
        Debug.Log("postId: all comms " + postId);
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                List<Comment> comentarios = JsonConvert.DeserializeObject<List<Comment>>(json);
                callback(comentarios);
            }
            else
            {
                Debug.LogError("Error al obtener comentarios: " + request.error);
                callback(null);
            }
        }
    }



}

[System.Serializable]
public class PostData
{
    public int user_id;
    public string image_url;
    public string description;
    public float? latitude; // <- nullable
    public float? longitude;
}


[Serializable]
public class CommentData
{
    public int user_id;
    public string text;
}

[System.Serializable]
public class Comment
{
    public int id;
    public int user_id;
    public string username;
    public string text;
    public string created_at;
}


[Serializable]
public class LikeData
{
    public int user_id;
}

[System.Serializable]
public class Post
{
    public int id;
    public int user_id;
    public string username;
    public string image_url;
    public string description;
    [JsonProperty("latitude")]
    public float? latitude;

    [JsonProperty("longitude")]
    public float? longitude;

    public int total_comments;
    public int total_likes;
    public string created_at;
    public int likes;
}


