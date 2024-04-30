using NavMeshPlus.Components;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
namespace players
{

    public interface ITeamState<T>
    {
        void Enter(T entity); // ���� ���� �� ȣ��Ǵ� �޼���
        void Update(T entity); // ���� ���� �� ȣ��Ǵ� �޼���
        void Exit(T entity); // ���� ���� �� ȣ��Ǵ� �޼���
    }
    public class TeamStateMachine<T>
    {
        public ITeamState<T> currentState { get; private set; } // ���� ����
        public void ChangeState(ITeamState<T> newState, T entity) // ���� ���� �޼���
        {
            if (currentState != null)
            {
                currentState.Exit(entity); // ���� ���¿��� ������
            }
            currentState = newState; // ���ο� ���·� ����
            currentState.Enter(entity); // ���ο� ���� ����
        }
        // ���� ���� �޼���
        public void Update(T entity)
        {
            if (currentState != null)
            {
                currentState.Update(entity); // ���� ���� ����

            }

        }
    }

    // ���� �޽� ���� Ŭ����
    public class TeamIdleState : ITeamState<MesRay>
    {
        public void Enter(MesRay character)
        {

        }

        public void Update(MesRay character)
        {
            character.ObjectDistanceCheck();
        }

        public void Exit(MesRay character)
        {
        }
    }

    // �ջ�� ���󤿶�
    public class TeamMoveState : ITeamState<MesRay>
    {

        public void Enter(MesRay character)
        {
        }

        public void Update(MesRay character)
        {
            character.MoveToObject();
        }

        public void Exit(MesRay character)
        {
        }
    }
    public class TeamMonsterMoveState : ITeamState<MesRay>
    {

        public void Enter(MesRay character)
        {
        }

        public void Update(MesRay character)
        {
            character.TeamMove();
        }

        public void Exit(MesRay character)
        {
        }
    }
    // ���� ���� ���� Ŭ����
    public class TeamAttackState : ITeamState<MesRay>
    {
        float timeSet = 1f;
        float timeZero = 0f;
        public void Enter(MesRay character)
        {


        }

        public void Update(MesRay character)
        {
            if (character.target.activeSelf == false)
                character.stateMachine2.ChangeState(new TeamMoveState(), character);

            character.animator.SetBool("Atk", false);
            timeZero += Time.deltaTime;
            if (timeZero >= timeSet)
            {
                character.Attack(); // ���� �����϶�
                timeZero = 0f;


            }
        }

        public void Exit(MesRay character)
        {
            //character.stateMachine2.ChangeState(new TeamMonsterMoveState(), character);
        }
    }

    // �÷��̾� �� Ŭ����
    [DisallowMultipleComponent]
    public class MesRay : Unit
    {
        //private Animator animator;ddd
        private NavMeshAgent agent;
        public bool showPath;
        public bool showAhead;
        [SerializeField] FloatingHealthBar healthBar;

        [SerializeField]
        private NavMeshAgent agent2;
        public NavMeshAgent Agent2 { get { return agent2; } }
        public Animator animator { get; private set; }

        public GameObject target1;
        //public GameObject target2;
        public float target2rgdValue;
        public Statusinfo playerinfo;
        private void Awake()
        {
            healthBar = GetComponentInChildren<FloatingHealthBar>();

            playerinfo = GetComponent<Statusinfo>();

            animator = GetComponent<Animator>();
            target = null;
            agent2 = gameObject.GetComponent<NavMeshAgent>();

            agent = GetComponent<NavMeshAgent>();


        }
        private void Start()
        {
            playerinfo.Maxhealth = playerinfo.hp;

            stateMachine2 = new TeamStateMachine<MesRay>();
            stateMachine2.ChangeState(new TeamIdleState(), this); // �ʱ� ���´� �޽� ����
        }

        private void Update()
        {

            /*if (Agent2.velocity.magnitude > 0)
            {
                animator.SetBool("Run", true);
                //Debug.Log(navMeshAgent.velocity);
                Vector3 direction = Agent2.velocity.normalized;
                //Debug.Log(direction);
                UpdateAnimatorParameters(direction);
            }
            else
            {
                animator.SetBool("Run", false);
            }*/

            if (Input.GetMouseButtonUp(0))
            {

                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                List<MesRay> petColponent = new List<MesRay>();
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("mon"))
                    {
                        for (int i = 0; i < transform.childCount; i++)
                        {
                            if (i == 0)
                            {
                                Playerteam playerTeam = transform.GetChild(i).GetComponent<Playerteam>();
                                if (playerTeam != null)
                                {
                                    playerTeam.MoveMonster(hit.collider.gameObject);
                                }
                            }
                            else
                            {
                                transform.GetChild(i).GetComponent<Playerteam>().MoveMonster(hit.collider.gameObject);
                            }
                        }
                        if(transform.tag == "Player")
                        {
                            MoveMonster(hit.collider.gameObject);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {

                        if (i == 0)
                        {
                            Playerteam playerTeam = transform.GetChild(i).GetComponent<Playerteam>();
                            if (playerTeam != null)
                            {
                                // PlayerTeam ������Ʈ�� ������ MovePlayer �޼ҵ带 ȣ���մϴ�.
                                playerTeam.MovePlayer();
                            }
                            else
                            {
                                // PlayerTeam ������Ʈ�� ���ٸ� �α׸� ����ϴ�.
                                Debug.Log("0��° �ڽĿ� PlayerTeam ������Ʈ�� �����ϴ�.");
                            }
                        } else
                        {
                            transform.GetChild(i).GetComponent<Playerteam>().MovePlayer();

                        }
                    }
                    if (transform.tag == "Player")
                    {
                        MovePlayer();
                    }
                }
            }
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            if (agent.velocity.magnitude > 0)
            {
                animator.SetBool("Run", true);
                Vector3 direction = agent.velocity.normalized;
                UpdateAnimatorParameters(direction);
            }
            else
            {
                animator.SetBool("Run", false);
            }
            Debug.Log(stateMachine2.currentState);
            stateMachine2.Update(this); // ���� �ӽ� ����
        }
        public TeamStateMachine<MesRay> stateMachine2; // ���� �ӽ� �ν��Ͻ�

