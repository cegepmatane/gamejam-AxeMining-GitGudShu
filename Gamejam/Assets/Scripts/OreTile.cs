using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreTile : Tile
{
    [SerializeField] private GameObject _empty;

    public void Replace() {
        Grid t_grid = GetComponentInParent<Grid>();
        t_grid.tiles.Remove(this);
        Destroy(gameObject);

        GameObject t_wall = Instantiate(_empty, t_grid.transform);
        t_wall.transform.position = t_grid.GridToWorld(new Vector2Int((int)x, (int)y));
        Sprite t_sprite = t_wall.GetComponent<SpriteRenderer>().sprite;
        float t_newScale = t_grid.CellSize / t_sprite.bounds.size.x;
        t_wall.transform.localScale = new Vector3(t_newScale, t_newScale, t_newScale);
        Tile t_wallTile = t_wall.GetComponent<Tile>();
        t_wallTile.x = x;
        t_wallTile.y = y;
        t_grid.tiles.Add(t_wallTile);
    }
}
