using UnityEngine;

public class UniversalEnemyScript : MonoBehaviour
{
    public float baseHealthPoint;
    public float currentHealthPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealthPoint = baseHealthPoint;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
