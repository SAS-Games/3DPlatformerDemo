using UnityEngine;
using UnityEngine.UI;

public class MinimapExploration : MonoBehaviour
{
    public RawImage MinimapImage;      // The minimap UI
    public Transform Player;          // Reference to the player
    public Texture2D BaseMapTexture;  // Background map texture
    public Vector2 WorldMin;          // Minimum coordinates of the world (e.g., bottom-left corner)
    public Vector2 WorldMax;          // Maximum coordinates of the world (e.g., top-right corner)
    public int GridResolution = 128; // Resolution of the visited grid

    private Texture2D visitedTexture;  // Visited texture
    private bool[,] visitedGrid;       // Visited grid
    private Vector2 cellSize;// Size of each grid cell

    void Start()
    {
        // Initialize visited texture and grid
        visitedTexture = new Texture2D(GridResolution, GridResolution, TextureFormat.RGBA32, false);
        visitedGrid = new bool[GridResolution, GridResolution];

        // Calculate the size of each grid cell
        Vector2 worldSize = WorldMax - WorldMin;
        cellSize = new Vector2(worldSize.x / GridResolution, worldSize.y / GridResolution);

        // Fill visited texture with black (unvisited)
        Color[] colors = new Color[GridResolution * GridResolution];
        for (int i = 0; i < colors.Length; i++) colors[i] = Color.black;
        visitedTexture.SetPixels(colors);
        visitedTexture.Apply();

        // Assign visited texture to RawImage
        MinimapImage.texture = visitedTexture;
    }

    void Update()
    {
        MarkVisitedArea();
    }

    private void MarkVisitedArea()
    {
        // Convert player position to normalized grid coordinates
        Vector2 playerPosition = Player.position;
        Vector2 normalizedPosition = (playerPosition - WorldMin); // Shift position to start from (0, 0)

        // Map normalized position to grid coordinates
        int x = Mathf.FloorToInt((normalizedPosition.x / (WorldMax.x - WorldMin.x)) * GridResolution);
        int y = Mathf.FloorToInt((normalizedPosition.y / (WorldMax.y - WorldMin.y)) * GridResolution);

        // Validate grid position
        if (x < 0 || x >= GridResolution || y < 0 || y >= GridResolution) return;

        // Mark the grid cell as visited
        if (!visitedGrid[x, y])
        {
            visitedGrid[x, y] = true;
            visitedTexture.SetPixel(x, y, Color.white);
            visitedTexture.Apply(); // Only apply when new areas are revealed
        }
    }
}
