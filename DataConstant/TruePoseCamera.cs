using UnityEngine;

public class TruePoseCamera
{
    public string identity;
    public Vector3 position;
    public Vector3 rotation;
    public float duration;

    public TruePoseCamera(string id, Vector3 pos, Vector3 rot, float dur)
    {
        identity = id;
        position = pos;
        rotation = rot;
        duration = dur;
    }

}
