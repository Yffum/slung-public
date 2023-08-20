using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Evenly spaces the children of the attached GameObject, between _rightAnchor and _leftAnchor, 
/// every FixedUpdate (while the attached GameObject is active)
/// </summary>
public class ChildrenSpacer : MonoBehaviour
{
    [SerializeField] private Transform _leftAnchor;

    [SerializeField] private Transform _rightAnchor;

    /// <summary>
    /// A list of all child objects
    /// </summary>
    private List<Transform> _children;

    private void Awake()
    {
        InitializeListOfChildren();
    }

    /// <summary>
    /// Initialize beads list with every child object
    /// </summary>
    private void InitializeListOfChildren()
    {
        _children = new List<Transform>();

        foreach (Transform bead in transform.GetComponentsInChildren<Transform>())
        {
            _children.Add(bead);
        }
    }

    void Update()
    {
        AdjustChildrenPositions();
    }

    /// <summary>
    /// Evenly space children between _rightAnchor and _leftAnchor
    /// </summary>
    private void AdjustChildrenPositions()
    {
        // get spacing by dividing difference in position by number of children
        float xSpacing = (_rightAnchor.position.x - _leftAnchor.position.x) / _children.Count;
        float ySpacing = (_rightAnchor.position.y - _leftAnchor.position.y) / _children.Count;

        // reposition each child
        for (int i = 0; i < _children.Count; i++)
        {
            float xPosition = _leftAnchor.position.x + ((i) * xSpacing); 
            float yPosition = _leftAnchor.position.y + ((i) * ySpacing);

            _children[i].position = new Vector3(xPosition, yPosition);
        }
    }
}
