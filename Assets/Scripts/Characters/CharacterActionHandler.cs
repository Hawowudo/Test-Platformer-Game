using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterActionHandler : MonoBehaviour
{
    public Rigidbody2D _rigidBody2D;
    public Blackboard _blackboard;
    public BlackboardTimer _blackboardTimer;

    public ActionStateLogic[] _switchableStates;
    [SerializeReference] public ActionStateLogic _currentState;
    [SerializeReference] public ActionStateLogic _initialState;

    public bool debugMode;
    public TextMeshProUGUI debugText;
    int _actionSign = 0;

    #region Unity Functions
    void Awake()
    {
        _blackboard = GetComponent<Blackboard>();
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _blackboardTimer = GetComponent<BlackboardTimer>();

        //if (debugMode)
        //    debugText.text = _currentState.GetType().Name;
    }
    void Update()
    {
        _currentState.FrameUpdate(this);
    }
    void FixedUpdate()
    {
        _currentState.PhysicsUpdate(this);
    }
    #endregion

    // called by enemy spawning system
    public void SetToInitialState()
    {
        ChangeState(_initialState);
    }
    public void ChangeState(ActionStateLogic newState)
    {
        if (newState == null || newState == _currentState)
            return;
        //Debug.Log($"Changing state from {_currentState.GetType().Name} to {newState.GetType().Name}");
        if(debugMode)
            debugText.text = newState.GetType().Name;
        if(_currentState != null)
            _currentState.OnExitState(this);

        _currentState = newState;
        _currentState.OnEnterState(this);
    }
    public T GetState<T>() where T : ActionStateLogic
    {
        foreach (var state in _switchableStates)
        {
            if (state is T typedState)
                return typedState;
        }

        Debug.LogWarning($"State {typeof(T).Name} not found.");
        return null;
    }
    public int GetSignDirection(Vector2 origin, Vector2 targetPos)
    {
        Vector2 direction = targetPos - origin;
        return direction.x >= 0 ? 1 : -1;
    }
    public int GetSign(Vector2 direction)
    {
        _actionSign = direction.x == 0 ? 0 : (direction.x > 0 ? 1 : -1);
        return _actionSign;
    }

    public int GetSign()
    {
        if(_actionSign == 0) 
            _actionSign = _blackboard.Get<Vector2>("moveinput").x >= 0 ? 1 : -1;
        return _actionSign;
    }

    public void FlipSprite(int direction)
    {
        if (direction == 0)
            return;
        GetComponent<SpriteRenderer>().flipX = direction < 0;
    }
}
