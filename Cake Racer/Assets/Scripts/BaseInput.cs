
using UnityEngine;


namespace Karting.KartSystem
{
    public struct InputData
    {
        public bool Accelerate;
        public bool Brake;
        public bool Reset;
        public float TurnInput;
        public bool powerup;
    }

    public interface IInput
    {
        InputData GenerateInput();
    }

    public abstract class BaseInput : MonoBehaviour, IInput
    {
        public abstract InputData GenerateInput();
    }
}
