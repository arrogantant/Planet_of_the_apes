using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletLifetime = 5f;  // 총알이 존재하는 시간 (예: 5초)

    private void Start()
    {
        Destroy(gameObject, bulletLifetime);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // "Enemy" 레이어와 "Boss" 레이어의 인덱스를 가져옵니다.
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int bossLayer = LayerMask.NameToLayer("Boss");
        
        // collider의 레이어가 "Enemy" 또는 "Boss"인지 확인합니다.
        if (collider.gameObject.layer == enemyLayer || collider.gameObject.layer == bossLayer)
        {
            Destroy(gameObject);
        }
    }
}
