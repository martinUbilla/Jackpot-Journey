using UnityEngine;

public class DashAbility : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float invulnerabilityDuration = 0.3f;

    [Header("Damage Settings (opcional)")]
    [SerializeField] private int dashDamage = 20;
    [SerializeField] private LayerMask enemyLayer;

    private float lastDashTime;
    private bool isDashing = false;
    private Rigidbody2D rb;
    private Character playerHealth;
    [SerializeField] private UpgradeData dashUpgradeData;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<Character>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown)
        {
            Vector2 dashDir = GetInputDirection();
            if (dashDir != Vector2.zero)
            {
                StartCoroutine(PerformDash(dashDir));
                lastDashTime = Time.time;
            }
        }
    }

    private Vector2 GetInputDirection()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        return new Vector2(h, v).normalized;
    }

    private System.Collections.IEnumerator PerformDash(Vector2 direction)
    {
        if (dashUpgradeData == null)
            Debug.LogError("?? dashUpgradeData está NULL, no se puede activar cooldown");

        FindFirstObjectByType<Level>().TriggerCooldown(dashUpgradeData);
        isDashing = true;

        if (playerHealth != null)
            playerHealth.SetInvulnerable(true);

        float dashTime = dashDistance / dashSpeed;
        float elapsed = 0f;

        while (elapsed < dashTime)
        {
            rb.linearVelocity = direction * dashSpeed;

            

            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(invulnerabilityDuration);

        if (playerHealth != null)
            playerHealth.SetInvulnerable(false);

        isDashing = false;
        
    }
    public float GetCooldown()
    {
        return dashCooldown;
    }

    public void SetCooldown(float value)
    {
        dashCooldown = value;
    }
    public void SetUpgradeData(UpgradeData data)
    {
        dashUpgradeData = data;
    }

}
