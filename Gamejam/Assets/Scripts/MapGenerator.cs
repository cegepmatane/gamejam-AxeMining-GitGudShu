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
    public int ennemyAmount = 4;

    private enum TILES { GROUND, WALL, ORE, STAIRS}
    private TILES[,] _tiles;
    private Vector2Int[] _directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
    private int _spawnPosX;
    private int _spawnPosY;
    private int _spawnRadius = 3;

    [SerializeField] private Grid _grid;
    [SerializeField] private GameObject _groundTile;
    [SerializeField] private GameObject _wallTile;
    [SerializeField] private GameObject _oreTile;
    [SerializeField] private GameObject _stairsTile;
    [SerializeField] private GameObject _miner;
    [SerializeField] private GameObject _bomb;
    

    void Awake()
    {
        _grid.RowCount = (uint)Random.Range(minRow, maxRow);
        _grid.ColumnCount = (uint)Random.Range(minCol, maxCol);
        _tiles = new TILES[_grid.RowCount, _grid.ColumnCount];

        for (int y = 0; y < _grid.RowCount; y++) {
            for (int x = 0; x < _grid.ColumnCount; x++) {
                _tiles[y, x] = TILES.WALL;
            }
        }

        _spawnPosX = Random.Range(1, (int)_grid.ColumnCount - 1);
        _spawnPosY = Random.Range(1, (int)_grid.RowCount - 1);
        _tiles[_spawnPosY, _spawnPosX] = TILES.GROUND;

        GenerateGroundAround(_spawnPosX, _spawnPosY, true);
        GenerateOres();
        GenerateStairs();
        PlaceTiles();

        _miner.transform.position = _grid.GridToWorld(new Vector2Int(_spawnPosX, _spawnPosY));

        SpawnEnnemy();
    }

    void PlaceTile(GameObject a_tile, int a_x, int a_y) {
        Vector3 t_worldPos = _grid.GridToWorld(new Vector2Int(a_x, a_y));

        GameObject t_newTile = Instantiate(a_tile, _grid.transform);
        t_newTile.transform.position = t_worldPos;
        Sprite t_sprite = t_newTile.GetComponent<SpriteRenderer>().sprite;
        float t_newScale = _grid.CellSize / t_sprite.bounds.size.x;
        t_newTile.transform.localScale = new Vector3(t_newScale, t_newScale, t_newScale);

        Tile t_tile = t_newTile.GetComponent<Tile>();
        t_tile.x = (uint)a_x;
        t_tile.y = (uint)a_y;
    }

    void GenerateGroundAround(int a_x, int a_y, bool a_isFirstGen = false) {
        List<Vector2Int> t_validTiles = new List<Vector2Int>();
        bool t_isGroundPlace = false;
        foreach (Vector2Int t_direction in _directions) {
            int t_x = a_x + t_direction.x;
            int t_y = a_y + t_direction.y;

            if (_tiles[t_y, t_x] == TILES.WALL && t_x != 0 && t_x != _grid.ColumnCount - 1 && t_y != 0 && t_y != _grid.RowCount - 1) {
                if (((t_direction == Vector2Int.up || t_direction == Vector2Int.down) && _tiles[t_y, t_x - 1] == TILES.WALL && _tiles[t_y, t_x + 1] == TILES.WALL) || ((t_direction == Vector2Int.left || t_direction == Vector2Int.right) && _tiles[t_y - 1, t_x] == TILES.WALL && _tiles[t_y +1, t_x] == TILES.WALL))
                    t_validTiles.Add(new Vector2Int(t_x, t_y));
            }
        }

        foreach (Vector2Int tile in t_validTiles) {
            if (Random.Range(1, chanceOfGround) == 1) {
                _tiles[tile.y, tile.x] = TILES.GROUND;
                GenerateGroundAround(tile.x, tile.y);
                t_isGroundPlace = true;
            }
        }

        if (!t_isGroundPlace && t_validTiles.Count != 0) {
            Vector2Int t_tile = t_validTiles[Random.Range(0, t_validTiles.Count - 1)];
            _tiles[t_tile.y, t_tile.x] = TILES.GROUND;
            GenerateGroundAround(t_tile.x, t_tile.y);
            if (a_isFirstGen) { 
                t_validTiles.Remove(t_tile);
                t_tile = t_validTiles[Random.Range(0, t_validTiles.Count - 1)];
                _tiles[t_tile.y, t_tile.x] = TILES.GROUND;
                GenerateGroundAround(t_tile.x, t_tile.y);
            }
        }
    }

    void GenerateOres() {
        List<Vector2Int> t_validTiles = new List<Vector2Int>();
        for (int y = 0; y < _grid.RowCount; y++) {
            for (int x = 0; x < _grid.ColumnCount; x++) {
                if (_tiles[y, x] == TILES.WALL) {
                    foreach (Vector2Int t_direction in _directions) {
                        int t_x = x + t_direction.x;
                        int t_y = y + t_direction.y;
                        if (t_x >= 0 && t_x < _grid.ColumnCount && t_y >= 0 && t_y < _grid.RowCount) {
                            if (_tiles[t_y, t_x] == TILES.GROUND) {
                                t_validTiles.Add(new Vector2Int(x, y));
                                break;
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < oreAmount; i++) {
            if (t_validTiles.Count == 0) {
                oreAmount = i + 1;
                break;
            }
            Vector2Int t_tile = t_validTiles[Random.Range(0, t_validTiles.Count - 1)];
            _tiles[t_tile.y, t_tile.x] = TILES.ORE;
            t_validTiles.Remove(t_tile);
        }
        _grid.oreAmount = oreAmount;
    }
    void GenerateStairs()
    {
        bool t_findGoodTile = false;
        do
        {
            int t_x =(int)Random.Range(1, _grid.ColumnCount - 1);
            int t_y = (int)Random.Range(1, _grid.RowCount - 1);
            if(_tiles[t_y, t_x] == TILES.GROUND)
            {
                _tiles[t_y, t_x] = TILES.STAIRS;
                t_findGoodTile = true;
            }
        } while (!t_findGoodTile);
    }

    void SpawnEnnemy() {

        List<Vector2Int> t_validTiles = new List<Vector2Int>();
        for (int y = 0; y < _grid.RowCount; y++) {
            for (int x = 0; x < _grid.ColumnCount; x++) {
                if (_tiles[y, x] == TILES.GROUND && Mathf.Abs(x - _spawnPosX) >= _spawnRadius && Mathf.Abs(y - _spawnPosY) >= _spawnRadius) {
                    t_validTiles.Add(new Vector2Int(x, y));
                }
            }
        }

        for (int i = 0; i < ennemyAmount; i++) {
            if (t_validTiles.Count == 0) break;
            Vector2Int t_tile = t_validTiles[Random.Range(0, t_validTiles.Count - 1)];
            GameObject t_ennemy = Instantiate(_bomb);
            t_ennemy.GetComponent<Ennemy>().grid = _grid;
            t_ennemy.GetComponent<Ennemy>().Objective = _miner.transform;
            t_ennemy.GetComponent<Ennemy>().Pathfinder = _grid.GetComponent<Pathfinder>();
            t_ennemy.transform.position = _grid.GridToWorld(t_tile);
            t_validTiles.Remove(t_tile);
        }
    }

    void PlaceTiles() {
        for (int y = 0; y < _grid.RowCount; y++) {
            for (int x = 0; x < _grid.ColumnCount; x++) {
                switch (_tiles[y, x]) {
                    case TILES.WALL:
                        PlaceTile(_wallTile, x, y);
                        break;
                    case TILES.GROUND:
                        PlaceTile(_groundTile, x, y);
                        break;
                    case TILES.ORE:
                        PlaceTile(_oreTile, x, y);
                        break;
                    case TILES.STAIRS:
                        PlaceTile(_stairsTile, x, y);
                        break;
                    default:
                        break;
                }
            }
        }
        _grid.tiles = _grid.GetComponentsInChildren<Tile>().ToList();
    }

}