        public void ObjectDistanceCheck()
        {
            if (target1 != null && target1.transform != null)
            {
                if (agent.isOnNavMesh && (transform.position - target1.transform.position).magnitude > 2)
                {
                    stateMachine2.ChangeState(new TeamMoveState(), this);
                }
            }
        }
        public void MoveToObject()
        {
            if (Input.GetMouseButtonUp(0))
            {
                var target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                target.z = 0;
                agent.destination = target;
            }
           
        }
        // �����ϴ� �޼���
        public void Attack()
        {
            if (target == null)
            {
                Debug.Log("Attack called but target is null");
                return;
            }

            Vector3 a = target.transform.position - transform.position;
            animator.SetBool("Atk", true);
            animator.Play("Attack", 0, 0f);
            UpdateAnimatorParameters(a);

            var aa = target.GetComponent<MonsterController>().hp;
            if (aa < 0)
            {
                animator.SetBool("Atk", false);
            }
            else
            {
                target.GetComponent<MonsterController>().TakeDamage(playerinfo.str);
            }
            if ((transform.position - target.transform.position).magnitude >= 1.2f)
            {
                stateMachine2.ChangeState(new TeamMonsterMoveState(), this);
            }
        }
        public void TakeDamage(float damageAmount)
        {
            playerinfo.hp -= damageAmount;
            healthBar.UpdateHealthBar(playerinfo.hp, playerinfo.Maxhealth);
            if (playerinfo.hp < 0)
            {
            }
        }
        public void Attackend()
        {
            animator.SetBool("Atk", false);
        }
        public void TeamMove()
        {
            if (target != null)
            {
                if ((transform.position - target.transform.position).magnitude >= 1.2f)
                {

                    Debug.Log("���Ϳ��� ������...");
                    //if (Agent2.isOnNavMesh) Agent2.isStopped = false;
                    if (Agent2.isOnNavMesh) Agent2.SetDestination(target.transform.position);
                    Quaternion targetRotation = Quaternion.LookRotation(Agent2.velocity.normalized, Vector3.up);
                    targetRotation.eulerAngles = new Vector3(0f, 0f, targetRotation.eulerAngles.z);
                    transform.rotation = targetRotation;
                }
                else
                {
                    stateMachine2.ChangeState(new TeamAttackState(), this);

                }
            }
        }

        public void MoveMonster(GameObject mst)
        {
            //���� Ŭ���� ������ ���ӿ�����Ʈ�� Ÿ��2�� ����
            target = mst;
            stateMachine2.ChangeState(new TeamMonsterMoveState(), this);
        }
        public void MovePlayer()
        {
            stateMachine2.ChangeState(new TeamMoveState(), this);
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "mon" && stateMachine2.currentState is TeamAttackState)
            {
                stateMachine2.ChangeState(new TeamMonsterMoveState(), this);
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "mon" && stateMachine2.currentState is TeamMonsterMoveState)
            {
                stateMachine2.ChangeState(new TeamAttackState(), this);
            }
            else if (collision.tag == "mon" && stateMachine2.currentState is TeamAttackState)
                stateMachine2.ChangeState(new TeamMonsterMoveState(), this);
        }
        void UpdateAnimatorParameters(Vector3 direction)
        {
            Debug.Log("?");
            Vector3 localDirection = transform.InverseTransformDirection(direction);
            float angle = Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg;
            int directionIndex = Mathf.RoundToInt((angle + 360f) % 360f / 45f) % 8;
            switch (directionIndex)
            {
                case 0: // Right
                    animator.SetFloat("Horizontal", 1);
                    animator.SetFloat("Vertical", 0);
                    break;
                case 1: // Up-right
                    animator.SetFloat("Horizontal", 1);
                    animator.SetFloat("Vertical", 1);
                    break;
                case 2: // Up
                    animator.SetFloat("Horizontal", 0);
                    animator.SetFloat("Vertical", 1);
                    break;
                case 3: // Up-left
                    animator.SetFloat("Horizontal", -1);
                    animator.SetFloat("Vertical", 1);
                    break;
                case 4: // Left
                    animator.SetFloat("Horizontal", -1);
                    animator.SetFloat("Vertical", 0);
                    break;
                case 5: // Down-left
                    animator.SetFloat("Horizontal", -1);
                    animator.SetFloat("Vertical", -1);
                    break;
                case 6: // Down
                    animator.SetFloat("Horizontal", 0);
                    animator.SetFloat("Vertical", -1);
                    break;
                case 7: // Down-right
                    animator.SetFloat("Horizontal", 1);
                    animator.SetFloat("Vertical", -1);
                    break;
                default:
                    break;
            }
        }
    }
}
