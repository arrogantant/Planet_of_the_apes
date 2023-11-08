using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fate_Out : MonoBehaviour
{
    public Image targetImage; // 투명하게 만들 이미지를 연결.
    public float fadeDuration = 0.5f; // 페이드아웃에 걸리는 시간

    private void Start()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float startAlpha = targetImage.color.a;

        float rate = 1.0f / fadeDuration;
        float progress = 0.0f;

        while (progress < 1.0)
        {
            Color tempColor = targetImage.color;

            targetImage.color = new Color(tempColor.r, tempColor.g, tempColor.b, Mathf.Lerp(startAlpha, 0, progress));

            progress += rate * Time.deltaTime;

            yield return null;
        }

        targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, 0);

        // 페이드아웃이 끝나면 이미지를 비활성화.
        targetImage.gameObject.SetActive(false);
    }
}
