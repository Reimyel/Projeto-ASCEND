using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Estat�sticas dos Inimigos")]
    public int health = 10;
    public float speed = 12f;
    public int shield = 5;
    public float armor = 0f;
    //Determinamos o que atinge o inimigo com:
    //Esse c�digo para afetar o dano
    //E a camada "Enemy" pra identificar o que � inmigo

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
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
