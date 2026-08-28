using UnityEngine;

public class EnemyObject : CellObject
{
    public int Health = 3;
    private int m_CurrentHealth;

    public PlayerController PlayerController;
    public Animator m_Animator;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        GameManager.Instance.TurnManager.OnTick += TurnHappened;
    }

    public void OnDestroy()
    {
        GameManager.Instance.TurnManager.OnTick -= TurnHappened;
    }

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);
        m_CurrentHealth = Health;
    }

    public override bool PlayerWantsToEnter()
    {
        m_CurrentHealth -= 1;

        if (m_CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }

        return false;
    }

    bool MoveTo(Vector2Int coord)
    {
        var board = GameManager.Instance.BoardManager;
        var targetCell = board.GetCellData(coord);

        if (targetCell == null
            || !targetCell.Passable
            || targetCell.ContainerObject != null)
        {
            return false;
        }

        var currentCell = board.GetCellData(m_Cell);
        currentCell.ContainerObject = null;

        targetCell.ContainerObject = this;
        m_Cell = coord;
        transform.position = board.CellToWorld(coord);

        return true;
    }

    void TurnHappened()
    {
        var playerCell = GameManager.Instance.PlayerController.Cell;

        int xDist = playerCell.x - m_Cell.x;
        int yDist = playerCell.y - m_Cell.y;

        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        if (xDist == 0 && absYDist == 1
            || yDist == 0 && absXDist == 1)
        {
            m_Animator.SetTrigger("Attack");
            GameManager.Instance.PlayerController.TakeDamage(3);
        }
        else
        {
            if (absXDist > absYDist)
            {
                if (!TryMoveInX(xDist))
                {
                    TryMoveInY(yDist);
                }
            }
            else 
            {
                if (!TryMoveInY(yDist))
                {
                    TryMoveInX(xDist);
                }
            }
        }
    }

    bool TryMoveInX(int xDist)
    {
        if (xDist > 0)
        {
            return MoveTo(m_Cell + Vector2Int.right);
        }
        return MoveTo(m_Cell + Vector2Int.left);
    }

    bool TryMoveInY(int yDist)
    {
        if (yDist > 0)
        {
            return MoveTo(m_Cell + Vector2Int.up);
        }
        return MoveTo(m_Cell + Vector2Int.down);
    }
}
