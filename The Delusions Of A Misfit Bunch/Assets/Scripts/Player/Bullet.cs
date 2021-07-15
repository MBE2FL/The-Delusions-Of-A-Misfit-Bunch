using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      lifeTime += Time.deltaTime;
    }

    public float getLifeTime()
    {
        return lifeTime;
    }
    public void resetLifeTime()
    {
        lifeTime = 0.0f;
    }
}
