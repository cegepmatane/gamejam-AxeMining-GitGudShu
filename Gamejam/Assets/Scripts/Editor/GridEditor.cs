using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor
{
    private Grid m_Grid;

    private void OnEnable()
    {
        m_Grid = (Grid)target;
    }

    private void OnSceneGUI()
    {
        if (Event.current.type == EventType.MouseDown && Event.current.control)
        {
            GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);

            Vector3 t_ClickPos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            Vector2Int t_GridPos = m_Grid.WorldToGrid(t_ClickPos);
            Vector3 t_WorldPos = m_Grid.GridToWorld(t_GridPos);

            // Validations
            if (m_Grid.SelectedTile >= m_Grid.AvailableTiles.Length)
                throw new GridException("Selected tile out of bound!");

            GameObject t_TilePrefab = m_Grid.AvailableTiles[m_Grid.SelectedTile];

            // Supprimer ancienne tuile si elle existe
            List<Tile> t_Tiles = m_Grid.GetComponentsInChildren<Tile>().ToList();
            Tile t_OldTile = t_Tiles.FirstOrDefault(t => t_GridPos == m_Grid.WorldToGrid(t.transform.position));

            if (t_OldTile)
                Undo.DestroyObjectImmediate(t_OldTile.gameObject);

            //Spawner la nouvelle tuile
            GameObject t_NewTile = (GameObject)PrefabUtility.InstantiatePrefab(t_TilePrefab, m_Grid.transform);
            Undo.RegisterCreatedObjectUndo(t_NewTile, "Tile created");
            t_NewTile.transform.position = t_WorldPos;

            Sprite t_Sprite = t_NewTile.GetComponent<SpriteRenderer>().sprite;
            float t_NewScale = m_Grid.CellSize / t_Sprite.bounds.size.x;
            t_NewTile.transform.localScale = new Vector3(t_NewScale, t_NewScale, t_NewScale);
        }
    }
}
