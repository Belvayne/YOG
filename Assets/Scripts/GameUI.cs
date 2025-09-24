using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI healthText;
    public Slider healthBar;
    public Image crosshair;
    public GameObject reloadIndicator;
    
    [Header("Crosshair Settings")]
    public Color normalCrosshairColor = Color.white;
    public Color aimingCrosshairColor = Color.red;
    public float crosshairSize = 20f;
    
    private PlayerShooting playerShooting;
    private Health playerHealth;
    private Camera playerCamera;
    
    void Start()
    {
        // Find player components
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerShooting = player.GetComponent<PlayerShooting>();
            playerHealth = player.GetComponent<Health>();
        }
        
        // Get camera reference
        playerCamera = Camera.main;
        
        // Subscribe to health events
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.AddListener(UpdateHealthUI);
        }
        
        // Initialize UI
        UpdateAmmoUI();
        UpdateHealthUI(playerHealth != null ? playerHealth.GetCurrentHealth() : 100f);
    }
    
    void Update()
    {
        UpdateAmmoUI();
        UpdateCrosshair();
        UpdateReloadIndicator();
    }
    
    void UpdateAmmoUI()
    {
        if (ammoText != null && playerShooting != null)
        {
            ammoText.text = $"{playerShooting.GetCurrentAmmo()}/{playerShooting.GetMaxAmmo()}";
        }
    }
    
    void UpdateHealthUI(float currentHealth)
    {
        if (playerHealth == null) return;
        
        // Update health text
        if (healthText != null)
        {
            healthText.text = $"Health: {Mathf.Ceil(currentHealth)}/{playerHealth.GetMaxHealth()}";
        }
        
        // Update health bar
        if (healthBar != null)
        {
            healthBar.value = playerHealth.GetHealthPercentage();
        }
    }
    
    void UpdateCrosshair()
    {
        if (crosshair == null || playerCamera == null) return;
        
        // Position crosshair at screen center
        crosshair.rectTransform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        
        // Change color based on aiming state
        if (playerShooting != null && playerShooting.IsAiming())
        {
            crosshair.color = aimingCrosshairColor;
        }
        else
        {
            crosshair.color = normalCrosshairColor;
        }
        
        // Scale crosshair
        crosshair.rectTransform.sizeDelta = Vector2.one * crosshairSize;
    }
    
    void UpdateReloadIndicator()
    {
        if (reloadIndicator != null && playerShooting != null)
        {
            reloadIndicator.SetActive(playerShooting.IsReloading());
        }
    }
}
