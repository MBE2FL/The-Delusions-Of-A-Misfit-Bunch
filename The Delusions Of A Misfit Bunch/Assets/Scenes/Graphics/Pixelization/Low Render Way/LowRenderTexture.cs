using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[ExecuteAlways]
public class LowRenderTexture : MonoBehaviour
{
    [SerializeField]
    RenderTexture _renderTexture;
    [SerializeField]
    Transform _quad;
    Camera _camera;



    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        _quad.localScale = new Vector3(_renderTexture.width, _renderTexture.height, 1.0f);
        _camera.orthographicSize = _renderTexture.height * 0.5f;
#endif
    }
}
