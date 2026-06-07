using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerControlsBase : MonoBehaviour
{
    private Controls PlayerActions;
    [SerializeField] Animator PlayerAnims;
    public float speed = 1.0f;
    Vector2 direction = Vector2.zero;
    private NavMeshAgent _NavMeshAgent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    private void OnEnable()
    {
        Debug.Log("Setup controls");
        _NavMeshAgent = GetComponent<NavMeshAgent>();
        PlayerActions ??= new Controls();
        PlayerActions.Game.Enable();
        PlayerActions.Game.Move.performed += PlayerMove;
        PlayerActions.Game.Move.canceled += OnMoveOnCanceled;

        PlayerActions.Game.Attack.performed += PlayerAttack;
    }

    private void OnMoveOnCanceled(InputAction.CallbackContext ctx)
    {
        direction = Vector2.zero;
    }

    private void OnDisable()
    {
        PlayerActions.Game.Move.performed -= PlayerMove;
        PlayerActions.Game.Move.canceled -= OnMoveOnCanceled;
        PlayerActions.Game.Attack.performed -= PlayerAttack;
        PlayerActions.Game.Disable();
    }

    private void PlayerAttack(InputAction.CallbackContext callbackContext)
    {
        PlayerAnims.SetTrigger("Attack");
    }
    
    private void PlayerDie()
    {
        PlayerAnims.SetTrigger("Die");
    }


    private void PlayerMove(InputAction.CallbackContext Context)
    {
        direction = Context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        _NavMeshAgent.velocity = (new Vector3(direction.x, 0, direction.y) * speed * Time.deltaTime);
        // Instant snapping de la rotation psk flemme d'attendre
        if (_NavMeshAgent.velocity != Vector3.zero)
            _NavMeshAgent.transform.eulerAngles = new Vector3(0, Quaternion.LookRotation(_NavMeshAgent.velocity).eulerAngles.y, 0);
        
        PlayerAnims.SetFloat("WalkSpeed", Mathf.Abs(_NavMeshAgent.velocity.x) + Mathf.Abs(_NavMeshAgent.velocity.z));
    }
}
