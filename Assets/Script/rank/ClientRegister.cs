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
    

    //싱글톤
    public static ClientRegister singletonclient = null;
    void Awake()
    {
        if (singletonclient == null)//싱글톤클라이언트가 널이면
        {
            singletonclient = this;//이걸 싱글톤 클라이언트로 설정
            DontDestroyOnLoad(this.gameObject);//파괴되지 않게 설정
        }
        else//널이 아니면
        {
            if(singletonclient != this)//이게 싱글톤 클라이언트가 아닐 때
            {
                Destroy(this.gameObject);//이걸 파괴
            }
        }
    }

    public Text t; //시각적으로 데이터를 볼 수 있게 하는 텍스트
    public Text en; //에너지 보유량을 볼 수 있게 하는 텍스트
    public Text rt; //랭킹 텍스트
    public Text mrt; //자신의 랭킹 텍스트
    public bool already = false;//서버와 연결을 시도중인지 확인하는 값
    public int userenergy=0; //유저의 에너지 보유량
    bool clientReady=false; //서버와 연결이 됐는지 확인하는 불값
    bool conneting = false; //연결을 시도중인지 환인하는 불값
    TcpClient client; //클라이언트
    NetworkStream stream; //스트림
    StreamWriter writer; //스트림 라이터
    StreamReader reader; // 스트림 리더
    //string ip = "172.16.42.55"; //서버의 아이피 주소 -> 학교 컴퓨터
    string ip = "1.242.177.79"; //서버의 아이피 주소
    int port = 7777; //사용할 포트번호
    string userID = ""; //유저 ID
    string password = ""; //유저 비밀번호
    public int killcount=0; //적을 처치한 횟수

    //게임 시작 버튼을 누르면 작동할 것
    public void gamestartButton()
    {

        if (!clientReady&&!conneting) //서버와 연결중이 아니고 연결을 시도중이지 않을 때
        {
            connectserver(); //연결 시도
        }
    }

    void connectserver() //연결 시도
    {
        conneting = true; //연결 시도중임
        if (clientReady) return; //이미 연결되어있다면 아무 일도 일어나지 않음
        try
        {
            client = new TcpClient(ip, port); //클라이언트, 스트림, 라이터, 리더 설정
            stream = client.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            clientReady = true; //위 코드가 다 실행됐다면 서버와 연결된 것
        }
        catch (Exception e) //연결 실패시
        {
            clientReady = false; //연결 실패
            t.text = $"\n소켓에러 : {e.Message}"; //에러 메세지 표시
        }
        conneting = false; //연결 시도가 끝남
    }

    void Send(string data) //서버에 매개변수로 받은 데이터를 전달하는 함수
    {
        if (!clientReady) //서버와 연결되어있지 않으면
        {
            return; //반환하여 즉시 함수 종료
        }
        writer.WriteLine(data);//라이터를 통해 스트림에 데이터를 씀
        writer.Flush(); //서버로 보냄
    }

    // Update is called once per frame
    void Update() //매 프레임 실행할 함수
    {
        if (clientReady && stream.DataAvailable) //서버와 연결되어있고 스트림에서 데이터를 읽어올 수 있으면
        {
            string data = reader.ReadLine(); //리더로 데이터를 읽어온다
            if (data != null)//데이터가 있는지 검사
            {
                OnIncomingData(data);//받은 데이터를 매개변수로 함수 실행
            }
        }
    }

    void OnIncomingData(string data)//데이터를 받았을 때 실행하는 함수
    {
        if (data.StartsWith("&registcomplet"))//받은 데이터가 등록이 완료되었다는 신호로 시작하면(회원가입)
        {
            alreadyRegist = false;//회원가입중이지 않음
            t.text = "Regist Complete"; //회원가입이 완료되었다고 표시
        }

        if (data.StartsWith("&yesid"))//이미 똑같은 아이디가 있다는 신호로 시작하면(회원가입)
        {
            alreadyRegist = false; //회원가입중이지 않음
            t.text = data; //화면에 중복된 아이디라고 표시
        }

        if (data.StartsWith("&noid"))//(로그인 시)아이디가 없으면
        {
            alreadyLogin = false;//로그인중이지 않음
            t.text = data;//등록되지 않은 ID라고 표시
        }

        if (data.StartsWith("&nopassword"))//(로그인 시)패스워드가 다르다는 신호를 받았으면
        {
            alreadyLogin = false;//로그인중이지 않음
            t.text = data; //비밀번호가 다르다고 표시
        }

        if (data.StartsWith("&gomain")) //(로그인 시)메인화면으로 가라는 신호를 받았으면
        {
            alreadyLogin = false;//로그인중이지 않음
            userID = data.Split('|')[1]; //아이디 기억
            password = data.Split('|')[2]; //비밀번호 기억
            t.text = "main"; //로그인에 성공하여 메인화면으로 간다고 표시
            SceneManager.LoadScene("Main_menu"); //메인메뉴 씬 로드
        }
        if (data.StartsWith("&energy")) //(에너지 정보 요청)에너지 신호를 받았으면
        {
            if(Regex.IsMatch(data.Split('|')[1], @"^[0-9]+$"))//문자열에 숫자 문자만 있는지 확인 후
            {
                userenergy = Int32.Parse(data.Split('|')[1]); //숫자로 변환해 변수에 저장
            }
            if(en!=null)
                en.text = userenergy.ToString(); //에너지 표시
        }
        if (data.StartsWith("&gostage")) //(스테이지 입장) 스테이지 입장 허가
        {
            string stage = data.Split('|')[1]; //스테이지 데이터 받음
            alreadyStage = false; //스테이지 입장중이 아님
            //ClientEnergy();
            SceneManager.LoadScene(stage);//스테이지입장함수;
        }
        if (data.StartsWith("&nostage"))//(스테이지 입장) 스테이지 입장 불허
        {
            //에너지가 부족합니다
            Debug.Log("에너지가 부족합니다");
            ClientEnergy();//에너지 갱신
            alreadyStage = false;//입장중이 아님
            
        }
        if (data.StartsWith("&clear"))//스테이지 종료
        {
            killcount = 0;
            alreadyClear = false;
        }
        if (data.StartsWith("&rank"))//받은 데이터가 랭크로 시작하면
        {
            alreadyRank = false;//랭킹 정보 요청중이 아님
            Debug.Log(data);
            string[] splitdata = data.Split('|');//데이터를 |기준으로 나눔(서버에서 |를 구분문자로 랭킹 데이터를 보냄)
            
            if(splitdata.Length > 0)//데이터가 있는지 확인
            {
                for(int i = 0; i < splitdata.Length-1; i++)//데이터 길이만큼 작업
                {
                    if (i == 0) //첫 데이터는 무슨 데이터인지 구분하기 위한 데이터로 순위 작업에 영향없음
                    {
                        rt.text = "★ Rank ★" + "\n";//텍스트를 손봐줍니다
                    }
                    else if (i == 1)//순위라고 할만한 첫 데이터. 서버에서 자신의 등수와 아이디를 보냄. 
                    {
                        mrt.text = "My Rank" + "\n" + splitdata[i]; //텍스트를 손봐줍니다
                    }
                    else if (i == 2) //순위라고 할만한 두번째 데이터. 
                    {
                        mrt.text += " : " + splitdata[i];//자신의 적 처치 수 추가
                    }
                    else if (i % 2 == 1)//데이터를 한명당 2번씩 끊어서 받았으니 2로 나눠줍니다(현재 i는 3)
                    {
                        rt.text += $"{i/2} " + splitdata[i];//i를 2로 나눈 몫을 순위로 매겨줍니다, 이름을 표시합니다.
                    }
                    else
                    {
                        rt.text += ": " + splitdata[i] + "kill\n";//해당 순위 플레이어의 킬 수를 표시합니다.
                    }
                }
            }
        }
        already = false;//서버에 요청중인 데이터가 없습니다.
    }

    //회원가입, 로그인 시 ID와 비밀번호는 인풋필드.텍스트로 받아옴
    bool alreadyRegist = false;//이미 회원가입중인가
    public void ClientRegist(string id, string pw)//회원가입 함수 매개변수로 아이디, 패스워드 입력칸의 텍스트를 받아옴
    {
        if (!clientReady)//서버와 연결되지 않았으면
        {
            t.text = "서버와 연결이 끊어졌습니다";
            return;//종료
        }
        if (alreadyRegist)//이미 회원가입중이면
        {
            t.text = "이미 회원가입을 시도중입니다. 잠시만 기다려주세요";
            return;
        }
        if (already)//이미 통신중이면
        {
            t.text = "already processing";
            return;//종료
        }
        //위의 모든 조건을 피해가면
        Send($"regist|{id}|{pw}");//회원가입을 알리는 말과 id와 pw를 서버로 전달, 구분자로 | 사용
        if (clientReady) //서버와 연결되어있으면(한번 더 확인)
        {
            alreadyRegist = true; //회원가입을 시도중임
        }
    }

    //로그인
    bool alreadyLogin = false;//이미 로그인중인가
    public void ClientLogin(string id, string pw)//로그인 함수 매개변수로 아이디, 패스워드 입력칸의 텍스트를 받아옴
    {
        if (!clientReady)//클라이언트와 서버가 연결되어있지 않으면
        {
            t.text = "서버와 연결이 끊어졌습니다";
            return;
        }
        if (already)//이미 서버와 통신중이면
        {
            t.text = "already processing";
            return;
        }
        if (already)//이미 로그인중이면
        {
            t.text = "already processing";
            return;
        }
        Send($"login|{id}|{pw}");//로그인 신호와 id pw 서버로 전송
        if (clientReady)//
        {
            alreadyLogin = true;//로그인중
        }
    }

    //에너지 정보 요청
    public void ClientEnergy()
    {
        if (!clientReady)//클라이언트와 서버가 연결되어있지 않으면
        {
            t.text = "서버와 연결이 끊어졌습니다";
            return;
        }
        if (!already)//다른 통신중이지 않으면
            Send($"energy|{userID}|{password}");//에너지 정보 요청
    }

    //스테이지 입장 요청
    bool alreadyStage = false;
    public void ClientStage(string stage)
    {
        if (!clientReady)//클라이언트와 서버가 연결되어있지 않으면
        {
            t.text = "서버와 연결이 끊어졌습니다";
            return;
        }
        if (alreadyStage)//이미 스태이지 입장 요청중이면
        {
            t.text = "already processing";
            return;
        }
        if (clientReady)//서버와 연결되어있다면
        {
            Send($"stage|{userID}|{password}|{stage}");//스테이지 입장 요청
            alreadyStage = true;
        }
    }

    
    //스테이지 종료
    bool alreadyClear = false;
    public void ClientClear(int kill)
    {
        if (alreadyClear)//이미 스테이지 종료 처리 요청중이면
        {
            return;//리턴
        }
        if (clientReady)//서버와 연결되어있다면
        {
            Send($"clear|{userID}|{password}|{kill}");//스테이지 종료 처리 요청 - 킬 수를 넘겨줌
            alreadyClear = true;
            killcount = 0;//킬 카운트 초기화
        }
    }

    //랭킹정보
    bool alreadyRank;
    public void ClientRank()
    {
        if (alreadyRank)//이미 랭킹정보를 요청중이면
        {
            //t.text = "already processing";
            return;//리턴
        }
        Send($"rank|{userID}|{password}");//랭킹정보 요청
        if (clientReady)
        {
            alreadyRank = true;
        }
    }

    //

    void OnApplicationQuit()//앱종료시
    {
        CloseSocket();//연결해제
    }

    void CloseSocket()//연결끊는기능
    {
        if (!clientReady) return;//연결이 안 되어 있다면 아무일도 일어나지 않게 리턴

        writer.Close();//라이터 닫기
        reader.Close();//리더 닫기
        client.Close();//tcp클라이언트 닫기
        clientReady = false;//연결이 안 됐다는 불값으로 설정
    }

}