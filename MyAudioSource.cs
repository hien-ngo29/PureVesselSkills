using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using SFCore.Utils;
using Random = UnityEngine.Random;
using System.Collections;
using HutongGames.PlayMaker.Actions;


namespace PureVesselSkills
{
    public static class PureVesselAudioSource
    {
        public static void Init()
        {
            GameObject obj = new GameObject("PureVesselAudioSource");
            obj.AddComponent<AudioSource>();
            GameObject.DontDestroyOnLoad(obj);
        }

        public static void DestroySelf()
        {
            GameObject.Destroy(GameObject.Find("PureVesselAudioSource"));
        }
    }
}
