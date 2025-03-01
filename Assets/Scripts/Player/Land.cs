using UnityEngine;

public class Land : MonoBehaviour
{
    [SerializeField] private int[] sizeRows = new[] { 3, 4, 3 };
    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private float rowOffset = 0.8f;
    [SerializeField] private float columnOffset = 0.5f;

    private GameObject[,] _land;
    private LandCardManager[,] _landCardManagers;
    
    private void GenerateGrid()
    {
        int maxColumns = GetMaxColumns();
        _land = new GameObject[sizeRows.Length, maxColumns];
        _landCardManagers = new LandCardManager[sizeRows.Length, maxColumns];
        float squarePrefabWidth = Utils.GetGlobalBounds(squarePrefab).size.x;

        Transform tTransform = this.gameObject.GetComponent<Transform>();

        for (int row = 0; row < sizeRows.Length; row++)
        {
            float rowWidth = sizeRows[row];
            float totalWidthRow = rowWidth * squarePrefabWidth + (rowWidth - 1) * columnOffset;
            float startPoint = tTransform.position.x - (totalWidthRow / 2);
            

            for (int col = 0; col < sizeRows[row]; col++)
            {
                float x = startPoint + (squarePrefabWidth/2) + ((squarePrefabWidth) + columnOffset) * col;
                float y = tTransform.position.y - row * rowOffset;
                Vector3 position = new Vector3(x, y, 0);
                GameObject obj = Instantiate(squarePrefab, position, squarePrefab.transform.rotation, tTransform);
                obj.name = $"Cell_{row}_{col}";
                _land[row, col] = obj;
            }
        }
    }
    private int GetMaxColumns()
    {
        int maxColumns = 0;
        foreach (int cols in sizeRows)
        {
            if (cols > maxColumns)
            {
                maxColumns = cols;
            }
        }
        return maxColumns;
    }
    
    private void OnDrawGizmos()
    {
        if (squarePrefab == null || sizeRows == null || sizeRows.Length == 0)
            return;

        float squarePrefabWidth = Utils.GetGlobalBounds(squarePrefab).size.x;
        Transform tTransform = this.GetComponent<Transform>();

        for (int row = 0; row < sizeRows.Length; row++)
        {
            float rowWidth = sizeRows[row];
            float totalWidthRow = rowWidth * squarePrefabWidth + (rowWidth - 1) * columnOffset;
            float startPoint = tTransform.position.x - (totalWidthRow / 2);

            for (int col = 0; col < sizeRows[row]; col++)
            {
                float x = startPoint + (squarePrefabWidth / 2) + ((squarePrefabWidth) + columnOffset) * col;
                float y = tTransform.position.y - row * rowOffset;
                Vector3 position = new Vector3(x, y, 0);
                
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(position, new Vector3(squarePrefabWidth, squarePrefabWidth, squarePrefabWidth));
            }
        }
    }

    public void Init()
    {
        GenerateGrid();
    }

    public void UpdateLand(float deltaTime)
    {
        
    }
}
