using UnityEngine;
using System.Collections;

public class EnemySpawnerScript : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject player;
    public float spawnRate;
    public float maxSpawnDistance;
    public float minSpawnDistance;
    public int maxSpawnStacking;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine("SpawnTimer");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(spawnRate);
        
        // on fait apparaitre un ennemi autour du joueur dans une direction au hasard, a une didtance au hasard entre a et b
        float rngDistance = Random.Range(minSpawnDistance, maxSpawnDistance); // la distance d'instantiation en a et b

        float rngX = Random.Range(-1f, 1f); // la direction d'instantiation
        float rngY = Random.Range(-1f, 1f);

        float magnitude = Mathf.Sqrt (rngX * rngX + rngY * rngY); // on trouve la magnitude de la direction
        
        rngX = rngX * rngDistance / magnitude; // on modifie la magnitude
        rngY = rngY * rngDistance / magnitude;

        for (int y = 0; y < Random.Range(1, maxSpawnStacking); y++) // le nombre d'ennemi a empiler au spawn
        {
            var instatiated = Instantiate(enemies[Random.Range(0, enemies.Length)], player.transform.position + new Vector3(rngX, y, rngY), transform.rotation);
            instatiated.GetComponent<UniversalEnemyScript>().target = player;
        }
        StartCoroutine("SpawnTimer");
    }
}
