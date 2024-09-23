using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;   // 발사체 프리팹
    [SerializeField] private Transform firePoint;           // 발사체가 발사될 시작 지점 (카메라의 정면)
    [SerializeField] private float projectileSpeed = 20f;   // 발사체 속도
    [SerializeField] private float projectileLifetime = 3f; // 발사체 생존 시간

    private void Start()
    {
        // firePoint는 카메라 정면 방향의 약간 앞에 위치하도록 설정
        firePoint.position = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
        firePoint.rotation = Camera.main.transform.rotation; // 카메라와 같은 방향으로 발사
    }

    private void Update()
    {
        // 마우스 클릭 또는 터치 시 발사체 발사
        if (Input.GetMouseButtonDown(0))
        {
            ShootProjectile();
        }
    }

    private void ShootProjectile()
    {
        // 발사체를 firePoint 위치에서 생성
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // 발사체에 Rigidbody를 부여하여 물리 이동 처리
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * projectileSpeed; // Z축(정면)을 향해 발사체 나아감
        }

        // 발사체를 일정 시간이 지나면 파괴
        Destroy(projectile, projectileLifetime);
    }
}
