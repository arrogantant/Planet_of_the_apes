using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartGame : MonoBehaviour
{
    
    public TMP_InputField id;
    public TMP_InputField pw;
    public TMP_Text starttext;
    public Text clienttext;
    private void Start()
    {
        ClientRegister.singletonclient.t = clienttext;//�ؽ�Ʈ ����
    }

    /*
    public void StartButtonClick()
    {
        ClientRegister.singletonclient.gamestartButton();
    }
    */
    bool InputCheck()//��ȿ�� ���ڸ� �Է��ߴ��� �˻�
    {
        //�Ʒ� ���ǹ��鿡 �ش� �� false ��ȯ
        if (String.IsNullOrWhiteSpace(id.text) || String.IsNullOrWhiteSpace(pw.text))//id/pw�� ��ĭ�̸�
        {
            starttext.text = "Do not empty";//������� ����� �� �����
            return false;
        }
        else if (!(Regex.IsMatch(id.text, @"^[a-zA-z0-9_]+$")))//���̵� ���ĺ��� ����, ������ ����Ͽ� �Է��Ͽ����� Ȯ��
        {
            starttext.text = "���ĺ��� ����, ���ٸ� �Է��� �� �ֽ��ϴ�.";//�ȳ�����
            return false;
        }
        else if (!(Regex.IsMatch(pw.text, @"^[a-zA-z0-9_]+$")))//��й�ȣ�� ���ĺ��� ����, ������ ����Ͽ� �Է��Ͽ����� Ȯ��
        {
            starttext.text = "���ĺ��� ����, ���ٸ� �Է��� �� �ֽ��ϴ�.";//�ȳ�����
            return false;
        }
        else//���� �ش���� ������
        {
            return true;//true ��ȯ
        }
    }
    public void RegisterButtonClick()//ȸ������ ��ư Ŭ�� ��
    {
        if (InputCheck())//��ȿ�� ���ڸ� �Է��ߴٸ�
        {
            ClientRegister.singletonclient.ClientRegist(id.text, pw.text);//ȸ������
        }
        
    }

    public void LoginButtonClick()//�α��� ��ư Ŭ�� ��
    {
        if (InputCheck())//��ȿ�� ���ڸ� �Է��ߴٸ�
        {
            ClientRegister.singletonclient.ClientLogin(id.text, pw.text);//�α���
        }
    }
    

}
