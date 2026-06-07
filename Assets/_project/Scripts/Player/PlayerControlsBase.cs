using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace _project.Scripts.Player
{
    public class PlayerControlsBase : MonoBehaviour
    {
        private static readonly int walkSpeed = Animator.StringToHash("WalkSpeed");
        private static readonly int attack = Animator.StringToHash("Attack");
        private static readonly int die = Animator.StringToHash("Die");

        [FormerlySerializedAs("PlayerAnims"), SerializeField]
        private Animator _playerAnims;

        public float speed = 1.0f;
        private Vector2 direction = Vector2.zero;
        private NavMeshAgent navMeshAgent;

        private Controls playerActions;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            navMeshAgent.velocity = new Vector3(direction.x, 0, direction.y) * speed * Time.deltaTime;
            // Instant snapping de la rotation psk flemme d'attendre
            if (navMeshAgent.velocity != Vector3.zero)
                navMeshAgent.transform.eulerAngles = new Vector3(0, Quaternion.LookRotation(navMeshAgent.velocity).eulerAngles.y, 0);

            _playerAnims.SetFloat(walkSpeed, Mathf.Abs(navMeshAgent.velocity.x) + Mathf.Abs(navMeshAgent.velocity.z));
        }

        private void OnEnable()
        {
            Debug.Log("Setup controls");
            navMeshAgent = GetComponent<NavMeshAgent>();
            playerActions ??= new Controls();
            playerActions.Game.Enable();
            playerActions.Game.Move.performed += PlayerMove;
            playerActions.Game.Move.canceled += OnMoveOnCanceled;

            playerActions.Game.Attack.performed += PlayerAttack;
        }

        private void OnDisable()
        {
            playerActions.Game.Move.performed -= PlayerMove;
            playerActions.Game.Move.canceled -= OnMoveOnCanceled;
            playerActions.Game.Attack.performed -= PlayerAttack;
            playerActions.Game.Disable();
        }

        private void OnMoveOnCanceled(InputAction.CallbackContext ctx)
        {
            direction = Vector2.zero;
        }

        private void PlayerAttack(InputAction.CallbackContext callbackContext)
        {
            _playerAnims.SetTrigger(attack);
        }

        private void PlayerDie()
        {
            _playerAnims.SetTrigger(die);
        }


        private void PlayerMove(InputAction.CallbackContext context)
        {
            direction = context.ReadValue<Vector2>();
        }
    }
}