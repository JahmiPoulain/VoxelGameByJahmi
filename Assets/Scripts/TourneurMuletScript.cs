using UnityEngine;

public class TourneurMuletScript : MonoBehaviour
{
    public Vector3 averageVector3Test;
    public GameObject[] V3List;
    public GameObject visualPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        averageVector3Test = new Vector3(0, 0, 0);
        for (int x = 0; x < 4; x++)
        {
            var currentV3 = V3List[x].transform.position;
            averageVector3Test += currentV3;

        }
        Debug.Log(averageVector3Test);
        visualPoint.transform.position = transform.position + new Vector3(averageVector3Test.x, 0, averageVector3Test.z).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
