using UnityEngine;

public class SawTrap : MonoBehaviour
{
    public GameObject sawBladePrefab;
    public Transform shootPoint;
    public float shootInterval = 3f;
    public Transform playerRespawnPoint;

    public Vector3 sawScale = Vector3.one;  // Set saw scale here in Inspector

    private float shootTimer = 0f;

    private void Update()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootInterval)
        {
            ShootSawBlade();
            shootTimer = 0f;
        }
    }

    void ShootSawBlade()
    {
        // Get shoot direction: trap's right rotated 90 degrees counter-clockwise (up if right)
        Vector3 baseDirection = transform.right;
        Vector3 shootDirection = Quaternion.Euler(0, 0, 90) * baseDirection;

        // Calculate rotation to face shoot direction (convert vector to angle)
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        GameObject saw = Instantiate(sawBladePrefab, shootPoint.position, rotation);

        // Set scale
        saw.transform.localScale = sawScale;

        SawBlade sawBladeScript = saw.GetComponent<SawBlade>();
        sawBladeScript.moveDirection = shootDirection;
        sawBladeScript.playerRespawnPoint = playerRespawnPoint;

        Collider2D sawCollider = saw.GetComponent<Collider2D>();
        Collider2D trapCollider = GetComponent<Collider2D>();
        if (sawCollider != null && trapCollider != null)
        {
            Physics2D.IgnoreCollision(sawCollider, trapCollider);
        }
    }
}
