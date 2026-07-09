using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ScoreboardManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform       contentParent;
    [SerializeField] private GameObject      rowPrefab;
    [SerializeField] private TextMeshProUGUI statusText;

    private string scoreboardURL = "http://localhost/haptic/scoreboard.php";

    private void Start()
    {
        StartCoroutine(FetchScoreboard());
    }

    private IEnumerator FetchScoreboard()
    {
        if (statusText != null) statusText.text = "Loading...";

        using (UnityWebRequest request = UnityWebRequest.Get(scoreboardURL))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ScoreboardResponse response =
                    JsonUtility.FromJson<ScoreboardResponse>(request.downloadHandler.text);

                if (response.success)
                {
                    PopulateTable(response.players);
                    if (statusText != null) statusText.text = "";
                }
                else
                {
                    if (statusText != null) statusText.text = "Failed to load scoreboard.";
                }
            }
            else
            {
                if (statusText != null) statusText.text = "Server connection failed.";
            }
        }
    }

    private void PopulateTable(List<PlayerEntry> players)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        int rank = 1;
        foreach (PlayerEntry player in players)
        {
            GameObject row = Instantiate(rowPrefab, contentParent);
            TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();

            // texts[0]=rank, texts[1]=player, texts[2]=vase, texts[3]=combat,
            // texts[4]=frozen, texts[5]=total
            if (texts.Length >= 6)
            {
                texts[0].text = rank.ToString();
                texts[1].text = "****" + player.last4digits;

                // fill per-level cells
                var lvlMap = new Dictionary<int, LevelEntry>();
                foreach (var l in player.levels)
                    lvlMap[l.order_no] = l;

                for (int col = 0; col < 3; col++)
                {
                    int orderNo = col + 1;
                    if (lvlMap.ContainsKey(orderNo))
                        texts[2 + col].text = lvlMap[orderNo].best_score + "\n"
                                            + lvlMap[orderNo].best_time.ToString("F1") + "s";
                    else
                        texts[2 + col].text = "—";
                }

                texts[5].text = player.total_score.ToString();
            }

            rank++;
        }
    }

    // ── Data classes ──────────────────────────────────────────────────────

    [System.Serializable]
    private class ScoreboardResponse
    {
        public bool              success;
        public List<PlayerEntry> players;
    }

    [System.Serializable]
    private class PlayerEntry
    {
        public string           last4digits;
        public List<LevelEntry> levels;
        public int              total_score;
    }

    [System.Serializable]
    private class LevelEntry
    {
        public int    level_id;
        public string level_name;
        public int    order_no;
        public float  best_time;
        public int    best_score;
    }
}