using UnityEngine;

public class PoseCamera
{
    public string identity;
    public Vector3 minPosition;
    public Vector3 maxPosition;
    public Vector3 minRotation;
    public Vector3 maxRotation;
    public float minduration;
    public float maxduration;

    public PoseCamera()
    { }
    public PoseCamera(string id, Vector3 minPos, Vector3 maxPos, Vector3 minRot, Vector3 maxRot, float minDur, float maxDur)
    {
        identity = id;
        minPosition = minPos;
        maxPosition = maxPos;
        minRotation = minRot;
        maxRotation = maxRot;
        minduration = minDur;
        maxduration = maxDur;
    }

}
