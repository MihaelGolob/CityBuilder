using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Intersection : MonoBehaviour {
    [SerializeField] private List<TrafficLight> _trafficLights;
    [SerializeField] private List<Collider> _colliders;
    [SerializeField] private float _changeInterval = 15f;

    private float _randomOffset;
    private bool _firstPairGreen = true;
    private float _timer;

    private void Start() {
        _trafficLights[0].SetGreenLight();
        _trafficLights[2].SetGreenLight();
        _colliders[0].gameObject.SetActive(false);
        _colliders[2].gameObject.SetActive(false);

        _randomOffset = Random.Range(1, 10);
        _timer = _changeInterval + _randomOffset;
    }

    private void Update() {
        _timer -= Time.deltaTime;
        if (_timer >= 0) return;
        
        ToggleLights();
        _timer = _changeInterval;
    }

    private void ToggleLights() {
        if (_firstPairGreen) {
            _trafficLights[0].SetRedLight();
            _trafficLights[2].SetRedLight();
            
            
            _trafficLights[1].SetGreenLight();
            if (_trafficLights.Count > 3)
                _trafficLights[3].SetGreenLight();
        } 
        else {
            _trafficLights[0].SetGreenLight();
            _trafficLights[2].SetGreenLight();
            
            
            _trafficLights[1].SetRedLight();
            if (_trafficLights.Count > 3)
                _trafficLights[3].SetRedLight();
        }

        StartCoroutine(ToggleColliders());
        _firstPairGreen = !_firstPairGreen;
    }

    private IEnumerator ToggleColliders() {
        yield return new WaitForSeconds(2.1f);
        var old = !_firstPairGreen;

        if (old) {
            _colliders[0].gameObject.SetActive(true);
            _colliders[2].gameObject.SetActive(true); 
            
            _colliders[1].gameObject.SetActive(false);
            if (_colliders.Count > 3)
                _colliders[3].gameObject.SetActive(false); 
        }
        else {
            _colliders[0].gameObject.SetActive(false);
            _colliders[2].gameObject.SetActive(false);
            
            _colliders[1].gameObject.SetActive(true);
            if (_colliders.Count > 3)
                _colliders[3].gameObject.SetActive(true);
        }
    }
}