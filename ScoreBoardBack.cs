using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreBoardBack : MonoBehaviour
{
    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}