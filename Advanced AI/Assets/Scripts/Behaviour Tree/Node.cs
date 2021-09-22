using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Node
{
    //ENUM REPRESENTS A NODES TYPE.
    public enum Typing { SELECT, SEQUENCE, LEAF};
    //ENUM REPRESENTS A NODES RETURN STATUS.
    public enum State { SUCCESS, FAILURE, RUNNING};
    //LIST STORES REFERENCES TO CHILD NODES OF THIS NODE.
    public List<Node> children = new List<Node>();
    
    public Typing nodeTyping;
    public State nodeState;

    //DELEGATE USED TO REFERENCE AN ACTION FUNCTION
    public delegate State Process();
    public Process process;

    public Node(Typing _nodeType, Func<State> _process = null)
    {
        nodeTyping = _nodeType;
        if (_process != null)
            process = new Process(_process);
    }

}
