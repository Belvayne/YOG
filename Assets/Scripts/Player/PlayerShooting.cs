using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f; // Time between shots
    public float bulletSpeed = 20f;
    public float bulletLifetime = 5f;
    public int maxAmmo = 30;
    public float reloadTime = 1f;
    
    [Header("Input Actions")]
    public InputAction shootAction;
    public InputAction reloadAction;
    public InputAction aimAction;
    
    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    [Range(0f, 0.3f)]
    public float pitchVariation = 0.1f; // Random pitch variation for shoot sound
    
    // Private variables
    private float lastFireTime;
    private int currentAmmo;
    private bool isReloading = false;
    private bool isAiming = false;
    private Vector3 aimDirection;
    private RaycastHit aimHit;

    public float maxAimDistance = Mathf.Infinity;
    public LayerMask layersToIgnore;
    [SerializeField] private Transform playerTransform;

    void Start()
    {
        // Initialize ammo
        currentAmmo = maxAmmo;
        
        // Enable input actions
        if (shootAction != null) shootAction.Enable();
        if (reloadAction != null) reloadAction.Enable();
        if (aimAction != null) aimAction.Enable();
        
        
        // Create fire point if not assigned
        if (firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = new Vector3(0, 1.5f, 1f);
            firePoint = firePointObj.transform;
        }
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        // Shooting
        if (shootAction != null && shootAction.IsPressed() && CanShoot())
        {
            Shoot();
        }
        
        // Reloading
        if (reloadAction != null && reloadAction.triggered && !isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }
        
        // Aiming
        if (aimAction != null)
        {
            isAiming = aimAction.IsPressed();
        }
    }
    
    bool CanShoot()
    {
        return !isReloading && 
               currentAmmo > 0 && 
               Time.time >= lastFireTime + fireRate;
    }

    void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("No bullet prefab assigned!");
            return;
        }

        Camera mainCam = Camera.main;
        Vector3 bulletDirection;

        if (mainCam != null)
        {
            aimDirection = mainCam.transform.forward;

            // Raycast from camera position in camera's forward direction, ignoring LayersToIgnore
            if (Physics.Raycast(mainCam.transform.position, aimDirection, out aimHit, maxAimDistance, ~layersToIgnore))
            {
                Debug.Log($"Raycast hit: {aimHit.collider.gameObject.name} at {aimHit.point}");
                // Set bullet direction towards hit point
                bulletDirection = (aimHit.point - firePoint.position).normalized;
            }
            else
            {
                Debug.Log("Raycast did not hit anything.");
                bulletDirection = aimDirection;
            }
        }
        else
        {
            bulletDirection = transform.forward;
        }

        // Spawn the bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(bulletDirection));

        // Add velocity
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = bulletDirection * bulletSpeed;
        }

        lastFireTime = Time.time;
        PlayShootEffects();

        Debug.Log($"Shot fired in direction {bulletDirection}");
    }

    void PlayShootEffects()
    {
        // Muzzle flash
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        
        // Sound effect with random pitch variation
        if (audioSource != null && shootSound != null)
        {
            // Randomize pitch (1.0 is normal pitch)
            audioSource.pitch = 1.0f + Random.Range(-pitchVariation, pitchVariation);
            audioSource.PlayOneShot(shootSound);
        }
    }
    
    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        
        // Play reload sound
        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
        
        // Wait for reload time
        yield return new WaitForSeconds(reloadTime);
        
        // Refill ammo
        currentAmmo = maxAmmo;
        isReloading = false;
        
        Debug.Log("Reload complete!");
    }
    
    // Public getters for UI
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;
    public bool IsReloading() => isReloading;
    public bool IsAiming() => isAiming;
    
    void OnDestroy()
    {
        // Disable input actions
        if (shootAction != null) shootAction.Disable();
        if (reloadAction != null) reloadAction.Disable();
        if (aimAction != null) aimAction.Disable();
    }
}
