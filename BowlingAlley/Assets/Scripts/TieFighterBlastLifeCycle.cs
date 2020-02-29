using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TieFighterBlastLifeCycle : MonoBehaviour
{
    private float lifeDuration = 2.5f;
    private float lifeTimer;
    // Start is called before the first frame update
    void Start()
    {
        lifeTimer = lifeDuration;
    }

    // Update is called once per frame
    void Update()
    {
        lifeTimer -= Time.deltaTime; 
        if (lifeTimer <= 0f) {
            //Debug.Log("destroyed bullet");
           Destroy(gameObject);
        }
    }
}
