using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class ClientRegister : MonoBehaviour
{
    

    //�̱���
    public static ClientRegister singletonclient = null;
    void Awake()
    {
        if (singletonclient == null)//�̱���Ŭ���̾�Ʈ�� ���̸�
        {
            singletonclient = this;//�̰� �̱��� Ŭ���̾�Ʈ�� ����
            DontDestroyOnLoad(this.gameObject);//�ı����� �ʰ� ����
        }
        else//���� �ƴϸ�
        {
            if(singletonclient != this)//�̰� �̱��� Ŭ���̾�Ʈ�� �ƴ� ��
            {
                Destroy(this.gameObject);//�̰� �ı�
            }
        }
    }

    public Text t; //�ð������� �����͸� �� �� �ְ� �ϴ� �ؽ�Ʈ
    public Text en; //������ �������� �� �� �ְ� �ϴ� �ؽ�Ʈ
    public Text rt; //��ŷ �ؽ�Ʈ
    public Text mrt; //�ڽ��� ��ŷ �ؽ�Ʈ
    public bool already = false;//������ ������ �õ������� Ȯ���ϴ� ��
    public int userenergy=0; //������ ������ ������
    bool clientReady=false; //������ ������ �ƴ��� Ȯ���ϴ� �Ұ�
    bool conneting = false; //������ �õ������� ȯ���ϴ� �Ұ�
    TcpClient client; //Ŭ���̾�Ʈ
    NetworkStream stream; //��Ʈ��
    StreamWriter writer; //��Ʈ�� ������
    StreamReader reader; // ��Ʈ�� ����
    //string ip = "172.16.42.55"; //������ ������ �ּ� -> �б� ��ǻ��
    string ip = "1.242.177.79"; //������ ������ �ּ�
    int port = 7777; //����� ��Ʈ��ȣ
    string userID = ""; //���� ID
    string password = ""; //���� ��й�ȣ
    public int killcount=0; //���� óġ�� Ƚ��

    //���� ���� ��ư�� ������ �۵��� ��
    public void gamestartButton()
    {

        if (!clientReady&&!conneting) //������ �������� �ƴϰ� ������ �õ������� ���� ��
        {
            connectserver(); //���� �õ�
        }
    }

    void connectserver() //���� �õ�
    {
        conneting = true; //���� �õ�����
        if (clientReady) return; //�̹� ����Ǿ��ִٸ� �ƹ� �ϵ� �Ͼ�� ����
        try
        {
            client = new TcpClient(ip, port); //Ŭ���̾�Ʈ, ��Ʈ��, ������, ���� ����
            stream = client.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            clientReady = true; //�� �ڵ尡 �� ����ƴٸ� ������ ����� ��
        }
        catch (Exception e) //���� ���н�
        {
            clientReady = false; //���� ����
            t.text = $"\n���Ͽ��� : {e.Message}"; //���� �޼��� ǥ��
        }
        conneting = false; //���� �õ��� ����
    }

    void Send(string data) //������ �Ű������� ���� �����͸� �����ϴ� �Լ�
    {
        if (!clientReady) //������ ����Ǿ����� ������
        {
            return; //��ȯ�Ͽ� ��� �Լ� ����
        }
        writer.WriteLine(data);//�����͸� ���� ��Ʈ���� �����͸� ��
        writer.Flush(); //������ ����
    }

    // Update is called once per frame
    void Update() //�� ������ ������ �Լ�
    {
        if (clientReady && stream.DataAvailable) //������ ����Ǿ��ְ� ��Ʈ������ �����͸� �о�� �� ������
        {
            string data = reader.ReadLine(); //������ �����͸� �о�´�
            if (data != null)//�����Ͱ� �ִ��� �˻�
            {
                OnIncomingData(data);//���� �����͸� �Ű������� �Լ� ����
            }
        }
    }

    void OnIncomingData(string data)//�����͸� �޾��� �� �����ϴ� �Լ�
    {
        if (data.StartsWith("&registcomplet"))//���� �����Ͱ� ����� �Ϸ�Ǿ��ٴ� ��ȣ�� �����ϸ�(ȸ������)
        {
            alreadyRegist = false;//ȸ������������ ����
            t.text = "Regist Complete"; //ȸ�������� �Ϸ�Ǿ��ٰ� ǥ��
        }

        if (data.StartsWith("&yesid"))//�̹� �Ȱ��� ���̵� �ִٴ� ��ȣ�� �����ϸ�(ȸ������)
        {
            alreadyRegist = false; //ȸ������������ ����
            t.text = data; //ȭ�鿡 �ߺ��� ���̵��� ǥ��
        }

        if (data.StartsWith("&noid"))//(�α��� ��)���̵� ������
        {
            alreadyLogin = false;//�α��������� ����
            t.text = data;//��ϵ��� ���� ID��� ǥ��
        }

        if (data.StartsWith("&nopassword"))//(�α��� ��)�н����尡 �ٸ��ٴ� ��ȣ�� �޾�����
        {
            alreadyLogin = false;//�α��������� ����
            t.text = data; //��й�ȣ�� �ٸ��ٰ� ǥ��
        }

        if (data.StartsWith("&gomain")) //(�α��� ��)����ȭ������ ����� ��ȣ�� �޾�����
        {
            alreadyLogin = false;//�α��������� ����
            userID = data.Split('|')[1]; //���̵� ���
            password = data.Split('|')[2]; //��й�ȣ ���
            t.text = "main"; //�α��ο� �����Ͽ� ����ȭ������ ���ٰ� ǥ��
            SceneManager.LoadScene("Main_menu"); //���θ޴� �� �ε�
        }
        if (data.StartsWith("&energy")) //(������ ���� ��û)������ ��ȣ�� �޾�����
        {
            if(Regex.IsMatch(data.Split('|')[1], @"^[0-9]+$"))//���ڿ��� ���� ���ڸ� �ִ��� Ȯ�� ��
            {
                userenergy = Int32.Parse(data.Split('|')[1]); //���ڷ� ��ȯ�� ������ ����
            }
            if(en!=null)
                en.text = userenergy.ToString(); //������ ǥ��
        }
        if (data.StartsWith("&gostage")) //(�������� ����) �������� ���� �㰡
        {
            string stage = data.Split('|')[1]; //�������� ������ ����
            alreadyStage = false; //�������� �������� �ƴ�
            //ClientEnergy();
            SceneManager.LoadScene(stage);//�������������Լ�;
        }
        if (data.StartsWith("&nostage"))//(�������� ����) �������� ���� ����
        {
            //�������� �����մϴ�
            Debug.Log("�������� �����մϴ�");
            ClientEnergy();//������ ����
            alreadyStage = false;//�������� �ƴ�
            
        }
        if (data.StartsWith("&clear"))//�������� ����
        {
            killcount = 0;
            alreadyClear = false;
        }
        if (data.StartsWith("&rank"))//���� �����Ͱ� ��ũ�� �����ϸ�
        {
            alreadyRank = false;//��ŷ ���� ��û���� �ƴ�
            Debug.Log(data);
            string[] splitdata = data.Split('|');//�����͸� |�������� ����(�������� |�� ���й��ڷ� ��ŷ �����͸� ����)
            
            if(splitdata.Length > 0)//�����Ͱ� �ִ��� Ȯ��
            {
                for(int i = 0; i < splitdata.Length-1; i++)//������ ���̸�ŭ �۾�
                {
                    if (i == 0) //ù �����ʹ� ���� ���������� �����ϱ� ���� �����ͷ� ���� �۾��� �������
                    {
                        rt.text = "�� Rank ��" + "\n";//�ؽ�Ʈ�� �պ��ݴϴ�
                    }
                    else if (i == 1)//������� �Ҹ��� ù ������. �������� �ڽ��� ����� ���̵� ����. 
                    {
                        mrt.text = "My Rank" + "\n" + splitdata[i]; //�ؽ�Ʈ�� �պ��ݴϴ�
                    }
                    else if (i == 2) //������� �Ҹ��� �ι�° ������. 
                    {
                        mrt.text += " : " + splitdata[i];//�ڽ��� �� óġ �� �߰�
                    }
                    else if (i % 2 == 1)//�����͸� �Ѹ�� 2���� ��� �޾����� 2�� �����ݴϴ�(���� i�� 3)
                    {
                        rt.text += $"{i/2} " + splitdata[i];//i�� 2�� ���� ���� ������ �Ű��ݴϴ�, �̸��� ǥ���մϴ�.
                    }
                    else
                    {
                        rt.text += ": " + splitdata[i] + "kill\n";//�ش� ���� �÷��̾��� ų ���� ǥ���մϴ�.
                    }
                }
            }
        }
        already = false;//������ ��û���� �����Ͱ� �����ϴ�.
    }

    //ȸ������, �α��� �� ID�� ��й�ȣ�� ��ǲ�ʵ�.�ؽ�Ʈ�� �޾ƿ�
    bool alreadyRegist = false;//�̹� ȸ���������ΰ�
    public void ClientRegist(string id, string pw)//ȸ������ �Լ� �Ű������� ���̵�, �н����� �Է�ĭ�� �ؽ�Ʈ�� �޾ƿ�
    {
        if (!clientReady)//������ ������� �ʾ�����
        {
            t.text = "������ ������ ���������ϴ�";
            return;//����
        }
        if (alreadyRegist)//�̹� ȸ���������̸�
        {
            t.text = "�̹� ȸ�������� �õ����Դϴ�. ��ø� ��ٷ��ּ���";
            return;
        }
        if (already)//�̹� ������̸�
        {
            t.text = "already processing";
            return;//����
        }
        //���� ��� ������ ���ذ���
        Send($"regist|{id}|{pw}");//ȸ�������� �˸��� ���� id�� pw�� ������ ����, �����ڷ� | ���
        if (clientReady) //������ ����Ǿ�������(�ѹ� �� Ȯ��)
        {
            alreadyRegist = true; //ȸ�������� �õ�����
        }
    }

    //�α���
    bool alreadyLogin = false;//�̹� �α������ΰ�
    public void ClientLogin(string id, string pw)//�α��� �Լ� �Ű������� ���̵�, �н����� �Է�ĭ�� �ؽ�Ʈ�� �޾ƿ�
    {
        if (!clientReady)//Ŭ���̾�Ʈ�� ������ ����Ǿ����� ������
        {
            t.text = "������ ������ ���������ϴ�";
            return;
        }
        if (already)//�̹� ������ ������̸�
        {
            t.text = "already processing";
            return;
        }
        if (already)//�̹� �α������̸�
        {
            t.text = "already processing";
            return;
        }
        Send($"login|{id}|{pw}");//�α��� ��ȣ�� id pw ������ ����
        if (clientReady)//
        {
            alreadyLogin = true;//�α�����
        }
    }

    //������ ���� ��û
    public void ClientEnergy()
    {
        if (!clientReady)//Ŭ���̾�Ʈ�� ������ ����Ǿ����� ������
        {
            t.text = "������ ������ ���������ϴ�";
            return;
        }
        if (!already)//�ٸ� ��������� ������
            Send($"energy|{userID}|{password}");//������ ���� ��û
    }

    //�������� ���� ��û
    bool alreadyStage = false;
    public void ClientStage(string stage)
    {
        if (!clientReady)//Ŭ���̾�Ʈ�� ������ ����Ǿ����� ������
        {
            t.text = "������ ������ ���������ϴ�";
            return;
        }
        if (alreadyStage)//�̹� �������� ���� ��û���̸�
        {
            t.text = "already processing";
            return;
        }
        if (clientReady)//������ ����Ǿ��ִٸ�
        {
            Send($"stage|{userID}|{password}|{stage}");//�������� ���� ��û
            alreadyStage = true;
        }
    }

    
    //�������� ����
    bool alreadyClear = false;
    public void ClientClear(int kill)
    {
        if (alreadyClear)//�̹� �������� ���� ó�� ��û���̸�
        {
            return;//����
        }
        if (clientReady)//������ ����Ǿ��ִٸ�
        {
            Send($"clear|{userID}|{password}|{kill}");//�������� ���� ó�� ��û - ų ���� �Ѱ���
            alreadyClear = true;
            killcount = 0;//ų ī��Ʈ �ʱ�ȭ
        }
    }

    //��ŷ����
    bool alreadyRank;
    public void ClientRank()
    {
        if (alreadyRank)//�̹� ��ŷ������ ��û���̸�
        {
            //t.text = "already processing";
            return;//����
        }
        Send($"rank|{userID}|{password}");//��ŷ���� ��û
        if (clientReady)
        {
            alreadyRank = true;
        }
    }

    //

    void OnApplicationQuit()//�������
    {
        CloseSocket();//��������
    }

    void CloseSocket()//������±��
    {
        if (!clientReady) return;//������ �� �Ǿ� �ִٸ� �ƹ��ϵ� �Ͼ�� �ʰ� ����

        writer.Close();//������ �ݱ�
        reader.Close();//���� �ݱ�
        client.Close();//tcpŬ���̾�Ʈ �ݱ�
        clientReady = false;//������ �� �ƴٴ� �Ұ����� ����
    }

}