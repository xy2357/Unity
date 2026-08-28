using UnityEngine;
using UnityEngine.InputSystem;
using static BoardManager;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 5.0f;

    private BoardManager m_Board;
    private Vector2Int m_CellPosition;

    private bool m_IsGameOver;
    private Animator m_Animator;
    private bool m_IsMoving;
    private Vector3 m_MoveTarget;
    private WallObject m_AttackTarget;
    private bool m_IsAttacking;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        m_Board = boardManager;
        MoveTo(cell, true);
    }

    public void MoveTo(Vector2Int cell, bool immediate)
    {
        m_CellPosition = cell;

        if (immediate)
        {
            m_IsMoving = false;
            transform.position = m_Board.CellToWorld(m_CellPosition);
        }
        else
        {
            m_IsMoving = true;
            m_MoveTarget = m_Board.CellToWorld(m_CellPosition);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void Init()
    {
        if (m_Animator == null)
        {
            m_Animator = GetComponent<Animator>();
        }

        m_IsGameOver = false;
        m_IsMoving = false;
        m_Animator.SetBool("Moving", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsGameOver)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                GameManager.Instance.StartNewGame();
            }
            return;
        }

        if (m_IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_MoveTarget, MoveSpeed * Time.deltaTime);

            if (transform.position == m_MoveTarget)
            {
                m_IsMoving = false;
                m_Animator.SetBool("Moving", false);

                BoardManager.CellData targetCellData = m_Board.GetCellData(m_CellPosition);
                if (targetCellData.ContainerObject != null)
                {
                    targetCellData.ContainerObject.PlayerEntered();
                }
            }

            return;
        }

        if (m_IsAttacking)
        {
            return;
        }

        Vector2Int newCellTarget = m_CellPosition;
        bool hasMoved = false;

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y += 1;
            hasMoved = true;
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y -= 1;
            hasMoved = true;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x += 1;
            hasMoved = true;
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x -= 1;
            hasMoved = true;
        }

        if (hasMoved)
        {
            BoardManager.CellData cellData = m_Board.GetCellData(newCellTarget);

            if (cellData != null && cellData.Passable)
            {
                GameManager.Instance.TurnManager.Tick();

                if (cellData.ContainerObject == null)
                {
                    MoveTo(newCellTarget, false);
                    m_Animator.SetBool("Moving", true);
                }
                else if (cellData.ContainerObject is WallObject wall)
                {
                    m_AttackTarget = wall;
                    m_IsAttacking = true;
                    m_Animator.SetTrigger("Attack");
                }
                else if (cellData.ContainerObject.PlayerWantsToEnter())
                {
                    MoveTo(newCellTarget, false);
                    m_Animator.SetBool("Moving", true);
                }
            }
        }
    }

    public void AttackHit()
    {
        if (m_AttackTarget != null)
        {
            m_AttackTarget.PlayerWantsToEnter();
        }
    }

    public void AttackFinish()
    {
        m_IsAttacking = false;
        m_AttackTarget = null;
    }

    public void GameOver()
    {
        m_IsGameOver = true;
    }
}
