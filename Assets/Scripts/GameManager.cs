using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab; // Префаб объекта для создания
    [SerializeField] private GameObject Panel;

    [SerializeField] private Button btnSetObj;
    [SerializeField] private float cellSize;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 10f;
    [SerializeField] private float mapWidth = 20f;   // Ширина игрового поля
    [SerializeField] private float mapHeight = 12f;  // Высота игрового поля
    [SerializeField] private Color RedCol = Color.red;
    [SerializeField] private Vector2 minBorder;
    [SerializeField] private Vector2 maxBorder;

    SpriteRenderer SR;

    private Color DefaultCol;
    private GameObject currentObject;
    private bool isDragging = false;

    void Start()
    {
        // Подписываемся на событие нажатия кнопки
        btnSetObj.onClick.AddListener(CreateObject);
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            // Сохраняем позицию мыши в мировых координатах
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Меняем зум
            float newZoom = Camera.main.orthographicSize - scroll * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);

            // Смещаем камеру, чтобы мышь оставалась на той же позиции
            Vector3 newMouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Camera.main.transform.position += mouseWorldPos - newMouseWorldPos;
            LimitCameraPosition();
        }

        // Перетаскивание объекта при создании
        if (isDragging)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Фиксируем объект на месте
                isDragging = false;
                if (currentObject != null)
                {
                    SR = currentObject.GetComponent<SpriteRenderer>();
                    if (SR != null)
                        SR.color = DefaultCol;
                }
                currentObject = null;
            }
        }

        if (isDragging && currentObject != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.x = Mathf.Round(mousePos.x / cellSize) * cellSize;
            mousePos.y = Mathf.Round(mousePos.y / cellSize) * cellSize;
            currentObject.transform.position = mousePos;

            // Проверяем границы и меняем цвет
            UpdateObjectColor();
        }
    }
    void CreateObject()
    {
        // Закрываем панельку и создаем соответствующий ей объект по префабу на месте курсора
        Panel.SetActive(false);

        if (!isDragging)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.x = Mathf.Round(mousePos.x / cellSize) * cellSize;
            mousePos.y = Mathf.Round(mousePos.y / cellSize) * cellSize;
            currentObject = Instantiate(objectPrefab, mousePos, Quaternion.identity);
            // Получаем SpriteRenderer созданного объекта и сохраняем его цвет по умолчанию
            SR = currentObject.GetComponent<SpriteRenderer>();
            if (SR != null)
            {
                DefaultCol = SR.color;
            }

            isDragging = true;

            // Проверяем границы сразу после создания
            UpdateObjectColor();
        }
    }

    void UpdateObjectColor()
    {
        if (currentObject != null && SR != null)
        {
            if ((!Obj_in_Borders(currentObject)))
            {
                SR.color = RedCol;
            }
            else
            {
                SR.color = DefaultCol;
            }
        }
    }
    void LimitCameraPosition()
    {
        Camera cam = Camera.main;
        float verticalSize = cam.orthographicSize;
        float horizontalSize = verticalSize * cam.aspect;

        Vector3 pos = cam.transform.position;

        // Ограничиваем позицию, чтобы камера не выходила за границы карты
        pos.x = Mathf.Clamp(pos.x, -mapWidth + horizontalSize, mapWidth - horizontalSize);
        pos.y = Mathf.Clamp(pos.y, -mapHeight + verticalSize, mapHeight - verticalSize);

        cam.transform.position = pos;
    }

    bool Obj_in_Borders(GameObject obj)
    {
        return (obj.transform.position.x > minBorder.x && obj.transform.position.x < maxBorder.x
            && obj.transform.position.y > minBorder.y && obj.transform.position.y < maxBorder.y);
    }
}