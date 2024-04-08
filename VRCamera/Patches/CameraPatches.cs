using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentVR;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Profiling;
using Valve.VR;
namespace ContentVR
{
    [HarmonyPatch]
    internal class CameraPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MainCamera), nameof(MainCamera.Awake))]
        private static void OnCameraRigEnabled()
        {
            MainCamera mainCamera = MainCamera.instance;
            Camera cam = mainCamera.cam;

            Logs.WriteInfo("CameraRig OnEnable started");

            //Without this there is no headtracking
            cam.gameObject.AddComponent<SteamVR_TrackedObject>();

            Plugin.SecondEye = new GameObject("SecondEye");
            Plugin.SecondCam = Plugin.SecondEye.AddComponent<Camera>();
            Plugin.SecondCam.gameObject.AddComponent<SteamVR_TrackedObject>();
            Plugin.SecondCam.CopyFrom(cam);

            // Without this the right eye gets stuck at a very far point in the map
            Plugin.SecondCam.transform.parent = cam.transform.parent;


        }
        public static void HandleStereoRendering()
        {
            MainCamera mainCamera = MainCamera.instance;
            Camera cam = mainCamera.cam;

            cam.fieldOfView = SteamVR.instance.fieldOfView;
            cam.stereoTargetEye = StereoTargetEyeMask.Left;
            cam.projectionMatrix = cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
            cam.targetTexture = Plugin.MyDisplay.GetRenderTextureForRenderPass(0);

            Plugin.SecondEye.transform.position = cam.transform.position;
            Plugin.SecondEye.transform.rotation = cam.transform.rotation;
            Plugin.SecondEye.transform.localScale = cam.transform.localScale;
            Plugin.SecondCam.enabled = true;
            Plugin.SecondCam.stereoTargetEye = StereoTargetEyeMask.Right;
            Plugin.SecondCam.projectionMatrix = Plugin.SecondCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
            Plugin.SecondCam.targetTexture = Plugin.MyDisplay.GetRenderTextureForRenderPass(1);
        }

    }
}
