using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GoldManager : MonoBehaviour
{
    public List<int> costs; // 각 버튼별 지불할 골드의 양 리스트
    public List<Button> payButtons; // 지불 버튼 리스트
    public List<Button> otherButtons; // 활성화될 다른 버튼 리스트
    public Button resetButton; // 단일 초기화 버튼
    public List<Text> otherTexts;

    void Start()
    {
        // 게임 시작 시 모든 otherButtons를 비활성화합니다.
        foreach (var otherButton in otherButtons)
        {
            otherButton.gameObject.SetActive(false);
        }
        resetButton.interactable = true;  // 항상 활성화 상태로 변경
        UpdateButtonStates();
    }

    public void PayGold(int buttonIndex)
    {
        if(buttonIndex < 0 || buttonIndex >= costs.Count)
        {
            return;
        }

        int currentGold = PlayerPrefs.GetInt("PlayerGold", 0);
        int cost = costs[buttonIndex];

        if (currentGold >= cost)
        {
            currentGold -= cost;
            PlayerPrefs.SetInt("PlayerGold", currentGold);
            PlayerPrefs.SetInt("Paid" + buttonIndex, 1); // 지불 상태 저장
            PlayerPrefs.Save();

            // PlayerGoldManager의 골드 값을 갱신합니다.
            PlayerGoldManager.gold = currentGold;
            if (PlayerGoldManager.instance != null) 
            {
                PlayerGoldManager.instance.UpdateGoldText(); // UI를 갱신합니다.
            }

            // 해당 버튼과 연결된 텍스트를 활성화합니다.
            if (buttonIndex >= 0 && buttonIndex < otherTexts.Count)
            {
                otherTexts[buttonIndex].gameObject.SetActive(true);
            }

            // UI 갱신 및 상태 업데이트
            UpdateButtonStates();
        }
    }

    private void UpdateButtonStates()
    {
        for (int i = 0; i < payButtons.Count; i++)
        {
            int paid = PlayerPrefs.GetInt("Paid" + i, 0);
            // 각 버튼의 상태를 업데이트합니다.
            payButtons[i].interactable = (paid == 0);
            otherButtons[i].gameObject.SetActive(paid == 1);
            
            // 연결된 텍스트의 상태도 업데이트합니다.
            if (i < otherTexts.Count)
            {
                otherTexts[i].gameObject.SetActive(paid == 1);
            }
        }
    }
    private void CheckResetButton()
    {
        // 모든 구매가 완료되었는지 확인합니다.
        bool anyPurchases = false;
        for (int i = 0; i < payButtons.Count; i++)
        {
            if (PlayerPrefs.GetInt("Paid" + i, 0) == 1)
            {
                anyPurchases = true;
                break;
            }
        }
        // 초기화 버튼을 해당 상태에 따라 활성화 또는 비활성화합니다.
        resetButton.interactable = anyPurchases;
    }

    public void ResetPurchase(int buttonIndex)
    {
        // 모든 구매 상태를 초기화합니다.
        for (int i = 0; i < payButtons.Count; i++)
        {
            PlayerPrefs.DeleteKey("Paid" + i);
            
            // 각 otherButtons를 비활성화하고 payButtons를 활성화합니다.
            otherButtons[i].gameObject.SetActive(false);
            payButtons[i].interactable = true;
        }

        // 변경 사항을 저장합니다.
        PlayerPrefs.Save();

        // 상태를 업데이트합니다.
        UpdateButtonStates();
    }
}
