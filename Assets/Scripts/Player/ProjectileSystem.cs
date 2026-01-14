using StarterAssets;
using UnityEngine;

public class ProjectileSystem : MonoBehaviour
{
    Camera mainCamera;
    StarterAssetsInputs _input;

    [Tooltip("Time required to pass before being able to fire again. Set to 0f to instantly fire again")]
    public float FireTimeout = 0.1f;
    // timeout deltatime
    private float _fireTimeoutDelta;

    [Header("Projectile Settings")]
    [Tooltip("Projectile prefab to be instantiated when firing")]
    public GameObject[] projectilePrefabs;
    [Tooltip("Force applied to the projectile when fired")]
    private int prefabIndex = 0;
    public float projectileForce = 500f;
    [Tooltip("Point from which the projectile is fired")]
    public Transform firePoint;
    [Tooltip("Projectile lifetime in seconds")]
    public float projectileLifetime = 1f;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
    }

    void Update()
    {
        Fire();
        SwitchProjectile();
    }

    private void Fire()
    {
        if (_input.fire && _fireTimeoutDelta <= 0f)
        {
            // Instantiate the projectile at the fire point
            GameObject projectile = Instantiate(projectilePrefabs[prefabIndex], firePoint.position, Quaternion.identity);

            // Apply force to the projectile rigidbody
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(mainCamera.transform.forward * projectileForce);

            // Reset the timeout
            _fireTimeoutDelta = FireTimeout;

            // Reset the fire input
            _input.fire = false;

            // Destroy the projectile after its lifetime
            Destroy(projectile, projectileLifetime);
        }

        // Reduce the timeout timer
        if (_fireTimeoutDelta >= 0f)
        {
            _fireTimeoutDelta -= Time.deltaTime;
        }
    }

    private void SwitchProjectile()
    {
        if (_input.switchPressed)
        {
            if (_input.switchValue > 0f)
            {
                prefabIndex = (prefabIndex + 1) % projectilePrefabs.Length;
            }
            else if (_input.switchValue < 0f)
            {
                prefabIndex = (prefabIndex - 1 + projectilePrefabs.Length) % projectilePrefabs.Length;
            }
        }
    }
}
