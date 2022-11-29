using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class FloatingIslandSpawnManager : MonoBehaviour
{

    public GameObject tempFloatingIslandPrefab;

    [Header("Main Island")] public int mainIslandScale;

    [Header("Child Islands")]
    [SerializeField]
    private int pieceCount, radius, minSpawnRange, maxSpawnRange, minHeightRange, maxHeightRange, minScale, maxScale, scale;
    
    [SerializeField]
    private Vector3 centerPos;

// Start is called before the first frame update
    void OnEnable()
    {
        
        GameObject tempFloatingIsland = Instantiate(tempFloatingIslandPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        tempFloatingIsland.transform.localScale = new Vector3(mainIslandScale, mainIslandScale, mainIslandScale);


        InstantiateCircle();
    }
    
    void InstantiateCircle() 
    {
        float angle = 360f / (float)pieceCount;
        for (int i = 0; i < pieceCount; i++)
        {
            Quaternion rotation = Quaternion.AngleAxis(i * angle, Vector3.up);
            Vector3 direction = rotation * Vector3.forward;

            centerPos = new Vector3(0, Random.Range(minHeightRange, maxHeightRange), 0);
            radius = Random.Range(minSpawnRange, maxSpawnRange);
            scale = Random.Range(minScale, maxScale);
            
            Vector3 position = centerPos + (direction * radius);
            GameObject tempSubFloatingIsland = Instantiate(tempFloatingIslandPrefab, position, rotation);
            tempSubFloatingIsland.transform.localScale = new Vector3(scale, scale, scale);;
        }
    }
}
