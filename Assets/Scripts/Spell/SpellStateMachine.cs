using UnityEngine;
using System.Collections;

public class SpellStateMachine {

    public TreeTracker treeTracker;
    public StartState startState;
    
    public SpellStateMachine(TreeTracker tree)
    {
        treeTracker = tree;
    }

    public State GetNextState(SpellInfo spellInfo) //add the timer additions here
    {
        State currentPointer = spellInfo.currentState;
        foreach (Transition t in currentPointer.outwardsTransitions)
        {
            State toReturn = t.CheckTransition(spellInfo);
            if (toReturn != currentPointer)
            {
                spellInfo.relData.UpdateState();
                return toReturn;
            }
        }       

        return currentPointer;
    }

}
