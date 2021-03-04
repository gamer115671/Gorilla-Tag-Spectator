﻿using System;
using BepInEx;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using UnityEngine.XR;
using System.Collections;
using Cinemachine;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;

namespace MonkeWatcher
{
    [BepInPlugin("org.bepinex.plugins.SpectatorCam", "Spectator Camera", "1.0.0.0")]

    

    
    public class MyPatcher : BaseUnityPlugin
    {
        public void Awake()
        {
            var harmony = new Harmony("com.kfc.monkeytag.spectator");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void OnGUI()
        {
            if (ShowGui)
            {

                if (GUI.Button(new Rect(20, 20, 160, 20), "SPECTATOR"))
                {
                    ShowHide = !ShowHide;
                }

                if (ShowHide)
                {
                    GUI.Box(new Rect(20, 50, 170, 150), "KFC'S Spectator Client, F6 to Hide.");


                    if (GUI.Button(new Rect(25, 80, 160f, 30f), string1))
                    {
                        Spectate = !Spectate;
                    }
                    if (Spectate)
                    {
                        string1 = "Spectator <color=green>ON</color>";

                    }
                    else
                    {
                        string1 = "Spectator <color=red>OFF</color>";

                    }
                    roomCode = GUI.TextArea(new Rect(20, 120, 160, 20), roomCode, 200);

                    if (GUI.Button(new Rect(25, 150, 160f, 30f), "Join Room"))
                    {
                        PhotonNetworkController __instance = PhotonNetworkController.instance;
                        if (PhotonNetwork.InRoom)
                        {
                            PhotonNetwork.LeaveRoom();
                        }

                        __instance.currentGameType = "privatetag";
                        __instance.customRoomID = roomCode;
                        __instance.isPrivate = true;
                        __instance.attemptingToConnect = true;
                        __instance.AttemptToConnectToRoom();

                    }

                    if (PhotonNetwork.InRoom)
                    {
                        GUI.Box(new Rect(20, 200, 170, 30), "Current Room: " + PhotonNetwork.CurrentRoom.Name);
                    }
                    else
                    {
                        GUI.Box(new Rect(20, 200, 170, 30), "Currently Not in a room");
                    }

                   





                }
            }

        }
        public static bool Spectate;
        public static string string1;
        public static string string2;
        public static bool ShowHide = false;
        public static string roomCode = "CAM";
        public static bool ShowGui = true;
        public static bool PlayerMover = false;
    }

    [HarmonyPatch(typeof(PhotonNetworkController))]
    [HarmonyPatch("OnConnectedToMaster", MethodType.Normal)]
    class Join : BaseUnityPlugin
    {
        static void Prefix(PhotonNetworkController __instance)
        {
            __instance.currentGameType = "privatetag";
            __instance.customRoomID = MyPatcher.roomCode;
            __instance.isPrivate = true;
            __instance.attemptingToConnect = true;
            __instance.AttemptToConnectToRoom();
        }
        
    }

        [HarmonyPatch(typeof(GorillaTagger))]
    [HarmonyPatch("LateUpdate", MethodType.Normal)]
    class CameraPatch : BaseUnityPlugin
    {

