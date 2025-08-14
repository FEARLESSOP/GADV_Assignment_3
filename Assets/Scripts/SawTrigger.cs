using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawTrigger : MonoBehaviour
{
    public Transform respawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.position = respawnPoint.position;

            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }
}
