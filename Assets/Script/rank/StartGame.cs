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
        ClientRegister.singletonclient.t = clienttext;//텍스트 설정
    }

    /*
    public void StartButtonClick()
    {
        ClientRegister.singletonclient.gamestartButton();
    }
    */
    bool InputCheck()//유효한 문자를 입력했는지 검사
    {
        //아래 조건문들에 해당 시 false 반환
        if (String.IsNullOrWhiteSpace(id.text) || String.IsNullOrWhiteSpace(pw.text))//id/pw가 빈칸이면
        {
            starttext.text = "Do not empty";//비워두지 말라는 말 남기기
            return false;
        }
        else if (!(Regex.IsMatch(id.text, @"^[a-zA-z0-9_]+$")))//아이디를 알파벳과 숫자, 영문만 사용하여 입력하였는지 확인
        {
            starttext.text = "알파벳과 숫자, 밑줄만 입력할 수 있습니다.";//안내문구
            return false;
        }
        else if (!(Regex.IsMatch(pw.text, @"^[a-zA-z0-9_]+$")))//비밀번호를 알파벳과 숫자, 영문만 사용하여 입력하였는지 확인
        {
            starttext.text = "알파벳과 숫자, 밑줄만 입력할 수 있습니다.";//안내문구
            return false;
        }
        else//위에 해당되지 않으면
        {
            return true;//true 반환
        }
    }
    public void RegisterButtonClick()//회원가입 버튼 클릭 시
    {
        if (InputCheck())//유효한 문자를 입력했다면
        {
            ClientRegister.singletonclient.ClientRegist(id.text, pw.text);//회원가입
        }
        
    }

    public void LoginButtonClick()//로그인 버튼 클릭 시
    {
        if (InputCheck())//유효한 문자를 입력했다면
        {
            ClientRegister.singletonclient.ClientLogin(id.text, pw.text);//로그인
        }
    }
    

}
