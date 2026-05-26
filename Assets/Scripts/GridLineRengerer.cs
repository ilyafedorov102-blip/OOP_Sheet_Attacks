using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GridLineRenderer : MonoBehaviour
{
    [Header("Параметры сетки")]
    public float cellSize = 1f;           // Размер одной клетки
    public int gridSize = 10;             // Количество клеток в каждую сторону (от центра)
    public Color lineColor = Color.gray;  // Цвет линий
    public Transform gridTransform;       // Объект, относительно которого строится сетка (можно оставить null — будет this.transform)

    private LineRenderer horizontalLines;
    private LineRenderer verticalLines;

    void Awake()
    {
        SetupLineRenderer(ref horizontalLines, "HorizontalLines");
        SetupLineRenderer(ref verticalLines, "VerticalLines");

        UpdateGrid();
    }

    void SetupLineRenderer(ref LineRenderer lr, string name)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.SetParent(transform);
        lineObj.transform.localPosition = Vector3.zero;
        lineObj.transform.localRotation = Quaternion.identity;

        lr = lineObj.AddComponent<LineRenderer>();

        // Настройка LineRenderer
        lr.material = new Material(Shader.Find("Sprites/Default")); // Простой шейдер
        lr.startWidth = 0.02f;
        lr.endWidth = 0.02f;
        lr.startColor = lineColor;
        lr.endColor = lineColor;
        lr.textureMode = LineTextureMode.RepeatPerSegment;
        lr.alignment = LineAlignment.View;
        lr.useWorldSpace = false; // Чтобы двигалась с родителем
    }

    void UpdateGrid()
    {
        if (horizontalLines == null || verticalLines == null) return;

        Transform center = gridTransform != null ? gridTransform : transform;
        float halfSize = cellSize * gridSize;
        int lineCount = gridSize * 2 + 1; // Линии слева-направо и сверху-вниз

        // Обновляем цвет
        horizontalLines.startColor = lineColor;
        horizontalLines.endColor = lineColor;
        verticalLines.startColor = lineColor;
        verticalLines.endColor = lineColor;

        // Горизонтальные линии (горизонтальные полосы)
        horizontalLines.positionCount = lineCount * 2;
        for (int i = 0; i <= gridSize * 2; i++)
        {
            float y = -halfSize + i * cellSize;
            int index = i * 2;
            horizontalLines.SetPosition(index, new Vector3(-halfSize, y, 0));
            horizontalLines.SetPosition(index + 1, new Vector3(halfSize, y, 0));
        }

        // Вертикальные линии (вертикальные полосы)
        verticalLines.positionCount = lineCount * 2;
        for (int i = 0; i <= gridSize * 2; i++)
        {
            float x = -halfSize + i * cellSize;
            int index = i * 2;
            verticalLines.SetPosition(index, new Vector3(x, -halfSize, 0));
            verticalLines.SetPosition(index + 1, new Vector3(x, halfSize, 0));
        }
    }

    // Опционально: обновлять при изменениях в инспекторе
#if UNITY_EDITOR
    void OnValidate()
    {
        if (Application.isPlaying)
            UpdateGrid();
    }
#endif
}
