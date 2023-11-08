using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
public class start_Ui : MonoBehaviour
{   
    public Image image1;
    public Image image2;
    public TextMeshProUGUI tmpText;

    public float moveSpeed = 1.0f;
    public float moveDistance = 3.0f;
    public static bool canMove = false;
    private bool imagesAreMoving = false; // 추가된 변수: 이미지가 움직이고 있는지 확인
    private bool imagesHaveMoved = false;
    private bool isReadyForInput = false;
    private bool inputReceived = false;

    private void Update()
    {
        if (canMove && !imagesAreMoving)
        {
            canMove = false;
            StartCoroutine(MoveImages());
        }

        if (imagesAreMoving) // 이미지가 움직이고 있을 때만 터치 인식
        {
            #if UNITY_EDITOR
            if (Mouse.current.leftButton.isPressed)
            {
                moveSpeed = 4.0f;
            }
            #else
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                moveSpeed = 4.0f;
            }
            #endif
        }

        if (imagesHaveMoved && !inputReceived)
        {
            #if UNITY_EDITOR
            if (Mouse.current.leftButton.isPressed)
            {
                inputReceived = true;
                StartCoroutine(BlinkTextMeshPro());
            }
            #else
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                inputReceived = true;
                StartCoroutine(BlinkTextMeshPro());
            }
            #endif
        }
    }

    IEnumerator MoveImages()
    {
        imagesAreMoving = true;

        Vector3 image1TargetPosition = image1.transform.position - new Vector3(0, moveDistance, 0);
        Vector3 image2TargetPosition = image2.transform.position + new Vector3(0, moveDistance, 0);

        Vector3 image1StartPosition = image1.transform.position;
        Vector3 image2StartPosition = image2.transform.position;

        float journeyLength1 = Vector3.Distance(image1StartPosition, image1TargetPosition);
        float journeyLength2 = Vector3.Distance(image2StartPosition, image2TargetPosition);

        float startTime = Time.time;

        while (true)
        {
            float distanceCovered1 = (Time.time - startTime) * moveSpeed;
            float distanceCovered2 = (Time.time - startTime) * moveSpeed;

            float fractionOfJourney1 = distanceCovered1 / journeyLength1;
            float fractionOfJourney2 = distanceCovered2 / journeyLength2;

            image1.transform.position = Vector3.Lerp(image1StartPosition, image1TargetPosition, fractionOfJourney1);
            image2.transform.position = Vector3.Lerp(image2StartPosition, image2TargetPosition, fractionOfJourney2);

            if (fractionOfJourney1 >= 1.0f && fractionOfJourney2 >= 1.0f)
            {
                break;
            }

            yield return null;
        }

        imagesHaveMoved = true;
        imagesAreMoving = false;
    }

    IEnumerator BlinkTextMeshPro()
    {
        float blinkTime = 2.0f;
        float elapsedTime = 0f;

        while (elapsedTime < blinkTime)
        {
            tmpText.enabled = !tmpText.enabled;
            yield return new WaitForSeconds(0.2f);
            elapsedTime += 0.2f;
        }

        tmpText.enabled = true;
        SceneManager.LoadScene("Main_menu");
    }
}