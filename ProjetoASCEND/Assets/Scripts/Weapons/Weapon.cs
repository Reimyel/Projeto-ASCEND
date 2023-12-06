using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Hitscan ou Projétil?")]
    public bool isHitscan;
    public bool isProjectile;

    [Header("Dano da Arma Hitscan")]
    public int damage;

    [Header("Projétil da Arma Projétil")]
    public GameObject bullet;
    public float shootForce;
    public float upwardForce;

    [Header("Estatísticas Gerais")]
    public float timeBetweenShooting;
    public float horizontalSpread;
    public float verticalSpread;
    public float range;
    public float reloadTime;
    public float timeBetweenShots;
    public int magazineSize;
    public int bulletsPerTap;
    public bool allowButtonHold;
    public float rotationSpeed;
    public float recoilForce;
    int bulletsLeft;
    int bulletsShot;
    bool shooting;
    bool readyToShoot;
    bool reloading;

    [Header("Gráficos")]
    public GameObject gunSprite;
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammoDisplay;
    public GameObject impactEffect;
    //public CameraShake camShake;
    //public float camShakeDuration;
    //public float camShakeMagnitude;

    [Header("Referências")]
    public Camera playerCamera;
    public Transform attackPoint;
    public LayerMask enemies;
    public Rigidbody playerRb;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        ShootingInput();

        if (ammoDisplay != null)
        {
            ammoDisplay.SetText(bulletsLeft + " / " + magazineSize);
        }

        if (reloading)
        {
            //gunSprite.transform.localRotation = Quaternion.Euler(60f, 0f, 0f);
            StartCoroutine(RotateWhileReloading());
            //Adicionar animação de recarregar quando tiver uma
        }
        else
        {
            gunSprite.transform.localRotation = Quaternion.identity;
        }
    }

    private void ShootingInput()
    {
        if (allowButtonHold)
        {
            shooting = Input.GetMouseButton(0);
        }
        else
        {
            shooting = Input.GetMouseButtonDown(0);
        }

        //RECARREGAR
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0)
        {
            Reload();
        }

        //ATIRAR
        if (isHitscan && readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            ShootHitscan();
        }
        if (isProjectile && readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            ShootProjectile();
        }
    }

    private void ShootHitscan()
    {
        readyToShoot = false;

        //PROPAGAÇÃO DE BALA
        float x = Random.Range(-horizontalSpread, horizontalSpread);
        float y = Random.Range(-horizontalSpread, verticalSpread);
        Vector3 direction = playerCamera.transform.forward + new Vector3(x, y, 0);

        //RAYCAST
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, direction, out hit, range))
        {
            Debug.Log(hit.transform.name);

            EnemyStats enemyStats = hit.transform.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(damage);
            }

            GameObject impactPrefab = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactPrefab, 3f);
        }

        //BALANÇAR CAMERA
        //StartCoroutine(camShake.Shake(camShakeDuration, camShakeMagnitude));

        //MUZZLE EFFECT
        GameObject muzzleFlashInstance = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        Destroy(muzzleFlashInstance, 0.1f);

        bulletsLeft--;
        bulletsShot--;

        //RECÚO
        playerRb.AddForce(-direction * recoilForce, ForceMode.Impulse);

        Invoke("ResetShot", timeBetweenShooting);
        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            Invoke("ShootHitscan", timeBetweenShots);
        }
    }

    private void ShootProjectile()
    {
        readyToShoot = false;

        //RAYCAST
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75);
        }

        //PROPAGAÇÃO DE BALA
        float x = Random.Range(-horizontalSpread, horizontalSpread);
        float y = Random.Range(-horizontalSpread, verticalSpread);
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        //PROJÉTIL
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(playerCamera.transform.up * upwardForce, ForceMode.Impulse);

        //BALANÇAR CAMERA
        //StartCoroutine(camShake.Shake(camShakeDuration, camShakeMagnitude));

        //MUZZLE FLASH
        GameObject muzzleFlashInstance = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        Destroy(muzzleFlashInstance, 0.1f);

        bulletsLeft--;
        bulletsShot--;

        //RECÚO
        playerRb.AddForce(-directionWithSpread * recoilForce, ForceMode.Impulse);

        Invoke("ResetShot", timeBetweenShooting);
        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            Invoke("ShootProjectile", timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        Debug.Log("RECARREGANDO!");
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }

    IEnumerator RotateWhileReloading()
    {
        while (reloading)
        {
            // Adicione a lógica de rotação aqui
            gunSprite.transform.Rotate(0, 0, Time.deltaTime * rotationSpeed);

            yield return null;
        }
    }
}
