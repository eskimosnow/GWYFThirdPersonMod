using BepInEx;
using UnityEngine;
using UnityEngine.InputSystem;

[BepInPlugin("gwyf.thirdperson.camera", "GWYF Third Person Camera", "1.2.2")]
public class Class1 : BaseUnityPlugin
{
    private GameObject player;
    private GameObject cam;

    // --- ADDED: zoom state ---
    private float camDistance = -2.0f;
    private float minDistance = -4.0f;
    private float maxDistance = -0.5f;
    private float scrollSpeed = 0.5f;

    void Update()
    {
        // --- CACHE RUNTIME OBJECTS ---

        if (player == null)
            player = GameObject.Find("Player(Clone)");

        if (cam == null)
            cam = GameObject.Find("Camera(Clone)");

        if (player == null || cam == null)
            return;

        // --- ADDED: scroll wheel zoom control ---
        float scroll = Mouse.current.scroll.y.ReadValue();
        scroll *= 0.20f; // scale it down to match old sensitivity

        if (scroll != 0f)
        {
            camDistance += scroll * scrollSpeed;
            camDistance = Mathf.Clamp(camDistance, minDistance, maxDistance);
        }

        // --- CAMERA CONTROL ---

        var breathing = cam.GetComponent("CameraBreathing") as Behaviour;

        if (breathing != null)
            breathing.enabled = false;

        // Fixed shoulder third-person offset (modified ONLY Z distance)
        cam.transform.localPosition = new Vector3(
            0.6f,   // shoulder offset
            0.3f,   // height tweak
            camDistance   // now dynamic
        );

        // --- FORCE CAMERA TO RENDER PLAYER LAYERS ---

        Camera cameraComponent = cam.GetComponent<Camera>();

        if (cameraComponent != null)
        {
//            cameraComponent.cullingMask |= (1 << 6);
            cameraComponent.cullingMask |= (1 << 13);
        }

        // --- ADJUST INTERACTION RANGE (STABLE FRAME-WINNING PATCH) ---

        var interact = player.GetComponent("PlayerInteract");

        if (interact != null)
        {
            var field = interact.GetType().GetField("raycastDistance");

            if (field != null)
            {
                field.SetValue(interact, 6.0f);
            }
        }
    }
}