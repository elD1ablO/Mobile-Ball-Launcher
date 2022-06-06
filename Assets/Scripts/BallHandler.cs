using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] GameObject ballPrefab;
    [SerializeField] Rigidbody2D pivot;
    [SerializeField] float detachDelay;
    [SerializeField] float respawnDelay;

    Camera cam;
    Rigidbody2D currentBallRB;
    SpringJoint2D currentSpringJoint;

    bool isDragging;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        SpawnNewBall();
    }

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if( currentBallRB == null ) { return; }

        if (Touch.activeTouches.Count == 0)
        {
            if (isDragging)
            {
                LaunchBall();
            }
            isDragging = false;
            
            return;
        }

        isDragging = true;
        currentBallRB.isKinematic = true;

        Vector2 touchPosition = new();

        foreach (Touch touch in Touch.activeTouches)
        {
            touchPosition += touch.screenPosition;
        }

        touchPosition /= Touch.activeTouches.Count;

        Vector3 worldPosition = cam.ScreenToWorldPoint(touchPosition);
        currentBallRB.position = worldPosition;
        
    }

    void SpawnNewBall()
    {
        GameObject ballInstance =  Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        currentBallRB = ballInstance.GetComponent<Rigidbody2D>();
        currentSpringJoint = ballInstance.GetComponent<SpringJoint2D>();

        currentSpringJoint.connectedBody = pivot;
    }
    void LaunchBall()
    {
        currentBallRB.isKinematic = false;
        currentBallRB = null;

        Invoke(nameof(DetachBall), detachDelay);       

    }

    void DetachBall()
    {
        currentSpringJoint.enabled = false;
        currentSpringJoint = null;

        Invoke(nameof(SpawnNewBall), respawnDelay);
    }
}
