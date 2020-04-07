using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyExplosionLifeCycle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 3.6f);
    }


}
