using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class start_Text : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float fadeInDuration = 2.0f; // Text가 완전히 보이기까지의 시간
    public float fadeOutDuration = 2.0f; // Text가 완전히 사라지기까지의 시간

    private void Start()
    {
        // 시작 시 텍스트는 보이지 않게 설정
        Color initialColor = textComponent.color;
        initialColor.a = 0f;
        textComponent.color = initialColor;

        // 투명도를 천천히 증가시키는 코루틴 시작
        StartCoroutine(FadeInText());
    }

    IEnumerator FadeInText()
    {
        yield return new WaitForSeconds(0.5f);

        float startTime = Time.time;
        while (Time.time - startTime < fadeInDuration)
        {
            float t = (Time.time - startTime) / fadeInDuration;
            Color currentColor = textComponent.color;
            currentColor.a = t;
            textComponent.color = currentColor;
            yield return null;
        }

        // 마지막으로 텍스트를 완전히 불투명하게 설정
        Color finalColor = textComponent.color;
        finalColor.a = 1f;
        textComponent.color = finalColor;

        // 텍스트가 완전히 보인 뒤에 천천히 투명하게 하는 코루틴 시작
        yield return FadeOutText();
    }

    IEnumerator FadeOutText()
    {
        float startTime = Time.time;
        while (Time.time - startTime < fadeOutDuration)
        {
            float t = (Time.time - startTime) / fadeOutDuration;
            Color currentColor = textComponent.color;
            currentColor.a = 1f - t;
            textComponent.color = currentColor;
            yield return null;
        }

        // 마지막으로 텍스트를 완전히 투명하게 설정
        Color finalColor = textComponent.color;
        finalColor.a = 0f;
        textComponent.color = finalColor;

        // 텍스트 게임 오브젝트 비활성화
        textComponent.gameObject.SetActive(false);

        // UI 움직임 플래그 설정
        start_Ui.canMove = true;
    }
}
