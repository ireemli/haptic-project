using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathPanelManager : MonoBehaviour
{
    [Header("Button Texts")]
    [SerializeField] private TextMeshProUGUI restartButtonText;
    [SerializeField] private TextMeshProUGUI menuButtonText;

    private void OnEnable()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGloveMode)
        {
            if (restartButtonText != null)
                restartButtonText.text = "RESTART\n<size=70%>Flex index finger</size>";
            if (menuButtonText != null)
                menuButtonText.text = "MAIN MENU\n<size=70%>Flex ring finger</size>";
        }
        else
        {
            if (restartButtonText != null)
                restartButtonText.text = "RESTART";
            if (menuButtonText != null)
                menuButtonText.text = "MAIN MENU";
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}