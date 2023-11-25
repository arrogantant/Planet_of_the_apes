using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class St_1_Boss : Monster
{
    public float Speed = 4f;
    public float Health = 80f;
    public Sprite fastMonsterSprite;
    public int stageNumber = 0;
    private float specialAttackThreshold = 0.8f; // 체력이 50% 이하일 때 특수 공격 사용
    private float lastAttackTime;
    private float attackCooldown = 2f;
    public GameObject projectilePrefab;
    private void Update()
    {
        base.Update();

        if (Time.time > lastAttackTime + attackCooldown)
        {
            if (health / Health <= specialAttackThreshold)
            {
                SpecialAttack();
            }
            lastAttackTime = Time.time;
        }
    }
    private void SpecialAttack()
    {
        // 여러 방향으로 에너지 파동 발사
        int numberOfProjectiles = 8; // 발사할 프로젝타일 수
        float angleStep = 360f / numberOfProjectiles;
        float angle = 0f;

        for (int i = 0; i <= numberOfProjectiles - 1; i++)
        {
            float projectileDirXposition = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180);
            float projectileDirYposition = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180);

            Vector3 projectileVector = new Vector3(projectileDirXposition, projectileDirYposition, 0);
            Vector3 projectileMoveDirection = (projectileVector - transform.position).normalized * 5f;

            GameObject tmpObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            tmpObj.GetComponent<Rigidbody2D>().velocity = new Vector2(projectileMoveDirection.x, projectileMoveDirection.y);

            angle += angleStep;
        }
    }

    public void OnBossDefeated()
    {
        // 보스가 패배했을 때 호출되는 메서드
        StageClear();
    }

    protected override void InitializeMonster()
    {
        speed = Speed;
        health = Health;
        spriteRenderer.sprite = fastMonsterSprite;
    }

    protected override void OnDeath()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.OnBossDefeated();
        }

        // 체력이 0이 되어 보스가 사망하면 스테이지 클리어
        StageClear();

        base.OnDeath();
    }

    void StageClear()
    {
        // 현재 스테이지 번호를 사용하여 클리어 상태를 저장
        PlayerPrefs.SetInt("STAGE_CLEAR_" + stageNumber, 1);

        // 다음 스테이지의 번호를 계산
        int nextStageNumber = stageNumber + 1;

        // 다음 스테이지를 '잠금 해제' 상태로 만듭니다.
        PlayerPrefs.SetInt("STAGE_CLEAR_" + nextStageNumber, 1); // 1은 활성화, 잠금 해제

        // 변경 사항 저장
        PlayerPrefs.Save();
    }

}