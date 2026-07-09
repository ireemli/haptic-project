using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    private string loginURL      = "http://localhost/haptic/login.php";
    private string saveResultURL = "http://localhost/haptic/save_result.php";

    public int CurrentUserId { get; private set; } = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Login(string studentNo, System.Action<bool, string> callback)
    {
        StartCoroutine(LoginRoutine(studentNo, callback));
    }

    private IEnumerator LoginRoutine(string studentNo, System.Action<bool, string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("student_no", studentNo);

        using (UnityWebRequest request = UnityWebRequest.Post(loginURL, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
                if (response.success)
                {
                    CurrentUserId = response.user_id;
                    callback(true, response.message);
                }
                else
                {
                    callback(false, response.message);
                }
            }
            else
            {
                callback(false, "Server connection failed.");
            }
        }
    }

    public void SaveResult(int levelId, float completionTime, int wrongBreaks, System.Action<bool, int> callback)
    {
        StartCoroutine(SaveResultRoutine(levelId, completionTime, wrongBreaks, callback));
    }

    private IEnumerator SaveResultRoutine(int levelId, float completionTime, int wrongBreaks, System.Action<bool, int> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id",         CurrentUserId.ToString());
        form.AddField("level_id",        levelId.ToString());
        form.AddField("completion_time", completionTime.ToString("F2"));
        form.AddField("wrong_breaks",    wrongBreaks.ToString());

        using (UnityWebRequest request = UnityWebRequest.Post(saveResultURL, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                SaveResultResponse response = JsonUtility.FromJson<SaveResultResponse>(request.downloadHandler.text);
                callback(response.success, response.score);
            }
            else
            {
                callback(false, 0);
            }
        }
    }

    [System.Serializable]
    private class LoginResponse
    {
        public bool   success;
        public int    user_id;
        public string message;
    }

    [System.Serializable]
    private class SaveResultResponse
    {
        public bool   success;
        public int    score;
        public string message;
    }
}