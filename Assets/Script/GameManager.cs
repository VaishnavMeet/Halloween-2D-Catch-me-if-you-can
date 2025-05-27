using Pathfinding;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Scriptable Object")]
    public LevelData levelData;

    [Header("Prefabs")]
    public GameObject bronzePrefab;
    public GameObject silverPrefab;
    public GameObject goldPrefab;
    public GameObject enemyPrefab;
    
    public GameObject playerPrefab;
    public GameObject winBoxPrefab;


    private void Start()
    {
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
}
