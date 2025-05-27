using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("Transforms")]
    public Vector3 playerPosition;
    public Vector3 playerRotation;
    public Vector3 enemyPositions;
    public Vector3 gridPosition;
    public Vector3 bronzePositions;
    public Vector3 silverPositions;
    public Vector3 goldPositions;
    public Vector3 winBoxPosition;

    [Header("Grid Level")]
    public GameObject gridPrefab;

    [Header("Level Status")]
    public int levelNo;
    public bool isLevelCompleted;
    public float waitForSecond;
    public float enemySpeed;
}
