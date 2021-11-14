using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Tutorial : MonoBehaviour, ITutorial
{
    public bool TutorialActionComplete { get; set; } = false;

    public virtual void StartTutorial()
    {

    }
}
