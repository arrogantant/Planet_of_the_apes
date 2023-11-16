using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class St_3_Boss : Monster
{
    public float Speed = 4f;
    public float Health = 80f;
    public Sprite fastMonsterSprite;
    public int stageNumber = 2;

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
        stageNumber = 2;
        StageClear();
        base.OnDeath();
    }

    void StageClear()
    {
        // 현재 스테이지 번호를 사용하여 클리어 상태를 저장
        PlayerPrefs.SetInt("STAGE_CLEAR_" + stageNumber, 1); // 스테이지 클리어 상태 저장

        // 다음 스테이지의 번호를 계산
        int nextStageNumber = stageNumber + 1;

        // 다음 스테이지를 '잠금 해제' 상태로 만듭니다.
        PlayerPrefs.SetInt("STAGE_CLEAR_" + nextStageNumber, 1); // 다음 스테이지 잠금 해제

        // 변경 사항 저장
        PlayerPrefs.Save();
        Debug.Log("Stage " + stageNumber + " cleared. Unlocking stage " + nextStageNumber);

    }

}
