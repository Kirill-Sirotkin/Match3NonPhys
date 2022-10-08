using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3NonPhys
{
    public class PatternState : State
    {
        public PatternState(GameManager manager, Piece[] lastSwappedPieces = null) : base(manager)
        {
            _lastSwappedPieces = lastSwappedPieces;
        }

        public override void StartAction()
        {
            gameManager.SetState(new PlayerState(gameManager));
        }

        #region Own methods

        Piece[] _lastSwappedPieces;
        List<Piece> _checkedPieces = new List<Piece>();
        Dictionary<int, Piece> _matchedPieces = new Dictionary<int, Piece>();

        private void MatchPieces()
        {

        }

        #endregion
    }
}
