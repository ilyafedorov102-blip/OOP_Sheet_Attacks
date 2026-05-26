using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GridSystem2D : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Transform gridContainer;
    
    [Header("Object Settings")]
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color validColor = Color.green;
    [SerializeField] private Color invalidColor = Color.red;
    
    [Header("Keybindings")]
    [SerializeField] public KeyCode editModeKey = KeyCode.E;
    [SerializeField] public KeyCode deleteKey = KeyCode.Delete;
    
    // Режимы
    public enum GameMode { Edit, Play }
    public GameMode currentMode = GameMode.Edit;
    
    // Система сетки
    private Dictionary<Vector2Int, GridCell> gridCells = new Dictionary<Vector2Int, GridCell>();
    private GameObject currentlySelectedObject = null;
    private Vector2Int? lastValidPosition = null;
    private bool isDragging = false;
    private SpriteRenderer currentHighlight;
    
    private Camera mainCamera;
    
    [System.Serializable]
    public class GridCell
    {
        public Vector2Int gridPosition;
        public Vector2 worldPosition;
        public GameObject placedObject;
        public bool IsOccupied => placedObject != null;
        
        public GridCell(Vector2Int pos, Vector2 worldPos)
        {
            gridPosition = pos;
            worldPosition = worldPos;
            placedObject = null;
        }
    }
    
    void Start()
    {
        mainCamera = Camera.main;
        CreateGrid();
        CreateGridVisuals();
        UpdateModeUI();
    }
    
    void Update()
    {
        // Переключение режимов
        if (Input.GetKeyDown(editModeKey))
        {
            ToggleMode();
        }
        
        // Режим редактирования
        if (currentMode == GameMode.Edit)
        {
            HandleEditMode();
        }
        
        // Удаление объекта в режиме редактирования
        if (currentMode == GameMode.Edit && Input.GetKeyDown(deleteKey) && currentlySelectedObject != null)
        {
            DeleteSelectedObject();
        }
    }
    
    void HandleEditMode()
    {
        // Получаем позицию мыши в мировых координатах
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector2Int gridPos = WorldToGrid(mouseWorldPos);
        
        if (IsValidGridPosition(gridPos))
        {
            GridCell cell = gridCells[gridPos];
            
            // Визуальное выделение клетки
            HighlightCell(cell, cell.IsOccupied);
            
            // ЛКМ для установки/перемещения объекта
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
            {
                if (cell.IsOccupied)
                {
                    // Начинаем перемещение существующего объекта
                    currentlySelectedObject = cell.placedObject;
                    lastValidPosition = cell.gridPosition;
                    isDragging = true;
                    
                    // Временно убираем объект с клетки
                    cell.placedObject = null;
                    
                    // Добавляем компонент для следования за мышью
                    FollowMouse follow = currentlySelectedObject.GetComponent<FollowMouse>();
                    if (follow == null)
                        follow = currentlySelectedObject.AddComponent<FollowMouse>();
                    follow.enabled = true;
                }
                else
                {
                    // Создаем новый объект
                    CreateObjectAtCell(cell);
                }
            }
        }
        else
        {
            // Сбрасываем выделение при наведении на пустоту
            ResetHighlight();
        }
        
        // Обработка перетаскивания
        if (isDragging && currentlySelectedObject != null)
        {
            // Получаем текущую позицию мыши
            Vector3 currentMousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            currentMousePos.z = 0;
            Vector2Int dragGridPos = WorldToGrid(currentMousePos);
            
            if (IsValidGridPosition(dragGridPos))
            {
                GridCell targetCell = gridCells[dragGridPos];
                
                // Визуальная обратная связь на сетке
                ShowPlacementPreview(dragGridPos, targetCell.IsOccupied);
                
                // Отпускаем кнопку мыши
                if (Input.GetMouseButtonUp(0))
                {
                    if (targetCell.IsOccupied)
                    {
                        // Клетка занята - возвращаем объект на прежнее место
                        ReturnObjectToLastPosition();
                    }
                    else
                    {
                        // Перемещаем объект в новую клетку
                        Vector3 newPosition = GridToWorld(dragGridPos);
                        currentlySelectedObject.transform.position = newPosition;
                        targetCell.placedObject = currentlySelectedObject;
                        
                        // Обновляем позицию в компоненте GridObject
                        GridObject gridObj = currentlySelectedObject.GetComponent<GridObject>();
                        if (gridObj != null)
                            gridObj.UpdateGridPosition(dragGridPos);
                    }
                    
                    // Отключаем следование за мышью
                    FollowMouse follow = currentlySelectedObject.GetComponent<FollowMouse>();
                    if (follow != null)
                        follow.enabled = false;
                    
                    currentlySelectedObject = null;
                    lastValidPosition = null;
                    isDragging = false;
                    HidePlacementPreview();
                }
            }
        }
        
        // Отмена перемещения по правой кнопке мыши
        if (isDragging && Input.GetMouseButtonDown(1))
        {
            ReturnObjectToLastPosition();
            
            FollowMouse follow = currentlySelectedObject.GetComponent<FollowMouse>();
            if (follow != null)
                follow.enabled = false;
            
            isDragging = false;
            HidePlacementPreview();
        }
    }
    
    void CreateObjectAtCell(GridCell cell)
    {
        if (cell.IsOccupied) return;
        
        GameObject newObject = Instantiate(objectPrefab, cell.worldPosition, Quaternion.identity);
        newObject.transform.SetParent(gridContainer);
        
        // Добавляем компонент для возможности выделения
        GridObject gridObj = newObject.GetComponent<GridObject>();
        if (gridObj == null)
            gridObj = newObject.AddComponent<GridObject>();
        
        gridObj.Initialize(this, cell.gridPosition);
        
        // Настраиваем спрайт
        SpriteRenderer renderer = newObject.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = normalColor;
        }
        
        cell.placedObject = newObject;
    }
    
    void DeleteSelectedObject()
    {
        if (currentlySelectedObject == null) return;
        
        // Находим клетку с этим объектом и удаляем
        foreach (var cell in gridCells.Values)
        {
            if (cell.placedObject == currentlySelectedObject)
            {
                cell.placedObject = null;
                Destroy(currentlySelectedObject);
                currentlySelectedObject = null;
                break;
            }
        }
        
        ResetHighlight();
    }
    
    void ReturnObjectToLastPosition()
    {
        if (currentlySelectedObject != null && lastValidPosition.HasValue)
        {
            GridCell lastCell = gridCells[lastValidPosition.Value];
            currentlySelectedObject.transform.position = lastCell.worldPosition;
            lastCell.placedObject = currentlySelectedObject;
            
            // Обновляем позицию в компоненте
            GridObject gridObj = currentlySelectedObject.GetComponent<GridObject>();
            if (gridObj != null)
                gridObj.UpdateGridPosition(lastValidPosition.Value);
            
            currentlySelectedObject = null;
            lastValidPosition = null;
        }
    }
    
    void HighlightCell(GridCell cell, bool isOccupied)
    {
        if (currentHighlight != null)
        {
            Destroy(currentHighlight.gameObject);
        }
        
        // Создаем визуальный индикатор клетки
        GameObject highlight = new GameObject("CellHighlight");
        highlight.transform.position = cell.worldPosition;
        highlight.transform.SetParent(gridContainer);
        
        SpriteRenderer renderer = highlight.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateSquareSprite();
        renderer.color = isOccupied ? invalidColor : validColor;
        renderer.sortingOrder = 100;
        
        // Настраиваем размер
        highlight.transform.localScale = new Vector3(cellSize, cellSize, 1);
        
        currentHighlight = renderer;
    }
    
    void ResetHighlight()
    {
        if (currentHighlight != null)
        {
            Destroy(currentHighlight.gameObject);
            currentHighlight = null;
        }
    }
    
    void ShowPlacementPreview(Vector2Int gridPos, bool isInvalid)
    {
        if (currentHighlight != null)
        {
            Destroy(currentHighlight.gameObject);
        }
        
        GameObject preview = new GameObject("PlacementPreview");
        preview.transform.position = GridToWorld(gridPos);
        preview.transform.SetParent(gridContainer);
        
        SpriteRenderer renderer = preview.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateSquareSprite();
        renderer.color = isInvalid ? invalidColor : validColor;
        renderer.sortingOrder = 99;
        preview.transform.localScale = new Vector3(cellSize, cellSize, 1);
        
        currentHighlight = renderer;
    }
    
    void HidePlacementPreview()
    {
        if (currentHighlight != null)
        {
            Destroy(currentHighlight.gameObject);
            currentHighlight = null;
        }
    }
    
    Sprite CreateSquareSprite()
    {
        // Создаем простой квадратный спрайт для выделения
        Texture2D texture = new Texture2D(32, 32);
        Color[] colors = new Color[32 * 32];
        
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.white;
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
    }
    
    void CreateGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2 worldPos = GridToWorld(new Vector2Int(x, y));
                GridCell cell = new GridCell(new Vector2Int(x, y), worldPos);
                gridCells[cell.gridPosition] = cell;
            }
        }
    }
    
    void CreateGridVisuals()
    {
        // Создаем визуализацию сетки
        GameObject gridVisuals = new GameObject("GridVisuals");
        gridVisuals.transform.SetParent(gridContainer);
        
        for (int x = 0; x <= gridSize.x; x++)
        {
            for (int y = 0; y <= gridSize.y; y++)
            {
                // Создаем линии сетки (опционально)
                if (x < gridSize.x && y < gridSize.y)
                {
                    GameObject cellVisual = new GameObject($"Cell_{x}_{y}");
                    cellVisual.transform.position = GridToWorld(new Vector2Int(x, y));
                    cellVisual.transform.SetParent(gridVisuals.transform);
                    
                    LineRenderer lineX = CreateGridLine(
                        GridToWorld(new Vector2Int(x, y)),
                        GridToWorld(new Vector2Int(x + 1, y))
                    );
                    lineX.transform.SetParent(cellVisual.transform);
                    
                    LineRenderer lineY = CreateGridLine(
                        GridToWorld(new Vector2Int(x, y)),
                        GridToWorld(new Vector2Int(x, y + 1))
                    );
                    lineY.transform.SetParent(cellVisual.transform);
                }
            }
        }
    }
    
    LineRenderer CreateGridLine(Vector2 start, Vector2 end)
    {
        GameObject lineObj = new GameObject("GridLine");
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.gray;
        line.endColor = Color.gray;
        line.sortingOrder = -1;
        
        return line;
    }
    
    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / cellSize);
        int y = Mathf.RoundToInt(worldPosition.y / cellSize);
        return new Vector2Int(x, y);
    }
    
    public Vector2 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector2(gridPosition.x * cellSize, gridPosition.y * cellSize);
    }
    
    bool IsValidGridPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridSize.x && pos.y >= 0 && pos.y < gridSize.y;
    }
    
    public void ToggleMode()
    {
        currentMode = currentMode == GameMode.Edit ? GameMode.Play : GameMode.Edit;
        
        if (currentMode == GameMode.Play)
        {
            // Выходим из режима перетаскивания, если были в нем
            if (isDragging)
            {
                ReturnObjectToLastPosition();
                
                FollowMouse follow = currentlySelectedObject?.GetComponent<FollowMouse>();
                if (follow != null)
                    follow.enabled = false;
                
                isDragging = false;
            }
            
            currentlySelectedObject = null;
            ResetHighlight();
            
            // Блокируем все объекты для игрового режима
            foreach (var obj in FindObjectsOfType<GridObject>())
            {
                obj.SetDraggable(false);
            }
        }
        else
        {
            // Разблокируем объекты для режима редактирования
            foreach (var obj in FindObjectsOfType<GridObject>())
            {
                obj.SetDraggable(true);
            }
        }
        
        UpdateModeUI();
    }
    
    bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
    
    void UpdateModeUI()
    {
        Debug.Log($"Режим: {(currentMode == GameMode.Edit ? "Редактирование" : "Игра")}");
        // Здесь можно обновить UI текст или другие элементы интерфейса
    }
}

// Компонент для следования за мышью
public class FollowMouse : MonoBehaviour
{
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos;
    }
}

// Компонент для объектов на сетке
public class GridObject : MonoBehaviour
{
    private GridSystem2D gridSystem;
    private Vector2Int gridPosition;
    private bool isDraggable = true;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }
    
    public void Initialize(GridSystem2D system, Vector2Int pos)
    {
        gridSystem = system;
        gridPosition = pos;
    }
    
    public void UpdateGridPosition(Vector2Int newPosition)
    {
        gridPosition = newPosition;
    }
    
    public void SetDraggable(bool draggable)
    {
        isDraggable = draggable;
    }
    
    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }
    
    void OnMouseEnter()
    {
        if (gridSystem.currentMode == GridSystem2D.GameMode.Edit && isDraggable && spriteRenderer != null)
        {
            spriteRenderer.color = Color.yellow;
        }
    }
    
    void OnMouseExit()
    {
        if (gridSystem.currentMode == GridSystem2D.GameMode.Edit && isDraggable && spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
}