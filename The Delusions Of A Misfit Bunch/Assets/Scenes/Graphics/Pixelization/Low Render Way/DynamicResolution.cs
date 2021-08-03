using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;



[ExecuteAlways]
public class DynamicResolution : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DynamicResolutionHandler.SetDynamicResScaler(setDynamicResolution, DynamicResScalePolicyType.ReturnsMinMaxLerpFactor);
    }

    public float setDynamicResolution()
    {
        //return 20.0f;
        return 0.1f;
    }
}
