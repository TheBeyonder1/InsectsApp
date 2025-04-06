using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class RevenuePredictor : MonoBehaviour
{
    // Entradas
    public TMP_InputField customersInput;
    public TMP_InputField marketingInput;
    public TMP_InputField orderValueInput;

    // Textos de salida
    public TMP_Text resultText;
    public TMP_Text summaryText;

    // Paneles
    public GameObject inputPanel;
    public GameObject resultPanel;

    public string apiUrl = "https://tu-api.com/predict"; // 

    public void OnSubmit()
    {
        int customers = int.Parse(customersInput.text);
        float marketing = float.Parse(marketingInput.text);
        float orderValue = float.Parse(orderValueInput.text);

        StartCoroutine(SendPredictionRequest(customers, marketing, orderValue));
    }

    IEnumerator SendPredictionRequest(int customers, float marketing, float orderValue)
    {
        Debug.Log("Predixt Rerquest");
        PredictionData data = new PredictionData
        {
            Number_of_Customers_Per_Day = customers,            
            Average_Order_Value = orderValue,
            Marketing_Spend_Per_Day = marketing 
        };

        string jsonData = JsonUtility.ToJson(data);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        Debug.Log("sent");

        if (request.result == UnityWebRequest.Result.Success)
        {
            PredictionResult prediction = JsonUtility.FromJson<PredictionResult>(request.downloadHandler.text);
            Debug.Log("prediccion");
            Debug.Log(prediction);
            resultText.text = "$" + prediction.predicted_revenue.ToString("N2");
            summaryText.text = $"Con {customers} clientes, ${marketing} de inversión en marketing y ${orderValue} promedio por orden, la predicción es:";
        
            inputPanel.SetActive(false);
            resultPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Error");
            resultText.text = "Error: " + request.error;
            summaryText.text = "No se pudo obtener la predicción.";
        }
    }

    [System.Serializable]
    public class PredictionData
    {
        public int Number_of_Customers_Per_Day;        
        public float Average_Order_Value;
        public float Marketing_Spend_Per_Day;
    }

    [System.Serializable]
    public class PredictionResult
    {
        public float predicted_revenue;
    }
}
