using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Monster : MonoBehaviour
{
    public Transform playerTransform;
    protected float speed = 2f;
    private Rigidbody2D rb;
    protected float health = 100f;
    public SpriteRenderer spriteRenderer;
    private GameManager gameManager;

    [Header("Avoidance Settings")]
    public float minAvoidForce = 0.2f;
    public float maxAvoidForce = 1.0f;
    public float detectionRadius = 2f;
    [Header("Despawn Settings")]
    public float despawnDistance = 20f;


    private Pathfinder pathfinder;
    private List<Node> path;
    private int targetIndex;
    public static event Action<Monster> OnMonsterDisabled;
    private void OnDisable()
    {
        // Monster 게임 오브젝트가 비활성화되거나 파괴될 때 이벤트를 발생시킵니다.
        OnMonsterDisabled?.Invoke(this);
    }
    private void Start()
    {
        pathfinder = GetComponent<Pathfinder>();
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!playerTransform)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        InitializeMonster();
        if (pathfinder != null && playerTransform != null)
        {
            FindPathToPlayer();
            StartCoroutine(FollowPath());
        }
    }

    protected virtual void Update()
    {
        Vector2 desiredVelocity = Seek(playerTransform.position) + AvoidOtherMonstersAndObstacles() + AlignWithOthers() + CohesionWithOthers();
        rb.velocity = desiredVelocity.normalized * speed;

        // 플레이어와의 거리 체크하여 파괴
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > despawnDistance & gameObject.tag !="Boss")
        {
            Destroy(gameObject);
        }

        // 플레이어를 바라보는 코드 (바라보는 방향을 바꿈)
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        if (directionToPlayer.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (directionToPlayer.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
        private void FindPathToPlayer()
        {
        if (pathfinder == null || playerTransform == null) 
        {
            Debug.LogError("Pathfinder or PlayerTransform is not set!");
            return;
        }

        Vector2Int startPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        Vector2Int targetPos = new Vector2Int(Mathf.RoundToInt(playerTransform.position.x), Mathf.RoundToInt(playerTransform.position.y));
        
        path = pathfinder.FindPath(startPos, targetPos);
        targetIndex = 0;
        }

    private IEnumerator FollowPath()
    {
        while(true)
        {
            if (path != null && targetIndex < path.Count)
            {
                Vector3 targetPosition = new Vector3(path[targetIndex].position.x, path[targetIndex].position.y, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                
                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    targetIndex++;
                }
            }
            else
            {
                FindPathToPlayer();  // Recalculate path when it ends or is null
            }

            yield return null;  // Wait for the next frame
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Attack")) 
        {
            WeaponDamage weapon = collision.GetComponent<WeaponDamage>();
            if (weapon != null) 
            {
                health -= weapon.damage;
                if (health <= 0) 
                {
                    GameManager gameManager = FindObjectOfType<GameManager>();
                    if (gameManager != null)
                    {
                        gameManager.IncrementKilledMonsters();
                    }
                    OnDeath(); // Call the new method here
                }
            }
        }
    }

    Vector2 Seek(Vector2 target)
    {
        return (target - (Vector2)transform.position).normalized;
    }

    Vector2 AvoidOtherMonsters()
    {
        Vector2 avoidanceForce = Vector2.zero;

        int monsterLayer = LayerMask.GetMask("Monsters");
        Collider2D[] nearbyMonsters = Physics2D.OverlapCircleAll(transform.position, detectionRadius, monsterLayer);

        foreach (var monster in nearbyMonsters)
        {
            if (monster.gameObject == this.gameObject) continue; // 자기 자신은 무시

            Vector2 avoidDir = (Vector2)transform.position - (Vector2)monster.transform.position;
            float distanceToMonster = avoidDir.magnitude;
            float randomizedAvoidForce = Mathf.Lerp(minAvoidForce, maxAvoidForce, UnityEngine.Random.value);
            float combinedAvoidForce = randomizedAvoidForce / (distanceToMonster * distanceToMonster);

            avoidanceForce += avoidDir.normalized * combinedAvoidForce;
        }

        return avoidanceForce;
    }

    Vector2 AlignWithOthers()
    {
        Vector2 averageHeading = Vector2.zero;
        int count = 0;

        int monsterLayer = LayerMask.GetMask("Monsters");
        Collider2D[] nearbyMonsters = Physics2D.OverlapCircleAll(transform.position, detectionRadius, monsterLayer);

        foreach (var monster in nearbyMonsters)
        {
            if (monster.gameObject == this.gameObject) continue; 

            averageHeading += (Vector2)monster.transform.up;
            count++;
        }

        if (count > 0)
        {
            averageHeading /= count;
            return averageHeading.normalized;
        }

        return Vector2.zero;
    }

    Vector2 CohesionWithOthers()
    {
        Vector2 centerOfMass = Vector2.zero;
        int count = 0;

        int monsterLayer = LayerMask.GetMask("Monsters");
        Collider2D[] nearbyMonsters = Physics2D.OverlapCircleAll(transform.position, detectionRadius, monsterLayer);

        foreach (var monster in nearbyMonsters)
        {
            if (monster.gameObject == this.gameObject) continue; 

            centerOfMass += (Vector2)monster.transform.position;
            count++;
        }

        if (count > 0)
        {
            centerOfMass /= count;
            return Seek(centerOfMass);
        }

        return Vector2.zero;
    }
    private void OnDestroy()
    {
        if (gameManager)
        {
            gameManager.DecreaseMonsterCount();
        }
    }
    protected virtual void InitializeMonster()
    {
        speed = 2f; // 모든 몬스터의 기본 속도
        health = 100f; // 모든 몬스터의 기본 체력
    }
    protected virtual void OnDeath()
    {
        OnMonsterDisabled?.Invoke(this);
        Destroy(gameObject);
    }
    Vector2 AvoidOtherMonstersAndObstacles()
    {
        Vector2 avoidanceForce = Vector2.zero;
        int layerMask = LayerMask.GetMask("Enemy", "Wall,Obj"); 
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, detectionRadius, layerMask);

        foreach (var obj in nearbyObjects)
        {
            if (obj.gameObject == this.gameObject) continue;

            Vector2 avoidDir = (Vector2)transform.position - (Vector2)obj.transform.position;
            float distanceToObject = avoidDir.magnitude;
            float randomizedAvoidForce = Mathf.Lerp(minAvoidForce, maxAvoidForce, UnityEngine.Random.value);
            float combinedAvoidForce = randomizedAvoidForce / (distanceToObject * distanceToObject);

            avoidanceForce += avoidDir.normalized * combinedAvoidForce;
        }

        return avoidanceForce;
    }



    public class Node
    {
        public bool walkable;
        public Vector3 position; // 이렇게 position을 추가합니다.
        public int gridX;
        public int gridY;

        public int gCost;
        public int hCost;
        public Node parent;

        public Node(bool _walkable, Vector3 _position, int _gridX, int _gridY)
        {
            walkable = _walkable;
            position = _position;
            gridX = _gridX;
            gridY = _gridY;
        }

        public int fCost
        {
            get { return gCost + hCost; }
        }
    }

public class Pathfinder : MonoBehaviour
{
    GridManager grid;

    void Awake()
    {
        grid = GetComponent<GridManager>();
    }

    public List<Node> FindPath(Vector2Int startPos, Vector2Int targetPos)
    {
        Node startNode = grid.nodes[startPos.x, startPos.y];
        Node targetNode = grid.nodes[targetPos.x, targetPos.y];

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost && openSet[i].hCost < node.hCost)
                {
                    node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            if (node == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbour in grid.GetNeighbours(node))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        return null; // No path found
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.RoundToInt(Mathf.Abs(nodeA.position.x - nodeB.position.x));
        int dstY = Mathf.RoundToInt(Mathf.Abs(nodeA.position.y - nodeB.position.y));


        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}

public class GridManager : MonoBehaviour
{
    public Node[,] nodes; 
    public Vector2 gridSize;
    public float nodeRadius;
    public LayerMask unwalkableMask;

    private void Awake()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        int gridCountX = Mathf.RoundToInt(gridSize.x / (nodeRadius * 2)); // X축 노드 개수
        int gridCountY = Mathf.RoundToInt(gridSize.y / (nodeRadius * 2)); // Y축 노드 개수

        nodes = new Node[gridCountX, gridCountY];

        Vector3 worldBottomLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.up * gridSize.y / 2; // 그리드의 왼쪽 하단 좌표 계산

        // 그리드 생성
        for (int x = 0; x < gridCountX; x++)
        {
            for (int y = 0; y < gridCountY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeRadius * 2 + nodeRadius) + Vector3.up * (y * nodeRadius * 2 + nodeRadius);
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask)); // 해당 위치에서 원 형태로 충돌 검사를 하여 걷기 가능한지 확인
                Vector2Int gridPosition = new Vector2Int(x, y);
                nodes[x, y] = new Node(walkable, worldPoint, x, y);// 노드 생성 후 배열에 추가
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.y, 1)); // 그리드 영역 표시

        if (nodes != null) // 그리드가 생성되면
        {
            foreach (Node n in nodes)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red; // 걷기 가능한 노드는 하얀색, 불가능한 노드는 빨간색으로 표시
                Gizmos.DrawCube(n.position, Vector3.one * (nodeRadius * 2 - .1f)); // 각 노드를 정사각형으로 그림
            }
        }
    }
        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                    {
                        neighbours.Add(nodes[checkX, checkY]);
                    }
                }
            }
            return neighbours;
        }
    }
}
