using UnityEngine;
using System.Collections;
using TMPro;
public class BillboardSpriteScript : MonoBehaviour
{
    public TextMeshPro billboardText;
    float rngX;
    float y;
    void Start()
    {
        y = Random.Range(-2f, 4);
        rngX = Random.Range(-1f, 1f) * 5;
        if (rngX > 0f)
        {
            rngX += Random.Range(10, 20);
        }
        else
        {
            rngX -= Random.Range(10, 20);
        }
        StartCoroutine("Timer");
    }
    void Update()
    {
        rngX -= 10f * rngX * Time.deltaTime;
        y -= 20f * Time.deltaTime;
        transform.forward = Camera.main.transform.forward;
        transform.position += (new Vector3 (rngX * Camera.main.transform.right.x, y, rngX * Camera.main.transform.right.z) + Camera.main.transform.forward) * Time.deltaTime;
        transform.localScale -= new Vector3 (transform.localScale.x, transform.localScale.y, transform.localScale.z) * 3 * Time.deltaTime;
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);
    }
}
