using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TanukiSpawner : MonoBehaviour
{
    [Header("TanukiClasses")]
    public GameObject commonTanuki;
    public GameObject uncommonTanuki;
    public GameObject rareTanuki;
    private GameObject tanukiToSpawn;

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
        while (numberTanukiSpawned < numberOfSpawns)
        {
            //Randomizar spawn de Tanuki
            int randomRate = Random.Range(0, (commonSpawnRate + uncommonSpawnRate + rareSpawnRate));
            Debug.Log(randomRate);
            //Spawna um Tanuki comum
            if (randomRate > 0 && randomRate < commonSpawnRate)
            {
                tanukiToSpawn = commonTanuki;
            }//Spawna um Tanuki fora do comum
            else if (randomRate > commonSpawnRate && randomRate < (commonSpawnRate + uncommonSpawnRate))
            {
                tanukiToSpawn = uncommonTanuki;
            }//Spawna um Tanuki raro
            else if (randomRate > (commonSpawnRate + uncommonSpawnRate) && randomRate < (commonSpawnRate + uncommonSpawnRate + rareSpawnRate))
            {
                tanukiToSpawn = rareTanuki;
            }

            //Gerar uma posição aleatórea para o Tanuki spawnar
            float randomX = Random.Range(areaCenter.x - areaExtents.x, areaCenter.x + areaExtents.x);
            float randomZ = Random.Range(areaCenter.z - areaExtents.z, areaCenter.z + areaExtents.z);

            randomX += tanukiToSpawn.transform.position.x;
            randomZ += tanukiToSpawn.transform.position.z;

            Vector3 randomPos = new Vector3(randomX, transform.position.y + tanukiToSpawn.transform.position.y, randomZ);

            //Gerar rotação aleatórea para o Tanuki
            float randomRot = Random.Range(0, 360);
            Quaternion spawnRot = Quaternion.Euler(new Vector3(transform.rotation.x, randomRot, transform.rotation.z));

            Instantiate(tanukiToSpawn, randomPos, spawnRot, tanukiParent.transform);

            //Contar mais 1 Tanuki spawnado
            numberTanukiSpawned++;
        }
    }

    //Ativado quando o jogador sai de dentro do collider
    private void OnTriggerExit(Collider other)
    {
        //Fazer com que os Tanuki desapareçam
        for (int i = tanukiParent.transform.childCount - 1; i >= 0; i--)
        {
            numberTanukiSpawned = 0;
            Destroy(tanukiParent.transform.GetChild(i).gameObject); 
        }
    }
}
