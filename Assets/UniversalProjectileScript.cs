using UnityEngine;

public class UniversalProjectileScript : MonoBehaviour
{
    [Header("Degats")]
    public float damageToDeal;
    [Header("Mouvement")]
    public float travelSpeed;   
    public bool isSeeking;
    [Header("Explosions")]
    public bool explodes;
    [Header("Detection des coup")]
    public int hitsBeforeDestroy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
