using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour {
    // singleton pattern
    private static CoroutineRunner _instance;
    public static CoroutineRunner Instance => _instance;
    
    #region UNITY methods
    
    private void Awake() {
        // setup singleton
        if (_instance != null && _instance != this)
            Destroy(this);
        else
            _instance = this;
    }
    
    #endregion
    
    #region public api

    public void DelayedCall(Action callback, float delay) => StartCoroutine(DelayedCallInternal(callback, delay));
    
    #endregion
    
    #region private methods
    
    private IEnumerator DelayedCallInternal(Action callback, float delay) {
        yield return new WaitForSeconds(delay);
        callback();
    }
    
    #endregion
   
}