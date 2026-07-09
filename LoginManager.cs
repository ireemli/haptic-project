using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField studentNoInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button startKeyboardButton;
    [SerializeField] private Button startGloveButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TextMeshProUGUI statusText;

    private void Start()
    {
        startKeyboardButton.interactable = false;
        startGloveButton.interactable    = false;
        statusText.text = "";
    }

    public void OnLoginButtonClicked()
    {
        string studentNo = studentNoInput.text.Trim();

        if (string.IsNullOrEmpty(studentNo))
        {
            statusText.text  = "Please enter your student number.";
            statusText.color = Color.red;
            return;
        }

        loginButton.interactable = false;
        statusText.text  = "Logging in...";
        statusText.color = Color.white;

        DatabaseManager.Instance.Login(studentNo, (success, message) =>
        {
            if (success)
            {
                statusText.text  = "Welcome! Choose your play mode.";
                statusText.color = new Color(0.79f, 0.66f, 0.30f, 1f); // #C9A84C
                startKeyboardButton.interactable = true;
                startGloveButton.interactable    = true;
            }
            else
            {
                statusText.text  = "Login failed: " + message;
                statusText.color = new Color(0.55f, 0.23f, 0.23f, 1f); // #8B3A3A
                loginButton.interactable = true;
            }
        });
    }

    public void OnStartKeyboardClicked()
    {
        GameManager.Instance.SetKeyboardMode();
        SceneManager.LoadScene("VasePuzzleRoom");
    }

    public void OnStartGloveClicked()
    {
        GameManager.Instance.SetGloveMode();
        SceneManager.LoadScene("VasePuzzleRoom");
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}