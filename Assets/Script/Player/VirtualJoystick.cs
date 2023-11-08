using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private RectTransform baseRect; // 조이스틱 배경의 RectTransform
    private RectTransform handleRect; // 조이스틱 핸들의 RectTransform
    private CanvasGroup canvasGroup; // 조이스틱의 CanvasGroup
    public Vector2 InputDirection { get; private set; } // 조이스틱의 입력 방향

    void Start()
    {
        baseRect = GetComponent<RectTransform>();
        handleRect = transform.GetChild(0).GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        InputDirection = Vector2.zero;

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0; // 시작할 때 조이스틱을 투명하게 만듭니다.
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransform canvasRect = baseRect.parent as RectTransform;
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, eventData.position, eventData.pressEventCamera, out localPointerPosition))
        {
            Vector2 direction = localPointerPosition - (Vector2)baseRect.localPosition;
            InputDirection = direction.normalized;
            // 이동 범위를 좁히기 위해 0.4f 대신 더 작은 값을 사용합니다. 예: 0.2f
            handleRect.anchoredPosition = (InputDirection * (baseRect.sizeDelta.x / 2f)) * 0.05f; 
            canvasGroup.alpha = 1;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransform canvasRect = baseRect.parent as RectTransform;
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, eventData.position, eventData.pressEventCamera, out localPointerPosition))
        {
            baseRect.localPosition = localPointerPosition; // baseRect의 로컬 위치를 업데이트합니다.
            handleRect.anchoredPosition = Vector2.zero; // 핸들 위치를 초기화합니다.
            canvasGroup.alpha = 1; // 조이스틱을 불투명하게 만듭니다.
            OnDrag(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InputDirection = Vector2.zero;
        handleRect.anchoredPosition = Vector2.zero;
        canvasGroup.alpha = 0; // 조이스틱을 다시 투명하게 만듭니다.
    }
    public void EnableJoystick(bool enable)
    {
        if (enable)
        {
            canvasGroup.alpha = 1; // 조이스틱을 불투명하게 만듭니다.
            canvasGroup.blocksRaycasts = true; // 조이스틱이 입력을 받을 수 있도록 합니다.
        }
        else
        {
            canvasGroup.alpha = 0; // 조이스틱을 투명하게 만듭니다.
            canvasGroup.blocksRaycasts = false; // 조이스틱이 입력을 받지 않도록 합니다.
        }
    }
}