using System.Collections;
using System.Collections.Generic;
using Ineor.Utils.AudioSystem;
using UnityEngine;

public class AudioOnClick : MonoBehaviour {
    [SerializeField] private AudioCollection audioCollection;

    public void PlayClip(int i) {
        AudioSystem.Instance.PlaySound(audioCollection, audioCollection[i], Vector3.zero);
    }
}
