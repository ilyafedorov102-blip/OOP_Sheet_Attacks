using UnityEngine;

public class ResourceBuilding : BuildingManager
{
    [SerializeField] private BuildingManager obj;
    
    public string typeResourse;
    public override void OnTurn() { }
    public override void OnBuild()
    {
        switch (typeResourse)
        {
            case "iron":
                {
                    buildingName = "ironMiner";
                    obj.PlayerBuild.ironMiner++; // повышаем количество добывающих заводов
                    break;
                }
            case "gold":
                {
                    buildingName = "goldMiner";
                    obj.PlayerBuild.goldMiner++;
                    break;
                }
            case "titanium":
                {
                    obj.PlayerBuild.titaniumMiner++;
                    break;
                }
            case "oil":
                {
                    obj.PlayerBuild.oilMiner++;
                    break;
                }
            case "materials":
                {
                    obj.PlayerBuild.buildMaterialsMiner++;
                    break;
                }
            default: { Debug.Log("ERROR!!! Ошибка при определении типа рудника!"); break; }
        }
    }
}
