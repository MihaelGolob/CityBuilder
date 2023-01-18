using System;
using UnityEngine;

public class ParticleSystemSelfDestroy : MonoBehaviour {
    private ParticleSystem _particleSystem;

    private void Start() {
        _particleSystem = GetComponent<ParticleSystem>();
        Destroy(gameObject, _particleSystem.main.duration);
    }
}