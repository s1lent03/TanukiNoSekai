using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TanukiSpawner : MonoBehaviour
{
    [Header("TanukiClasses")]
    public Tanuki commonTanuki;
    public Tanuki uncommonTanuki;
    public Tanuki rareTanuki;
    public Tanuki nightTanuki;
    [SerializeField] Tanuki newTanuki;

    [Header("SpawnArea")]
    public GameObject spawnArea;
    public GameObject tanukiParent;
    private Collider areaCollider;
    private Vector3 areaCenter;
    private Vector3 areaExtents;

    [Header("NumbersOfSpawns")]
    public int minNumberSpawns;
    public int maxNumberSpawns;
    private int numberTanukiSpawned = 0;

    [Header("SpawnPercentage")]
    [Range(0, 100)] public int commonSpawnRate;
    [Range(0, 100)] public int uncommonSpawnRate;
    [Range(0, 100)] public int rareSpawnRate;

    [Header("SpawnLevels")]
    [SerializeField] int minSpawnLevel;
    [SerializeField] int maxSpawnLevel;

    void Start()
    {
        //Get collider
        areaCollider = spawnArea.GetComponent<Collider>();

        //Get limites do collider
        areaCenter = areaCollider.bounds.center;
        areaExtents = areaCollider.bounds.extents;

        //Avisar caso não tenha encontrado o collider
        if (areaCollider == null)
            Debug.Log("Não encontrou collider");
    }

    //Ativado quando o jogador entra dentro do collider
    private void OnTriggerEnter(Collider other)
    {
        int numberOfSpawns = Random.Range(minNumberSpawns, maxNumberSpawns);
        if (numberTanukiSpawned < numberOfSpawns)
        {
            //Randomizar spawn de Tanuki
            int randomRate = Random.Range(0, (commonSpawnRate + uncommonSpawnRate + rareSpawnRate));

            //Spawna um Tanuki comum
            if (randomRate > 0 && randomRate < commonSpawnRate)
            {
                newTanuki = commonTanuki;
            }//Spawna um Tanuki fora do comum
            else if (randomRate > commonSpawnRate && randomRate < (commonSpawnRate + uncommonSpawnRate))
            {
                newTanuki = uncommonTanuki;
            }//Spawna um Tanuki raro
            else if (randomRate > (commonSpawnRate + uncommonSpawnRate) && randomRate < (commonSpawnRate + uncommonSpawnRate + rareSpawnRate))
            {
                newTanuki = rareTanuki;
            }

            //Gerar uma posição aleatórea para o Tanuki spawnar
            float randomX = Random.Range(areaCenter.x - areaExtents.x, areaCenter.x + areaExtents.x);
            float randomZ = Random.Range(areaCenter.z - areaExtents.z, areaCenter.z + areaExtents.z);

            randomX += newTanuki.Base.TanukiModel.transform.position.x;
            randomZ += newTanuki.Base.TanukiModel.transform.position.z;

            Vector3 randomPos = new Vector3(randomX, transform.position.y + newTanuki.Base.TanukiModel.transform.position.y, randomZ);

            //Gerar rotação aleatórea para o Tanuki
            float randomRot = Random.Range(0, 360);
            Quaternion spawnRot = Quaternion.Euler(new Vector3(transform.rotation.x, randomRot, transform.rotation.z));

            //Spawnar o Tanuki com a tag WildTanuki
            GameObject newTanukiObject = Instantiate(newTanuki.Base.TanukiModel, randomPos, spawnRot, tanukiParent.transform);
            newTanukiObject.tag = "WildTanuki";
            newTanuki.Level = RandomizeWildTanukiLevels();

            newTanuki.Init(false);
            newTanukiObject.GetComponent<BattleUnit>().tanukiUnitData = newTanuki;
            newTanukiObject.GetComponent<TanukiMovement>().SendCollider(areaCollider);
            
            //Animação de spawnar tanuki
            newTanukiObject.transform.Find("ModelObject").transform.DOScale(new Vector3(1, 1, 1), 1);

            numberTanukiSpawned++;
        }
    }

    public int RandomizeWildTanukiLevels()
    {
        //Dar um level ao tanuki spawnado
        return Random.Range(minSpawnLevel, maxSpawnLevel + 1);
    }

    //Ativado quando o jogador sai de dentro do collider
    private void OnTriggerExit(Collider other)
    {
        //Fazer com que os Tanuki desapareçam caso o player saia da trigger box
        if (other.tag == "Player")
        {
            for (int i = tanukiParent.transform.childCount - 1; i >= 0; i--)
            {
                numberTanukiSpawned = 0;
                Destroy(tanukiParent.transform.GetChild(i).gameObject);
            }
        }
    }
}
