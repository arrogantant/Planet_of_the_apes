using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Energy : MonoBehaviour
{
    
    public Text e;
    
    void Start()
    {
        ClientRegister.singletonclient.en = e; //텍스트 설정
        getenergy();//에너지 표기
    }

    void getenergy()
    {
        ClientRegister.singletonclient.ClientEnergy();//서버에서 에너지 보유량를 받아옴
    }

}
