using UnityEngine;
public class DataCamera
{
    public enum CAMSTATE { STILL, DYNAMIC };
    public static CAMSTATE SCSTATE = CAMSTATE.STILL;

    public static PoseCamera[] poseRangeFromCUDAE = new PoseCamera[2]{
                    new PoseCamera("MS-D-HA", new Vector3(0f, 1.71f, 0f), new Vector3(0f, 1.9f, 1f), new Vector3(20f, 180f, 0f), new Vector3(27f, 180f, 0f)),
                    new PoseCamera("LS-D-HA", new Vector3(0f, 1.71f, 1f), new Vector3(0f, 1.9f, 2f), new Vector3(27f, 180f, 0f), new Vector3(32f, 180f, 0f))
                };

    public static PoseCamera[] poseRangeFromMSDHA = new PoseCamera[2] {
                    new PoseCamera("MS-D", new Vector3(0f, 0.8f, 1f), new Vector3(0f, 1.5f, 1f), new Vector3(01f, 1f, 0f), new Vector3(0f, 1f, 0f)),
                    new PoseCamera("CU-D-AE", new Vector3(0f, 0.8f, 0.2f), new Vector3(0f, 1.5f, 1f), new Vector3(0f, 180f, 0f), new Vector3(0f, 180f, 0f)),
                };

    public static PoseCamera[] poseRangeFromMSD = new PoseCamera[2] {
                    new PoseCamera("LS-D-HA", new Vector3(0f, 0.8f, 1f), new Vector3(0f, 1.5f, 1f), new Vector3(0f, 1f, 0f), new Vector3(0f, 1f, 0f)),
                    new PoseCamera("MS-D-HA", new Vector3(0f, 1.71f, 0f), new Vector3(0f, 1.9f, 1f), new Vector3(20f, 180f, 0f), new Vector3(27f, 180f, 0f)),
                };

    public static PoseCamera[] poseRangeFromLSDHA = new PoseCamera[3] {
                    new PoseCamera("CU-D-AE", new Vector3(0f, 0.8f, 0.2f), new Vector3(0f, 1.5f, 1f), new Vector3(0f, 180f, 0f), new Vector3(0f, 180f, 0f)),
                    new PoseCamera("MS-D-HA", new Vector3(0f, 1.71f, 0f), new Vector3(0f, 1.9f, 1f), new Vector3(20f, 180f, 0f), new Vector3(27f, 180f, 0f)),
                    new PoseCamera("MS-D", new Vector3(0f, 0.8f, 1f), new Vector3(0f, 1.5f, 1f), new Vector3(01f, 1f, 0f), new Vector3(0f, 1f, 0f))
                };


    public static TruePoseCamera[] poseFix = new TruePoseCamera[4] {
                    new TruePoseCamera("CU-D-AE", new Vector3(0f, 1.38f, 0.3f), new Vector3(0f, 180f, 0f), 4f),
                    new TruePoseCamera("MS-D-HA", new Vector3(0f, 1.81f, 1f), new Vector3(25f, 180f, 0f), 6f),
                    new TruePoseCamera("MS-D", new Vector3(0f, 1.11f, 1f), new Vector3(0f, 180f, 0f), 4f),
                    //new TruePoseCamera("MS-D_L", new Vector3(0f, 0.61f, 1f), new Vector3(-25f, 180f, 0f), 2f),
                    //new TruePoseCamera("LS-D", new Vector3(0f, 0.61f, 2f), new Vector3(0f, 180f, 0f), 3f),
                    new TruePoseCamera("LS-D-HA", new Vector3(0f, 1.81f, 2f), new Vector3(30f, 180f, 0f), 3f),
                };
}
