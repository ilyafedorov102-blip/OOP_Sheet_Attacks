using UnityEngine;

public class DefaultObjects
{
    GameObject Image;
    private double x, y;
    private int hp;
    private int gold, iron, titanium, oil, buildMaterials, electricity;

    DefaultObjects(int newHp, int gold, int iron, int titanium, int oil, int buildMaterials, int electricity)
    {
        // установить изображение объекта
        SetCoord(0, 0);
        SetHp(newHp);
        SetResources(gold, iron, titanium, oil, buildMaterials, electricity);
    }

    public void SetCoord(double newX, double newY)
    {
        x = newX; 
        y = newY;
    }

    public void SetHp(int newHp)
    {
        hp = newHp;
    }

    public void SetResources(int newGold, int newIron, int newTitanium, int newOil, int newBuildMaterials, int newElectricity)
    {
        gold = newGold;
        iron = newIron;
        titanium = newTitanium;
        oil = newOil;
        buildMaterials = newBuildMaterials;
        electricity = newElectricity;
    }
}