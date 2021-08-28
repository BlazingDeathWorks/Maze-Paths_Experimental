using UnityEngine;
using System.Collections.Generic;
using System.Collections;

#region DirectionEnum
public enum DirectionCheck { Up, Down, Left, Right, None }
#endregion

public class PathFinder : MonoBehaviour
{
    //Cache References
    [SerializeField] private PathFinder pathObject = null;
    private SpriteRenderer sr = null;
    [SerializeField] private HandleRelationship handleRelationshipObject = null;

    //Parent List
    private List<GameObject> parentList = new List<GameObject>();

    //Keyword
    const string endKeyword = "End";

    //Direction Vectors
    List<Vector2> directions = new List<Vector2>();
    Vector2 up { get => new Vector3(transform.position.x, transform.position.y + 1, RegisterPosition.registeredZPos); }
    Vector2 down { get => new Vector3(transform.position.x, transform.position.y - 1, RegisterPosition.registeredZPos); }
    Vector2 right { get => new Vector3(transform.position.x + 1, transform.position.y, RegisterPosition.registeredZPos); }
    Vector2 left { get => new Vector3(transform.position.x - 1, transform.position.y, RegisterPosition.registeredZPos); }

    //Direction Check Enum Instance
    public DirectionCheck directionAssignment { private get; set; } = DirectionCheck.None;

    //How many walls are around you?
    private int deadEnds = 0;
    private bool isDeadEnd = false;

    //Is there a successful path?
    private bool pathSuccess = false;
    //Is this a shared object?
    private bool isShared = false;
    public bool IsShared { get => isShared; }

    //How many seconds before path execution?
    private float timeBeforeExecution = 1f;

    public void AddParentToList(GameObject gameobject)
    {
        parentList.Add(gameobject);
    }

    public DirectionCheck GetDirectionAssignment()
    {
        return directionAssignment;
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        RegisterAsPlayerPos(transform.position);
    }

    IEnumerator Start()
    {
        TakeAwayAllChildren();
        yield return new WaitForSecondsRealtime(timeBeforeExecution);
        InstantiateEachDirection();
    }

    private void TakeAwayAllChildren()
    {
        if(transform.childCount > 0)
        {
            foreach(Transform child in transform)
            {
                if(child.TryGetComponent(out PathFinder pathFinder))
                {
                    pathFinder.RemovePlayerPosEmergency(child.position);
                }
                Destroy(child.gameObject);
            }
        }
    }

    private void Update()
    {
        if(isShared && pathSuccess)
        {
            DisplayRightPathToAllParents();
        }
        if (pathSuccess)
        {
            DisplayRightPathToAllChildren();
        }
    }

    private void DisplayRightPathToAllParents()
    {
        foreach(GameObject parent in parentList)
        {
            if(parent.TryGetComponent(out PathFinder pathFinder))
            {
                pathFinder.DisplayRightPath();
            }
        }
    }

