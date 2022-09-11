using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const int W = 0x01;
    private const int S = 0x02;
    private const int A = 0x04;
    private const int D = 0x08;

    public const int NORTH = 0;
    public const int WEST = 1;
    public const int SOUTH = 2;
    public const int EAST = 3;
    
    private const float mass = 174.6f; // kg for a Kawasaki Ninja 300
    private const float one_over_mass = 1.0f / mass;
    private const float timestep = 1.0f / 60.0f;
    private const float max_velocity = 200.0f;
    private const float max_acceleration = 100.0f;
    private const float max_force = 5000.0f;

    private GameObject player;
    private Vector3 initial_position;
    private Vector3 oldPosition;
    private Vector3 new_position;
    public Vector3 rot;
    private int initial_dir;
    public int dir;
    private Vector3 direction;
    private Vector3 new_direction;
    private Vector3 initial_velocity;
    public Vector3 velocity;
    private Vector3 initial_acceleration;
    private Vector3 acceleration;
    private Vector3 initial_force;
    private Vector3 force;
    private bool rotate_left = false;
    private bool rotate_right = false;
    private AudioSource motor;
    private AudioSource turning;
    float minPitch = 1.0f;
    float maxPitch = 2.75f;
    private float pitchModifier;
    int gear = 1;
    int controls;
    private GameObject cam;

    Vector3 rotate_vector(float angle, Vector3 vec)
    {
        Vector3 result = new Vector3();
        float c = Mathf.Cos(Mathf.PI * angle / 180.0f);
        float s = -Mathf.Sin(Mathf.PI * angle / 180.0f);
        float x = vec.x;
        float z = vec.z;
        result.x = x * c - z * s;
        result.z = x * s + z * c;

        return result;
    }

    void light_cycle_update(float dt)
    {
        Vector3 k1, k2, k3, k4;
        Vector3 l1, l2, l3, l4;

        const float half = 1.0f / 2.0f;
        const float third = 1.0f / 3.0f;
        const float sixth = 1.0f / 6.0f;

        acceleration.x = force.x * one_over_mass;
        acceleration.y = force.y * one_over_mass;
        acceleration.z = force.z * one_over_mass;

        k1.x = dt * velocity.x;
        k1.y = dt * velocity.y;
        k1.z = dt * velocity.z;

        l1.x = dt * acceleration.x;
        l1.y = dt * acceleration.y;
        l1.z = dt * acceleration.z;

        k2.x = dt * (velocity.x + k1.x * half);
        k2.y = dt * (velocity.y + k1.y * half);
        k2.z = dt * (velocity.z + k1.z * half);

        l2.x = dt * acceleration.x;
        l2.y = dt * acceleration.y;
        l2.z = dt * acceleration.z;

        k3.x = dt * (velocity.x + k2.x * half);
        k3.y = dt * (velocity.y + k2.y * half);
        k3.z = dt * (velocity.z + k2.y * half);

        l3.x = dt * acceleration.x;
        l3.y = dt * acceleration.y;
        l3.z = dt * acceleration.z;

        k4.x = dt * (velocity.x + k3.x);
        k4.y = dt * (velocity.y + k3.y);
        k4.z = dt * (velocity.z + k3.z);

        l4.x = dt * acceleration.x;
        l4.y = dt * acceleration.y;
        l4.z = dt * acceleration.z;

        new_position.x = new_position.x + k1.x * sixth + k2.x * third + k3.x * third + k4.x * sixth;
        new_position.y = new_position.y + k1.y * sixth + k2.y * third + k3.y * third + k4.y * sixth;
        new_position.z = new_position.z + k1.z * sixth + k2.z * third + k3.z * third + k4.z * sixth;

        velocity.x = velocity.x + l1.x * sixth + l2.x * third + l3.x * third + l4.x * sixth;
        velocity.y = velocity.y + l1.y * sixth + l2.y * third + l3.y * third + l4.y * sixth;
        velocity.z = velocity.z + l1.z * sixth + l2.z * third + l3.z * third + l4.z * sixth;

        if (dir == NORTH)
        {
            if (velocity.z >= max_velocity)
            {
                velocity.z = max_velocity;
            }

            if (velocity.z <= 0.0f)
            {
                velocity.z = 0.0f;
            }

            if (acceleration.z >= max_acceleration)
            {
                acceleration.z = max_acceleration;
            }

            if (acceleration.z <= 0.0f)
            {
                acceleration.z = 0.0f;
            }

            if (force.z >= max_force)
            {
                force.z = max_force;
            }

            if (force.z <= -max_force)
            {
                force.z = -max_force;
            }
        }
        else if (dir == SOUTH)
        {
            if (velocity.z <= -max_velocity)
            {
                velocity.z = -max_velocity;
            }

            if (velocity.z >= 0.0f)
            {
                velocity.z = 0.0f;
            }

            if (acceleration.z <= -max_acceleration)
            {
                acceleration.z = -max_acceleration;
            }

            if (acceleration.z >= 0.0f)
            {
                acceleration.z = 0.0f;
            }

            if (force.z <= -max_force)
            {
                force.z = -max_force;
            }

            if (force.z >= max_force)
            {
                force.z = max_force;
            }
        }
        else if (dir == WEST)
        {
            if (velocity.x <= -max_velocity)
            {
                velocity.x = -max_velocity;
            }

            if (velocity.x >= 0.0f)
            {
                velocity.x = 0.0f;
            }

            if (acceleration.x <= -max_acceleration)
            {
                acceleration.x = -max_acceleration;
            }

            if (acceleration.x >= 0.0f)
            {
                acceleration.x = 0.0f;
            }

            if (force.x <= -max_force)
            {
                force.x = -max_force;
            }

            if (force.x >= max_force)
            {
                force.x = max_force;
            }
        }
        else if (dir == EAST)
        {
            if (velocity.x >= max_velocity)
            {
                velocity.x = max_velocity;
            }

            if (velocity.x <= 0.0f)
            {
                velocity.x = 0.0f;
            }

            if (acceleration.x >= max_acceleration)
            {
                acceleration.x = max_acceleration;
            }

            if (acceleration.x <= 0.0f)
            {
                acceleration.x = 0.0f;
            }

            if (force.x >= max_force)
            {
                force.x = max_force;
            }

            if (force.x <= -max_force)
            {
                force.x = -max_force;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        initial_dir = NORTH;
        dir = initial_dir;
        direction = new Vector3(0.0f, 0.0f, 1.0f);
        direction.Normalize();
        new_direction = new Vector3(direction.x, direction.y, direction.z);
        rot = transform.rotation.eulerAngles;
        initial_position = new Vector3(0f, 0f, -1890f);
        new_position = initial_position;
        initial_velocity = new Vector3(0f, 0f, 0f);
        initial_acceleration = new Vector3(0f, 0f, 0f);
        initial_force = new Vector3(0.0f, 0.0f, 0.0f);
        force = initial_force;
        motor = GameObject.Find("light cycle motor").GetComponent<AudioSource>();
        turning = GameObject.Find("turning").GetComponent<AudioSource>();
        motor.Play(0);
        gear = 1;
        cam = GameObject.FindGameObjectWithTag("VR Camera");
    }

    void Update()
    {
        oldPosition = player.transform.position;
        light_cycle_update(timestep);

        Vector2 meta_quest_2_primary_thumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        Vector2 meta_quest_2_secondary_thumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        float meta_quest_2_primary_hand_trigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
        float meta_quest_2_primary_index_trigger= OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        float meta_quest_2_secondary_hand_trigger = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);
        float meta_quest_2_secondary_index_trigger = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);

        //newPosition = new Vector3(oldPosition.x + new_direction.x * speed, oldPosition.y + new_direction.y * speed, oldPosition.z + new_direction.z * speed);

        if ((Input.GetKeyDown(KeyCode.W)) || (meta_quest_2_primary_thumbstick.y >= 0.5f))// || (meta_quest_2_secondary_thumbstick.y >= 0.5f))
        {
            controls |= W;
        }

        if (Input.GetKeyDown(KeyCode.S) || (meta_quest_2_primary_thumbstick.y <= -0.5f))// || (meta_quest_2_secondary_thumbstick.y <= -0.5f))
        {
            controls |= S;
        }

        if (Input.GetKeyUp(KeyCode.W) || (meta_quest_2_primary_thumbstick.y < 0.5f))// || (meta_quest_2_secondary_thumbstick.y < 0.5f))
        {
            controls ^=  W;
        }

        if (Input.GetKeyUp(KeyCode.S) || (meta_quest_2_primary_thumbstick.y > -0.5f))// || (meta_quest_2_secondary_thumbstick.y > -0.5f))
        {
            controls ^= S;
        }

        if (controls == 0)
        {
            if (dir == NORTH)
            {
                force.z = 0.0f;
                velocity.z -= 0.01f;
                if (velocity.z <= 0.0f)
                    velocity.z = 0.0f;
                if (force.z <= 0.0f)
                    force.z = 0.0f;

                if (gear == 4 && velocity.z <= 100f)
                    gear -= 1;
                else if (gear == 3 && velocity.z <= 75f)
                    gear -= 1;
                else if (gear == 2 && velocity.z <= 50f)
                    gear -= 1;

                if (gear <= 1)
                    gear = 1;
            }
            else if (dir == SOUTH)
            {
                force.z = 0.0f;
                velocity.z += 0.01f;

                if (velocity.z >= 0.0f)
                    velocity.z = 0.0f;
                if (force.z >= 0.0f)
                    force.z = 0.0f;

                if (gear == 4 && velocity.z >= -100f)
                    gear -= 1;
                else if (gear == 3 && velocity.z >= -75f)
                    gear -= 1;
                else if (gear == 2 && velocity.z >= -50f)
                    gear -= 1;

                if (gear <= 1)
                    gear = 1;
            }
            else if (dir == EAST)
            {
                force.x = 0.0f;
                velocity.x -= 0.01f;

                if (velocity.x <= 0.0f)
                    velocity.x = 0.0f;
                if (force.x <= 0.0f)
                    force.x = 0.0f;

                if (gear == 4 && velocity.x <= 100f)
                    gear -= 1;
                else if (gear == 3 && velocity.x <= 75f)
                    gear -= 1;
                else if (gear == 2 && velocity.x <= 50f)
                    gear -= 1;

                if (gear <= 1)
                    gear = 1;
            }
            else if (dir == WEST)
            {
                force.x = 0.0f;
                velocity.x += 0.01f;

                if (velocity.x >= 0.0f)
                    velocity.x = 0.0f;
                if (force.x >= 0.0f)
                    force.x = 0.0f;

                if (gear == 4 && velocity.x >= -100f)
                    gear -= 1;
                else if (gear == 3 && velocity.x >= -75f)
                    gear -= 1;
                else if (gear == 2 && velocity.x >= -50f)
                    gear -= 1;

                if (gear <= 1)
                    gear = 1;
            }
        }

        if (controls == W)
        {
            if (dir == NORTH)
            {
                force.z += 500.0f;
                if (force.z >= max_force)
                    force.z = max_force;
                if (velocity.z >= max_velocity)
                    velocity.z = max_velocity;

                if (gear == 1 && velocity.z >= 50f)
                {
                    velocity.z = 10.0f;
                    gear += 1;
                }
                if (gear == 2 && velocity.z >= 75f)
                {
                    velocity.z = 25.0f;
                    gear += 1;
                }
                if (gear == 3 && velocity.z >= 100f)
                {
                    velocity.z = 50.0f;
                    gear += 1;
                }

                if (gear >= 4)
                    gear = 4;
            }
            else if (dir == SOUTH)
            {
                force.z -= 500.0f;
                if (force.z <= -max_force)
                    force.z = -max_force;
                if (velocity.z <= -max_velocity)
                    velocity.z = -max_velocity;

                if (gear == 1 && velocity.z <= -50f)
                {
                    velocity.z = -10.0f;
                    gear += 1;
                }
                if (gear == 2 && velocity.z <= -75f)
                {
                    velocity.z = -25.0f;
                    gear += 1;
                }
                if (gear == 3 && velocity.z <= -100f)
                {
                    velocity.z = -50.0f;
                    gear += 1;
                }

                if (gear >= 4)
                    gear = 4;
            }
            else if (dir == EAST)
            {
                force.x += 500.0f;
                if (force.x >= max_force)
                    force.x = max_force;
                if (velocity.x >= max_velocity)
                    velocity.x = max_velocity;

                if (gear == 1 && velocity.x >= 50f)
                {
                    velocity.x = 10.0f;
                    gear += 1;
                }
                if (gear == 2 && velocity.x >= 75f)
                {
                    velocity.x = 25.0f;
                    gear += 1;
                }
                if (gear == 3 && velocity.x >= 100f)
                {
                    velocity.x = 50.0f;
                    gear += 1;
                }

                if (gear >= 4)
                    gear = 4;
            }
            else if (dir == WEST)
            {
                force.x -= 500.0f;
                if (force.x <= -max_force)
                    force.x = -max_force;
                if (velocity.x <= -max_velocity)
                    velocity.x = -max_velocity;

                if (velocity.z <= -max_velocity)
                    velocity.z = -max_velocity;

                if (gear == 1 && velocity.x <= -50f)
                {
                    velocity.x = -10.0f;
                    gear += 1;
                }
                if (gear == 2 && velocity.x <= -75f)
                {
                    velocity.x = -25.0f;
                    gear += 1;
                }
                if (gear == 3 && velocity.x <= -100f)
                {
                    velocity.x = -50.0f;
                    gear += 1;
                }

                if (gear >= 4)
                    gear = 4;
            }
        }

        if (controls == S)
        {
            if (dir == NORTH)
            {
                force.z-= 0.0f;
                velocity.z -= 5.0f;
                if (velocity.z <= 0.0f)
                    velocity.z = 0.0f;
                if (force.z <= 0.0f)
                    force.z = 0.0f;

                if (gear == 4 && velocity.z <= 100f)
                    gear -= 1;
                else if (gear == 3 && velocity.z <= 75f)
                    gear -= 1;
                else if (gear == 2 && velocity.z <= 50f)
                    gear -= 1;

                if (gear <= 1)
                    gear = 1;
            }
            else if (dir == SOUTH)
            {
                force.z = 0.0f;
                velocity.z += 5.0f;
                if (velocity.z >= 0.0f)
                    velocity.z = 0.0f;
                if (force.z >= 0.0f)
                    force.z = 0.0f;

                if (gear == 4 && velocity.z >= -100f)
                    gear -= 1;
                else if (gear == 3 && velocity.z >= -75f)
                    gear -= 1;
                else if (gear == 2 && velocity.z >= -50f)
                    gear -= 1;

                if (gear <= 1)
                    gear = 1;
            }
            else if (dir == EAST)
            {
                force.x = 0.0f;
                velocity.x -= 5.0f;
                if (velocity.x <= 0.0f)
                    velocity.x = 0.0f;
                if (force.x <= 0.0f)
                    force.x = 0.0f;

                if (gear == 4 && velocity.x <= 100f)
                    gear -= 1;
                else if (gear == 3 && velocity.x <= 75f)
                    gear -= 1;
                else if (gear == 2 && velocity.x <= 50f)
                    gear -= 1;

                if (gear <= 1)
                    gear = 1;
            }
            else if (dir == WEST)
            {
                force.x = 0.0f;
                velocity.x += 5.0f;
                if (velocity.x >= 0.0f)
                    velocity.x = 0.0f;
                if (force.x >= 0.0f)
                    force.x = 0.0f;

                if (gear == 4 && velocity.x >= -100f)
                    gear -= 1;
                else if (gear == 3 && velocity.x >= -75f)
                    gear -= 1;
                else if (gear == 2 && velocity.x >= -50f)
                    gear -= 1;

                if (gear <= 1)
                    gear = 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.A) || (meta_quest_2_primary_thumbstick.x <= -0.5f) || (meta_quest_2_secondary_thumbstick.x <= -0.5f))
        {
            if (rotate_left == false)
            {
                rotate_left = true;
                rot.y -= 90.0f;
                new_direction = rotate_vector(rot.y, direction);
                transform.rotation = Quaternion.Euler(rot);
                cam.transform.rotation = Quaternion.Euler(rot);
                turning.Play(0);

                if (dir == NORTH)
                {
                    dir = WEST;
                    force.x = -Mathf.Abs(force.z);
                    force.z = 0.0f;
                    velocity.x = -Mathf.Abs(velocity.z);
                    velocity.z = 0.0f;
                }
                else if (dir == WEST)
                {
                    dir = SOUTH;
                    force.z = -Mathf.Abs(force.x);
                    force.x = 0.0f;
                    velocity.z = -Mathf.Abs(velocity.x);
                    velocity.x = 0.0f;
                }
                else if (dir == SOUTH)
                {
                    dir = EAST;
                    force.x = Mathf.Abs(force.z);
                    force.z = 0.0f;
                    velocity.x = Mathf.Abs(velocity.z);
                    velocity.z = 0.0f;
                }
                else if (dir == EAST)
                {
                    dir = NORTH;
                    force.z = Mathf.Abs(force.x);
                    force.x = 0.0f;
                    velocity.z = Mathf.Abs(velocity.x);
                    velocity.x = 0.0f;
                }
            }
        }
        else
        {
            rotate_left = false;
        }

        if (Input.GetKeyDown(KeyCode.D) || (meta_quest_2_primary_thumbstick.x >= 0.5f) || (meta_quest_2_secondary_thumbstick.x >= 0.5f))
        {
            if (rotate_right == false)
            {
                rotate_right = true;
                rot.y += 90f;
                new_direction = rotate_vector(rot.y, direction);
                transform.rotation = Quaternion.Euler(rot);
                cam.transform.rotation = Quaternion.Euler(rot);
                turning.Play(0);

                if (dir == NORTH)
                {
                    dir = EAST;
                    force.x = Mathf.Abs(force.z);
                    force.z = 0.0f;
                    velocity.x = Mathf.Abs(velocity.z);
                    velocity.z = 0.0f;
                }
                else if (dir == EAST)
                {
                    dir = SOUTH;
                    force.z = -Mathf.Abs(force.x);
                    force.x = 0.0f;
                    velocity.z = -Mathf.Abs(velocity.x);
                    velocity.x = 0.0f;
                }
                else if (dir == SOUTH)
                {
                    dir = WEST;
                    force.x = -Mathf.Abs(force.z);
                    force.z = 0.0f;
                    velocity.x = -Mathf.Abs(velocity.z);
                    velocity.z = 0.0f;
                }
                else if (dir == WEST)
                {
                    dir = NORTH;
                    force.z = Mathf.Abs(force.x);
                    force.x = 0.0f;
                    velocity.z = Mathf.Abs(velocity.x);
                    velocity.x = 0.0f;
                }
            }
        }
        else
        {
            rotate_right = false;
        }

        transform.position = new_position;

        if (dir == NORTH)
            cam.transform.position = new Vector3(new_position.x, new_position.y + 1.19f, new_position.z + 0.5f);
        else if (dir == SOUTH)
            cam.transform.position = new Vector3(new_position.x, new_position.y + 1.19f, new_position.z - 0.5f);
        else if (dir == WEST)
            cam.transform.position = new Vector3(new_position.x - 0.5f, new_position.y + 1.19f, new_position.z);
        else if (dir == EAST)
            cam.transform.position = new Vector3(new_position.x + 0.5f, new_position.y + 1.19f, new_position.z );

        pitchModifier = maxPitch - minPitch;

        if (dir == NORTH || dir == SOUTH)
        {
            motor.pitch = minPitch + (Mathf.Abs(velocity.z) / max_velocity) * pitchModifier;
        }
        else if (dir == WEST || dir == EAST)
        {
            motor.pitch = minPitch + (Mathf.Abs(velocity.x) / max_velocity) * pitchModifier;
        }
    }

}
