using System;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] private float moveStep = 0.5f;
    [Header("Restrictions")] 
    [SerializeField] private Vector2 maxHorizontalMovement = new(-30, 30);
    [SerializeField] private Vector2 maxVerticalMovement = new(-30, 30);
    [SerializeField] private Vector2 maxScrollMovement = new(15, 30);

    private Camera _camera;
    private bool _cameraExists;

    private void Start() {
        _camera = Camera.main;
        if (_camera != null) _cameraExists = true;
    }

    private void Update() {
        if (!_cameraExists) return;

        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        var pos = _camera.transform.position;
        // apply movement
        var newpos = pos + Vector3.forward * vertical + Vector3.right * horizontal;
        // apply scroll wheel
        var scroll = Input.mouseScrollDelta.y;
        if ((pos.y < maxScrollMovement.y && scroll < 0) || (pos.y > maxScrollMovement.x && scroll > 0))
            newpos += _camera.transform.forward * (moveStep * Input.mouseScrollDelta.y);
        // apply restrictions
        var restrictedX = Mathf.Clamp(newpos.x, maxHorizontalMovement.x, maxHorizontalMovement.y);
        var restrictedY = Mathf.Clamp(newpos.y, maxScrollMovement.x, maxScrollMovement.y);
        var restrictedZ = Mathf.Clamp(newpos.z, maxVerticalMovement.x, maxVerticalMovement.y);

        _camera.transform.position = new Vector3(restrictedX, restrictedY, restrictedZ);
    }
}