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

    private int[,] _tiles;

    [SerializeField] private Grid _grid;
    [SerializeField] private GameObject _groundTile;
    [SerializeField] private GameObject _wallTile;

    // Start is called before the first frame update
    void Start()
    {
        _grid.RowCount = (uint)Random.Range(minRow, maxRow);
        _grid.ColumnCount = (uint)Random.Range(minCol, maxCol);
        _tiles = new int[_grid.ColumnCount, _grid.RowCount];


        for (int x = 0; x < _grid.ColumnCount; x++) {
            for (int y = 0; y < _grid.RowCount; y++) {
                _tiles[x, y] = 0;
            }
        }

        int t_spawnPosX = Random.Range(1, (int)_grid.ColumnCount - 1);
        int t_spawnPosY = Random.Range(1, (int)_grid.RowCount - 1);
        _tiles[t_spawnPosX, t_spawnPosY] = 1;

        GenerateAround(t_spawnPosX, t_spawnPosY);

        PlaceTiles();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PlaceTile(GameObject a_tile, int x, int y) {
        Vector3 t_worldPos = _grid.GridToWorld(new Vector2Int(x, y));

        GameObject t_newTile = Instantiate(a_tile, _grid.transform);
        t_newTile.transform.position = t_worldPos;
        Sprite t_sprite = t_newTile.GetComponent<SpriteRenderer>().sprite;
        float t_newScale = _grid.CellSize / t_sprite.bounds.size.x;
        t_newTile.transform.localScale = new Vector3(t_newScale, t_newScale, t_newScale);

        Tile t_tile = a_tile.GetComponent<Tile>();
        t_tile.x = (uint)x;
        t_tile.y = (uint)y;
    }

    void GenerateAround(int x, int y) {
        Vector2Int[] t_directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        List<Vector2Int> t_validTiles = new List<Vector2Int>();
        bool t_isGroundPlace = false;
        foreach (Vector2Int t_direction in t_directions) {
            int t_x = x + t_direction.x;
            int t_y = y + t_direction.y;

            if (_tiles[t_x, t_y] == 0 && t_x != 0 && t_x != _grid.ColumnCount - 1 && t_y != 0 && t_y != _grid.RowCount - 1) {
                if (((t_direction == Vector2Int.up || t_direction == Vector2Int.down) && _tiles[t_x - 1, t_y] == 0 && _tiles[t_x + 1, t_y] == 0) || ((t_direction == Vector2Int.left || t_direction == Vector2Int.right) && _tiles[t_x, t_y - 1] == 0 && _tiles[t_x, t_y + 1] == 0))
                    t_validTiles.Add(new Vector2Int(t_x, t_y));
            }
        }

        foreach (Vector2Int tile in t_validTiles) {
            if (Random.Range(1, chanceOfGround) == 1) {
                _tiles[tile.x, tile.y] = 1;
                GenerateAround(tile.x, tile.y);
                t_isGroundPlace = true;
            }
        }

        if (!t_isGroundPlace && t_validTiles.Count != 0) {
            Vector2Int tile = t_validTiles[Random.Range(0, t_validTiles.Count - 1)];
            _tiles[tile.x, tile.y] = 1;
            GenerateAround(tile.x, tile.y);
        }
    }

    void PlaceTiles() {
        for (int x = 0; x < _grid.ColumnCount; x++) {
            for (int y = 0; y < _grid.RowCount; y++) {
                Debug.Log(x + ", " + y);
                switch (_tiles[x, y]) {
                    case 0:
                        PlaceTile(_wallTile, x, y);
                        break;
                    case 1:
                        PlaceTile(_groundTile, x, y);
                        break;
                    default:
                        break;
                }
            }
        }
        _grid.tiles = _grid.GetComponentsInChildren<Tile>().ToList();
    }

}