        static GameObject camParent = new GameObject("CameraParentForShot");
        static CinemachineVirtualCamera cb = FindObjectOfType<CinemachineVirtualCamera>();

       
            static void Free()
        {
            if (!FreeCam)
            {
                FreeCam = true;
                camParent.transform.position = PhotonNetworkController.instance.currentGorillaParent.GetComponentsInChildren<VRRig>()[Current - 1].head.rigTarget.position;
                camParent.transform.rotation = PhotonNetworkController.instance.currentGorillaParent.GetComponentsInChildren<VRRig>()[Current - 1].head.rigTarget.rotation;
                cb.Follow = camParent.transform;
            }
        }
            static void Prefix(GorillaTagger __instance)
        {

            if (MyPatcher.Spectate)
            {

              
                __instance.thirdPersonCamera.transform.SetParent(camParent.transform);

                float x = 0;
                float z = 0;
                float y = 0;

                if (MyPatcher.PlayerMover)
                {
                    
                    camParent.transform.position = GorillaLocomotion.Player.Instance.transform.parent.position;

                }
                if (!(Gamepad.current is null))
                {
                   
                    gamepad = Gamepad.current;

                    if (gamepad != null )
                    {
                        
                        if (gamepad.leftStick.ReadValue().x > 0.1 || gamepad.leftStick.ReadValue().x < -0.1 || gamepad.leftStick.ReadValue().y > 0.1 || gamepad.leftStick.ReadValue().y < -0.1)
                        {
                            Vector2 left = gamepad.leftStick.ReadValue();
                            z = left.y;
                            x = left.x;
                            Free();
                        }

                        if (gamepad.rightStick.ReadValue().x > 0.1 || gamepad.rightStick.ReadValue().x < -0.1 || gamepad.rightStick.ReadValue().y > 0.1 || gamepad.rightStick.ReadValue().y < -0.1)
                        {
                            Vector2 right = gamepad.rightStick.ReadValue();
                            rotX += right.x * sensX;
                            rotY += right.y * sensY;
                            Free();
                        }




                        if (gamepad.rightShoulder.isPressed)
                        {
                            y = 1;
                            Free();
                        }
                        if (gamepad.leftShoulder.isPressed)
                        {
                            y = -1;
                            Free();
                        }
                    }
                }

                if (Keyboard.current.wKey.isPressed)
                {
                   
                    z = 1;
                    Free();
                }
                if (Keyboard.current.cKey.isPressed)
                {
                    Free();
                }
                if (Keyboard.current.aKey.isPressed)
                {
                    
                    x = -1;
                    Free();
                }
                if (Keyboard.current.sKey.isPressed)
                {
                    
                    z = -1;
                    Free();
                }
                if (Keyboard.current.dKey.isPressed)
                {
                    
                    x = 1;
                    Free();
                }
                if (Keyboard.current.eKey.isPressed)
                {
                    y = 1;
                    Free();
                }
                if (Keyboard.current.qKey.isPressed)
                {
                    y = -1;
                    Free();
                }

                if (Keyboard.current.leftArrowKey.isPressed)
                {
                    rotX -= 1 * sensX;
                    Free();
                }
                if (Keyboard.current.rightArrowKey.isPressed)
                {
                    rotX += 1 * sensX;
                    Free();
                }
                if (Keyboard.current.upArrowKey.isPressed)
                {
                    rotY += 1 * sensY;
                    Free();
                }
                if (Keyboard.current.downArrowKey.isPressed)
                {
                    rotY -= 1 * sensY;
                    Free();
                }
                if (Keyboard.current.f6Key.isPressed)
                {
                    if (keyP == false)
                    {
                        MyPatcher.ShowGui = !MyPatcher.ShowGui;
                    }
                    keyP = true;
                }
                else
                {
                    keyP = false;
                }
                    if (Keyboard.current.digit1Key.isPressed)
                    {
                        Current = 1;
                        FreeCam = false;
                    }
                    if (Keyboard.current.digit2Key.isPressed)
                    {
                        Current = 2;
                        FreeCam = false;
                    }
                    if (Keyboard.current.digit3Key.isPressed)
                    {
                        Current = 3;
                        FreeCam = false;
                    }
                    if (Keyboard.current.digit4Key.isPressed)
                    {
                        Current = 4;
                        FreeCam = false;
                    }
                    if (Keyboard.current.digit5Key.isPressed)
                    {
                        Current = 5;
                        FreeCam = false;
                    }
                    if (Keyboard.current.digit6Key.isPressed)
                    {
                        Current = 6;
                        FreeCam = false;
                    }
                    if (Keyboard.current.digit7Key.isPressed)
                    {
                        Current = 7;
                        FreeCam = false;
                    }
                    if (Keyboard.current.digit8Key.isPressed)
                    {
                        Current = 8;
                        FreeCam = false;
                    }


                if (FreeCam)
                {

                    rotY = Mathf.Clamp(rotY, -90, 90);

                camParent.transform.rotation = Quaternion.Euler(-rotY, rotX, 0);
                
                Vector3 dir = camParent.transform.right * x + camParent.transform.up * y + camParent.transform.forward * z;
                    if (!(gamepad is null))
                    {
                        if (gamepad.rightTrigger.isPressed)
                        {
                            camParent.transform.position += dir * 20 * Time.deltaTime;
                        }
                        if (gamepad.leftTrigger.isPressed)
                        {
                            camParent.transform.position += dir * 5 * Time.deltaTime;
                        }
                        else
                        {
                            camParent.transform.position += dir * 10 * Time.deltaTime;
                        }

                    }
                    else if(Keyboard.current.shiftKey.isPressed)
                    {
                        camParent.transform.position += dir * 20 * Time.deltaTime;
                    }
                    else
                    {
                        camParent.transform.position += dir * 10 * Time.deltaTime;
                    }

                if (cb != null && !MyPatcher.PlayerMover)
                {
                    cb.Follow = camParent.transform;
                }

                    if (MyPatcher.PlayerMover)
                    {
                        GorillaLocomotion.Player.Instance.transform.parent.position = camParent.transform.position;
                        cb.Follow = GorillaLocomotion.Player.Instance.headCollider.transform;

                    }
                }
                else
                {
                   // Debug.Log(PhotonNetworkController.instance.currentGorillaParent.GetComponentsInChildren<VRRig>().Length);
                    if (MyPatcher.Spectate && FreeCam == false)
                    {
                        if(Current > PhotonNetworkController.instance.currentGorillaParent.GetComponentsInChildren<VRRig>().Length)
                        {
                            Current = PhotonNetworkController.instance.currentGorillaParent.GetComponentsInChildren<VRRig>().Length;
                        }
                        cb.Follow = PhotonNetworkController.instance.currentGorillaParent.GetComponentsInChildren<VRRig>()[Current-1].head.rigTarget;
                        
                    }
                }
            }
            else
            {
                cb.Follow = GorillaLocomotion.Player.Instance.headCollider.transform;
            }





       
        }
        public static float sensX = 1f;
        public static float sensY = 1f;

        public static float rotX;
        public static float rotY;
        public static bool FreeCam = true;
        public static int Current = 1;

        public static bool keyP = false;
        

        public static Gamepad gamepad;

        
    }

}