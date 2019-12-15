using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.Experimental.XR;

/// <summary>
/// Singleton used to control game state
/// </summary>
public class GameManager : MonoBehaviour
{
    public enum eGameState
    {
        RUNNING,
        GAMEOVER,
    }
    
    public delegate void StateChanged(eGameState state);
    public delegate void StateChangedNoParams();
    public StateChanged OnStateChanged;
    public StateChangedNoParams OnResetEarly;
    public StateChangedNoParams OnResetLate;

    public static GameManager Instance;
    
    public eGameState GameState { get; private set; } = eGameState.RUNNING;


    // Start is called before the first frame update
    void Awake()
    {
        //Enforce singleton
        if(Instance)
            Destroy(gameObject);
        else
            Instance = this; 
    }
    
    public void ChangeState(eGameState newState)
    {
        GameState = newState;
        OnStateChanged?.Invoke(newState);
        if (newState == eGameState.RUNNING) StartCoroutine(StartReset());
    }
    public void ChangeState(int i)
    {
        ChangeState((eGameState)i);
    }

    IEnumerator StartReset()
    {
        OnResetEarly?.Invoke();
        yield return null;
        OnResetLate?.Invoke();
    }
    
}
