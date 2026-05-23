using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public GameObject Pause;

    public GameObject BuildingsRoadAndTerritory;
    public GameObject BuildingsAttacks;
    public GameObject BuildingsDefence;
    public GameObject BuildingsOthers;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangePause();
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

    public void ChangePause()
    {
        if (Pause.activeSelf == true)
            Pause.SetActive(false);
        else
            Pause.SetActive(true);
    }

    public void ChangePanelBuildingsRoadAndTerritory()
    {
        ChangePanelStatus(BuildingsPanels.RoadAndTerritory);
    }

    public void ChangePanelBuildingsAttacks()
    {
        ChangePanelStatus(BuildingsPanels.Attacks);
    }

    public void ChangePanelBuildingsDefence()
    {
        ChangePanelStatus(BuildingsPanels.Defence);
    }

    public void ChangePanelBuildingsOthers()
    {
        ChangePanelStatus(BuildingsPanels.Others);
    }

    public void CloseAllBuildingsPanel()
    {
        BuildingsRoadAndTerritory.SetActive(false);
        BuildingsAttacks.SetActive(false);
        BuildingsDefence.SetActive(false);
        BuildingsOthers.SetActive(false);
    }

    public enum BuildingsPanels
    {
        RoadAndTerritory,
        Attacks,
        Defence,
        Others
    }

    public void ChangePanelStatus(BuildingsPanels panels)
    {
        CloseAllBuildingsPanel();

        switch (panels)
        {
            case BuildingsPanels.RoadAndTerritory:
                {
                    BuildingsRoadAndTerritory.SetActive(true);
                    break;
                }
            case BuildingsPanels.Attacks:
                {
                    BuildingsAttacks.SetActive(true);
                    break;
                }
            case BuildingsPanels.Defence:
                {
                    BuildingsDefence.SetActive(true);
                    break;
                }
            case BuildingsPanels.Others:
                {
                    BuildingsOthers.SetActive(true);
                    break;
                }
        }
    }
}