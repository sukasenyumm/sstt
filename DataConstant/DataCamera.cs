using UnityEngine;
public class DataCamera
{
    public enum CAMSTATE { STILL, DYNAMIC };
    public static CAMSTATE SCSTATE = CAMSTATE.STILL;

    public static PoseCamera[] poseRangeFromCUDAE = new PoseCamera[2]{
                    new PoseCamera("MS-D-HA", new Vector3(0f, 1.71f, 0f), new Vector3(0f, 1.9f, 1f), new Vector3(20f, 180f, 0f), new Vector3(27f, 180f, 0f),5f),
                    new PoseCamera("LS-D-HA", new Vector3(0f, 1.71f, 1f), new Vector3(0f, 1.9f, 2f), new Vector3(27f, 180f, 0f), new Vector3(32f, 180f, 0f),5f)
                };

    public static PoseCamera[] poseRangeFromMSDHA = new PoseCamera[2] {
                    new PoseCamera("MS-D", new Vector3(0f, 0.8f, 1f), new Vector3(0f, 1.5f, 1f), new Vector3(01f, 1f, 0f), new Vector3(0f, 1f, 0f),5f),
                    new PoseCamera("CU-D-AE", new Vector3(0f, 0.8f, 0.2f), new Vector3(0f, 1.5f, 1f), new Vector3(0f, 180f, 0f), new Vector3(0f, 180f, 0f),5f),
                };

    public static PoseCamera[] poseRangeFromMSD = new PoseCamera[2] {
                    new PoseCamera("LS-D-HA", new Vector3(0f, 0.8f, 1f), new Vector3(0f, 1.5f, 1f), new Vector3(0f, 1f, 0f), new Vector3(0f, 1f, 0f),5f),
                    new PoseCamera("MS-D-HA", new Vector3(0f, 1.71f, 0f), new Vector3(0f, 1.9f, 1f), new Vector3(20f, 180f, 0f), new Vector3(27f, 180f, 0f),5f),
                };

    public static PoseCamera[] poseRangeFromLSDHA = new PoseCamera[3] {
                    new PoseCamera("CU-D-AE", new Vector3(0f, 0.8f, 0.2f), new Vector3(0f, 1.5f, 1f), new Vector3(0f, 180f, 0f), new Vector3(0f, 180f, 0f),5f),
                    new PoseCamera("MS-D-HA", new Vector3(0f, 1.71f, 0f), new Vector3(0f, 1.9f, 1f), new Vector3(20f, 180f, 0f), new Vector3(27f, 180f, 0f),5f),
                    new PoseCamera("MS-D", new Vector3(0f, 0.8f, 1f), new Vector3(0f, 1.5f, 1f), new Vector3(01f, 1f, 0f), new Vector3(0f, 1f, 0f),5f)
                };


    //public static PoseCamera[] poseFix = new PoseCamera[4] {
    //                new PoseCamera("CU-D-AE", new Vector3(0f, 0.8f, 0.2f), new Vector3(0f, 1.5f, 1f), new Vector3(0f, 180f, 0f), new Vector3(0f, 180f, 0f),1f),
    //                new PoseCamera("MS-D-HA", new Vector3(0f, 1.71f, 0f), new Vector3(0f, 1.9f, 1f), new Vector3(20f, 180f, 0f), new Vector3(27f, 180f, 0f),1f),
    //                 new PoseCamera("LS-D-HA", new Vector3(0f, 0.8f, 1f), new Vector3(0f, 1.5f, 1f), new Vector3(0f, 1f, 0f), new Vector3(0f, 1f, 0f),1f),
    //                new PoseCamera("MS-D", new Vector3(0f, 0.8f, 1f), new Vector3(0f, 1.5f, 1f), new Vector3(01f, 1f, 0f), new Vector3(0f, 1f, 0f),1f)
    //            };
}
