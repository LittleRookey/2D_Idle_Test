using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using Sirenix.OdinInspector;
using DG.Tweening;

public class SpawnPoint : MonoBehaviour
{
    [InlineEditor]
    //[SerializeField] private MonsterTable monsterTable;
    [SerializeField] private int maxMonsterNum = 6;
    [SerializeField] private float spawnRange = 2.5f;
    [SerializeField] private float cellSize = 1f; // Size of each cell in the grid
    public bool isXZ; // 3D용
    [Range(0f, 360f)]
    public float XRotation;
    private int currentNumber;
    private List<Health> spawnedEnemy; // 소환된 몬스터들
    private bool[,] grid; // 2D grid to track occupied cells
    private int gridSize; // Size of the grid
    Dictionary<string, Pool<Health>> monsterPool;
    //Pool<Health> monsterPool;
    public bool startSpawn;
    public bool isLocked;
    [SerializeField] private float spawnTimer = 5;
    float timer = 0f;
    [SerializeField] private Health monsterToSpawn;
    [SerializeField] private SpriteRenderer spawnOutline;
    [SerializeField] private SpriteRenderer spawnEnemyIcon;

    [SerializeField] private Color enabledColor = new Color(0f, 0f, 0f, 255f);
    [SerializeField] private Color disabledColor = new Color(0f, 0f, 0f, 50f);

    private void OnValidate()
    {
        gridSize = Mathf.CeilToInt(spawnRange * 2f / cellSize);
        grid = new bool[gridSize, gridSize];
    }

    public void StartSpawn()
    {
        startSpawn = true;
        spawnOutline.DOColor(enabledColor, 0.5f);
        spawnEnemyIcon.DOColor(enabledColor, 0.5f);
    }
    public void StopSpawn()
    {
        startSpawn = false;
        spawnOutline.DOColor(disabledColor, 0.5f);
        spawnEnemyIcon.DOColor(disabledColor, 0.5f);
    }

    private void Awake()
    {
        timer = spawnTimer;
        currentNumber = 0;
        spawnedEnemy = new List<Health>();

        // Calculate the grid size based on the spawn range
        gridSize = Mathf.CeilToInt(spawnRange * 2f / cellSize);
        grid = new bool[gridSize, gridSize];

        monsterPool = new Dictionary<string, Pool<Health>>();
        //var monsters = monsterTable.GetAllMonsters();
        //for (int i = 0; i < monsters.Count; i++)
        //{
        //    var pool = Pool.Create<Health>(monsters[i]);
        //    pool.SetContainer(transform);
        //    monsterPool.Add(monsters[i].Name, pool);
        //}
        StopSpawn();
        if (monsterToSpawn != null)
        {
            SetSpawnPoint(monsterToSpawn);
        }
    }
    public void SetSpawnPoint(Health monster) 
    {
        Debug.Log($"SEt spawnpoint for monster {monster.Name}");
        this.monsterToSpawn = monster;
        SetMonsterPool(this.monsterToSpawn);

    }

    private void SetMonsterPool(Health monster)
    {
        if (monsterPool == null) monsterPool = new Dictionary<string, Pool<Health>>();

        var pool = Pool.Create<Health>(monster);
        pool.SetContainer(transform);
        if (!monsterPool.ContainsKey(monster.Name))
            monsterPool.Add(monster.Name, pool);
    }

    private void Update() 
    {
        if (isLocked) return;

        if (currentNumber < maxMonsterNum)
        {
            timer += Time.deltaTime;
            if (timer >= spawnTimer)
            {
                timer = 0f;
                startSpawn = true;
            }
        }
        if (!startSpawn) return;
        // Spawn monsters if the current number is less than the maximum
        if (currentNumber < maxMonsterNum && monsterToSpawn != null)
        {
            SpawnMonster(monsterToSpawn);
        } else
        {
            startSpawn = false;
        }
    }

