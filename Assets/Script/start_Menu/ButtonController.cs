using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class ButtonController : MonoBehaviour
{
    // 버튼과 오브젝트 리스트를 public으로 선언하여 인스펙터에서 조절할 수 있게 함
    public List<Button> buttons;
    public List<GameObject> objectsToActivate;
    public GameObject additionalObject; // 추가적으로 활성화/비활성화할 오브젝트

    void Start()
    {
        // 각 버튼에 대해 클릭 리스너를 추가
        foreach (var button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button));
        }
    }

    void OnButtonClicked(Button clickedButton)
    {
        // 모든 버튼을 비활성화
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(false);
        }

        // 클릭된 버튼에 해당하는 오브젝트만 활성화
        int buttonIndex = buttons.IndexOf(clickedButton);
        if (buttonIndex < objectsToActivate.Count)
        {
            objectsToActivate[buttonIndex].SetActive(true);
            additionalObject.SetActive(true); // 추가 오브젝트도 활성화
        }
        // 나머지 오브젝트를 비활성화
        for (int i = 0; i < objectsToActivate.Count; i++)
        {
            if (i != buttonIndex)
            {
                objectsToActivate[i].SetActive(false);
            }
        }
    }
}
