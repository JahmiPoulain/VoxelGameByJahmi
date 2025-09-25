using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;

public class TargetManagerScript : MonoBehaviour
{
    public int playTimeLimit;
    public int currentTime;
    public TMP_Text timerTMpro;
    public GameObject target;
    int chunkSize;
    public GameObject chunk;
    public int score;
    public TMP_Text scroreTMpro;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = playTimeLimit;
        chunkSize = chunk.GetComponent<ChunkScript>().chunkSize;
        Instantiate(target, new Vector3(Random.Range(0, chunkSize), Random.Range(0, chunkSize), Random.Range(0, chunkSize)) + new Vector3(0.5f, 0.5f, 0.5f), Quaternion.Euler(0, 0, 0));
        StartCoroutine("Timer");
    }

    // Update is called once per frame
    void Update()
    {
        scroreTMpro.text = "Bloons popped : " + score.ToString();
        if (currentTime < 0)
        {
            timerTMpro.text = "FIN DE PARTIE TON SCORE EST " + score.ToString();
            StopCoroutine("Timer");
        }
        //Instantiate(target, new Vector3(Random.Range(0,chunkSize), Random.Range(0, chunkSize), Random.Range(0, chunkSize)) + new Vector3(0.5f, 0.5f, 0.5f), Quaternion.Euler(0, 0, 0));
    }

    public void InstantiateNewTarget()
    {
        score++;
        if (currentTime > 0)
        {
            Instantiate(target, new Vector3(Random.Range(0, chunkSize), Random.Range(0, chunkSize), Random.Range(0, chunkSize)) + new Vector3(0.5f, 0.5f, 0.5f), Quaternion.Euler(0, 0, 0));
        }
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1);
        currentTime--;
        timerTMpro.text = "Time Left " + currentTime.ToString();
        StartCoroutine("Timer");
    }
}
