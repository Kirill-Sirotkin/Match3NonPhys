using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class GameManager : MonoBehaviour
    {
        private delegate void ActionOnSelectionDelegate(GameObject g);
        private ActionOnSelectionDelegate ActionOnSelection;

        private Piece _selectedPiece = null;

        public void PassSelection(RaycastHit hit)
        {
            IClickable clickable = hit.transform.GetComponent<IClickable>();
            Piece piece = hit.transform.GetComponent<Piece>();

            if (clickable == null) { return; }
            clickable.ClickAction();

            if (piece != null) { ActionOnSelection = PieceAction; }

            ActionOnSelection(piece.gameObject);
        }
        private void PieceAction(GameObject g)
        {
            Piece piece = g.GetComponent<Piece>();

            if (_selectedPiece == null)
            {
                _selectedPiece = piece;
                return;
            }

            if (Vector3.Distance(_selectedPiece.transform.position, piece.transform.position) > 1.1f)
            {
                _selectedPiece.ToggleHighlight(false);
                _selectedPiece = null;
                piece.ToggleHighlight(false);

                return;
            }

            SwapPieces(piece);
        }
        private void SwapPieces(Piece piece)
        {
            Vector3 swapPos = piece.transform.position;
            Sequence seq = DOTween.Sequence();

            seq.Append(piece.Move(_selectedPiece.transform.position));
            seq.Join(_selectedPiece.Move(swapPos));
            seq.OnComplete(() => { CheckForPatterns(); });

            _selectedPiece.ToggleHighlight(false);
            _selectedPiece = null;
            piece.ToggleHighlight(false);
        }
        private void CheckForPatterns()
        {
            Debug.Log("movements done");
        }
    }
}