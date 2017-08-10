using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public struct PlayerInput
{
    // Game Settings
    private float _f_zDir; // Forward or Backward
    private float _f_xDir; // Strafe Left or Right
    private float _f_LookHoriz;
    private float _f_LookVert;
    private bool _b_JumpPressed;
    private bool _b_JumpHeld;
    private bool _b_DPad_LeftPressed;
    private bool _b_DPad_RightPressed;
    private bool _b_DPad_UpPressed;
    private bool _b_DPad_DownPressed;
    private Vector2 _v2_DPad;
    private ButtonState _d_Button_A;
    private bool _d_Button_B;
    private bool _d_Button_X;
    private bool _d_Button_Y;

    // Analog Sticks
    public float zDir
    {
        internal set
        {
            float f_Temp = value;

            // Cap the value between -1 and 1
            if (f_Temp < -1.0f) f_Temp = -1.0f;
            if (f_Temp > 1.0f) f_Temp = 1.0f;

            // If very close to 0f, set to 0f
            if (f_Temp < .05f && f_Temp > -0.05f) f_Temp = 0f;

            _f_zDir = f_Temp;
        }
        get
        {
            return _f_zDir;
        }
    }
    public float xDir
    {
        set
        {
            float f_Temp = value;

            // Cap the value between -1 and 1
            if (f_Temp < -1.0f) f_Temp = -1.0f;
            if (f_Temp > 1.0f) f_Temp = 1.0f;

            // If very close to 0f, set to 0f
            if (f_Temp < .05f && f_Temp > -0.05f) f_Temp = 0f;

            _f_xDir = f_Temp;
        }
        get
        {
            return _f_xDir;
        }
    }

    public float LookHoriz
    {
        set
        {
            _f_LookHoriz = value;
        }
        get { return _f_LookHoriz; }
    }
    public float LookVert
    {
        set
        {
            _f_LookVert = value;
        }
        get { return _f_LookVert; }
    }

    // DPad
    public bool DPad_Pressed_Left
    {
        set { _b_DPad_LeftPressed = value; }
        get { return _b_DPad_LeftPressed; }
    }
    public bool DPad_Pressed_Right
    {
        set { _b_DPad_RightPressed = value; }
        get { return _b_DPad_RightPressed; }
    }
    public bool DPad_Pressed_Up
    {
        set { _b_DPad_UpPressed = value; }
        get { return _b_DPad_UpPressed; }
    }
    public bool DPad_Pressed_Down
    {
        set { _b_DPad_DownPressed = value; }
        get { return _b_DPad_DownPressed; }
    }

    public Vector2 DPadVector
    {
        set { _v2_DPad = value; }
        get { return _v2_DPad; }
    }

    // A/B/X/Y
    public ButtonState Button_A
    {
        internal set { _d_Button_A = value; }
        get { return _d_Button_A; }
    }

    public bool JumpPressed
    {
        set { _b_JumpPressed = value; }
        get { return _b_JumpPressed; }
    }
    public bool JumpHeld
    {
        set { _b_JumpHeld = value; }
        get { return _b_JumpHeld; }
    }
}

public class C_INPUT_MANAGER : MonoBehaviour
{
    // Controller scripts
    [SerializeField] internal PlayerIndex player = PlayerIndex.One;
    GamePadState player_State;
    GamePadState player_PrevState;

    // PlayerInput struct
    internal PlayerInput playerInput;

    // Use this for initialization
    internal virtual void Start ()
    {
        playerInput = new PlayerInput();

        // Controller input
        v2_DPad = new Vector2();
        v2_DPad_Old = new Vector2();
    }

    public PlayerIndex SetPlayerIndex
    {
        set { player = value; }
    }

    float f_InputMinimum = 0.05f;
    Vector2 v2_DPad;
    Vector2 v2_DPad_Old;
    void UpdatePlayerInput()
    {
        player_State = GamePad.GetState(player);

        #region Analog Sticks
        // Move Horizontally
        if (player_State.ThumbSticks.Left.X >= f_InputMinimum || player_State.ThumbSticks.Left.X <= -f_InputMinimum) playerInput.xDir = player_State.ThumbSticks.Left.X;
        else playerInput.xDir = 0f;

        // Move Forward / Backward
        if (player_State.ThumbSticks.Left.Y >= f_InputMinimum || player_State.ThumbSticks.Left.Y <= -f_InputMinimum) playerInput.zDir = player_State.ThumbSticks.Left.Y;
        else playerInput.zDir = 0f;

        // Look Up / Down
        if (player_State.ThumbSticks.Right.X >= f_InputMinimum || player_State.ThumbSticks.Right.X <= -f_InputMinimum) playerInput.LookHoriz = player_State.ThumbSticks.Right.X;
        else playerInput.LookHoriz = 0f;

        // Look Up / Down
        if (player_State.ThumbSticks.Right.Y >= f_InputMinimum || player_State.ThumbSticks.Right.Y <= -f_InputMinimum) playerInput.LookVert = player_State.ThumbSticks.Right.Y;
        else playerInput.LookVert = 0f;
        #endregion

        #region DPad Bools and Vector2
        // Reset values
        playerInput.DPad_Pressed_Left = false;
        playerInput.DPad_Pressed_Right = false;
        playerInput.DPad_Pressed_Up = false;
        playerInput.DPad_Pressed_Up = false;
        v2_DPad = new Vector2();
        
        if (player_State.DPad.Left == ButtonState.Pressed)
        {
            if (v2_DPad_Old.x >= 0f) playerInput.DPad_Pressed_Left = true;

            v2_DPad.x = -1f;
        }
        else if(player_State.DPad.Right == ButtonState.Pressed)
        {
            if (v2_DPad_Old.x <= 0f) playerInput.DPad_Pressed_Right = true;

            v2_DPad.x = 1f;
        }

        if(player_State.DPad.Up == ButtonState.Pressed)
        {
            if (v2_DPad_Old.y <= 0f) playerInput.DPad_Pressed_Up = true;

            v2_DPad.y = 1f;
        }
        else if(player_State.DPad.Down == ButtonState.Pressed)
        {
            if (v2_DPad_Old.y >= 0f) playerInput.DPad_Pressed_Down = true;

            v2_DPad.y = -1f;
        }

        // Normalize
        v2_DPad.Normalize();

        // Set value
        playerInput.DPadVector = v2_DPad;

        // Store old value for evaluation
        v2_DPad_Old = v2_DPad;
        #endregion

        #region A/B/X/Y
        playerInput.Button_A = ButtonState.Released;
        if(player_State.Buttons.A == ButtonState.Pressed)
        {
            playerInput.Button_A = ButtonState.Pressed;
        }
        #endregion

        player_PrevState = player_State;
    }
	
	// Update is called once per frame
	internal virtual void Update ()
    {
        UpdatePlayerInput();
    }
}
