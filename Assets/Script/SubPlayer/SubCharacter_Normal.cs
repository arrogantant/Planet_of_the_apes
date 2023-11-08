using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCharacter_Normal : MonoBehaviour
{
    public Transform player; 
    public float followSpeed = 5f;
    public float detectionRadius = 5f;
    public LayerMask enemyLayer;
    public GameObject bulletPrefab; // 총알 프리팹
    public float bulletSpeed = 10f;
    public float attackCooldown = 1f;
    public Transform firePoint; // 총알이 발사되는 위치
    public float stayDistance = 2f; // 플레이어와 유지하려는 거리
    public float smoothTime = 0.2f; // 움직임의 부드러움을 제어. 작은 값은 더 빠르게 반응.
    private Vector2 velocity; // Lerp의 속도를 위한 private 변수
    private float lastAttackTime;
    public float rotationSmoothness = 5f;
    private Quaternion targetRotation;  // 회전 목표를 저장할 변수
    private bool shouldRotate = false;  // 회전해야 하는지 확인할 변수
    private Vector2 lastPlayerPosition;

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        if (player == null)
        {
            return;
        }

        lastPlayerPosition = player.position;
    }
    void Update()
    {
        if (shouldRotate)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothness);
        }

        Collider2D[] detectedEnemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);
        if (detectedEnemies.Length > 0)
        {
            FaceAwayFromClosestEnemy(GetClosestEnemy(detectedEnemies).transform.position);
        }
        else
        {
            FaceDirectionBasedOnPlayerMovement();
        }
    }
    void FixedUpdate()
    {
        FollowPlayer();
        AutoAttack();
    }

    void FollowPlayer()
    {
        Vector2 targetPosition = (Vector2)player.position - (Vector2)transform.position;
        float distance = targetPosition.magnitude;

        if (distance > stayDistance)
        {
            // 원하는 위치 계산
            Vector2 desiredPosition = (Vector2)player.position - targetPosition.normalized * stayDistance;
            
            // 현재 위치와 원하는 위치 사이를 부드럽게 이동
            transform.position = Vector2.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        }
    }

    void AutoAttack()
    {
        // 먼저 쿨다운이 끝났는지 확인.
        if (Time.time - lastAttackTime > attackCooldown)
        {
            Collider2D[] detectedEnemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);

            if (detectedEnemies.Length > 0)
            {
                Collider2D closestEnemy = GetClosestEnemy(detectedEnemies);
                Debug.Log("Enemy Detected!");

                // 쿨다운 확인 후 적을 감지하면 즉시 발사합니다.
                Shoot(closestEnemy);
                lastAttackTime = Time.time;
            }
            else
            {
                Debug.Log("No enemy detected in the radius.");
            }
        }
    }
    Collider2D GetClosestEnemy(Collider2D[] enemies)
    {
        Collider2D closest = null;
        float shortestDistance = Mathf.Infinity;
        foreach (var enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closest = enemy;
            }
        }
        return closest;
    }
    void FaceAwayFromClosestEnemy(Vector3 enemyPosition)
    {
        Vector3 directionToEnemy = enemyPosition - transform.position;

        if (directionToEnemy.x > 0)
        {
            targetRotation = Quaternion.Euler(0f, 180f, 0f);
            shouldRotate = true;
        }
        else if (directionToEnemy.x < 0)
        {
            targetRotation = Quaternion.Euler(0f, 0f, 0f);
            shouldRotate = true;
        }
    }

    void Shoot(Collider2D detectedEnemy)
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab is missing!");
            return;
        }

        Vector2 enemyPosition = detectedEnemy.transform.position;

        // 총이 적을 향하게 회전
        Vector2 direction = enemyPosition - (Vector2)firePoint.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0f, 0f, angle);

        Rigidbody2D enemyRb = detectedEnemy.GetComponent<Rigidbody2D>();

        if (enemyRb != null)
        {
            Vector2 enemyDirection = enemyRb.velocity;
            float distanceToEnemy = Vector2.Distance(firePoint.position, enemyPosition);
            float timeToReach = distanceToEnemy / bulletSpeed;

            Vector2 predictedPosition = enemyPosition + enemyDirection * timeToReach;

            Vector2 shootDirection = (predictedPosition - (Vector2)firePoint.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.velocity = shootDirection * bulletSpeed;
        }
        else
        {
            Vector2 shootDirection = direction.normalized;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.velocity = shootDirection * bulletSpeed;
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    void FaceDirectionBasedOnPlayerMovement()
    {
        // 플레이어의 이동 방향 계산
        Vector2 playerMovementDirection = (Vector2)player.position - lastPlayerPosition;
        
        // 플레이어의 이동 방향에 따라서 서브 캐릭터의 Y축 회전을 조절
        if (playerMovementDirection.x > 0)
        {
            targetRotation = Quaternion.Euler(0f, 180f, 0f);
            shouldRotate = true;
        }
        else if (playerMovementDirection.x < 0)
        {
            targetRotation = Quaternion.Euler(0f, 0f, 0f);
            shouldRotate = true;
        }

        // 현재 플레이어 위치를 이전 위치로 저장
        lastPlayerPosition = player.position;
    }
}
