using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ResourcesManager : MonoBehaviour
{
    ResourcesData pl1 = new ResourcesData();
    ResourcesData pl2 = new ResourcesData();
    internal int resFlow = 0;
    [Header("Timer & Resurces fields")]
    [SerializeField] private TMP_Text textResources_Pl1;
    [SerializeField] private TMP_Text textResources_Pl2;
    [SerializeField] private Timer timer;
    [Header("Resources prefabs")]
    [SerializeField] private GameObject Sp_gold;
    [SerializeField] private GameObject Sp_iron;
    [SerializeField] private GameObject Sp_oil;
    [SerializeField] private GameObject Sp_Titanium;
    [SerializeField] private GameObject Sp_materials;
    [Header("Objects' cell & borders")]
    [SerializeField] private float cellSize;
    [SerializeField] private Vector2 minResBorder;
    [SerializeField] private Vector2 maxResBorder;

    void Start()
    {
        Spawn_res(Sp_iron, Random.Range(4, 6), minResBorder, maxResBorder);
        Spawn_res(Sp_gold, Random.Range(4, 6), minResBorder, maxResBorder);
        Spawn_res(Sp_oil, Random.Range(4, 6), minResBorder, maxResBorder);
        Spawn_res(Sp_Titanium, Random.Range(4, 6), minResBorder, maxResBorder);
        Spawn_res(Sp_materials, Random.Range(4, 6), minResBorder, maxResBorder);
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
    void Spawn_res(GameObject Obj, int amount, Vector2 Min_crd, Vector2 Max_crd)
    {
        Vector2 Pos;
        while (amount > 0)
        {
            Pos.x = amount % 2 == 0 ? Random.Range(0, Max_crd.x) : Random.Range(Min_crd.x, 0);
            Pos.y = Random.Range(Min_crd.y, Max_crd.y);
            Pos.x = Mathf.Round(Pos.x / 2 / cellSize) * cellSize * 2;
            Pos.y = Mathf.Round(Pos.y / 2 / cellSize) * cellSize * 2;
            Instantiate(Obj, Pos, Quaternion.identity);
            amount--;
        }
    }
}
internal class ResourcesData
{
    internal int iron;
    internal int gold;
    internal int titanium;
    internal int oil;
    internal int buildMaterials;
    internal int electricity;
    internal int money;

    internal int ironMiner = 1;
    internal int goldMiner = 1;
    internal int titaniumMiner = 1;
    internal int oilMiner = 1;
    internal int buildMaterialsMiner = 1;
}