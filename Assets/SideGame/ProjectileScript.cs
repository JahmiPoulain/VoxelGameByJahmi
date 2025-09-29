using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public GameObject instantiator;
    public float vitesseDeplacement;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * vitesseDeplacement * Time.deltaTime;
    }
    void OnColliderEnter(Collision laColision)
    {
        if (laColision.gameObject.GetComponent<VoxelPlayerController>())
        {
           // Debug.Log("joueur HIT");
        }
        //laColision.GetComponent<
       // Debug.Log("hit");
    }
}
