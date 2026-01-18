using HutongGames.PlayMaker.Actions;
using SFCore.Utils;
using UnityEngine;

namespace PureVesselSkills
{    
    public static class SoundClips
    {
        private static PlayMakerFSM hkPrimeBlastControl = PureVesselSkills.preloadedGO["HKPrimeBlast"].LocateMyFSM("Control");
        
        public static AudioClip blastAudio = (AudioClip)hkPrimeBlastControl.GetAction<AudioPlayerOneShotSingle>("Sound", 1).audioClip.Value;
    }
}