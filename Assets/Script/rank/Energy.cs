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
        ClientRegister.singletonclient.en = e; //�ؽ�Ʈ ����
        getenergy();//������ ǥ��
    }

    void getenergy()
    {
        ClientRegister.singletonclient.ClientEnergy();//�������� ������ �������� �޾ƿ�
    }

}
