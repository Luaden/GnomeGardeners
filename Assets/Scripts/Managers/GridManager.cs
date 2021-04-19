using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using TilemapExtensions;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Grid gridMap;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private int halfMapSize;
    [SerializeField] private List<GroundTileAssociation> groundTiles;

    private List<GridCell> gridCells = new List<GridCell>();
    private GridCell targetCell;
    private TilePaletteObject targetTilePalette;

    public List<GridCell> GridCells { get => gridCells; }

    #region Unity Methods

    private void Awake()
    {
        if (gridMap == null)
            gridMap = GetComponent<Grid>();
        if (groundTilemap == null)
            throw new System.NotImplementedException("Did not assign the tilemap.");
    }

    private void Start()
    {
        CreateTileMap(halfMapSize);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns Vector3Ints on the grid map that are -1 and +1 x and y from the origin.
    /// </summary>
    /// <param name="origin"></param>
    /// <returns></returns>
    public List<GridCell> GetNeighborCells(Vector3Int origin, int checkDistance = 1)
    {
        List<GridCell> neighbors = new List<GridCell>();

        for (int i = origin.x - checkDistance; i <= origin.x + checkDistance; i++)
            for(int j = origin.y - checkDistance; j <= origin.y + checkDistance; j++)
            {
                Vector3Int checkGrid = new Vector3Int(i, j, 0);

                if (checkGrid == origin)
                    continue;

                foreach (GridCell cell in gridCells)
                    if (checkGrid == cell.GridPosition)
                    {
                        neighbors.Add(cell);
                        break;
                    }
            }
        
        return neighbors;
    }

    public void ChangeTile(Vector3Int gridPosition, GroundType groundType)
    {
        AssignTargetCell(gridPosition);

        if(targetCell == null)
        {
            Debug.Log("Could not find target position in cell list.");
            return;
        }

        targetTilePalette = null;

        foreach(GroundTileAssociation groundTileAssociation in groundTiles)
            if(groundType == groundTileAssociation.groundType)
            {
                targetTilePalette = groundTileAssociation.tilePalette;
                break;
            }
        
        if(targetTilePalette == null)
        {
            Debug.Log("Could not find target tile palette associated with Ground Type.");
            return;
        }


        PaintTile(gridPosition, targetCell.MapPosition, targetTilePalette);
        targetCell.GroundType = groundType;
    }

    public void ChangeTileOccupant(Vector3Int gridPosition, IInteractable interactableOccupant)
    {
        AssignTargetCell(gridPosition);

        if (targetCell == null)
        {
            Debug.Log("Could not find target position in cell list.");
            return;
        }

        targetCell.CellOccupant = interactableOccupant;
    }

    public Vector3Int GetClosestGrid(Vector3 origin)
    {
        targetCell = null;

        foreach(GridCell cell in gridCells)
        {
            if (targetCell == null || Vector3.Distance(cell.WorldPosition, origin) <= Vector3.Distance(targetCell.WorldPosition, origin))
                targetCell = cell;
        }

        return targetCell.GridPosition;
    }

    public GridCell GetClosestCell(Vector3 origin)
    {
        targetCell = null;

        foreach (GridCell cell in gridCells)
        {
            if (targetCell == null || Vector3.Distance(cell.WorldPosition, origin) <= Vector3.Distance(targetCell.WorldPosition, origin))
                targetCell = cell;
        }

        return targetCell;
    }

    public GridCell GetGridCell(Vector3Int gridPosition)
    {
        foreach (GridCell cell in gridCells)
            if (cell.GridPosition == gridPosition)
                return cell;

        return null;
    }

    #endregion

    #region Private Methods

    private void AssignTargetCell (Vector3Int gridPosition)
    {
        targetCell = null;

        foreach (GridCell cell in gridCells)
        {
            if (cell.GridPosition == gridPosition)
            {
                targetCell = cell;
                break;
            }
        }
    }

    #endregion

    #region MapGeneration
    private void CreateTileMap(int mapSize)
    {
        for (int i = -mapSize; i <= mapSize; i++)
            for (int j = -mapSize; j <= mapSize; j++)
            {
                Vector3Int gridPosition = new Vector3Int(i, j, 0);
                Vector3 worldPosition = gridMap.CellToWorld(gridPosition);

                GridCell cell = CreateCell(gridPosition, worldPosition, groundTiles[0].groundType, mapSize);
                gridCells.Add(cell);

                PaintTile(cell.GridPosition, cell.MapPosition, groundTiles[0].tilePalette);
            }
    }

    private GridCell CreateCell(Vector3Int gridPosition, Vector3 worldPosition, GroundType typeOfGround, int mapSize)
    {
        return new GridCell(gridPosition, worldPosition, typeOfGround, GetMapPosition(gridPosition, mapSize), null);
    }

    private MapPosition GetMapPosition(Vector3Int gridPosition, int mapSize)
    {

        if (gridPosition.x == mapSize && gridPosition.y == mapSize)
            return MapPosition.TopRight;

        else if (gridPosition.x == mapSize && gridPosition.y == -mapSize)
            return MapPosition.BottomRight;

        else if (gridPosition.x == -mapSize && gridPosition.y == mapSize)
            return MapPosition.TopLeft;

        else if (gridPosition.x == -mapSize && gridPosition.y == -mapSize)
            return MapPosition.BottomLeft;

        else if (gridPosition.x == mapSize)
            return MapPosition.Right;

        else if (gridPosition.x == -mapSize)
            return MapPosition.Left;

        else if (gridPosition.y == mapSize)
            return MapPosition.TopMiddle;

        else if (gridPosition.y == -mapSize)
            return MapPosition.BottomMiddle;

        return MapPosition.Middle;
    }

    private void PaintTile(Vector3Int gridPosition, MapPosition mapPosition, TilePaletteObject tilePalette)
    {
        switch(mapPosition)
        {
            case MapPosition.TopLeft:
                groundTilemap.PaintTile(gridPosition, tilePalette.TopLeft);
                break;
            case MapPosition.TopMiddle:
                groundTilemap.PaintTile(gridPosition, tilePalette.TopMiddle);
                break;
            case MapPosition.TopRight:
                groundTilemap.PaintTile(gridPosition, tilePalette.TopRight);
                break;
            case MapPosition.Left:
                groundTilemap.PaintTile(gridPosition, tilePalette.Left);
                break;
            case MapPosition.Middle:
                groundTilemap.PaintTile(gridPosition, tilePalette.Middle);
                break;
            case MapPosition.Right:
                groundTilemap.PaintTile(gridPosition, tilePalette.Right);
                break;
            case MapPosition.BottomLeft:
                groundTilemap.PaintTile(gridPosition, tilePalette.BottomLeft);
                break;
            case MapPosition.BottomMiddle:
                groundTilemap.PaintTile(gridPosition, tilePalette.BottomMiddle);
                break;
            case MapPosition.BottomRight:
                groundTilemap.PaintTile(gridPosition, tilePalette.BottomRight);
                break;
        }
    }
    #endregion
}
