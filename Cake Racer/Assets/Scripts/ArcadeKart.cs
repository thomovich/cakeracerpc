using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.VFX;

namespace Karting.KartSystem
{
    
    public class ArcadeKart : MonoBehaviour
    {
        private Vector3 lastpos;
        private bool usedpowerup = false;
        public bool recievedpowerup = false;
        private RocketScripts script;
        public GameObject rocket;
        public event EventHandler onpowerupused;
        private new AudioManager audio;
        private bool isPlaying;
        
        
        [System.Serializable]
        public class StatPowerup
        {
            public ArcadeKart.Stats modifiers;
            public string PowerUpID;
            public float ElapsedTime;
            public float MaxTime;
        }

        [System.Serializable]
        public struct Stats
        {
            
            [Min(0.001f), Tooltip("Top speed attainable when moving forward.")]
            public float TopSpeed;

            
            public float Acceleration;

            [Min(0.001f)]
            public float ReverseSpeed;

            
            public float ReverseAcceleration;

            
            [Range(0.2f, 1)]
            public float AccelerationCurve;

          
            public float Braking;

            
            public float CoastingDrag;

            [Range(0.0f, 1.0f)]
          
            public float Grip;

           
            public float Steer;

           
            public float AddedGravity;

            // allow for stat adding for powerups.
            public static Stats operator +(Stats a, Stats b)
            {
                return new Stats
                {
                    Acceleration        = a.Acceleration + b.Acceleration,
                    AccelerationCurve   = a.AccelerationCurve + b.AccelerationCurve,
                    Braking             = a.Braking + b.Braking,
                    CoastingDrag        = a.CoastingDrag + b.CoastingDrag,
                    AddedGravity        = a.AddedGravity + b.AddedGravity,
                    Grip                = a.Grip + b.Grip,
                    ReverseAcceleration = a.ReverseAcceleration + b.ReverseAcceleration,
                    ReverseSpeed        = a.ReverseSpeed + b.ReverseSpeed,
                    TopSpeed            = a.TopSpeed + b.TopSpeed,
                    Steer               = a.Steer + b.Steer,
                };
            }
        }

        public Rigidbody Rigidbody { get; private set; }
        public InputData Input     { get; private set; }
        public float AirPercent    { get; private set; }
        public float GroundPercent { get; private set; }
        
        

        public ArcadeKart.Stats baseStats = new ArcadeKart.Stats
        {
            TopSpeed            = 10f,
            Acceleration        = 5f,
            AccelerationCurve   = 4f,
            Braking             = 10f,
            ReverseAcceleration = 5f,
            ReverseSpeed        = 5f,
            Steer               = 5f,
            CoastingDrag        = 4f,
            Grip                = .95f,
            AddedGravity        = 1f,
        };
        

       
        //Center of mass for the car in order to have the wheel colliders function well
        public Transform CenterOfMass;

        

        

        
        
        //Varaibles for controlling suspension in the whell colliders
        [Range(0.0f, 1.0f)]
        public float SuspensionHeight = 0.2f;
        [Range(10.0f, 100000.0f)]
        public float SuspensionSpring = 20000.0f;
        [Range(0.0f, 5000.0f)]
        public float SuspensionDamp = 500.0f;
        [Range(-1.0f, 1.0f)]
        public float WheelsPositionVerticalOffset = 0.0f;

        //The physical reprensentation of the wheels
        public WheelCollider FrontLeftWheel;
        public WheelCollider FrontRightWheel;
        public WheelCollider RearLeftWheel;
        public WheelCollider RearRightWheel;

      

        // the input sources that can control the kart
        IInput[] m_Inputs;

        const float k_NullInput = 0.01f;
        const float k_NullSpeed = 0.01f;
        Vector3 m_VerticalReference = Vector3.up;

        float m_CurrentGrip = 1.0f;
        float m_PreviousGroundPercent = 1.0f;
        
        

        // can the kart move?
        bool m_CanMove = true;
        //List for holding the powerups
        List<StatPowerup> m_ActivePowerupList = new List<StatPowerup>();
        ArcadeKart.Stats m_FinalStats;

        private Vector3 m_LastValidPosition;
        private Quaternion m_LastValidRotation;
        
        Vector3 m_LastCollisionNormal;
        bool m_HasCollision;
        

        public void AddPowerup(StatPowerup statPowerup)
        {
            if (!recievedpowerup)
            {
                if (statPowerup.PowerUpID.Equals("tripleboost") || statPowerup.PowerUpID.Equals("triplerocket"))
                {
                    m_ActivePowerupList.Add(statPowerup);
                    m_ActivePowerupList.Add(statPowerup);
                    m_ActivePowerupList.Add(statPowerup);
                    recievedpowerup = true;
                }
                else
                {
                    m_ActivePowerupList.Add(statPowerup);
                    recievedpowerup = true; 
                }
                
            }
        }


      
        

