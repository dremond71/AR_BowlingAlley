using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMilleniumFalconBlastLifeCycle : MonoBehaviour
{
    // Start is called before the first frame update
    private float lifeDuration = 2.5f;
    private float lifeTimer;
    // Start is called before the first frame update
    void Start()
    {
        lifeTimer = lifeDuration;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (
              (collision.gameObject.tag  == "PlayerShooter") ||
              (collision.gameObject.tag == "starDestroyer") ||
               (collision.gameObject.tag == "slave1") ||
               (collision.gameObject.tag == "viperDroid")
        )
        {
            destroySelf();
        }

    }
    // Update is called once per frame
    void Update()
    {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {

            destroySelf();
        }
    }

    void destroySelf()
    {
        Destroy(gameObject);
    }
}
