using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPerson : MonoBehaviour
{

    enum MovementState
    {
        WALKING, SWINGING, PULLING
    }

    [SerializeField] float mouse_sensitivity = 1f;
    [SerializeField] Transform debug_hit_transform;

    CharacterController character_controller;
    float camera_verticle_angle;
    float character_velocity_y;
    private Vector3 character_momentum;
    [SerializeField] Camera camera;
    float jump_speed = 30;
    float tether_length = 0.0f;

    MovementState state = MovementState.WALKING;


    Vector3 hook_position;

    // Start is called before the first frame update
    void Start()
    {
        character_controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        switch (state)
        {
            case MovementState.WALKING:
                {
                    HandleCharacterLook();
                    HandleCharacterMovement();
                    HandleHookStart();
                }
                break;
            case MovementState.SWINGING:
                {
                    HandleCharacterLook();
                    SwingMovement();
                }
                break;
            case MovementState.PULLING:
                {
                    HandleCharacterLook();
                    PullingMovement();
                }
                break;
            default:
                break;
        }



    }

    void HandleCharacterLook()
    {
        float look_x = Input.GetAxisRaw("Mouse X");
        float look_y = Input.GetAxisRaw("Mouse Y");

        //rotate transform aroun Y axis
        transform.Rotate(new Vector3(0f, look_x * mouse_sensitivity, 0f), Space.Self);

        camera_verticle_angle -= look_y * mouse_sensitivity;

        camera_verticle_angle = Mathf.Clamp(camera_verticle_angle, -89f, 89f);

        camera.transform.localEulerAngles = new Vector3(camera_verticle_angle, 0, 0);
    }

    void HandleCharacterMovement()
    {
        float move_x = Input.GetAxisRaw("Horizontal");
        float move_z = Input.GetAxisRaw("Vertical");

        float move_speed = 20f;

        Vector3 character_velocity = transform.right * move_x * move_speed + transform.forward * move_z * move_speed;

        if(character_controller.isGrounded)
        {
            character_velocity_y = 0f;

            if(JumpInput())
            {
                
                character_velocity_y = jump_speed;

            }
        }

        float gravity_down_force = -60f;
        character_velocity_y += gravity_down_force * Time.deltaTime;

        //apply gravity
        character_velocity.y = character_velocity_y;

        //apply momentum
        character_velocity += character_momentum;

        //Move the character
        character_controller.Move(character_velocity * Time.deltaTime);

        if (character_momentum.magnitude >= 0f)
        {
            float drag = 3f;
            character_momentum -= character_momentum * drag * Time.deltaTime;

            if (character_momentum.magnitude < 0f)
                character_momentum = Vector3.zero;
        }

    }


    void HandleHookStart()
    {
        if(HookInput())
        {
            if(Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit))
            {

                hook_position = hit.point;

                if (!character_controller.isGrounded)
                {
                   // state = MovementState.SWINGING;
                   // tether_length = Vector3.Distance(transform.position, hook_position);
                }
                else
                {
                    state = MovementState.PULLING;
                }
            }
        }
    }

    void PullingMovement()
    {
        Vector3 hook_dir = (hook_position - transform.position).normalized;

        float pull_speed = Mathf.Clamp( Vector3.Distance(transform.position, hook_position),30, 100);

        //move towards
        character_controller.Move(hook_dir * pull_speed * 2 * Time.deltaTime);

        float cut_distance = 1f;
        if(Vector3.Distance(transform.position, hook_position) < cut_distance)
        {
            state = MovementState.WALKING;
            ResetGravity();
        }

        //stop pulling and fall
        if(HookInput())
        {
            state = MovementState.WALKING;
            ResetGravity();
        }

        //cancel the pull with a jump

        if(JumpInput())
        {

            float boost = 3f;
            

            character_momentum = hook_dir * pull_speed * boost;
            character_momentum += Vector3.up * jump_speed;

            state = MovementState.WALKING;
            ResetGravity();
        }
    }

    void SwingMovement()
    {
        Vector3 hook_dir;

        if (Vector3.Distance(hook_position, transform.position) > tether_length)
        {
            hook_dir = (hook_position - transform.position).normalized * tether_length;
        }
        else
        {
            hook_dir = (hook_position - transform.position).normalized;
        }



        float gravity_down_force = -60f;
        character_velocity_y += gravity_down_force * Time.deltaTime;

        //apply gravity
        hook_dir.y = character_velocity_y;

        float pull_speed = Mathf.Clamp(Vector3.Distance(transform.position, hook_position), 30, 100);

        //move towards
        character_controller.Move(hook_dir * pull_speed * 2 * Time.deltaTime);

        float cut_distance = 1f;
        if (Vector3.Distance(transform.position, hook_position) < cut_distance)
        {
            state = MovementState.WALKING;
            ResetGravity();
        }

        //stop pulling and fall
        if (HookInput())
        {
            state = MovementState.WALKING;
            ResetGravity();
        }

        //cancel the pull with a jump

        if (JumpInput())
        {

            float boost = 3f;


            character_momentum = hook_dir * pull_speed * boost;
            character_momentum += Vector3.up * jump_speed;

            state = MovementState.WALKING;
            ResetGravity();
        }
    }


    bool HookInput()
    {
        return Input.GetMouseButtonDown(0);
    }

    bool JumpInput()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    void ResetGravity()
    {
        character_velocity_y = 0f;
    }

}
