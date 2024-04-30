using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

// �� ���¸� �����ϴ� �������̽�

public interface ITeamState<T>
{
    void Enter(T entity); // ���� ���� �� ȣ��Ǵ� �޼���
    void Update(T entity); // ���� ���� �� ȣ��Ǵ� �޼���
    void Exit(T entity); // ���� ���� �� ȣ��Ǵ� �޼���
}

// �� ���¸� �����ϴ� ���� �ӽ� Ŭ����
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
public class TeamIdleState : ITeamState<Playerteam>
{
    public void Enter(Playerteam character)
    {
       
    }

    public void Update(Playerteam character)
    {
        character.ObjectDistanceCheck();
    }

    public void Exit(Playerteam character)
    {
    }
}

// �ջ�� ���󤿶�
public class TeamMoveState : ITeamState<Playerteam>
{

    public void Enter(Playerteam character)
    {
    }

    public void Update(Playerteam character)
    {
        character.MoveToObject();
    }

    public void Exit(Playerteam character)
    {
    }
}
public class TeamMonsterMoveState : ITeamState<Playerteam>
{

    public void Enter(Playerteam character)
    {
    }

    public void Update(Playerteam character)
    {
        character.TeamMove();
    }

    public void Exit(Playerteam character)
    {
    }
}
// ���� ���� ���� Ŭ����
public class TeamAttackState : ITeamState<Playerteam>
{
    float timeSet = 1f;
    float timeZero = 0f;
    public void Enter(Playerteam character)
    {
       
       
    }

    public void Update(Playerteam character)
    {
        if( character.target.activeSelf == false)
            character.stateMachine2.ChangeState(new TeamMoveState(), character);

        character.animator.SetBool("Atk", false);
        timeZero += Time.deltaTime;
        if(timeZero >= timeSet)
        {
            character.Attack(); // ���� �����϶�
            timeZero = 0f;
           

        }
    }

    public void Exit(Playerteam character)
    {
        //character.stateMachine2.ChangeState(new TeamMonsterMoveState(), character);
    }
}

// �÷��̾� �� Ŭ����
[DisallowMultipleComponent]
public class Playerteam : Unit
{
    public int level;
    public float hp;
    public float maxhealth;
    public float str;
    public string name;
    public string rangetype;
    public int range;
    [SerializeField] FloatingHealthBar healthBar;
    public void Init(TeamData TD)
    {
        this.level = TD.level;
        this.hp = TD.health;
        this.maxhealth = TD.health;
        this.str = TD.str;
        this.name = TD.name;
        this.rangetype = TD.rangetype;
    }

    [SerializeField]
    private NavMeshAgent agent1;
    [SerializeField]
    private NavMeshAgent agent2;
    public NavMeshAgent Agent1 { get { return agent1; } }
    public NavMeshAgent Agent2 { get { return agent2; } }
    public Animator animator { get; private set; }
    public GameObject target1;
    //public GameObject target2;
    public float target2rgdValue;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        target = null;
        agent1 = gameObject.GetComponent<NavMeshAgent>();
        agent2 = gameObject.GetComponent<NavMeshAgent>();

    }
    private void Start()
    {

        stateMachine2 = new TeamStateMachine<Playerteam>();
        stateMachine2.ChangeState(new TeamIdleState(), this); // �ʱ� ���´� �޽� ����
        FindAndSetTarget();
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        if (Agent1.velocity.magnitude > 0)
        {
            animator.SetBool("Run", true);
            //
            Vector3 direction = Agent1.velocity.normalized;

            UpdateAnimatorParameters(direction);
        }
        else
        {
            animator.SetBool("Run", false);
        }

        stateMachine2.Update(this); // ���� �ӽ� ����
    }
    public TeamStateMachine<Playerteam> stateMachine2; // ���� �ӽ� �ν��Ͻ�

    public void ObjectDistanceCheck()
    {
        if (target1 != null && target1.transform != null)
        {
            if (Agent1.isOnNavMesh && (transform.position - target1.transform.position).magnitude > 2)
            {
                stateMachine2.ChangeState(new TeamMoveState(), this);
            }
        }

    }
    public void MoveToObject()
    {
        //Debug.Log("�� Ÿ����");
            //if (Agent1.isOnNavMesh) Agent1.isStopped = false;
            if (Agent1.isOnNavMesh) Agent1.SetDestination(target1.transform.position);
        Quaternion targetRotation = Quaternion.LookRotation(Agent1.velocity.normalized, Vector3.up);
        targetRotation.eulerAngles = new Vector3(0f, 0f, targetRotation.eulerAngles.z);
        transform.rotation = targetRotation;
        stateMachine2.ChangeState(new TeamIdleState(), this);
    }
    // �����ϴ� �޼���
    public void Attack()
    {
        Debug.Log("����!!!");
        Vector3 a = target.transform.position - transform.position;
        animator.SetBool("Atk", true);
        animator.Play("Attack", 0, 0f);
        UpdateAnimatorParameters(a);
        var aa = target.GetComponent<MonsterController>().hp;
        if (aa < 0) {
            animator.SetBool("Atk", false);
        } else
        {
            target.GetComponent<MonsterController>().TakeDamage(str);

        }
        //Debug.Log("���� ������");
        if ((transform.position - target.transform.position).magnitude >= 1.2f)
        {
            stateMachine2.ChangeState(new TeamMonsterMoveState(), this);

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

                //Debug.Log("���Ϳ��� ������...");
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
            //Debug.Log("Exit.............." + stateMachine2.currentState);
            stateMachine2.ChangeState(new TeamMonsterMoveState(), this);

        }
     }

      private void OnTriggerEnter2D(Collider2D collision)
      {
        if (collision.tag == "mon" && stateMachine2.currentState is TeamMonsterMoveState)
        {
            //Debug.Log("Entry.............." + stateMachine2.currentState);
            stateMachine2.ChangeState(new TeamAttackState(), this);

        } else if(collision.tag == "mon" && stateMachine2.currentState is TeamAttackState)
            stateMachine2.ChangeState(new TeamMonsterMoveState(), this);
    } 
    void FindAndSetTarget()
    {
        string myTag = gameObject.tag;

        if (myTag == "team3")
        {
            // Look for team2 first
            target1 = FindWithTag("team2");
            if (target1 == null)
            {
                // If team2 not found, look for team1
                target1 = FindWithTag("team1");
            }
        }
        else if (myTag == "team2")
        {
            // Look for team1 first
            target1 = FindWithTag("team1");
        }

        // If no suitable target found, target the player
        if (target1 == null)
        {
            target1 = FindWithTag("Player");
        }

        if (target1 != null)
        {
            //Debug.Log("Target set to: " + target1.name);
        }
        else
        {
          //  Debug.Log("No target found.");
        }
    }
    GameObject FindWithTag(string tag)
    {
        GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject potentialTarget in potentialTargets)
        {
            if (potentialTarget != gameObject)
            {
                return potentialTarget;
            }
        }
        return null;
    }
    void UpdateAnimatorParameters(Vector3 direction)
    {
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