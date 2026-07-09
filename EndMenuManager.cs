using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenuManager : MonoBehaviour
{
    public void RestartExperience()
    {
        SceneManager.LoadScene("VasePuzzleRoom");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OpenScoreboard()
    {
        SceneManager.LoadScene("ScoreBoard");
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}