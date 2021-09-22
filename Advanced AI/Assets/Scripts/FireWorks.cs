using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWorks : MonoBehaviour
{
    public GameObject explosion;
    void Update()
    {
        transform.Translate(transform.up);
    }

    private void OnDestroy()
    {
        GameObject exp = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(exp, 0.2f);
    }
}
