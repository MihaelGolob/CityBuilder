using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectSystem : MonoBehaviour {
    // inspector assigned
    [SerializeField] private RoadRenderer roadRenderer;

    // public
    public bool LockSelection { get; set; }

    // private
    private static Grid _grid;
    private Camera _mainCamera;

    // mouse click
    private bool _mouseDown;
    private bool _mouseUp;

    private Vector3 _firstTilePosition;
    private Orientation _lockBuildingDirection = Orientation.None;

    private LayerMask _raycastMask;

    #region Unity Methods

    private void Start() {
        _grid = GetComponent<Grid>();
        _mainCamera = Camera.main;

        _raycastMask = LayerMask.GetMask("Ground");
    }

    #endregion

    #region public methods

    public void Build() {
        if (LockSelection) return;

        var gridPos = GetMouseGridPosition();

        // check for pressed mouse button
        if (Input.GetMouseButtonDown(0)) {
            _mouseDown = true;
            _firstTilePosition = gridPos;
        }

        // check for released mouse button
        if (Input.GetMouseButtonUp(0)) {
            _mouseUp = true;
        }

        // create preview tile
        if (_mouseDown && !_mouseUp) {
            // set the direction of building
            if (_lockBuildingDirection == Orientation.None && _firstTilePosition != gridPos) {
                _lockBuildingDirection = CheckDirection(gridPos);
            }

            // only build in one direction
            if (_lockBuildingDirection is Orientation.Down or Orientation.Up)
                roadRenderer.AddPreviewRoad(new Vector3(_firstTilePosition.x, gridPos.y, gridPos.z), false);
            else if (_lockBuildingDirection is Orientation.Left or Orientation.Right)
                roadRenderer.AddPreviewRoad(new Vector3(gridPos.x, gridPos.y, _firstTilePosition.z), false);
        }
        else if (!_mouseDown && !_mouseUp) {
            roadRenderer.AddPreviewRoad(gridPos);
        }


        if (_mouseDown && _mouseUp && !LockSelection) {
            if (roadRenderer.RoadExists(gridPos)) return;
            // put down a new road
            roadRenderer.AddRoads();

            // reset mouse click
            _mouseDown = false;
            _mouseUp = false;

            // reset lock building direction
            _lockBuildingDirection = Orientation.None;
        }
    }

    public void Destroy() {
        var gridPos = GetMouseGridPosition();
        // highlight tile to destroy
        roadRenderer.HighlightRoadToDestroy(gridPos);

        if (Input.GetMouseButtonDown(0) && !LockSelection) {
            if (!roadRenderer.RoadExists(gridPos)) return;

            // destroy tile
            roadRenderer.RemoveRoad(gridPos);
        }
    }

    public void ChangeStateTransitions() {
        roadRenderer.RemovePreviewRoad();
    }
    
    public static Vector3 ToCellPosition(Vector3 worldPosition) {
        return _grid == null ? Vector3.zero : _grid.WorldToCell(worldPosition);
    }

    #endregion

    #region private methods

    private Orientation CheckDirection(Vector3 pos) {
        var dir = Orientation.None;

        if (Math.Abs(pos.x - _firstTilePosition.x) < 0.2f && pos.z > _firstTilePosition.z) dir = Orientation.Up;
        else if (Math.Abs(pos.x - _firstTilePosition.x) < 0.2f && pos.z < _firstTilePosition.z) dir = Orientation.Down;
        else if (pos.x > _firstTilePosition.x && Math.Abs(pos.z - _firstTilePosition.z) < 0.2f) dir = Orientation.Right;
        else if (pos.x < _firstTilePosition.x && Math.Abs(pos.z - _firstTilePosition.z) < 0.2f) dir = Orientation.Left;

        return dir;
    }

    private Vector3 GetMouseGridPosition() {
        var mousePosition = Input.mousePosition;
        var mouseWorldPosition = _mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10));

        var ray = _mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out var hit, _mainCamera.farClipPlane, _raycastMask)) {
            mouseWorldPosition = hit.point;
        }

        var cellPosition = _grid.WorldToCell(mouseWorldPosition);
        var cellCenter = _grid.GetCellCenterWorld(cellPosition);

        return cellCenter;
    }

    #endregion
}