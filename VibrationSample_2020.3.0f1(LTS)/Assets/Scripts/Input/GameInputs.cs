// GENERATED AUTOMATICALLY FROM 'Assets/Setting/GameInputs.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @GameInputs : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @GameInputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameInputs"",
    ""maps"": [
        {
            ""name"": ""PlayGame"",
            ""id"": ""cccbfc59-9709-4603-8210-e7ff9784b241"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""f4052f3a-c8ea-4f57-b936-e3fa6638c0ed"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shot"",
                    ""type"": ""Button"",
                    ""id"": ""b4333782-8d1d-41f7-b6ef-19c858fe8ba3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c3409697-07ea-4daf-be82-86f7b73bc2ac"",
                    ""path"": ""<Gamepad>/dpad/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MainGame"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0e5f4e45-9b69-429e-a8cb-4154cde8cee1"",
                    ""path"": ""<XInputController>/leftStick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MainGame"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Arrow"",
                    ""id"": ""4ff92a0e-7a7a-4547-b652-45cf924e60ac"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""53e0330f-5898-4887-991a-2276c40711b0"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MainGame"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""b2503fad-38ea-403d-9dd8-6c5933b9246d"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MainGame"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""AD"",
                    ""id"": ""81d9ab8a-2384-451e-8dad-b3478259fc45"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MainGame"",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""19c596e3-6f87-48d2-a2fd-300cc2ccd214"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MainGame"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""6090fabf-4195-4020-809c-85ae516285c5"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MainGame"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""e62b5125-63b5-4774-a8bf-5f46523af97f"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MainGame"",
                    ""action"": ""Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bc0504f3-2996-4f01-a73b-495206c51bbc"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MainGame"",
                    ""action"": ""Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""13fc9d27-1fd2-4c20-9cc8-3e7b685626c7"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MainGame"",
                    ""action"": ""Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e2e6ec19-236d-40eb-b741-10a7a2cc75d4"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MainGame"",
                    ""action"": ""Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""MainGame"",
            ""bindingGroup"": ""MainGame"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // PlayGame
        m_PlayGame = asset.FindActionMap("PlayGame", throwIfNotFound: true);
        m_PlayGame_Move = m_PlayGame.FindAction("Move", throwIfNotFound: true);
        m_PlayGame_Shot = m_PlayGame.FindAction("Shot", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // PlayGame
    private readonly InputActionMap m_PlayGame;
    private IPlayGameActions m_PlayGameActionsCallbackInterface;
    private readonly InputAction m_PlayGame_Move;
    private readonly InputAction m_PlayGame_Shot;
    public struct PlayGameActions
    {
        private @GameInputs m_Wrapper;
        public PlayGameActions(@GameInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_PlayGame_Move;
        public InputAction @Shot => m_Wrapper.m_PlayGame_Shot;
        public InputActionMap Get() { return m_Wrapper.m_PlayGame; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayGameActions set) { return set.Get(); }
        public void SetCallbacks(IPlayGameActions instance)
        {
            if (m_Wrapper.m_PlayGameActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayGameActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayGameActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayGameActionsCallbackInterface.OnMove;
                @Shot.started -= m_Wrapper.m_PlayGameActionsCallbackInterface.OnShot;
                @Shot.performed -= m_Wrapper.m_PlayGameActionsCallbackInterface.OnShot;
                @Shot.canceled -= m_Wrapper.m_PlayGameActionsCallbackInterface.OnShot;
            }
            m_Wrapper.m_PlayGameActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Shot.started += instance.OnShot;
                @Shot.performed += instance.OnShot;
                @Shot.canceled += instance.OnShot;
            }
        }
    }
    public PlayGameActions @PlayGame => new PlayGameActions(this);
    private int m_MainGameSchemeIndex = -1;
    public InputControlScheme MainGameScheme
    {
        get
        {
            if (m_MainGameSchemeIndex == -1) m_MainGameSchemeIndex = asset.FindControlSchemeIndex("MainGame");
            return asset.controlSchemes[m_MainGameSchemeIndex];
        }
    }
    public interface IPlayGameActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnShot(InputAction.CallbackContext context);
    }
}