    private void SpawnMonster(Health monsterToSpawn)
    {
        Vector3 spawnPosition = GetEmptyCell();
        if (spawnPosition != Vector3.zero)
        {
            // Spawn a monster at the empty cell
            Health monster = monsterPool[monsterToSpawn.Name].Get();
            
            // Set Stat based on its difficulty
            monster.GetComponent<StatContainer>().ApplyDifficulty(StageManager.Instance.GetCurrentDifficulty());
            // Set Skill


            // set spawn position
            monster.transform.position = GetRandomPositionInCell(spawnPosition) + transform.position;
            //monster.transform.GetChildCou
            //monster.transform.rotation = Quaternion.Euler(XRotation, 0f, 0f);
            // Set EnemyAI baseState
            monster.GetComponent<EnemyAI>().Init();
            
            monster.OnDeath.AddListener(OnMonsterDelayedDeath);
            spawnedEnemy.Add(monster);
            currentNumber++;

            // Mark the cell as occupied
            MarkCellOccupied(spawnPosition);
        }
    }

    private Vector3 GetEmptyCell()
    {
        // Try a maximum of 100 times to find an empty cell
        for (int i = 0; i < 100; i++)
        {
            int x = Random.Range(0, gridSize);
            int y = Random.Range(0, gridSize);
            if (!grid[x, y])
            {
                // Convert grid coordinates to local position
                float localX = (x - gridSize / 2) * cellSize;
                float localY = (y - gridSize / 2) * cellSize;
                if (isXZ) return new Vector3(localX, 0, localY);
                else return new Vector3(localX, localY, 0f);
            }
        }

        // No empty cell found
        return Vector3.zero;
    }

    private Vector3 GetRandomPositionInCell(Vector3 cellPosition)
    {
        Vector3 randomOffset = Random.insideUnitCircle * (cellSize / 2f);
        return cellPosition + randomOffset;
    }

    private void MarkCellOccupied(Vector3 localPosition)
    {
        int x = Mathf.RoundToInt((localPosition.x + spawnRange) / cellSize);
        int y = Mathf.RoundToInt((localPosition.y + spawnRange) / cellSize);
        if (isXZ) y = Mathf.RoundToInt((localPosition.z + spawnRange) / cellSize);
        grid[x, y] = true;
    }

    private void OnMonsterDelayedDeath(LevelSystem attacker)
    {
        
        StartCoroutine(OnMonsterDeath(attacker));
    }

    private IEnumerator OnMonsterDeath(LevelSystem attacker)
    {
        var monster = spawnedEnemy.Find((Health health) => health.IsDead);
        yield return new WaitForSeconds(1f);
        
        monsterPool[monster.Name].Take(monster);
        
        monster.OnDeath.RemoveListener(OnMonsterDelayedDeath);
        
        spawnedEnemy.Remove(monster);
        
        currentNumber--;

        DeselectOneGrid();
        //Vector3 monsterPosition = monster.transform.position - transform.position;
        //int x = Mathf.RoundToInt((monsterPosition.x + spawnRange) / cellSize);
        //int y = Mathf.RoundToInt((monsterPosition.y + spawnRange) / cellSize);
        //grid[x, y] = false;
    }

    private void DeselectOneGrid()
    {
        List<Vector2Int> occupiedCells = new List<Vector2Int>();

        // Find all occupied cells
        for (int i = 0; i < gridSize; i++)
        {
            for (int k = 0; k < gridSize; k++)
            {
                if (grid[i, k])
                {
                    occupiedCells.Add(new Vector2Int(i, k));
                }
            }
        }

        // If there are no occupied cells, return
        if (occupiedCells.Count == 0)
            return;

        // Select a random occupied cell
        int randomIndex = Random.Range(0, occupiedCells.Count);
        Vector2Int randomCell = occupiedCells[randomIndex];

        // Set the selected cell to false
        grid[randomCell.x, randomCell.y] = false;
    }


    public void DespawnAllMonsters()
    {
        for (int i = 0; i < spawnedEnemy.Count; i++)
        {
            var monster = spawnedEnemy[i];
            monsterPool[monster.Name].Take(monster);
            monster.OnDeath.RemoveListener(OnMonsterDelayedDeath);
            currentNumber--;

        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, spawnRange);

        // Draw the grid
        Gizmos.color = Color.yellow;
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Gizmos.DrawWireCube(GetWorldPosition(x, y) + transform.position, Vector3.one * (cellSize - 0.1f));
                if (grid[x, y])
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireCube(GetWorldPosition(x, y) + transform.position, Vector3.one * (cellSize - 0.1f));
                }
            }
        }
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        float localX = (x - gridSize / 2) * cellSize;
        float localY = (y - gridSize / 2) * cellSize;
        if (isXZ) return new Vector3(localX, 0f, localY);
        else return new Vector3(localX, localY, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartSpawn();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopSpawn();
        }
    }
}