        void UpdateSuspensionParams(WheelCollider wheel)
        {
            wheel.suspensionDistance = SuspensionHeight;
            wheel.center = new Vector3(0.0f, WheelsPositionVerticalOffset, 0.0f);
            JointSpring spring = wheel.suspensionSpring;
            spring.spring = SuspensionSpring;
            spring.damper = SuspensionDamp;
            wheel.suspensionSpring = spring;
        }

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            m_Inputs = GetComponents<IInput>();
            audio = FindObjectOfType<AudioManager>();
            

            UpdateSuspensionParams(FrontLeftWheel);
            UpdateSuspensionParams(FrontRightWheel);
            UpdateSuspensionParams(RearLeftWheel);
            UpdateSuspensionParams(RearRightWheel);

            m_CurrentGrip = baseStats.Grip;

            
        }

        private void Start()
        {
            m_FinalStats = baseStats;
            script = gameObject.AddComponent<RocketScripts>();
        }


        void FixedUpdate()
        {
            UpdateSuspensionParams(FrontLeftWheel);
            UpdateSuspensionParams(FrontRightWheel);
            UpdateSuspensionParams(RearLeftWheel);
            UpdateSuspensionParams(RearRightWheel);

            GatherInputs();

            if (Rigidbody.velocity.magnitude >= 0.5f && !isPlaying)
            {
                audio.Play("CarDrive");
                isPlaying = true;
            }

            if (Rigidbody.velocity.magnitude <= 0.5f)
            {
                audio.stop("CarDrive");
                isPlaying = false;
            }
            // apply our powerups to create our finalStats
            

            // apply our physics properties
            Rigidbody.centerOfMass = transform.InverseTransformPoint(CenterOfMass.position);

            int groundedCount = 0;
            if (FrontLeftWheel.isGrounded && FrontLeftWheel.GetGroundHit(out WheelHit hit))
                groundedCount++;
            if (FrontRightWheel.isGrounded && FrontRightWheel.GetGroundHit(out hit))
                groundedCount++;
            if (RearLeftWheel.isGrounded && RearLeftWheel.GetGroundHit(out hit))
                groundedCount++;
            if (RearRightWheel.isGrounded && RearRightWheel.GetGroundHit(out hit))
                groundedCount++;

            // calculate how grounded and airborne we are
            GroundPercent = (float) groundedCount / 4.0f;
            AirPercent = 1 - GroundPercent;
            

            // apply vehicle physics
            if (m_CanMove)
            {
                MoveVehicle(Input.Accelerate, Input.Brake, Input.TurnInput);
            }

            //If the car should tumble over
            if (Input.Reset && transform.position == lastpos)
            {
                
                Reset(); 
            }

            if (Input.powerup && !usedpowerup)
            {
                usePowerup();
            }

            lastpos = transform.position;

            GroundAirbourne();

            m_PreviousGroundPercent = GroundPercent;
            
        }

        void GatherInputs()
        {
            // reset input
            Input = new InputData();
            

            // gather nonzero input from our sources
            for (int i = 0; i < m_Inputs.Length; i++)
            {
                Input = m_Inputs[i].GenerateInput();
                
            }
        }

        void usePowerup()
        {
            
            
            if (m_ActivePowerupList.Count > 0)
            {
                var p = m_ActivePowerupList[0];
               
                var powerups = new Stats();

                if (p.PowerUpID.Contains("boost"))
                {
                    
                    powerups.TopSpeed = 15;
                    powerups.Acceleration = 5;
                    powerups += p.modifiers;
                    m_FinalStats = baseStats + powerups;
                    usedpowerup = true;
                    m_ActivePowerupList.RemoveAt(0);
                    Invoke(nameof(boostEnder), 2);
                }

                if (p.PowerUpID.Contains("rocket"))
                {
                    script.shootMissile(Rigidbody.transform.position, Rigidbody.transform.forward, Rigidbody.transform.rotation, rocket);
                    m_ActivePowerupList.RemoveAt(0);
                    usedpowerup = true;
                    Invoke(nameof(boostEnder), 2);
                    
                }
                
                onpowerupused?.Invoke(this, EventArgs.Empty);
                
                
            }
            else
            {
                recievedpowerup = false;
            }
        }

        void boostEnder()
        {
            m_FinalStats = baseStats;
            usedpowerup = false;
            
            
            //makes it possible to recieve a new powerup
            if (m_ActivePowerupList.Count == 0)
            {
                recievedpowerup = false;
            }
        }

        
        
        
        

        void GroundAirbourne()
        {
            // while in the air, fall faster
            if (AirPercent >= 1)
            {
                Rigidbody.velocity += Physics.gravity * Time.fixedDeltaTime * m_FinalStats.AddedGravity;
            }
        }

