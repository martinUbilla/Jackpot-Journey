using UnityEngine;

public class PickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Character c = collision.GetComponent<Character>();
        if(c != null)
        {
            GetComponent<IpickupObject>().OnPickUp(c);
            Destroy(gameObject);
        }
    }
}

