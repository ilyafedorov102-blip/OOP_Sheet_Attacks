using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public GameObject Pause;
    private bool isPause = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPause = !isPause;
            Pause.SetActive(isPause);
        }
    }

    public void Play()
    {
        SceneManager.LoadScene("GameWindow"); // для открытия игрового окна
    }

    public void Back()
    {
        SceneManager.LoadScene("SampleScene"); // возвращаемся назад в главное меню
    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("Game has been closed");
    }

    public void ClosePause()
    {
        isPause = !isPause;
        Pause.SetActive(isPause);
    }
}