using UnityEngine;

public class SawBlade : MonoBehaviour
{
    public float bladeSpeed = 10f;
    public Vector3 moveDirection = Vector3.right;
    public Transform playerRespawnPoint;

    public float maxTravelDistance = 20f;

    private Vector3 initialPosition;
    private Rigidbody2D bladeRigidbody;

    private void Start()
    {
        //get rigidbody
        bladeRigidbody = GetComponent<Rigidbody2D>();
        //set blade velocity
        bladeRigidbody.velocity = moveDirection.normalized * bladeSpeed;
        //save start position
        initialPosition = transform.position;
    }

    private void Update()
    {
        //destroy blade if it goes too far
        if (Vector3.Distance(initialPosition, transform.position) >= maxTravelDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //check if hit player
        if (other.CompareTag("Player"))
        {
            //move player to respawn point
            other.transform.position = playerRespawnPoint.position;
            //destroy blade
            Destroy(gameObject);
        }
        //destroy blade if hit ground
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
