using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class St_1_eye : Monster
{
    public float Speed = 4f;
    public float Health = 80f;
    public Sprite fastMonsterSprite; // Unity 에디터에서 설정할 FastMonster 스프라이트

    protected override void InitializeMonster()
    {
        speed = Speed;
        health = Health;
        spriteRenderer.sprite = fastMonsterSprite;
    }
}
