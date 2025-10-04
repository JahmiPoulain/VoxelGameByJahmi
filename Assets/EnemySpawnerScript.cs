using UnityEngine;
using System.Collections;

public class EnemySpawnerScript : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject player;
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
        yield return new WaitForSeconds(0.5f);
        var instatiated = Instantiate(enemies[Random.Range(0, enemies.Length)], player.transform.position + new Vector3(Random.Range(-10F, 10), 0, Random.Range(-10F, 10)), transform.rotation);
        instatiated.GetComponent<UniversalEnemyScript>().target = player;
        StartCoroutine("SpawnTimer");
    }
}
