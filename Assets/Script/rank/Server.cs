using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.IO;

public class Server : MonoBehaviour
{

    //싱글톤
    private static Server singletonserver = null;
    void Awake()
    {
        if (singletonserver == null)
        {
            singletonserver = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (singletonserver != this)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public Text t;

    public int port = 7779;
    TcpListener server;
    bool serverReady=false;

    string phppw = "1234";
    public string sid;
    public string snick;
    public string spw;

    public string adduserURL = "http://localhost/TestDB/register.php";
    public string loginURL = "http://localhost/TestDB/login.php";
    public string energyURL = "http://localhost/TestDB/energy.php";
    public string stageURL = "http://localhost/TestDB/stage.php";
    public string clearURL = "http://localhost/TestDB/clear.php";

    List<ServerClient> clients;
    List<ServerClient> disconnectList;

    public void ServerCreate()
    {
        try
        {
            clients = new List<ServerClient>();
            disconnectList = new List<ServerClient>();

            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            StartListening();
            serverReady = true;
            Debug.Log($"{port}번 포트를 사용하여 서버를 생성함.");
        }
        catch(Exception e)//서버 생성 오류 시
        {
            Debug.Log($"서버 생성 오류 : {e.Message}");
        }
    }

    void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));
        StartListening();
        SendData("Welcome!",clients[clients.Count - 1]);
    }
    void SendData(string data, ServerClient c)
    {
        try
        {
            StreamWriter writer = new StreamWriter(c.tcp.GetStream());
            writer.WriteLine(data);
            writer.Flush();
        }
        catch(Exception e)
        {
            Debug.Log($"데이터 전송 오류 : {e.Message}");
        }
    }

    void Broadcast(string data, List<ServerClient> cl)
    {
        foreach (var c in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(c.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception e)
            {
                Debug.Log($"브로드캐스트 오류 메세지 : {e.Message}");
            }
        }
    }

    
    void Start()
    {
    }

    
    void Update()
    {
        if (serverReady)
        {
            foreach (ServerClient c in clients)
            {
                
                if (!IsConnected(c.tcp))
                {
                    c.tcp.Close();
                    disconnectList.Add(c);
                    continue;
                }
                
                else
                {
                    NetworkStream s = c.tcp.GetStream();
                    if (s.DataAvailable)
                    {
                        string data = new StreamReader(s, true).ReadLine();
                        if (data != null)
                            OnIncomingData(c, data);
                    }
                }
            }
        }

        
    }

    bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                return true;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }

    //받은 데이터에 따라 서버에서 실행할 코드
    bool usedb = false;
    void OnIncomingData(ServerClient c, string data)
    {
        if (data.StartsWith("regist|"))
        {
            string uid = data.Split('|')[1];
            string upw = data.Split('|')[2];
            StartCoroutine(RegisterPost(c, uid, upw, phppw));
        }
        if (data.StartsWith("login|"))
        {
            string uid = data.Split('|')[1];
            string upw = data.Split('|')[2];
            StartCoroutine(LoginPost(c, uid, upw, phppw));
        }
        if (data.StartsWith("energy|"))
        {
            string uid = data.Split('|')[1];
            string upw = data.Split('|')[2];
            StartCoroutine(EnergyPost(c, uid, upw, phppw));
        }
        if (data.StartsWith("stage|"))
        {
            string uid = data.Split('|')[1];
            string upw = data.Split('|')[2];
            string stage = data.Split('|')[3];
            StartCoroutine(StagePost(c, uid, upw, stage, phppw));
        }
        if (data.StartsWith("clear|"))
        {
            string uid = data.Split('|')[1];
            string upw = data.Split('|')[2];
            string stage = data.Split('|')[3];
            string kill = data.Split('|')[4];
            StartCoroutine(ClearPost(c, uid, upw, stage, kill, phppw));
        }
    }

    IEnumerator RegisterPost(ServerClient c, string userid, string userpassword, string pw)
    {
        WWWForm form = new WWWForm();
        form.AddField("userid", userid);
        form.AddField("userpassword", userpassword);
        form.AddField("phppassword", pw);
        using (UnityWebRequest www = UnityWebRequest.Post(adduserURL, form))
        {
            yield return www.SendWebRequest();
            if (!(www.error == null))
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log(data);
                if (data == "0")
                {
                    SendData("&registcomplete",c);
                }
                else
                {
                    SendData("&yesid", c);
                }
            }
        }

    }

    IEnumerator LoginPost(ServerClient c, string userid, string userpassword, string pw)
    {
        WWWForm form = new WWWForm();
        form.AddField("userid", userid);
        form.AddField("userpassword", userpassword);
        form.AddField("phppassword", pw);
        using (UnityWebRequest www = UnityWebRequest.Post(loginURL, form))
        {
            yield return www.SendWebRequest();
            if (!(www.error == null))
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log(data);
                if (data == "login")
                {
                    SendData($"&gomain|{userid}|{userpassword}", c);
                }
                else if(data == "passwordissue")
                {
                    SendData("&nopassword", c);
                }
                else if (data == "idissue")
                {
                    SendData("&noid", c);
                }
            }
        }

    }

    IEnumerator EnergyPost(ServerClient c, string userid, string userpassword, string pw)
    {
        WWWForm form = new WWWForm();
        form.AddField("userid", userid);
        form.AddField("userpassword", userpassword);
        form.AddField("phppassword", pw);
        using (UnityWebRequest www = UnityWebRequest.Post(energyURL, form))
        {
            yield return www.SendWebRequest();
            if (!(www.error == null))
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log(data);
                SendData($"&energy|{data}", c);
            }
        }

    }

    IEnumerator StagePost(ServerClient c, string userid, string userpassword, string stage, string pw)
    {
        WWWForm form = new WWWForm();
        form.AddField("userid", userid);
        form.AddField("userpassword", userpassword);
        form.AddField("phppassword", pw);
        form.AddField("useenergy", stage);
        using (UnityWebRequest www = UnityWebRequest.Post(stageURL, form))
        {
            yield return www.SendWebRequest();
            if (!(www.error == null))
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log(data);
                if(data == "gostage")
                {
                    SendData($"&gostage|{stage}", c);
                }
                else
                {
                    SendData("&nostage", c);
                }
            }
        }

    }

    IEnumerator ClearPost(ServerClient c, string userid, string userpassword, string stage, string kill, string pw)
    {
        WWWForm form = new WWWForm();
        form.AddField("userid", userid);
        form.AddField("userpassword", userpassword);
        form.AddField("phppassword", pw);
        using (UnityWebRequest www = UnityWebRequest.Post(adduserURL, form))
        {
            yield return www.SendWebRequest();
            if (!(www.error == null))
            {
                Debug.Log(www.error);
            }
            else
            {
                string data = www.downloadHandler.text;
                Debug.Log(data);
            }
        }

    }
}


public class ServerClient
{
    public TcpClient tcp;
    public string clientuid;
    public string clientpw="0";

    public ServerClient(TcpClient clientSocket)
    {
        clientuid = "";
        tcp = clientSocket;
    }
}
