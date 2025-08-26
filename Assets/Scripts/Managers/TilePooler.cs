using System.Collections.Generic;
using UnityEngine;

public class TilePooler : MonoBehaviour
{
    [Header("Tile Pool Settings")]
    public GameObject tilePrefab;
    public int initialPoolSize = 64;

    private List<GameObject> pool = new List<GameObject>();

    void Awake()
    {
        // Pre-instantiate pool
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject tile = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity, transform);
            tile.SetActive(false);
            pool.Add(tile);
        }
    }

    // This is the method GridManager is calling
    public GameObject GetTile()
    {
        foreach (var tile in pool)
        {
            if (!tile.activeInHierarchy)
            {
                tile.SetActive(true);
                return tile;
            }
        }

        // Optional: Expand pool if all are in use
        GameObject newTile = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity, transform);
        newTile.SetActive(true);
        pool.Add(newTile);
        return newTile;
    }

    public void ReturnTile(GameObject tile)
    {
        tile.SetActive(false);
    }
}
