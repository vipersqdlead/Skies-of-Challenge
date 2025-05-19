using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAIModel : MonoBehaviour
{
    public void Direction(Vector3 Dir, float MaxTurn)
    {
        Quaternion rotation = Quaternion.LookRotation(Dir - transform.position);
        float angle = Quaternion.Angle(transform.rotation, rotation);
        float timetocomplete = angle / MaxTurn;
        float donePercentage = Mathf.Min(1f, Time.deltaTime / timetocomplete);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, donePercentage);
    }
}
