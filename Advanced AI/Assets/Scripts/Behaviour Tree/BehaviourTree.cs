using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class BehaviourTree
{
    //STORES ALL NODES PART OF THIS BEHAVIOUR TREE
    public List<Node> nodes;

    public BehaviourTree()
    {
        nodes = new List<Node>();
        //CREATES THE ROOT NODE
        nodes.Add(new Node(Node.Typing.SELECT));
    }

    /// <summary>
    /// Adds a new node to the behaviour tree
    /// </summary>
    /// <param name="_nodeType"></param>
    /// Specify new nodes type.
    /// <param name="_parentID"></param>
    /// Specify the index of this nodes parent. 0 for ROOT node.
    /// <param name="_leaf"></param>
    /// Specify Action Function if new node is a LEAF node, else leave blank.
    public void AddNode(Node.Typing _nodeType, int _parentID, Func<Node.State> _leaf = null)
    {
        Node newNode = new Node(_nodeType, _leaf);
        if(_parentID >= nodes.Count)
        {
            Debug.LogError("Parent ID invalid!");
            return;
        }
        nodes[_parentID].children.Add(newNode);
        if(_nodeType != Node.Typing.LEAF)
            nodes.Add(newNode);
    }
    /// <summary>
    /// Adds a new node to the behaviour tree
    /// </summary>
    /// <param name="_nodeType"></param>
    /// Specify new nodes type.
    /// <param name="_parent"></param>
    /// Specify the Node Object of this nodes parent.
    /// <param name="_leaf"></param>
    /// Specify Action Function if new node is a LEAF node, else leave blank.
    public void AddNode(Node.Typing _nodeType, Node _parent, Func<Node.State> _leaf = null)
    {
        Node newNode = new Node(_nodeType, _leaf);
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] == _parent)
            {
                nodes[i].children.Add(newNode);
                break;
            }
        }
        if (_nodeType != Node.Typing.LEAF)
            nodes.Add(newNode);
    }

    /// <summary>
    /// Interrogate the Behaviour tree
    /// </summary>
    /// <returns></returns>
    /// Returns the state of the last processed node.
    public Node.State Process()
    {
         return TravelTree(nodes[0]);
    }

    /// <summary>
    /// Travels the behaviour tree, executing nodes.
    /// </summary>
    /// <param name="_node"></param>
    /// _node represents the currently looked at Node.
    /// <returns></returns>
    /// Returns the Node Status of the currently looked at node.
    private Node.State TravelTree(Node _node)
    {
        switch (_node.nodeTyping)
        {               
            //If the node is a SELECTOR Node
            case Node.Typing.SELECT:
                //Loop through its children until one returns either a SUCCESS or a RUNNING Status
                foreach (Node child in _node.children)
                {
                    Node.State ans = TravelTree(child);
                    if (ans == Node.State.SUCCESS)
                        return Node.State.SUCCESS;
                    if (ans == Node.State.RUNNING)
                        return Node.State.RUNNING;                    
                }
                //If no children nodes returned either a SUCESS or RUNNING Status, return a FAILED state.
                return Node.State.FAILURE;
            //If the node is a SEQUENCE Node
            case Node.Typing.SEQUENCE:
                //Loop through its children until one returns either a FAILURE or a RUNNING Status
                foreach (Node child in _node.children)
                {
                    Node.State ans = TravelTree(child);
                    if (ans == Node.State.FAILURE)
                        return Node.State.FAILURE;
                    if (ans == Node.State.RUNNING)
                        return Node.State.RUNNING;                    
                }
                //If no children nodes returned either a FAILURE or RUNNING Status, return a SUCCESS State.
                return Node.State.SUCCESS;
            //If the node is a LEAF Node, return its state after "Ticking" it.
            case Node.Typing.LEAF:
                return _node.process();
        }
        //Else return a SUCCESS State for this Node.
        return Node.State.SUCCESS;
    }
}
