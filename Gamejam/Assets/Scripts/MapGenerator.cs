using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    public int minRow = 10;
    public int maxRow = 20;
    public int minCol = 10;
    public int maxCol = 20;
    public int chanceOfGround = 4;
    public int oreAmount = 10;

    private enum TILES { GROUND, WALL, ORE}
    private TILES[,] _tiles;
    private Vector2Int[] _directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    [SerializeField] private Grid _grid;
    [SerializeField] private GameObject _groundTile;
    [SerializeField] private GameObject _wallTile;
    [SerializeField] private GameObject _oreTile;

    // Start is called before the first frame update
    void Start()
    {
        _grid.RowCount = (uint)Random.Range(minRow, maxRow);
        _grid.ColumnCount = (uint)Random.Range(minCol, maxCol);
        _tiles = new TILES[_grid.ColumnCount, _grid.RowCount];


        for (int x = 0; x < _grid.ColumnCount; x++) {
            for (int y = 0; y < _grid.RowCount; y++) {
                _tiles[x, y] = TILES.WALL;
            }
        }

        int t_spawnPosX = Random.Range(1, (int)_grid.ColumnCount - 1);
        int t_spawnPosY = Random.Range(1, (int)_grid.RowCount - 1);
        _tiles[t_spawnPosX, t_spawnPosY] = TILES.GROUND;

        GenerateGroundAround(t_spawnPosX, t_spawnPosY, true);
        GenerateOres();
        PlaceTiles();
    }

    void PlaceTile(GameObject a_tile, int a_x, int a_y) {
        Vector3 t_worldPos = _grid.GridToWorld(new Vector2Int(a_x, a_y));

        GameObject t_newTile = Instantiate(a_tile, _grid.transform);
        t_newTile.transform.position = t_worldPos;
        Sprite t_sprite = t_newTile.GetComponent<SpriteRenderer>().sprite;
        float t_newScale = _grid.CellSize / t_sprite.bounds.size.x;
        t_newTile.transform.localScale = new Vector3(t_newScale, t_newScale, t_newScale);

        Tile t_tile = a_tile.GetComponent<Tile>();
        t_tile.x = (uint)a_x;
        t_tile.y = (uint)a_y;
    }

    void GenerateGroundAround(int a_x, int a_y, bool a_isFirstGen = false) {
        List<Vector2Int> t_validTiles = new List<Vector2Int>();
        bool t_isGroundPlace = false;
        foreach (Vector2Int t_direction in _directions) {
            int t_x = a_x + t_direction.x;
            int t_y = a_y + t_direction.y;

            if (_tiles[t_x, t_y] == TILES.WALL && t_x != 0 && t_x != _grid.ColumnCount - 1 && t_y != 0 && t_y != _grid.RowCount - 1) {
                if (((t_direction == Vector2Int.up || t_direction == Vector2Int.down) && _tiles[t_x - 1, t_y] == TILES.WALL && _tiles[t_x + 1, t_y] == TILES.WALL) || ((t_direction == Vector2Int.left || t_direction == Vector2Int.right) && _tiles[t_x, t_y - 1] == TILES.WALL && _tiles[t_x, t_y + 1] == TILES.WALL))
                    t_validTiles.Add(new Vector2Int(t_x, t_y));
            }
        }

        foreach (Vector2Int tile in t_validTiles) {
            if (Random.Range(1, chanceOfGround) == 1) {
                _tiles[tile.x, tile.y] = TILES.GROUND;
                GenerateGroundAround(tile.x, tile.y);
                t_isGroundPlace = true;
            }
        }

        if (!t_isGroundPlace && t_validTiles.Count != 0) {
            Vector2Int t_tile = t_validTiles[Random.Range(0, t_validTiles.Count - 1)];
            _tiles[t_tile.x, t_tile.y] = TILES.GROUND;
            GenerateGroundAround(t_tile.x, t_tile.y);
            if (a_isFirstGen) { 
                t_validTiles.Remove(t_tile);
                t_tile = t_validTiles[Random.Range(0, t_validTiles.Count - 1)];
                _tiles[t_tile.x, t_tile.y] = TILES.GROUND;
                GenerateGroundAround(t_tile.x, t_tile.y);
            }
        }
    }

    void GenerateOres() {
        List<Vector2Int> t_validTiles = new List<Vector2Int>();
        for (int x = 0; x < _grid.ColumnCount; x++) {
            for (int y = 0; y < _grid.RowCount; y++) {
                if (_tiles[x, y] == TILES.WALL) {
                    foreach (Vector2Int t_direction in _directions) {
                        int t_x = x + t_direction.x;
                        int t_y = y + t_direction.y;
                        if (t_x >= 0 && t_x < _grid.ColumnCount && t_y >= 0 && t_y < _grid.RowCount) {
                            if (_tiles[t_x, t_y] == TILES.GROUND) {
                                t_validTiles.Add(new Vector2Int(x, y));
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < oreAmount; i++) {
            Vector2Int t_tile = t_validTiles[Random.Range(0, t_validTiles.Count - 1)];
            _tiles[t_tile.x, t_tile.y] = TILES.ORE;
            t_validTiles.Remove(t_tile);
        }
    }

    void PlaceTiles() {
        for (int x = 0; x < _grid.ColumnCount; x++) {
            for (int y = 0; y < _grid.RowCount; y++) {
                switch (_tiles[x, y]) {
                    case TILES.WALL:
                        PlaceTile(_wallTile, x, y);
                        break;
                    case TILES.GROUND:
                        PlaceTile(_groundTile, x, y);
                        break;
                    case TILES.ORE:
                        PlaceTile(_oreTile, x, y);
                        break;
                    default:
                        break;
                }
            }
        }
        _grid.tiles = _grid.GetComponentsInChildren<Tile>().ToList();
    }

}
