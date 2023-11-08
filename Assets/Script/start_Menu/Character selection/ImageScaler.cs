using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageScaler : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;

    public Image image1;
    public Image image2;
    public Image image3;

    public Vector2 enlargedScale = new Vector2(1.5f, 1.5f);
    private Vector2 originalScale;

    private void Start()
    {
        // 초기 이미지 스케일 저장
        originalScale = image1.rectTransform.localScale;

        // 버튼 클릭 이벤트에 함수 연결
        button1.onClick.AddListener(() => ScaleImage(image1));
        button2.onClick.AddListener(() => ScaleImage(image2));
        button3.onClick.AddListener(() => ScaleImage(image3));

        // 처음 시작 시 button2가 눌러진 것처럼 설정
        button2.Select(); // button2를 선택한 상태로 만듦
        ScaleImage(image2); // image2를 확대
    }

    public void ScaleImage(Image targetImage)
    {
        // 모든 이미지 크기를 초기화
        ResetAllImageScales();

        // 대상 이미지 크기 확대
        targetImage.rectTransform.localScale = enlargedScale;
    }

    public void ResetAllImageScales()
    {
        image1.rectTransform.localScale = originalScale;
        image2.rectTransform.localScale = originalScale;
        image3.rectTransform.localScale = originalScale;
    }
}
