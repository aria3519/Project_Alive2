using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;





public class NpcQuest : Npcbase
{
    protected override void Yes()
    {
        isYes = true;
    }

    protected override void No()
    {
        isYes = false;
    }

    protected override void Exit()
    {
    }
}
