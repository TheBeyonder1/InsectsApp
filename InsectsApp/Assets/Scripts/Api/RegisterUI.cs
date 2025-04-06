using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RegisterUI : MonoBehaviour
{
    public TMP_InputField nombreInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button registerButton;
    public TMP_Text messageText;

    private AuthManager authManager;

    void Start()
    {
        authManager = gameObject.AddComponent<AuthManager>();
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
    }

    private void OnRegisterButtonClicked()
    {
        string nombre = nombreInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;

        authManager.Register(nombre, email, password, 
            success => messageText.text = success,
            error => messageText.text = error);
    }
}
