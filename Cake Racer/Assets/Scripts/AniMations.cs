using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Karting.KartSystem
{
    public class AniMations : MonoBehaviour
    {
        [Serializable] public class Wheel
        {
            [Tooltip("A reference to the transform of the wheel.")]
            public Transform wheelTransform;
            [Tooltip("A reference to the WheelCollider of the wheel.")]
            public WheelCollider wheelCollider;
            
            Quaternion m_SteerlessLocalRotation;

            public void Setup() => m_SteerlessLocalRotation = wheelTransform.localRotation;

            public void StoreDefaultRotation() => m_SteerlessLocalRotation = wheelTransform.localRotation;
            public void SetToDefaultRotation() => wheelTransform.localRotation = m_SteerlessLocalRotation;
        }

        [Tooltip("What kart do we want to listen to?")]
        public ArcadeKart kartController;

        [Space]
        [Tooltip("The damping for the appearance of steering compared to the input.  The higher the number the less damping.")]
        public float steeringAnimationDamping = 10f;

        [Space]
        [Tooltip("The maximum angle in degrees that the front wheels can be turned away from their default positions, when the Steering input is either 1 or -1.")]
        public float maxSteeringAngle;
        [Tooltip("Information referring to the front left wheel of the kart.")]
        public Wheel frontLeft;
        [Tooltip("Information referring to the front right wheel of the kart.")]
        public Wheel frontRight;
        [Tooltip("Information referring to the rear left wheel of the kart.")]
        public Wheel backLeft;
        [Tooltip("Information referring to the rear right wheel of the kart.")]
        public Wheel backRight;


        float m_SmoothedSteeringInput;

        void Start()
        {
            frontLeft.Setup();
            frontRight.Setup();
            backLeft.Setup();
            backRight.Setup();
        }

        void FixedUpdate() 
        {
            m_SmoothedSteeringInput = Mathf.MoveTowards(m_SmoothedSteeringInput, kartController.Input.TurnInput, 
                steeringAnimationDamping * Time.deltaTime);

            // Steer front wheels
            float rotationAngle = m_SmoothedSteeringInput * maxSteeringAngle;

            frontLeft.wheelCollider.steerAngle = rotationAngle;
            frontRight.wheelCollider.steerAngle = rotationAngle;

            // Update position and rotation from WheelCollider
            UpdateWheelFromCollider(frontLeft);
            UpdateWheelFromCollider(frontRight);
            UpdateWheelFromCollider(backLeft);
            UpdateWheelFromCollider(backRight);
        }

        void LateUpdate()
        {
            // Update position and rotation from WheelCollider
            UpdateWheelFromCollider(frontLeft);
            UpdateWheelFromCollider(frontRight);
            UpdateWheelFromCollider(backLeft);
            UpdateWheelFromCollider(backRight);
        }

        void UpdateWheelFromCollider(Wheel wheel)
        {
            wheel.wheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);
            wheel.wheelTransform.position = position;
            wheel.wheelTransform.rotation = rotation;
        }
    }
    
}

