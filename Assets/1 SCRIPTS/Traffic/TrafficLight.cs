using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrafficLight : MonoBehaviour {
    // inspector assigned
    [SerializeField] private GameObject redLight;
    [SerializeField] private GameObject yellowLight;
    [SerializeField] private GameObject greenLight;

    private bool _isGreen;

    public bool IsGreen => _isGreen;

    #region unity methods

    private void Awake() {
        redLight.SetActive(true);
        yellowLight.SetActive(false);
        greenLight.SetActive(false);

        _isGreen = false;
    }

    #endregion
    
    #region public methods

    public void SetRedLight() => StartCoroutine(SetRedLightInternal());

    public void SetGreenLight() => StartCoroutine(SetGreenLightInternal());

    #endregion
    
    #region private methods

    private IEnumerator SetRedLightInternal() {
        if (!_isGreen) yield return null;
        
        yellowLight.SetActive(true);
        yield return new WaitForSeconds(2f);
        greenLight.SetActive(false);
        yellowLight.SetActive(false);
        redLight.SetActive(true);

        _isGreen = false;
    }

    private IEnumerator SetGreenLightInternal() {
        if (_isGreen) yield return null;
        
        redLight.SetActive(false);
        yellowLight.SetActive(true);
        yield return new WaitForSeconds(2f);
        yellowLight.SetActive(false);
        greenLight.SetActive(true);

        _isGreen = true;
    }
    
    #endregion
}