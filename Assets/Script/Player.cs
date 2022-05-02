using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isGrounded;
    public bool isSprinting;

    private Transform cam;
    private World world;

    public float camSpeed = 5f;
    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float jumpForce = 5f;
    public float gravity = -9.8f;

    public float playerWidth = 0.15f;
    public float playerHeight = 2f;
    public float boundsTolerance = 0.1f;

    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;
    private float verticalMomentum = 0;
    private bool jumpRequest;

    private void Start () {

        cam = GameObject.Find("Main Camera").transform;
        world = GameObject.Find("World").GetComponent<World>();
    }

    private void FixedUpdate () {

        CalculateVelocity ();
        if (jumpRequest) {
            verticalMomentum = 0;
            Jump ();
        }

        CalculateColision ();

        transform.Rotate (Vector3.up * mouseHorizontal * camSpeed);
        cam.Rotate (Vector3.right * -mouseVertical * camSpeed);
        transform.Translate (velocity, Space.World);
    }

    private void Update () {

        GetPlayerInputs ();
    }

    void Jump () {

        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;
    }

    private void CalculateVelocity () {

        // Affect vertical momentum with gravity.
        // if (verticalMomentum > gravity) {
        //     verticalMomentum += Time.fixedDeltaTime * gravity;
        // }
        verticalMomentum += Time.fixedDeltaTime * gravity;

        // if we're sprinting, use the sprinting multiplier.
        if (isSprinting) {
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)).normalized * Time.fixedDeltaTime * sprintSpeed;
        }
        else {
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)).normalized * Time.fixedDeltaTime * walkSpeed;
        }

        // Apply vertical momentum (falling/jumping).
        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;
    }
    

    private void CalculateColision () {

        Vector3 pos = transform.position;
        Vector3 vel = velocity;
        Vector3Int box;
        Vector3 posBox;
        BlockType block;

        world.GetCoordFromPos (pos, out box, out posBox);
        block = world.BlockFromCoord (box);

        // Compute next boxes list (the first must be present one, the last is the one + vel and everyone in the way has a face in common);

        // for every boxes in the list
            // if not a solid pass;
            // else compute norm / sqrt(2)
                // vel += - (norm dot vel) norm;
                // break and CalculateColision all over again;

        // Debug.Log ("ERROR: Inside a solid");


        // BoxelData.faceChecks;
        // world.CheckForVoxel(transform.position + playerWidth);
        // world.CheckForVoxel(transform.position.x, transform.position.y + playerHeight/2, transform.position.z + playerWidth)

        // if ((velocity.z > 0 && front) || (velocity.z < 0 && back)) {
        //     velocity.z = 0;
        // }
        // if ((velocity.x > 0 && right) || (velocity.x < 0 && left)) {
        //     velocity.x = 0;
        // }

        // if (velocity.y <= 0) {
        //     velocity.y = checkDownSpeed(velocity.y);
        // }
        // else if (velocity.y > 0) {
        //     velocity.y = checkUpSpeed(velocity.y);
        // }
    }
    private void GetPlayerInputs () {
        
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");

        if (Input.GetButtonDown("Sprint")) {
            isSprinting = true;
        }
        if (Input.GetButtonUp("Sprint")) {
            isSprinting = false;
        }

        if (isGrounded && Input.GetButton("Jump")) {
            jumpRequest = true;
        }
    }
}

//     private float checkDownSpeed (float downSpeed) {

//         if (
//             world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth) ||
//             world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth) ||
//             world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth) ||
//             world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth) 
//             ) {
            
//             isGrounded = true;
//             return 0f;
//         }
//         else {
            
//             isGrounded = false;
//             return downSpeed;
//         }
//     }

//     private float checkUpSpeed (float UpSpeed) {

//         if (
//             world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + playerHeight + UpSpeed, transform.position.z - playerWidth) ||
//             world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + playerHeight + UpSpeed, transform.position.z + playerWidth) ||
//             world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + playerHeight + UpSpeed, transform.position.z - playerWidth) ||
//             world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + playerHeight + UpSpeed, transform.position.z + playerWidth) 
//             ) {
            
//             return 0f;
//         }
//         else {
            
//             return UpSpeed;
//         }
//     }

//     public bool front {

//         get {
//             if (
//                 world.CheckForVoxel(transform.position.x, transform.position.y, transform.position.z + playerWidth) ||
//                 world.CheckForVoxel(transform.position.x, transform.position.y + playerHeight/2, transform.position.z + playerWidth)
//             ) {
//                 return true;
//             }
//             else {
//                 return false;
//             }
//         }
//     }

//     public bool back {

//         get {
//             if (
//                 world.CheckForVoxel(transform.position.x, transform.position.y, transform.position.z - playerWidth) ||
//                 world.CheckForVoxel(transform.position.x, transform.position.y + playerHeight/2, transform.position.z - playerWidth)
//             ) {
//                 return true;
//             }
//             else {
//                 return false;
//             }
//         }
//     }

//     public bool left {

//         get {
//             if (
//                 world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y, transform.position.z) ||
//                 world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + playerHeight/2, transform.position.z)
//             ) {
//                 return true;
//             }
//             else {
//                 return false;
//             }
//         }
//     }

//     public bool right {

//         get {
//             if (
//                 world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y, transform.position.z) ||
//                 world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + playerHeight/2, transform.position.z)
//             ) {
//                 return true;
//             }
//             else {
//                 return false;
//             }
//         }
//     }
// }
