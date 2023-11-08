using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public Button[] buttons; // 모든 버튼을 참조할 배열
    public Image[] images; // 활성화/비활성화할 이미지들을 참조할 배열

    // 버튼이 클릭될 때 호출될 메서드
    void Start()
    {
        // 시작할 때 모든 이미지를 비활성화합니다.
        foreach (var image in images)
        {
            image.enabled = false;
        }

        // 0번째 이미지만 활성화합니다.
        if (images.Length > 0)
        {
            images[0].enabled = true;
        }
    }

    public void OnButtonSelected(int buttonIndex)
    {
        // 모든 이미지를 순회하면서
        for (int i = 0; i < images.Length; i++)
        {
            // 선택된 버튼의 인덱스와 일치하면 이미지를 활성화, 그렇지 않으면 비활성화
            images[i].enabled = (i == buttonIndex);
        }
    }
}
