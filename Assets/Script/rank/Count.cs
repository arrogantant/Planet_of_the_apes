using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Count : MonoBehaviour
{
    //ī��Ʈ�ϴ� Ŭ����

    public int stage1kill = 0; //��������123,��ü ų ��
    public int stage2kill = 0;
    public int stage3kill = 0;
    public int kill = 0;
    public int playerkill = 0;//����ĳ���ͷ� óġ�� �� ��
    public int soldierkill = 0;//����ĳ���ͷ� óġ�� ��



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

    public void MainKill(int a) //���� ĳ���ͷ� óġ//�Ű������� ���� óġ�� ��������
    {
        playerkill++;
        KillMon(a);
    }

    public void SubKill(int a) //���� ĳ���ͷ� óġ
    {
        soldierkill++;
        KillMon(a);
    }

    void GameOver() // ���� ���� �� ȣ��
    {
        
    }

}
