using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAim : MonoBehaviour
{
    public Transform owner; // 총을 소유한 캐릭터 (SubCharacter_Normal)의 Transform
    public float detectionRadius = 5f;
    public LayerMask enemyLayer;
    private CircleCollider2D ownerCircleCollider;  // 캐릭터의 Circle Collider
    [SerializeField]
    private Vector3 defaultGunRotation; // 기본 총 회전
    [SerializeField]
    private Vector3 defaultGunPosition; // 기본 총 위치
    [SerializeField]
    private float defaultGunRotationY = 0f; // 기본 총 회전 Y축
    public float lerpSpeed = 5f;

    private void Start()
    {
        ownerCircleCollider = owner.GetComponent<CircleCollider2D>();
        if (ownerCircleCollider == null)
        {
            Debug.LogError("No CircleCollider2D found on the owner!");
        }
    }

    void Update()
    {
        AimAtClosestEnemy();
    }

    void AimAtClosestEnemy()
    {
        Collider2D[] detectedEnemies = Physics2D.OverlapCircleAll(owner.position, detectionRadius, enemyLayer);
        
        if (detectedEnemies.Length > 0)
        {
            Collider2D closestEnemy = GetClosestEnemy(detectedEnemies);
            
            Vector2 directionToEnemy = closestEnemy.transform.position - owner.position;
            float angleToEnemy = Mathf.Atan2(directionToEnemy.y, directionToEnemy.x) * Mathf.Rad2Deg;

            // 총의 새 위치를 Circle Collider의 겉부분에 설정
            Vector2 targetGunPosition = owner.position + Quaternion.Euler(0, 0, angleToEnemy) * Vector3.right * ownerCircleCollider.radius;
            transform.position = Vector3.Lerp(transform.position, targetGunPosition, Time.deltaTime * lerpSpeed);

            // 적이 왼쪽에 있을 경우 총 스프라이트를 뒤집습니다.
            if(directionToEnemy.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                angleToEnemy += 180; // 기존 각도에 180을 추가하여 회전
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

            // 총이 적을 향하게 회전
            Quaternion targetGunRotation = Quaternion.Euler(0f, 0f, angleToEnemy);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetGunRotation, Time.deltaTime * lerpSpeed);
        }
        else
        {
            // 적이 감지되지 않을 경우 기본 위치로 설정
            transform.localPosition = Vector3.Lerp(transform.localPosition, defaultGunPosition, Time.deltaTime * lerpSpeed);

            // 총이 현재 오른쪽을 바라보고 있다면 defaultGunRotationY를 180으로 설정
            if (transform.localScale.x < 0)
            {
                defaultGunRotationY = 0f;
            }
            else
            {
                defaultGunRotationY = 180f;
            }

            Quaternion targetRotation = Quaternion.Euler(0, defaultGunRotationY, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * lerpSpeed);
        }
    }

    Collider2D GetClosestEnemy(Collider2D[] enemies)
    {
        Collider2D closest = null;
        float shortestDistance = Mathf.Infinity;
        foreach (var enemy in enemies)
        {
            float distance = Vector2.Distance(owner.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closest = enemy;
            }
        }
        return closest;
    }
}
