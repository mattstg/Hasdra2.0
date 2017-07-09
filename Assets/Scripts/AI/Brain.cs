using UnityEngine;
using System.Collections;

public class Brain {

    BrainStem brainStem;
    Senses senses;
    Memory memory;
    Cortex cortex;
    EmotionControl emotionalControl;

    public Brain()
    {
        brainStem = new BrainStem();
        senses = new Senses();
        memory = new Memory();
        cortex = new Cortex();
        emotionalControl = new EmotionControl();
    }
}
