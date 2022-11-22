using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3NonPhys
{
    interface IClickable
    {
        void ClickAction() { Debug.Log("Click"); }
    }
}
