using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3NonPhys
{
    public class GenericPiece : Piece, IClickable
    {
        public void ClickAction()
        {
            ToggleHighlight();
        }
    }
}