        //Function that turns the car around if it should happen to flip over
        public void Reset()
        {
            Vector3 euler = transform.rotation.eulerAngles;
            euler.x = euler.z = 0f;
            transform.rotation = Quaternion.Euler(euler);
        }

        

        void OnCollisionEnter(Collision collision) => m_HasCollision = true;
        void OnCollisionExit(Collision collision) => m_HasCollision = false;

        void OnCollisionStay(Collision collision)
        {
            m_HasCollision = true;
            m_LastCollisionNormal = Vector3.zero;
            float dot = -1.0f;

            foreach (var contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > dot)
                    m_LastCollisionNormal = contact.normal;
            }
        }

        

        void MoveVehicle(bool accelerate, bool brake, float turnInput)
        {
            
            float accelInput = (accelerate ? 1.0f : 0.0f) - (brake ? 1.0f : 0.0f);

            // manual acceleration curve coefficient scalar
            float accelerationCurveCoeff = 5;
            Vector3 localVel = transform.InverseTransformVector(Rigidbody.velocity);

            bool accelDirectionIsFwd = accelInput >= 0;
            bool localVelDirectionIsFwd = localVel.z >= 0;

            // use the max speed for the direction we are going--forward or reverse.
            float maxSpeed = localVelDirectionIsFwd ? m_FinalStats.TopSpeed : m_FinalStats.ReverseSpeed;
            float accelPower = accelDirectionIsFwd ? m_FinalStats.Acceleration : m_FinalStats.ReverseAcceleration;

            float currentSpeed = Rigidbody.velocity.magnitude;
            float accelRampT = currentSpeed / maxSpeed;
            float multipliedAccelerationCurve = m_FinalStats.AccelerationCurve * accelerationCurveCoeff;
            float accelRamp = Mathf.Lerp(multipliedAccelerationCurve, 1, accelRampT * accelRampT);

            bool isBraking = (localVelDirectionIsFwd && brake) || (!localVelDirectionIsFwd && accelerate);

            // if we are braking (moving reverse to where we are going)
            // use the braking accleration instead
            float finalAccelPower = isBraking ? m_FinalStats.Braking : accelPower;

            float finalAcceleration = finalAccelPower * accelRamp;

            
            // apply inputs to forward/backward
            float turningPower = turnInput * m_FinalStats.Steer;

            Quaternion turnAngle = Quaternion.AngleAxis(turningPower, transform.up);
            Vector3 fwd = turnAngle * transform.forward;
            Vector3 movement = fwd * accelInput * finalAcceleration * ((m_HasCollision || GroundPercent > 0.0f) ? 1.0f : 0.0f);

            // forward movement
            bool wasOverMaxSpeed = currentSpeed >= maxSpeed;

            // if over max speed, cannot accelerate faster.
            if (wasOverMaxSpeed && !isBraking) 
                movement *= 0.0f;

            Vector3 newVelocity = Rigidbody.velocity + movement * Time.fixedDeltaTime;
            newVelocity.y = Rigidbody.velocity.y;

            //  clamp max speed if we are on ground
            if (GroundPercent > 0.0f && !wasOverMaxSpeed)
            {
                newVelocity = Vector3.ClampMagnitude(newVelocity, maxSpeed);
            }

            // coasting is when we aren't touching accelerate
            if (Mathf.Abs(accelInput) < k_NullInput && GroundPercent > 0.0f)
            {
                newVelocity = Vector3.MoveTowards(newVelocity, 
                    new Vector3(0, Rigidbody.velocity.y, 0),
                     Time.fixedDeltaTime * m_FinalStats.CoastingDrag);
            }

            Rigidbody.velocity = newVelocity;

            // Turning the car
            if (GroundPercent > 0.0f)
            {
                // manual angular velocity coefficient
                float angularVelocitySteering = 0.4f;
                float angularVelocitySmoothSpeed = 20f;

                // turning is reversed if we're going in reverse and pressing reverse
                if (!localVelDirectionIsFwd && !accelDirectionIsFwd)
                    angularVelocitySteering *= -1.0f;

                var angularVel = Rigidbody.angularVelocity;

                // move the Y angular velocity towards our target
                angularVel.y = Mathf.MoveTowards(angularVel.y, turningPower * angularVelocitySteering,
                    Time.fixedDeltaTime * angularVelocitySmoothSpeed);

                // apply the angular velocity
                Rigidbody.angularVelocity = angularVel;

                // rotate rigidbody's velocity as well to generate immediate velocity redirection
                // manual velocity steering coefficient
                

                bool validPosition = false;
                
                validPosition = GroundPercent < 0.7f && !m_HasCollision && Vector3.Dot(m_VerticalReference, Vector3.up) > 0.9f;

                if (validPosition)
                {
                    m_LastValidPosition = transform.position;
                    m_LastValidRotation.eulerAngles = new Vector3(0.0f, transform.rotation.y, 0.0f);
                    
                }

            }


        }
    }
}
