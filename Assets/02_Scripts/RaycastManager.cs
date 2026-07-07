using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class RaycastManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager _raycastManager;
    [SerializeField] private ARAnchorManager _anchorManager;
    
    [SerializeField] private GameObject _mummyPrefab;

    // 레이에 충돌한 Tracked 오브젝트 정보를 저장하는 컬렉션
    private static readonly List<ARRaycastHit> _hits = new();
    
    void Start()
    {
        _raycastManager = GetComponent<ARRaycastManager>();    
        _anchorManager = GetComponent<ARAnchorManager>();
    }

    void Update()
    {
        var mouse = Mouse.current;
        if (!mouse.leftButton.wasPressedThisFrame) return;
        
        // 스크린 2차원 좌표 산출
        Vector2 screenPos = mouse.position.ReadValue();
        // 레이케스팅
        if (_raycastManager.Raycast(screenPos, _hits, TrackableType.PlaneWithinPolygon))
        {
            // 검출된 바닥의 좌표, 회전값 (Pose)
            // Pose hitPose = _hits[0].pose;
            
            ARRaycastHit hit = _hits[0];
            
            // 객체 생성
            // Instantiate(_mummyPrefab, hitPose.position, hitPose.rotation);
            
            // 앵커 생성 및 객체 증강
            CreateAnchor(hit);
        }
    }

    void CreateAnchor(ARRaycastHit hit)
    {
        ARAnchor anchor = null;
        
        // 히트한 좌표의 객체가 Trackable , Plane
        if (hit.trackable is ARPlane plane)
        {
            // 앵커 생성
            anchor = _anchorManager.AttachAnchor(plane, hit.pose);
        }

        var instance = Instantiate(_mummyPrefab, anchor.transform);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
    }
}
