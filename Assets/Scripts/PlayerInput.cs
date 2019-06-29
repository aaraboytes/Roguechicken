// GENERATED AUTOMATICALLY FROM 'Assets/PlayerInput.inputactions'

using System;
using UnityEngine;
using UnityEngine.Experimental.Input;


[Serializable]
public class PlayerInput : InputActionAssetReference
{
    public PlayerInput()
    {
    }
    public PlayerInput(InputActionAsset asset)
        : base(asset)
    {
    }
    private bool m_Initialized;
    private void Initialize()
    {
        // Player
        m_Player = asset.GetActionMap("Player");
        m_Player_Shoot = m_Player.GetAction("Shoot");
        m_Player_Dash = m_Player.GetAction("Dash");
        m_Player_Movement = m_Player.GetAction("Movement");
        m_Player_Direction = m_Player.GetAction("Direction");
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        m_Player = null;
        m_Player_Shoot = null;
        m_Player_Dash = null;
        m_Player_Movement = null;
        m_Player_Direction = null;
        m_Initialized = false;
    }
    public void SetAsset(InputActionAsset newAsset)
    {
        if (newAsset == asset) return;
        if (m_Initialized) Uninitialize();
        asset = newAsset;
    }
    public override void MakePrivateCopyOfActions()
    {
        SetAsset(ScriptableObject.Instantiate(asset));
    }
    // Player
    private InputActionMap m_Player;
    private InputAction m_Player_Shoot;
    private InputAction m_Player_Dash;
    private InputAction m_Player_Movement;
    private InputAction m_Player_Direction;
    public struct PlayerActions
    {
        private PlayerInput m_Wrapper;
        public PlayerActions(PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Shoot { get { return m_Wrapper.m_Player_Shoot; } }
        public InputAction @Dash { get { return m_Wrapper.m_Player_Dash; } }
        public InputAction @Movement { get { return m_Wrapper.m_Player_Movement; } }
        public InputAction @Direction { get { return m_Wrapper.m_Player_Direction; } }
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
    }
    public PlayerActions @Player
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new PlayerActions(this);
        }
    }
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get

        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.GetControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get

        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.GetControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
}
