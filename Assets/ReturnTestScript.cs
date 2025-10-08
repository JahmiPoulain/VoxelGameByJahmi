using UnityEngine;

public class ReturnTestScript : MonoBehaviour
{
    public float vary;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vary = ClampAngle(vary); // on prend vary en entree et on obtient vari en sortie vary = vari
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static float ClampAngle(float vari)
    {
        vari += vari;
        return vari; // return etablit le résultat de sortie de la fonction
    }
}
