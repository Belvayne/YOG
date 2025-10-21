using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float sprintMultiplier = 1.8f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private bool shouldFaceMoveDirection = false;

    private CharacterController controller;
    private Vector3 moveInput;
    private Vector3 velocity;
    private bool isSprinting = false;

    [Header("Weapon Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f; // Time between shots
    public float bulletSpeed = 20f;
    public float bulletLifetime = 5f;
    public int maxAmmo = 30;
    public float reloadTime = 1f;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    [Range(0f, 0.3f)]
    public float pitchVariation = 0.1f; // Random pitch variation for shoot sound

    // Weapon switch prefabs and point
    [Header("Weapon Switch")]
    [SerializeField] private Transform weaponPoint; // Parent transform where weapons are attached
    [SerializeField] private GameObject weaponPrefab1;
    [SerializeField] private GameObject weaponPrefab2;
    [SerializeField] private GameObject weaponPrefab3;

    // Private variables
    private float lastFireTime;
    private int currentAmmo;
    private bool isReloading = false;
    private bool isAiming = false;
    private Vector3 aimDirection;
    private RaycastHit aimHit;
    private bool isShooting = false;

    public float maxAimDistance = Mathf.Infinity;
    public LayerMask layersToIgnore;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private float rotateToCameraSpeed = 10f;
    private Quaternion? targetRotation = null;

    private Coroutine resetFaceMoveDirectionCoroutine;

    private GameObject currentWeapon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Initialize ammo
        currentAmmo = maxAmmo;

        // Create fire point if not assigned
        if (firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = new Vector3(0, 1.5f, 1f);
            firePoint = firePointObj.transform;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        Debug.Log($"Move Input: {moveInput}");
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log($"Jumping {context.performed} - Is Grounded: {controller.isGrounded}");
        if (context.performed && controller.isGrounded)
        {
            Debug.Log("Jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
            isSprinting = true;
        else if (context.canceled)
            isSprinting = false;
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started)
            isShooting = true;
        else if (context.canceled)
            isShooting = false;
    }

    private void Shoot()
    {
        if (!isReloading &&
            currentAmmo > 0 &&
            Time.time >= lastFireTime + fireRate)
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

                if (Physics.Raycast(mainCam.transform.position, aimDirection, out aimHit, maxAimDistance, ~layersToIgnore))
                {
                    bulletDirection = (aimHit.point - firePoint.position).normalized;
                }
                else
                {
                    bulletDirection = aimDirection;
                }
            }
            else
            {
                bulletDirection = transform.forward;
            }

            SetTargetRotationToCamera();

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(bulletDirection));
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = bulletDirection * bulletSpeed;
            }

            lastFireTime = Time.time;
            PlayShootEffects();

            Debug.Log($"Shot fired in direction {bulletDirection}");
            // Start/reset coroutine to set shouldFaceMoveDirection after 1 second
            if (resetFaceMoveDirectionCoroutine != null)
                StopCoroutine(resetFaceMoveDirectionCoroutine);
            resetFaceMoveDirectionCoroutine = StartCoroutine(ResetFaceMoveDirectionAfterDelay(1f));
        }
    }

    public void OnWeaponSwitch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            int weaponIndex = -1;

            // Try to read an integer value from the action value
            try
            {
                weaponIndex = context.ReadValue<int>();
            }
            catch
            {
                // Try reading as float and convert
                try
                {
                    float f = context.ReadValue<float>();
                    weaponIndex = Mathf.RoundToInt(f);
                }
                catch
                {
                    // Fallback: inspect control's display name or control name
                    if (context.control != null)
                    {
                        int parsed;
                        if (int.TryParse(context.control.displayName, out parsed))
                        {
                            weaponIndex = parsed;
                        }
                        else if (int.TryParse(context.control.name, out parsed))
                        {
                            weaponIndex = parsed;
                        }
                    }
                }
            }

            switch (weaponIndex)
            {
                case 1:
                    EquipWeapon(weaponPrefab1);
                    break;
                case 2:
                    EquipWeapon(weaponPrefab2);
                    break;
                case 3:
                    EquipWeapon(weaponPrefab3);
                    break;
                default:
                    Debug.Log($"Weapon switch: Unrecognized input ({weaponIndex}).");
                    break;
            }
        }
    }

    private IEnumerator ResetFaceMoveDirectionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        shouldFaceMoveDirection = true;
    }

    public IEnumerator OnReload(InputAction.CallbackContext context)
    {
        if (context.performed)
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
    }

    void SetTargetRotationToCamera()
    {
        if (playerTransform != null)
        {
            Vector3 lookDir = Camera.main.transform.forward;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                targetRotation = Quaternion.LookRotation(lookDir, Vector3.up);
                shouldFaceMoveDirection = false;
            }
        }
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

    private void EquipWeapon(GameObject prefab)
    {
        if (weaponPoint == null)
        {
            Debug.LogWarning("Weapon point not assigned!");
            return;
        }

        // Remove existing weapon
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = null;
        }

        if (prefab == null)
        {
            Debug.Log("No weapon prefab assigned for this slot.");
            return;
        }

        currentWeapon = Instantiate(prefab, weaponPoint);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        // If the new weapon contains a child named "FirePoint", use that as the fire point
        Transform newFire = currentWeapon.transform.Find("FirePoint");
        if (newFire != null)
        {
            firePoint = newFire;
        }

        Debug.Log($"Equipped weapon: {prefab.name}");
    }

    // Public getters for UI
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;
    public bool IsReloading() => isReloading;
    public bool IsAiming() => isAiming;

    // Update is called once per frame
    void Update()
    {
        if (isShooting && !isReloading && currentAmmo > 0 && Time.time >= lastFireTime + fireRate)
        {
            Shoot();
        }

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

        float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        if (shouldFaceMoveDirection && moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (playerTransform != null && targetRotation.HasValue)
        {
            playerTransform.rotation = Quaternion.Slerp(
                playerTransform.rotation,
                targetRotation.Value,
                rotateToCameraSpeed * Time.deltaTime
            );

            if (Quaternion.Angle(playerTransform.rotation, targetRotation.Value) < 0.5f)
            {
                targetRotation = null;
            }
        }
    }
}
