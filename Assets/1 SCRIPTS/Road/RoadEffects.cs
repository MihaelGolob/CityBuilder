using System.Collections;
using System.Collections.Generic;
using Ineor.Utils.AudioSystem;
using UnityEngine;

public class RoadEffects : MonoBehaviour {
    [Header("Audio effects")]
    [SerializeField] private AudioCollection roadBuildSound;
    [SerializeField] private AudioCollection roadDestroySound;
    
    [Header("Particle effects")]
    [SerializeField] private GameObject roadBuildParticleEffect;

    public void PlayRoadBuildAudioEffect(int clip = -1, bool playSound = true) {
        if (clip >= 0 && playSound) 
            AudioSystem.Instance.PlaySound(roadBuildSound, roadBuildSound[clip], Vector3.zero);
        else if (playSound)
            AudioSystem.Instance.PlaySound(roadBuildSound, Vector3.zero);
    }

    public void PlayRoadBuildParticleEffect(Vector3 position) {
        Instantiate(roadBuildParticleEffect, position, Quaternion.identity);
    }

    public void RoadDestroyEffects(Vector3 position, bool playSound = true) {
        if (!playSound) return;
        AudioSystem.Instance.PlaySound(roadDestroySound, position);
        // Instantiate(roadBuildParticleEffect, position, Quaternion.identity);
    }
}