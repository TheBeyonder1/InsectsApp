using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public TMP_Text  messageText;

    private AuthManager authManager;

    void Start()
    {
        authManager = gameObject.AddComponent<AuthManager>();
        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    private void OnLoginButtonClicked()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        authManager.Login(email, password, 
            success => messageText.text = success,
            error => messageText.text = error);
        
    }
}
