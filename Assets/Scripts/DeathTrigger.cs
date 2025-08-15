using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawTrigger : MonoBehaviour
{
    public Transform respawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //check if hit object is player
        if (collision.CompareTag("Player"))
        {
            //move player to respawn point
            collision.transform.position = respawnPoint.position;

            //get player rigidbody
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                //stop player movement
                rb.velocity = Vector2.zero;
                //stop player rotation
                rb.angularVelocity = 0f;
            }
        }
    }
}
