using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class CustomProjectile : MonoBehaviour
{
    [Header("Estat�sticas")]
    [Range(0f, 1f)]
    public float bounciness;
    public bool useGravity;
    public int explosionDamage;
    public float explosionRange;
    [Space]
    public int maxCollisions;
    public float maxLifetime;
    public bool explodeOnTouch = true;
    int collisions;
    PhysicMaterial physicsMat;

    [Header("Refer�ncias")]
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask enemies;

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        //QUANDO EXPLODIR
        if (collisions > maxCollisions)
        {
            Explode();
        }

        //VIDA �TIL DO PROJ�TIL
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0)
        {
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisions++;

        if (collision.collider.CompareTag("Enemy") && explodeOnTouch)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Debug.Log("BOOM");
        if (explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);

            Collider[] enemiesToAffect = Physics.OverlapSphere(transform.position, explosionRange, enemies);
            for (int i = 0; i < enemiesToAffect.Length; i++)
            {
                enemiesToAffect[i].GetComponent<EnemyStats>().TakeDamage(explosionDamage); //Causa dano ao inimigo
            }

            //DELAY PARA SUMIR
            Invoke("Delay", 0.05f);
        }
    }

    private void Delay()
    {
        Destroy(gameObject);
    }

    private void Setup()
    {
        //Novo material de f�sica
        physicsMat = new PhysicMaterial();
        physicsMat.bounciness = bounciness;
        physicsMat.frictionCombine = PhysicMaterialCombine.Minimum; //Proj�til quique bem nas superf�cies
        physicsMat.bounceCombine = PhysicMaterialCombine.Maximum;

        //COLLIDER
        GetComponent<SphereCollider>().material = physicsMat;

        //GRAVIDADE DO PROJ�TIL
        rb.useGravity = useGravity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
