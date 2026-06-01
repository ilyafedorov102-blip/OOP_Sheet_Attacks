using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [Header("Основные параметры")]
    public string buildingName;

    public int ironCost;
    public int goldCost;
    public int titaniumCost;
    public int oilCost;
    public int materialsCost;
    public int electricityCost;
    public int isTurn; // потрачен ли ход на создание объекта

    public Vector2 gridPosition;
    public ResourcesData PlayerBuild;

    public Timer timer;

    // вызывается каждый ход
    public virtual void OnTurn() { } // что делает здание в течение хода

    // вызываются один раз при создании и уничтожении
    public virtual void OnBuild() 
    {
        Debug.Log($"Здание {buildingName} было построено");
    }
}