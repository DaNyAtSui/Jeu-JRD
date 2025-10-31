using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TilemapPuzzleManager : MonoBehaviour
{
    [Header("Tilemap de spots (cases où placer les blocs)")]
    public Tilemap spotsTilemap;

    [Header("Nombre de blocs requis pour résoudre le puzzle")]
    public int requiredBlocks = 3;

    [Header("Référence du script qui gère la porte")]
    public DoorController_U1 doorController_U1;

    [Tooltip("Tag des objets poussables")]
    public string blockTag = "PushBlock";

    private bool puzzleCompleted = false;

    void Update()
    {
        if (puzzleCompleted) return;

        int blocksOnSpots = CountBlocksOnSpots();

        Debug.Log("👉 Blocs détectés sur Spots: " + blocksOnSpots);

        if (blocksOnSpots >= requiredBlocks)
        {
            puzzleCompleted = true;
            Debug.Log("✅ Puzzle Résolu !");

            if (doorController_U1 != null)
            {
                Debug.Log("🔓 UnlockDoor() appelé !");
                doorController_U1.UnlockDoor();
            }
            else
            {
                Debug.LogWarning("⚠️ doorController_U1 est NULL dans le PuzzleManager !");
            }
        }
    }

    private int CountBlocksOnSpots()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag(blockTag);
        int count = 0;

        foreach (var block in blocks)
        {
            Vector3Int cellPos = spotsTilemap.WorldToCell(block.transform.position);

            bool tileFound = spotsTilemap.HasTile(cellPos);
            Debug.Log($"🔍 Check Bloc '{block.name}' → Cell {cellPos} → Tile: {tileFound}");

            if (tileFound)
            {
                count++;
            }
        }

        return count;
    }

    // ─────────────────────────────────────────────────────────────
    // GIZMOS DEBUG AAA
    // ─────────────────────────────────────────────────────────────
    private void OnDrawGizmos()
    {
        if (spotsTilemap == null) return;

        // 🟩 Affiche toutes les tiles Spots en vert
        foreach (var pos in spotsTilemap.cellBounds.allPositionsWithin)
        {
            if (spotsTilemap.HasTile(pos))
            {
                Vector3 world = spotsTilemap.GetCellCenterWorld(pos);

#if UNITY_EDITOR
                Handles.color = Color.green;
                Handles.DrawWireCube(world, Vector3.one * 0.9f);
#else
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(world, Vector3.one * 0.9f);
#endif
            }
        }

        // 🔵 Affiche les blocs + 🟨 ceux posés sur Spots
        GameObject[] blocks = GameObject.FindGameObjectsWithTag(blockTag);
        int detected = 0;

        foreach (var block in blocks)
        {
            if (block == null) continue;

            Vector3Int cell = spotsTilemap.WorldToCell(block.transform.position);
            Vector3 cellWorld = spotsTilemap.GetCellCenterWorld(cell);
            bool onSpot = spotsTilemap.HasTile(cell);

            // 🔵 Cellule du bloc
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(cellWorld, Vector3.one * 1.05f);

            // 🟨 Si le bloc est sur un spot
            if (onSpot)
            {
                detected++;
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(cellWorld, Vector3.one * 1.2f);
            }
        }

#if UNITY_EDITOR
        // 🧠 Label récap
        Handles.color = Color.magenta;
        Handles.Label(transform.position + Vector3.up * 2f,
            $"Blocks on Spots: {detected}/{requiredBlocks}");
#endif
    }
}
