using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterActionHandler : MonoBehaviour
{
    /// <summary>
    /// This script handles the action states of the character
    /// </summary>
    public Rigidbody2D _rigidBody2D;
    public Blackboard _blackboard;
    #region Unity Functions
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
    }
    #endregion

    [SerializeReference] public ActionStateLogic _initialState;
    [SerializeReference] public ActionStateLogic _currentState;

    public void ChangeState(ActionStateLogic newState)
    {
        _currentState = newState;
    }


}
