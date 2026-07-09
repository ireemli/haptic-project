using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("VasePuzzleRoom");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
