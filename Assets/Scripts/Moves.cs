using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Moves : MonoBehaviour
{
    public TMP_Text txtMove;
    public Button btnMove;
    private bool isMoveFirst = true;
    public Timer timer;
    private int fixTime;
    private Coroutine currentFadeCoroutine;

    void Start()
    {
        Color startColor = txtMove.color;
        startColor.a = 0f;
        txtMove.color = startColor;
        txtMove.enabled = false;
        btnMove.onClick.AddListener(OnMoveButtonClicked);
    }

    void OnMoveButtonClicked()
    {
        // Если уже идет анимация, останавливаем её
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);

        // Запускаем новую анимацию
        currentFadeCoroutine = StartCoroutine(ShowMoveText());
    }

    IEnumerator ShowMoveText()
    {
        isMoveFirst = !isMoveFirst;

        if (isMoveFirst)
        {
            txtMove.color = Color.blue;
            txtMove.text = "Ход синего игрока!";
        }
        else
        {
            txtMove.color = Color.red;
            txtMove.text = "Ход красного игрока!";
        }

        txtMove.enabled = true;

        // Плавное появление (0 -> 1)
        float fadeInDuration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeInDuration);
            Color newColor = txtMove.color;
            newColor.a = alpha;
            txtMove.color = newColor;
            yield return null; // Ждем следующий кадр
        }

        // Ждем 2 секунды с полностью видимым текстом
        yield return new WaitForSeconds(2f);

        // Плавное исчезновение (1 -> 0)
        float fadeOutDuration = 0.5f;
        elapsedTime = 0f;
        Color currentColor = txtMove.color;
        float startAlpha = currentColor.a;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeOutDuration);
            Color newColor = txtMove.color;
            newColor.a = alpha;
            txtMove.color = newColor;
            yield return null;
        }

        // Скрываем текст
        txtMove.enabled = false;
        currentFadeCoroutine = null;
    }
}