using UnityEngine;

public class CardShooter : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] public float shootInterval = 1.5f;
    [SerializeField] public float cardSpeed = 6f;
    public int cardDamage = 10;
    [SerializeField] private Sprite[] cardSprites;

    private void Start()
    {
        InvokeRepeating(nameof(ShootCards), 0f, shootInterval);
    }

    private void ShootCards()
    {
        Vector2[] directions = {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right,
            new Vector2(1,1).normalized, new Vector2(-1,1).normalized,
            new Vector2(1,-1).normalized, new Vector2(-1,-1).normalized
        };

        foreach (var dir in directions)
        {
            GameObject card = Instantiate(cardPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = card.GetComponent<Rigidbody2D>();
            rb.linearVelocity = dir * cardSpeed;

            SpriteRenderer sr = card.GetComponent<SpriteRenderer>();
            if (sr != null && cardSprites.Length > 0)
            {
                sr.sprite = cardSprites[Random.Range(0, cardSprites.Length)];
            }
            CardProjectile projectile = card.GetComponent<CardProjectile>();
            if (projectile != null)
            {
                projectile.SetDamage(Mathf.RoundToInt(cardDamage));
            }
        }
    }
    public float GetInterval()
    {
        return shootInterval;
    }

    public void SetInterval(float value)
    {
        shootInterval = value;

        // Reset el intervalo si ya se estaba invocando
        CancelInvoke(nameof(ShootCards));
        InvokeRepeating(nameof(ShootCards), 0f, shootInterval);
    }

    public void SetProjectileDamage(int damage)
    {
        cardDamage = damage;
    }



}
