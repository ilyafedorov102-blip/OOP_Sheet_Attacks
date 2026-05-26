using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ResourcesManager : MonoBehaviour
{
    ResourcesData pl1 = new ResourcesData();
    ResourcesData pl2 = new ResourcesData();
    public TMP_Text textResources_Pl1;
    public TMP_Text textResources_Pl2;
    public Timer timer;
    internal int resFlow = 0;

    void Start()
    {
        
    }

    void Update()
    {
        textResources_Pl1.text = $"\t{pl1.iron}(+{pl1.ironMiner})\t\t{pl1.gold}(+{pl1.goldMiner})\t\t{pl1.titanium}(+{pl1.titaniumMiner})\t\t{pl1.oil}(+{pl1.oilMiner})\n\t{pl1.buildMaterials}(+{pl1.buildMaterialsMiner})\t\t{pl1.electricity}\t\t{pl1.money}";
        textResources_Pl2.text = $"\t{pl2.iron}(+{pl2.ironMiner})\t\t{pl2.gold}(+{pl2.goldMiner})\t\t{pl2.titanium}(+{pl2.titaniumMiner})\t\t{pl2.oil}(+{pl2.oilMiner})\n\t{pl2.buildMaterials}(+{pl2.buildMaterialsMiner})\t\t{pl2.electricity}\t\t{pl2.money}";

        if (timer.minute % 1 == 0 && timer.minute != resFlow)
        {
            pl1.iron += pl1.ironMiner;
            pl1.gold += pl1.goldMiner;
            pl1.titanium += pl1.titaniumMiner;
            pl1.oil += pl1.oilMiner;
            pl1.buildMaterials += pl1.buildMaterialsMiner;

            pl2.iron += pl2.ironMiner;
            pl2.gold += pl2.goldMiner;
            pl2.titanium += pl1.titaniumMiner;
            pl2.oil += pl1.oilMiner;
            pl2.buildMaterials += pl1.buildMaterialsMiner;

            resFlow = timer.minute;
        }
    }
}

public class ResourcesData
{
    public int iron;
    public int gold;
    public int titanium;
    public int oil;
    public int buildMaterials;
    public int electricity;
    public int money;

    public int ironMiner = 1;
    public int goldMiner = 1;
    public int titaniumMiner = 1;
    public int oilMiner = 1;
    public int buildMaterialsMiner = 1;
}