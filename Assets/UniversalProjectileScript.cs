using UnityEngine;
using System.Collections;

public class UniversalProjectileScript : MonoBehaviour
{
    [Header("degats")]
    public float baseDamage;
    public float damageToDeal;
    [Header("mouvement")]
    public float velocity;
    [Header("collision")]
    public SphereCollider sphereCollider;
    public int hitsBeforeDestroy;
    public int currentHitsAmount;
    [Header("tete chercheuse")]
    public GameObject target;
    [Header("explosion")]
    public float explosionRadius;
    [Header("taille")]
    public float size;
    [Header("destruction")]
    public float lifeSpan;
    //[Header("destruction")]

    void Start()
    {
        StartCoroutine("LifeSpanTimer");
        sphereCollider = GetComponent<SphereCollider>();
        damageToDeal = baseDamage;
        gameObject.transform.localScale = new Vector3(size, size, size); // modifier la taille de l'objet
    }

    void Update()
    {
        transform.position += transform.forward * velocity * Time.deltaTime; // avancer devant
        if (target != null && currentHitsAmount == 0) // si il y a une cible et qu'il n'a rien touche
        {
            SeekTarget();
        }
        
    }

    void OnTriggerEnter(Collider collision) // le collider est un trigger pour passer a travers les ennemis
    {
        sphereCollider.radius = size * explosionRadius; // l'explosion c'est juste faire grossir la hitbox de base
        currentHitsAmount++; // compter le nombre de collision
        if (currentHitsAmount >= hitsBeforeDestroy) // detruire l'objet si il est a court de collision
        {
            InitiateDestruction();
        }
        if (collision.gameObject.GetComponent<UniversalEnemyScript>()) // verifier l'identite de l'objet touche
        {
            collision.gameObject.GetComponent<UniversalEnemyScript>().currentHealthPoint -= damageToDeal; // infliger des degats a l'objet touche
        }
    }

    void SeekTarget()
    {
        transform.LookAt(target.transform.position); // s'oriente vers la position de la cible
    }

    IEnumerator LifeSpanTimer()
    {
        yield return new WaitForSeconds(lifeSpan);
        InitiateDestruction();
    }

    void InitiateDestruction() // detruire
    {
        Destroy(gameObject);
    }
}
