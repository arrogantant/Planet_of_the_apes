using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapSelect : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public RectTransform stagesPanel;  // 모든 스테이지 이미지를 포함하는 패널
    public RectTransform[] stageImages; // 각 스테이지의 RectTransform 배열
    public string[] stageScenes;  // 각 스테이지의 씬 이름 배열
    private const string STAGE_PREFIX = "STAGE_CLEAR_";

    private Vector2 originalPosition;
    private Vector2 lastDrag;

    void Start()
    {
        originalPosition = stagesPanel.anchoredPosition;
    }

    void OnEnable()
    {
        UpdateStageImages();
    }

    void UpdateStageImages()
    {
        for (int i = 0; i < stageImages.Length; i++)
        {
            // 자식 오브젝트에서 모든 Image 컴포넌트를 가져옵니다.
            Image[] imageComponents = stageImages[i].GetComponentsInChildren<Image>();
            if (imageComponents.Length == 0)
            {
                Debug.LogError("Image 컴포넌트가 stageImages[" + i + "]의 자식에 없습니다!");
                continue;
            }

            // 스테이지가 클리어되었는지 여부를 확인
            bool stageCleared = PlayerPrefs.GetInt(STAGE_PREFIX + i, (i == 0 ? 1 : 0)) == 1;
            Color targetColor = stageCleared ? Color.white : new Color(1, 1, 1, 0.5f);

            // 모든 Image 컴포넌트의 색상을 설정합니다.
            foreach (Image image in imageComponents)
            {
                image.color = targetColor;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        stagesPanel.anchoredPosition += new Vector2(eventData.delta.x, 0);
        lastDrag = eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float closestDistance = float.MaxValue;
        RectTransform closestStage = null;

        Vector2 centerPosition = new Vector2(0, 0);

        foreach (RectTransform stage in stageImages)
        {
            float distance = Mathf.Abs(stagesPanel.anchoredPosition.x + stage.anchoredPosition.x);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestStage = stage;
            }
        }

        float offset = centerPosition.x - (stagesPanel.anchoredPosition.x + closestStage.anchoredPosition.x);
        stagesPanel.anchoredPosition += new Vector2(offset, 0);
    }

    public void StartButtonClicked()
    {
        Vector2 centerPosition = new Vector2(Screen.width / 2, Screen.height / 2);
        for (int i = 0; i < stageImages.Length; i++)
        {
            if (Mathf.Abs(centerPosition.x - stageImages[i].position.x) < stageImages[i].rect.width / 2)
            {
                if (PlayerPrefs.GetInt(STAGE_PREFIX + i, (i == 0 ? 1 : 0)) == 1)
                {
                    SceneManager.LoadScene(stageScenes[i]);
                }
                else
                {
                    Debug.Log("스테이지 " + i + "는 아직 잠겨있습니다.");
                }
                return;
            }
        }
    }

    public void ResetStageClearData()
    {
        for (int i = 0; i < stageScenes.Length; i++)
        {
            PlayerPrefs.DeleteKey(STAGE_PREFIX + i);
        }

        Debug.Log("모든 스테이지 클리어 데이터가 삭제되었습니다.");
        UpdateStageImages();
    }
}
