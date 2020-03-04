using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionLifeCycle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Destroying Explosion!");

        Destroy(gameObject, 1.8f);
    }


}
