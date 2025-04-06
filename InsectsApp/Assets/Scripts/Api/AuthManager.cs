using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using UnityEngine.SceneManagement;


public class AuthManager : MonoBehaviour
{
    private string apiBaseUrl = "http://3.145.57.195/api/usuarios"; // Cambia TU_IP_PUBLICA por la IP de tu servidor

    // Método para Login
    public void Login(string email, string password, Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(LoginCoroutine(email, password, onSuccess, onError));
    }

    // Método para Registro
    public void Register(string nombre, string email, string password, Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(RegisterCoroutine(nombre, email, password, onSuccess, onError));
    }

    // Coroutine para hacer Login
    private IEnumerator LoginCoroutine(string email, string password, Action<string> onSuccess, Action<string> onError)
    {
        LoginData data = new LoginData { email = email, password = password };
        string jsonData = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(apiBaseUrl + "/login", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(response);

                // Guardar token en PlayerPrefs
                PlayerPrefs.SetString("auth_token", authResponse.token);
                PlayerPrefs.SetInt("user_id", authResponse.user_id);
                PlayerPrefs.SetString("username", authResponse.nombre);
                PlayerPrefs.Save();

                onSuccess("Login exitoso");
                Debug.Log("Token: " + authResponse.token);
                Debug.Log("User ID: " + authResponse.user_id);    
                SceneManager.LoadScene("Predict");
            }
            else
            {
                onError("Error: " + request.error);
            }
        }
    }

    // Coroutine para hacer Registro
    private IEnumerator RegisterCoroutine(string nombre, string email, string password, Action<string> onSuccess, Action<string> onError)
    {
        RegisterData data = new RegisterData { username = nombre, email = email, password = password };
        string jsonData = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(apiBaseUrl + "/register", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess("Registro exitoso");
            }
            else
            {
                onError("Error: " + request.error);
            }
        }
    }
}

// 📌 Clases auxiliares para manejar datos
[Serializable]
public class LoginData
{
    public string email;
    public string password;
}

[Serializable]
public class RegisterData
{
    public string username;
    public string email;
    public string password;
}

[Serializable]
public class AuthResponse
{
    public string token;
    public int user_id;
    public string nombre;
}
