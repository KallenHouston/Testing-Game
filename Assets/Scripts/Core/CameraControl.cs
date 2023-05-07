using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Room camera variables
    [SerializeField] private float speed; // The speed at which the camera moves
    private float currentPosX; // The current X position of the camera
    //private Vector3 velocity = Vector3.zero; // The velocity of the camera

    // Follow player variables
    [SerializeField] private Transform player; // The player object that the camera follows
    [SerializeField] private float aheadDistance; // The initial distance in front of the player that the camera looks
    [SerializeField] private float upDistance; // The inital distance above the player that the camera looks
    [SerializeField] private float cameraSpeed; // The speed at which the camera follows the player
    [SerializeField] private float minCameraX; // The minimum X position of the camera
    [SerializeField] private float maxCameraX; // The maximum X position of the camera
    [SerializeField] private float minCameraY; // The minimum Y position of the camera
    [SerializeField] private float maxCameraY; // The maximum Y position of the camera
    private float lookAhead; // The current distance that the camera is looking ahead of the player
    private float lookUp; // The current distance that the camera is looking above the player

    private void Update()
    {
        // Room camera
        // Move the camera smoothly to the current X position
        // transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPosX, transform.position.y, transform.position.z), ref velocity, speed);

        // Follow player
        // Move the camera to follow the player, clamping (constraining) its position within the specified limits
        transform.position = new Vector3(Mathf.Clamp(player.position.x + lookAhead, -minCameraX, maxCameraX), 
                                         Mathf.Clamp(player.position.y + lookUp, -minCameraY, maxCameraY), 
                                         transform.position.z);
        // Move the camera ahead of the player by an amount proportional to the player's scale, using Lerp for smoothness
        lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * cameraSpeed);
        // Move the camera above the player by an amount proportional to the player's scale, using Lerp for smoothness
        lookUp = Mathf.Lerp(lookUp, (upDistance * player.localScale.y), Time.deltaTime * cameraSpeed);
    }

    // Move the camera to a new room
   public void MoveToNewRoom(Transform _newRoom)
    {
        currentPosX = _newRoom.position.x;
    }
}