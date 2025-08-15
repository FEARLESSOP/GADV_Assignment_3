using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
    public float pushStrength = 10f;

    private void OnTriggerStay2D(Collider2D other)
    {
        //check if object is player
        if (other.CompareTag("Player"))
        {
            //get player rigidbody
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                //push player up
                rb.AddForce(Vector2.up * pushStrength, ForceMode2D.Force);
            }
        }
    }
}
