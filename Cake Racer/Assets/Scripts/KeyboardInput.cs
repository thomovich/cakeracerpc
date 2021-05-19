using UnityEngine;



namespace Karting.KartSystem
{
    public class KeyboardInput : BaseInput
    {
        public string TurnInputName = "Horizontal";
        public string AccelerateButtonName = "Jump";
        public string BrakeButtonName = "Sumbit";
        public string reset = "Reset";
        public string powerup = "Powerup";

        public override InputData GenerateInput()
        {
            return new InputData
            {
                Accelerate = Input.GetButton(AccelerateButtonName),
                Brake = Input.GetButton(BrakeButtonName),
                Reset = Input.GetButton(reset),
                TurnInput = Input.GetAxis(TurnInputName),
                powerup = Input.GetButton(powerup)
            };
        }
    }
}

