using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public DualGridSystem dualGridSystem;


    // Update is called once per frame
    void Update()
    {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = GetWorldPosTile(mouseWorldPos);
        transform.position = tilePos + new Vector3(0.5f, 0.5f, 0);

        if (Input.GetMouseButton(0))
        {
            dualGridSystem.RenderTile(tilePos, dualGridSystem.dirtTile);
        }
        else if (Input.GetMouseButton(1))
        {
            dualGridSystem.RenderTile(tilePos, dualGridSystem.grassTile);
        }
    }

    public static Vector3Int GetWorldPosTile(Vector3 worldPos)
    {
        int xInt = Mathf.FloorToInt(worldPos.x);
        int yInt = Mathf.FloorToInt(worldPos.y);
        return new(xInt, yInt, 0);
    }
}
