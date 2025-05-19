using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairOnScreen : MonoBehaviour
{
    [SerializeField] Transform aimPoint;
    Vector3 pos;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(aimPoint != null)
        {
            transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, aimPoint.transform.TransformPoint(Vector3.zero));
        }
        

    }
}
