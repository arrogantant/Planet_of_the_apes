using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Count : MonoBehaviour
{
    //카운트하는 클래스

    public int stage1kill = 0; //스테이지123,전체 킬 수
    public int stage2kill = 0;
    public int stage3kill = 0;
    public int kill = 0;
    public int playerkill = 0;//메인캐릭터로 처치한 몹 수
    public int soldierkill = 0;//서브캐릭터로 처치한 몹



    public void KillMon(int stage) 
    {
        if(stage == 1)
        {
            stage1kill++;
        }
        else if (stage == 2)
        {
            stage2kill++;
        }
        else if (stage == 3)
        {
            stage3kill++;
        }
        kill++;
    }

    public void MainKill(int a) //메인 캐릭터로 처치//매개변수는 몹을 처치한 스테이지
    {
        playerkill++;
        KillMon(a);
    }

    public void SubKill(int a) //서브 캐릭터로 처치
    {
        soldierkill++;
        KillMon(a);
    }

    void GameOver() // 게임 종료 시 호출
    {
        
    }

}
