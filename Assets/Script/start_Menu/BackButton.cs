using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    public GameObject thisButtonObject; // 이 스크립트가 붙어 있는 버튼의 게임 오브젝트
    public GameObject objectToDisable1; // 비활성화할 첫 번째 오브젝트
    public GameObject objectToDisable2; // 비활성화할 두 번째 오브젝트
    public GameObject objectToDisable3; // 비활성화할 세 번째 오브젝트
    public GameObject objectToActivate1; // 활성화할 첫 번째 버튼 오브젝트
    public GameObject objectToActivate2; // 활성화할 두 번째 버튼 오브젝트
    public GameObject additionalObject; // 되돌리기 버튼

    private Button thisButton; // 실제 버튼 컴포넌트 참조

    void Start()
    {
        // 버튼 컴포넌트를 찾습니다.
        thisButton = thisButtonObject.GetComponent<Button>();
        if (thisButton != null)
        {
            // 버튼의 onClick 이벤트에 DisableItems 메서드를 연결합니다.
            thisButton.onClick.AddListener(DisableItems);
        }
        else
        {
            // 버튼 컴포넌트가 없다면 오류 로그를 출력합니다.
            Debug.LogError("Button component not found on the button object!");
        }
    }

    void OnEnable()
    {
        // 게임 오브젝트가 활성화될 때 이전 상태를 복원합니다.
        // 예를 들어, thisButtonObject가 비활성화 상태라면 활성화합니다.
        thisButtonObject.SetActive(true);
        // 추가 로직이 필요하다면 여기에 상태 복원 코드를 추가합니다.
    }

    void DisableItems()
    {
        // objectToDisable1이 활성화 상태라면 비활성화합니다.
        if (objectToDisable1.activeSelf)
        {
            objectToDisable1.SetActive(false);
        }

        // objectToDisable2가 활성화 상태라면 비활성화합니다.
        if (objectToDisable2.activeSelf)
        {
            objectToDisable2.SetActive(false);
        }

        // objectToDisable3가 활성화 상태라면 비활성화합니다.
        if (objectToDisable3 != null && objectToDisable3.activeSelf)
        {
            objectToDisable3.SetActive(false);
        }

        // objectToActivate1과 objectToActivate2를 활성화합니다.
        if (objectToActivate1 != null)
        {
            objectToActivate1.SetActive(true);
        }

        if (objectToActivate2 != null)
        {
            objectToActivate2.SetActive(true);
        }

        // additionalObject가 비활성화 상태라면 활성화합니다.
        if (additionalObject != null && !additionalObject.activeSelf)
        {
            additionalObject.SetActive(true);
        }

        // 버튼이 포함된 게임 오브젝트를 비활성화합니다.
        // 주의: 이것을 주석 처리하거나 삭제하면 버튼이 계속 활성화 상태로 남습니다.
        // thisButtonObject.SetActive(false);
    }
}
