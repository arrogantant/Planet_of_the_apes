using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public GameObject[] monsterPrefabs;  // 몬스터 프리팹 배열
    public Transform playerTransform;    // 플레이어 Transform
    public float spawnInterval = 5f;     // 몬스터 생성 주기
    private bool isGameRunning = true;
    private int currentMonsterCount = 0;     // 현재 스폰된 몬스터의 수
    public int maxMonsterCount = 300;        // 최대 스폰 허용 몬스터 수
    public float minSpawnDistance = 10f; // 몬스터의 최소 스폰 거리
    public float maxSpawnDistance = 20f; // 몬스터의 최대 스폰 거리
    public GameObject bossPrefab; // 에디터에서 보스 몬스터의 프리팹을 할당합니다.
    private float gameTimer;
    private bool bossSpawned;
    public Text timerText; // 추가: 타이머 텍스트를 참조합니다.
    [SerializeField] private float startTime = 300f; 
    private bool isBlinking = false;
    private int killedMonstersCount = 0;
    public GameObject endGameUI;
    public GameObject gameOverUI;
    public int gold = 0; // 플레이어가 획득한 총 골드
    public static GameManager instance;
    private float initialStartTime;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // 새 씬에서 timerText를 찾아서 재할당
        AssignTimerText();
        initialStartTime = startTime;
    }
    private void Start()
    {
        // PlayerGoldManager에서 저장된 골드 값을 불러옵니다.
        PlayerGoldManager playerGoldManager = FindObjectOfType<PlayerGoldManager>();
        if (playerGoldManager != null)
        {
            playerGoldManager.LoadGold();
            gold = PlayerGoldManager.gold;
        }

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(SpawnMonsters());
        gameTimer = 0f;
        bossSpawned = false;
        UpdateTimerDisplay(); // 추가: 타이머 디스플레이를 초기화합니다.
        // GameManager의 gold 값을 PlayerGoldManager의 gold 값으로 설정합니다.
        gold = PlayerGoldManager.gold;
    }
    
    private void Update()
    {
        string currentScene = SceneManager.GetActiveScene().name; // 현재 씬 이름을 가져옵니다.

        if (currentScene == "Play") // 스테이지1의 로직
        {
            if (startTime > 0)
            {
                startTime -= Time.deltaTime;
            }
            else if (!bossSpawned)
            {
                SpawnBoss();
                bossSpawned = true;
            }
        }
        else if (currentScene == "Play2") // 스테이지2의 로직
        {
            if (killedMonstersCount >= 3 && !bossSpawned) // 이곳을 조절해서 몬스터처리 횟수를 조절!!
            {
                SpawnBoss();
                bossSpawned = true;
            }
        }
        if (currentScene == "Play3") // 스테이지3의 로직
        {
            if (startTime > 0)
            {
                startTime -= Time.deltaTime;
            }
            else if (!bossSpawned)
            {
                SpawnBoss();
                bossSpawned = true;
            }
        }
        if(timerText != null)
        {
            UpdateDisplay(); // UI를 업데이트하기 위해 UpdateDisplay를 호출합니다.
        }
    }
    void UpdateDisplay()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Play") // 스테이지1의 로직
        {
            int minutes = (int)(startTime / 60);
            int seconds = (int)(startTime % 60);
            timerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");

            if (startTime <= 0)
            {
                startTime = 0;
                timerText.color = new Color(timerText.color.r, timerText.color.g, timerText.color.b, 1);
                return;
            }

            if (startTime <= 10 && !isBlinking)
            {
                StartCoroutine(BlinkTimerText());
                isBlinking = true;
            }
        }
        else if (currentScene == "Play2") // 스테이지2의 로직
        {
            if (killedMonstersCount >= 3) // 이곳을 조절해서 몬스터처리 횟수를 조절!!
            {
                timerText.gameObject.SetActive(false); // 숫자가 3에 도달하면 Text를 비활성화합니다.
            }
            else
            {
                timerText.text = "Monster : " + (3 - killedMonstersCount).ToString(); // 이곳을 조절해서 몬스터처리 횟수의 UI를 조절!!
            }
        }
        else if (currentScene == "Play3") // 스테이지1의 로직
        {
            int minutes = (int)(startTime / 60);
            int seconds = (int)(startTime % 60);
            timerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");

            if (startTime <= 0)
            {
                startTime = 0;
                timerText.color = new Color(timerText.color.r, timerText.color.g, timerText.color.b, 1);
                return;
            }

            if (startTime <= 10 && !isBlinking)
            {
                StartCoroutine(BlinkTimerText());
                isBlinking = true;
            }
        }
    }
    private void AssignTimerText()
    {
        if (timerText != null) return;

        // 태그를 사용하여 timerText를 찾습니다.
        GameObject timerTextObject = GameObject.FindWithTag("TimerText");

        // 찾은 오브젝트가 null이 아닌지 확인하고, Text 컴포넌트를 할당합니다.
        if (timerTextObject != null)
        {
            timerText = timerTextObject.GetComponent<Text>();
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignTimerText();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        FindEndGameUI(); // EndGameUI 오브젝트를 찾는 메서드 호출
        FindGameOverUI();

        // 필요한 변수들을 초기화합니다.
        ResetGameStatus();

        // 몬스터 스폰 로직을 다시 시작합니다.
        RestartSpawning();
    }
    private void ResetGameStatus()
    {
        isGameRunning = true;
        startTime = initialStartTime; // 인스펙터에서 설정한 초기값을 사용합니다.
        bossSpawned = false;
        currentMonsterCount = 0;
        killedMonstersCount = 0;
        isBlinking = false;
    }

    private void RestartSpawning()
    {
        StopAllCoroutines(); // 모든 코루틴을 중지합니다.
        StartCoroutine(SpawnMonsters());
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    
    private void SpawnBoss()
    {
        // 일반 몬스터를 모두 제거합니다.
        var monsters = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var monster in monsters)
        {
            Destroy(monster);
        }

        // 보스 몬스터를 스폰합니다.
        Instantiate(bossPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    IEnumerator SpawnMonsters()
    {
        while (isGameRunning)
        {
            if(playerTransform == null) yield break;
            if (!bossSpawned) // bossSpawned가 false일 때만 몬스터 스폰
            {
                if (currentMonsterCount < maxMonsterCount)
                {
                    Vector3 playerPosition = playerTransform.position;
                    float randomRadius = Random.Range(minSpawnDistance, maxSpawnDistance);
                    float angle = Random.Range(0, 360);
                    Vector3 spawnDirection = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0).normalized;
                    Vector3 spawnPosition = playerPosition + spawnDirection * randomRadius;
                    GameObject selectedMonsterPrefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Length)];
                    Instantiate(selectedMonsterPrefab, spawnPosition, Quaternion.identity);
                    currentMonsterCount++;
                }
                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                yield return null; // bossSpawned가 true면 스폰을 중지하고 다음 프레임을 기다립니다.
            }
        }
    }

    public void StartGame()
    {
        isGameRunning = true;
        StartCoroutine(SpawnMonsters());
    }

    public void PauseGame()
    {
        isGameRunning = false;
        // 게임 일시정지 로직을 여기에 구현
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOver()
    {
        isGameRunning = false;
        
        // 게임 오버 UI를 활성화합니다.
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        VirtualJoystick virtualJoystick = FindObjectOfType<VirtualJoystick>();
        if (virtualJoystick != null)
        {
            virtualJoystick.EnableJoystick(false);
        }
        // 한 프레임 기다린 후 게임을 일시정지합니다.
        StartCoroutine(PauseAfterFrame());

    }
    private IEnumerator PauseAfterFrame()
    {
        // 한 프레임을 기다립니다.
        yield return null;

        // 플레이어의 움직임을 중지합니다.
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.SetCanMove(false);
        }

        // 게임을 일시정지합니다.
        Time.timeScale = 0;
    }
    
    public void DecreaseMonsterCount()
    {
        currentMonsterCount--;
    }
    void UpdateTimerDisplay()
    {
        if (timerText == null) return;
        int minutes = (int)(startTime / 60);
        int seconds = (int)(startTime % 60);
        timerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");

        if (startTime <= 0)
        {
            startTime = 0; // 추가: startTime이 음수가 되는 것을 방지합니다.
            timerText.color = new Color(timerText.color.r, timerText.color.g, timerText.color.b, 1); // 추가: 텍스트를 다시 보이게 합니다.
            return; // startTime이 0 이하이면 아무것도 하지 않고 반환합니다.
        }

        if (startTime <= 10 && !isBlinking)
        {
            StartCoroutine(BlinkTimerText());
            isBlinking = true;
        }
    }
    IEnumerator BlinkTimerText()
    {
        if(timerText == null) yield break;
        string currentScene = SceneManager.GetActiveScene().name;
        
        while (startTime <= 10 && startTime > 0 && currentScene == "Play") // "Play_2" 씬에서 깜빡이지 않도록 조건을 추가합니다.
        {
            timerText.color = new Color(timerText.color.r, timerText.color.g, timerText.color.b, 0);
            yield return new WaitForSeconds(0.5f);
            timerText.color = new Color(timerText.color.r, timerText.color.g, timerText.color.b, 1);
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void IncrementKilledMonsters()
    {
        killedMonstersCount++;
        if (Random.value <= 0.3f)
        {
            int goldAwarded = Random.Range(1, 4);
            gold += goldAwarded;

            // PlayerPrefs에 골드 값을 저장합니다.
            PlayerPrefs.SetInt("PlayerGold", gold);
            PlayerPrefs.Save();

            // PlayerGoldManager에 골드 값 업데이트
            PlayerGoldManager.gold = gold;
            Debug.Log($"Gold awarded: {goldAwarded}. Total gold: {gold}");

            // UI 갱신을 위한 메소드 호출
            if (PlayerGoldManager.instance != null)
            {
                PlayerGoldManager.instance.UpdateGoldText();
            }
        }
    }
    private void OnApplicationQuit()
    {
        PlayerGoldManager playerGoldManager = FindObjectOfType<PlayerGoldManager>();
        if (playerGoldManager != null)
        {
            playerGoldManager.SaveGold();
        }
    }
    public void OnBossDefeated()
    {
        if(endGameUI != null)
        {
            endGameUI.SetActive(true);
        }

        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.SetCanMove(false);
        }
        VirtualJoystick virtualJoystick = FindObjectOfType<VirtualJoystick>();
        if (virtualJoystick != null)
        {
            virtualJoystick.EnableJoystick(false);
        }
    }
    private void FindEndGameUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>(); // Canvas 오브젝트를 찾습니다.
        if (canvas != null)
        {
            Transform endGameUITransform = canvas.transform.Find("endGameUI");
            if (endGameUITransform != null)
            {
                endGameUI = endGameUITransform.gameObject;
            }
        }
    }
    private void FindGameOverUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>(); // Canvas 오브젝트를 찾습니다.
        if (canvas != null)
        {
            Transform gameOverUITransform = canvas.transform.Find("gameOverUI");
            if (gameOverUITransform != null)
            {
                gameOverUI = gameOverUITransform.gameObject;
            }
        }
    }
    public void ResumeGame()
    {
        // 게임 오버 UI를 비활성화합니다.
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        if (endGameUI != null)
        {
            endGameUI.SetActive(false);
        }

        // 플레이어의 움직임을 다시 허용합니다.
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.SetCanMove(true);
        }

        // 게임의 시간 흐름을 다시 정상으로 설정합니다.
        Time.timeScale = 1;

        // 게임 상태를 다시 실행 상태로 설정합니다.
        isGameRunning = true;
    }
}
