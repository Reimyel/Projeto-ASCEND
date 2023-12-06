using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public EnemyStats enemyStats;

    public void TakeDamage(int amount)
    {
        enemyStats.health -= amount;
        if (enemyStats.health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log(gameObject + " morreu!");
        Destroy(gameObject);
    }
}
