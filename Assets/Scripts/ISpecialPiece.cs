using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public interface ISpecialPiece
    {
        List<Piece> SpecialMove();
    }
}
