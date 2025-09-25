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
    
    [Header("Aiming")]
    public Camera playerCamera;
    public float aimSensitivity = 1f;
    public float maxAimDistance = 100f;
    
    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    
    // Private variables
    private float lastFireTime;
    private int currentAmmo;
    private bool isReloading = false;
    private bool isAiming = false;
    private Vector3 aimDirection;
    private RaycastHit aimHit;
    
    void Start()
    {
        // Initialize ammo
        currentAmmo = maxAmmo;
        
        // Enable input actions
        if (shootAction != null) shootAction.Enable();
        if (reloadAction != null) reloadAction.Enable();
        if (aimAction != null) aimAction.Enable();
        
        // Get camera reference
        if (playerCamera == null)
            playerCamera = Camera.main;
        
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
        UpdateAiming();
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
    
    void UpdateAiming()
    {
        if (playerCamera != null)
        {
            // Create ray from camera center
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            
            // Cast ray to find aim point
            if (Physics.Raycast(ray, out aimHit, maxAimDistance))
            {
                aimDirection = (aimHit.point - firePoint.position).normalized;
            }
            else
            {
                // If no hit, aim in camera forward direction
                aimDirection = playerCamera.transform.forward;
            }
        }
        else
        {
            // Fallback to transform forward
            aimDirection = transform.forward;
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
        
        // Create bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(aimDirection));
        
        // Set bullet velocity
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = aimDirection * bulletSpeed;
        }
        
        // Set bullet lifetime
        Destroy(bullet, bulletLifetime);
        
        // Update ammo and fire time
        //currentAmmo--;
        lastFireTime = Time.time;
        
        // Play effects
        PlayShootEffects();
        
        Debug.Log($"Shot fired! Ammo remaining: {currentAmmo}");
    }
    
    void PlayShootEffects()
    {
        // Muzzle flash
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        
        // Sound effect
        if (audioSource != null && shootSound != null)
        {
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
