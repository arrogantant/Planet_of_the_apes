using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DownUI : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;

    public GameObject empty1; // 첫 번째 버튼에 연결된 Empty
    public GameObject empty2; // 두 번째 버튼에 연결된 Empty
    public GameObject empty3; // 세 번째 버튼에 연결된 Empty

    public DragUI dragUIComponent;

    private void Start()
    {
        // 초기 설정: 첫 번째 버튼이 눌러진 상태를 표현하려면 첫 번째 Empty만 활성화
        ActivateEmpty(empty1);

        // 각 버튼 클릭 이벤트에 함수를 연결
        button1.onClick.AddListener(() => ActivateEmpty(empty1));
        button2.onClick.AddListener(() => ActivateEmpty(empty2));
        button3.onClick.AddListener(() => ActivateEmpty(empty3));
    }

    void ActivateEmpty(GameObject targetEmpty)
    {
        // 모든 Empty를 비활성화
        empty1.SetActive(false);
        empty2.SetActive(false);
        empty3.SetActive(false);

        // 드래그된 이미지를 원래 위치로 돌려놓음
        if (dragUIComponent)
        {
            dragUIComponent.ResetToInitialPosition(); // 여기서 원래 위치로 돌립니다.
        }
        // 대상 Empty만 활성화
        targetEmpty.SetActive(true);
    }
}
