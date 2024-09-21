using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;


[Serializable]
public class TerrainPrefab
{
    public string key;
    public Tile tile;
}

public struct TileData
{
    public Tile tile;
    public float rotaion_z;
}

public class DualGridSystem : MonoBehaviour
{
    /** 
     * dual grid: N1..N4 tiles in terrainTilemap, are neighbours of R tile in renderTilemap
     * |N1_R_N2|
     * |N3_R_N4|
     * depends on half offset of renderTilemap, if -0.5, then N2 has the same coordinates of R tile,
     * otherwis 0.5, then N3 has the same coordinates of R tile.
     */
    protected static Dictionary<string, Vector3Int> CORNER_COORDINATES = new Dictionary<string, Vector3Int>
    {
        {"TOP_RIGHT", new Vector3Int(0, 0, 0)},
        {"TOP_LEFT", new Vector3Int(1, 0, 0)},
        {"BOTTOM_LEFT", new Vector3Int(0, 1, 0)},
        {"BOTTOM_RIGHT", new Vector3Int(1, 1, 0)}
    };

    // Grid references
    public Tilemap terrainTilemap;
    public Tilemap renderTilemap;

    // Dirt and grass tiles references
    public Tile grassTile;
    public Tile dirtTile;

    // Match tiles
    // Terrain Matrix, 0 - presents grass, 1 - presents dirt  
    // Array of tiles to match 6 base casas: 0 - 1111, 1 - 0111, 2 - 0101, 3 - 0110, 4 - 0001, 5 - 0000
    public List<TerrainPrefab> terrainPrefabs;

    // Corresponding dictionary of terrainPrefabs in use 
    private Dictionary<string, TileData> terrainDictionary = new Dictionary<string, TileData>();


    // Start is called before the first frame update
    void Start()
    {
        SetUpTerrainDictionary();
        RenderMatchTiles();
    }

    private void SetUpTerrainDictionary()
    {
        foreach (var item in terrainPrefabs)
        {
            terrainDictionary.Add(item.key, new TileData { tile = item.tile, rotaion_z = 0f });
            // convert string to matrix
            Matrix<int> matrix = FromStringToMatrix(item.key);
            int count = Rotate90Count(matrix);
            if (count > 0)
            {
                // CounterClockwise 90 each time
                for (int i = 0; i < count; i++)
                {
                    matrix = matrix.Rotate90Clockwise();
                    terrainDictionary.Add(matrix.ToString(), new TileData { tile = item.tile, rotaion_z = 0 - (i+1) * 90.0f });
                }
            }
        }
    }

    private int GetTileTypeValueFromCoordinate(Vector3Int coords)
    {
        if (terrainTilemap.GetTile(coords) == grassTile)
        {
            return 0;
        }
        return 1;
    }

    private void RenderTilemap(Vector3Int coords)
    {
        // 4 neighbours, the demo half offset is -0.5, so coords presents top right neighbour
        int topRightNeighbour = GetTileTypeValueFromCoordinate(coords);
        int topLeftNeighbour = GetTileTypeValueFromCoordinate(coords - CORNER_COORDINATES["TOP_LEFT"]);
        int bottomRightNeighbour = GetTileTypeValueFromCoordinate(coords - CORNER_COORDINATES["BOTTOM_LEFT"]);
        int bottomLeftNeighbour = GetTileTypeValueFromCoordinate(coords - CORNER_COORDINATES["BOTTOM_RIGHT"]);
        // convert to binary string
        string str = $"{topLeftNeighbour}{topRightNeighbour}{bottomLeftNeighbour}{bottomRightNeighbour}";
        if (terrainDictionary.ContainsKey(str))
        {
            TileData tileData = terrainDictionary[str];
            renderTilemap.SetTile(coords, tileData.tile);
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, tileData.rotaion_z));
            // 得到新的变换矩阵
            renderTilemap.SetTransformMatrix(coords, rotationMatrix);
        }
    }

    public void RenderTile(Vector3Int coords, Tile tile)
    {
        terrainTilemap.SetTile(coords, tile);
        foreach (var item in CORNER_COORDINATES)
        {
            RenderTilemap(coords + item.Value);
        }
    }

    private void RenderMatchTiles()
    {
        for (int i = -50; i < 50; i++)
        {
            for (int j = -50; j < 50; j++)
            {
                RenderTilemap(new Vector3Int(i, j, 0));
            }
        }
    }

    public Matrix<int> FromStringToMatrix(string binaryString)
    {
        int length = binaryString.Length;
        int size = (int)Math.Sqrt(length);

        if (size * size != length)
            throw new ArgumentException("The string length must be a perfect square.");

        var matrix = new Matrix<int>(size, size);
        // 使用 Span<char> 提高性能
        var span = binaryString.AsSpan();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                // 直接将字符转换为整数
                matrix[i, j] = span[i * size + j] == '1' ? 1 : 0;
            }
        }
        return matrix;
    }

    // only works 2 * 2 matrix
    public int Rotate90Count(Matrix<int> matrix)
    {
        int[] data = matrix.GetData();
        int d1 = data[0];
        int d2 = data[1];
        int d3 = data[2];
        int d4 = data[3];
        if (d2 == d1 && d3 == d1 && d4 == d1)
        {
            return 0;
        }
        if (d4 == d1 && d2 == d3)
        {
            return 1;
        }
        return 3;
    }
}
