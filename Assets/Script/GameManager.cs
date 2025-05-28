using Pathfinding;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Scriptable Object")]
    public LevelData[] allLevels;
    public LevelData levelData;

    [Header("Prefabs")]
    public GameObject bronzePrefab;
    public GameObject silverPrefab;
    public GameObject goldPrefab;
    public GameObject enemyPrefab;
    
    public GameObject playerPrefab;
    public GameObject winBoxPrefab;
    private static int currentLevelIndex ;

    private void Start()
    {
        levelData = allLevels[currentLevelIndex];
        GenerateLevel();
        AstarPath.active.Scan();
    }

    void GenerateLevel()
    {
        // Instantiate Bronze
       
            Instantiate(bronzePrefab, levelData.bronzePositions, Quaternion.identity);
        

       
            Instantiate(silverPrefab, levelData.silverPositions, Quaternion.identity);
        

        // Instantiate Gold
        
            Instantiate(goldPrefab, levelData.goldPositions, Quaternion.identity);
     

        // Instantiate Enemies
        
          GameObject enemy=  Instantiate(enemyPrefab, levelData.enemyPositions, Quaternion.identity);
        enemy.GetComponent<EnemyFollow>().waitForSecond = levelData.waitForSecond;
        enemy.GetComponent<AILerp>().speed = levelData.enemySpeed;

        // Instantiate Grid
        Instantiate(levelData.gridPrefab, levelData.gridPosition, Quaternion.identity);

        // Instantiate Player
        Instantiate(playerPrefab, levelData.playerPosition, Quaternion.Euler(levelData.playerRotation));

        // Instantiate WinBox
        Instantiate(winBoxPrefab, levelData.winBoxPosition, Quaternion.identity);
    }
    private int GetLevelIndex(LevelData level)
    {
        for (int i = 0; i < allLevels.Length; i++)
        {
            if (allLevels[i] == level)
                return i;
        }
        return 0;
    }

    public void LoadNextLevel()
    {
        currentLevelIndex = (currentLevelIndex + 1) % allLevels.Length; // Loop to 0 after last
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartLevel()
    {
        // Reload same level
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
