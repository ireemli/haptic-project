using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public bool AreAllEnemiesDefeated()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        return enemies.Length == 0;
    }
}