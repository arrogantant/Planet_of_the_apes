using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Rank : MonoBehaviour
{

    public Text rank;
    public Text myrank;

    public void Start()
    {
        ClientRegister.singletonclient.rt = rank;
        ClientRegister.singletonclient.mrt = myrank;
        Rankload();
    }


    public void Rankload()
    {
        ClientRegister.singletonclient.ClientRank();
    }

    public void GoMain()//�������� ������
    {
        SceneManager.LoadScene("Main_menu");
    }

}
