using UnityEngine;

public class UniversalEnemyScript : MonoBehaviour
{
    public GameObject target;
    public float baseHealthPoint;
    public float currentHealthPoint;
    public float movementSpeed;
    public bool isWalking;
    public Rigidbody rb;
    public float fallTimer;
    void Start()
    {
        currentHealthPoint = baseHealthPoint;
        isWalking = true;
        rb.useGravity = true;
    }
    void Update()
    {
        if (isWalking && fallTimer > 0) // timer pour permettre au rb de grimper les bords de mesh, sans ca il retombe arrive en haut
        {
            fallTimer -= 10 * Time.deltaTime; // c'est moche, pas tres elegant
        }
        else
        {
            rb.useGravity = true;
        }
        var targetDir = target.transform.position - transform.position; // la direction de la cible par rapport a nous
        transform.rotation = Quaternion.LookRotation(new Vector3(targetDir.x, 0, targetDir.z)); // on s'oriente vers la cible uniquement dans l'axe x,z        
        if(isWalking)
        {
            Walk();
            GroundDetection();
        }
        else
        {
            transform.position += transform.up * movementSpeed * Time.deltaTime; // si on touche un mur on monte
            fallTimer = 1;
            rb.useGravity = false;
        }

        if (currentHealthPoint <= 0)
        {
            Destroy(gameObject);
        }
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            isWalking = false;
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            isWalking = true;
        }
    }
    void Walk()
    {
        transform.position += transform.forward * movementSpeed * Time.deltaTime; // on avance vers l'avant        
    }
    void GroundDetection()
    {
        //RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, 0.65f)) // on utilise un raycast au lieu d'un spherecast car c'est suffisant pour l'IA des enemis
        {
            fallTimer = 1;
            rb.useGravity = false;
        }
        Debug.DrawLine(transform.position, transform.position - transform.up * 0.65f, Color.red);
    }
}