    public void DisplayRightPathToAllChildren()
    {
        if(transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out PathFinder pathFinder))
                {
                    pathFinder.DisplayRightPathToChild();
                }
            }
        }
    }

    private void InstantiateEachDirection()
    {
        if (pathObject)
        {
            AssignDirection();
            DetectionPerDirection();
            InstantiateInAllDirections();
            DeadEndCheck();
        }
    }

    void DeadEndCheck()
    {
        if(deadEnds >= 3 && directionAssignment != DirectionCheck.None)
        {
            RegisterPosition.registeredWallPositions.Add(transform.position);
            GetComponent<SpriteRenderer>().color = new Color(174, 0, 202, 255); //Purple
            isDeadEnd = true;
            if (transform.parent)
            {
                transform.parent.GetComponent<PathFinder>().DetectionAndDeadendCheck();
                transform.parent = null;
            }
        }
    }

    private void AssignDirection()
    {
        switch (directionAssignment)
        {
            case DirectionCheck.Up:
                ReturnUp();
                break;
            case DirectionCheck.Down:
                ReturnDown();
                break;
            case DirectionCheck.Left:
                ReturnLeft();
                break;
            case DirectionCheck.Right:
                ReturnRight();
                break;
            default:
                ReturnAllDirections();
                break;
        }
    }

    public void DetectionAndDeadendCheck()
    {
        DetectionPerDirection();
        DeadEndCheck();
    }

    private DirectionCheck AssignEnum(Vector2 direction)
    {
        if(direction == up)
        {
            return DirectionCheck.Up;
        }
        else if (direction == down)
        {
            return DirectionCheck.Down;
        }
        else if (direction == right)
        {
            return DirectionCheck.Right;
        }
        return DirectionCheck.Left;
    }

    void DetectionPerDirection()
    {
        deadEnds = 0;
        for (int i = 0; i < directions.Count; i++)
        {
            Vector2 direction = directions[i];
            if (IsWall(direction))
            {
                deadEnds++;
            }
        }
    }

    void InstantiateInAllDirections()
    {
        //ReturnAllDirections(); Only unlock this if you want to instantiate in all directions
        for(int i = 0; i < directions.Count; i++)
        {
            Vector2 direction = directions[i];
            if (!IsWall(direction))
            {
                if (!IsPlayer(direction) || End.IsEnd(direction))
                {
                    InstantiatePath(direction);
                }
                else
                {
                    InstantiateRelationshipHandler(direction);
                }
            }
        }
    }

    private void InstantiateRelationshipHandler(Vector2 direction)
    {
        HandleRelationship relationshipHandlerScript = Instantiate(handleRelationshipObject, direction, Quaternion.identity);
        relationshipHandlerScript.transform.parent = transform;
    }

    void InstantiatePath(Vector2 direction)
    {
        PathFinder instantiatedPath = Instantiate(pathObject, direction, Quaternion.identity);
        instantiatedPath.transform.parent = transform;
        instantiatedPath.directionAssignment = AssignEnum(direction);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(endKeyword))
        {
            DisplayRightPath();
            Destroy(gameObject);
        }
    }

    public void DisplayRightPath()
    {
        DisplayRightPathColorsOnly();
        if (transform.parent)
        {
            transform.parent.TryGetComponent(out PathFinder parentPathFinder);
            if(parentPathFinder.GetDirectionAssignment() != DirectionCheck.None)
            {
                if (!parentPathFinder.IsShared)
                {
                    parentPathFinder.DisplayRightPath();
                }
                else
                {
                    parentPathFinder.DisplayRightPathColorsOnly();
                }
            }
        }
    }

    private void DisplayRightPathToChild()
    {
        if (!isDeadEnd)
        {
            DisplayRightPathColorsOnly();
        }
    }

    public void DisplayRightPathColorsOnly()
    {
        SetPathSuccessToTrue();
        sr.color = new Color32(0, 244, 255, 255);
    }

    private bool IsPlayer(Vector2 direction)
    {
        if (RegisterPosition.registeredPlayerPositions.Contains(direction))
        {
            return true;
        }
        return false;
    }

    private void RegisterAsPlayerPos(Vector2 pos)
    {
        RegisterPosition.registeredPlayerPositions.Add(pos);
    }

    private void RemovePlayerPosEmergency(Vector2 pos)
    {
        if (RegisterPosition.registeredPlayerPositions.Contains(pos))
        {
            RegisterPosition.registeredPlayerPositions.Remove(pos);
        }
    }

    public void SetPathSuccessToTrue()
    {
        pathSuccess = true;
    }

    public void SetSharedToTrue()
    {
        isShared = true;
    }

    #region IsWall
    private bool IsWall(Vector2 direction)
    {
        if (RegisterPosition.registeredWallPositions.Contains(direction))
        {
            return true;
        }
        return false;
    }
    #endregion

    #region ReturnDirections
    void ReturnUp()
    {
        directions = new List<Vector2>() { up, right, left };
    }
    void ReturnDown()
    {
        directions = new List<Vector2>() { down, right, left };
    }
    void ReturnLeft()
    {
        directions = new List<Vector2>() { left, up, down };
    }
    void ReturnRight()
    {
        directions = new List<Vector2>() { right, up, down };
    }

    void ReturnAllDirections()
    {
        directions = new List<Vector2>() { up, down, right, left };
    }
    #endregion
}
