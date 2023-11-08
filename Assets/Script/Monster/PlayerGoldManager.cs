using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerGoldManager : MonoBehaviour
{
    public Text goldText;
    public static int gold = 0;
    public static PlayerGoldManager instance;

    private void Awake()
    {
        // 싱글턴 패턴 구현
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        LoadGold(); // 게임이 시작할 때 저장된 골드 값을 로드합니다.
        UpdateGoldText(); // 초기 텍스트 업데이트
    }

    private void Update()
    {
        UpdateGoldText();
    }

    public void UpdateGoldText()
    {
        if (goldText != null)
        {
            goldText.text = "Gold: " + gold.ToString();
        }
    }

    private void OnApplicationQuit()
    {
        SaveGold(); // 게임이 종료될 때 골드 값을 저장합니다.
    }

    public void SaveGold()
    {
        PlayerPrefs.SetInt("PlayerGold", gold);
        PlayerPrefs.Save(); // 변경사항을 디스크에 쓰기 위해 명시적으로 호출합니다.
    }

    public void LoadGold()
    {
        // "PlayerGold" 키로 저장된 값을 로드하고, 없을 경우 0을 반환합니다.
        gold = PlayerPrefs.GetInt("PlayerGold", 0);
        UpdateGoldText(); // UI를 갱신합니다.
    }
}
