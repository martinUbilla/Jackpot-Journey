using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Upgrade System/Negative/SlowMovement")]
public class SlowMovementUpgrade : UpgradeData
{
    public float duration = 5f;
    public float slowFactor = 0.6f; // Reduce velocidad al 60%

    public override bool IsNegative => true;

    public override void Apply(GameObject player)
    {
        Debug.Log("Reduciendo velocidad");
        var movement = player.GetComponent<PlayerMove>(); // tu script de movimiento
        if (movement != null)
        {
            movement.StartCoroutine(SlowTemporarily(movement));
        }
    }

    private IEnumerator SlowTemporarily(PlayerMove movement)
    {
        float originalSpeed = movement.speed;
        movement.speed *= slowFactor;
        yield return new WaitForSeconds(duration);
        movement.speed = originalSpeed;
    }
}
