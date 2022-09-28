using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class GameManager : MonoBehaviour
    {
        [field: SerializeField] private Player _player;
        [field: SerializeField] private Transform _piecesParent;

        private delegate void ActionOnSelectionDelegate(GameObject g);
        private ActionOnSelectionDelegate ActionOnSelection;

        private Piece _selectedPiece = null;

        private void Update()
        {
            bool b = true;
            foreach(Piece p in _piecesParent.transform.GetComponentsInChildren<Piece>())
            {
                if (p.gameObject.activeSelf == false) { continue; }
                if (!p._isIdle) { b = false; }
            }
            Debug.Log(b);
        }

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
            Piece[] _pieces = _piecesParent.GetComponentsInChildren<Piece>();
            Dictionary<int, Piece> piecesToDespawn = new Dictionary<int, Piece>();
            Vector3[] directions = new Vector3[] {
                Vector3.up,
                Vector3.down,
                Vector3.left,
                Vector3.right
            };

            foreach(Piece p in _pieces)
            {
                List<Piece> temporaryPieces = new List<Piece>();
                Vector3 origin = p.transform.position + new Vector3(0f, 0f, -2f);

                for (int i = 0; i < directions.GetLength(0); i++)
                {
                    Piece rayPiece = GetRayPiece(origin + directions[i]);
                    if (rayPiece == null) { continue; }

                    if (rayPiece._type == p._type) { temporaryPieces.Add(rayPiece); }
                }

                if (temporaryPieces.Count > 1)
                {
                    temporaryPieces.Add(p);
                    foreach (Piece tempP in temporaryPieces)
                    {
                        if (piecesToDespawn.ContainsKey(tempP.gameObject.GetInstanceID())) { continue; }
                        piecesToDespawn.Add(tempP.gameObject.GetInstanceID(), tempP);
                    }
                }

                temporaryPieces.Clear();
            }

            Debug.Log(piecesToDespawn.Count);
            DespawnMatchingPieces(piecesToDespawn);
        }
        private Piece GetRayPiece(Vector3 pos)
        {
            Vector3 direction = Vector3.forward;
            RaycastHit hit;

            if (!Physics.Raycast(pos, direction, out hit, 2f)) { return null; }
            return hit.transform.GetComponent<Piece>();
        }
        private void DespawnMatchingPieces(Dictionary<int, Piece> dic)
        {
            if (dic.Count < 1) { return; }
            float sequenceDuration = dic.ElementAt(0).Value.Despawn().Duration();
            Sequence seq = DOTween.Sequence();

            foreach(Piece p in dic.Values)
            {
                seq.Join(p.Despawn());
            }

            seq.OnComplete(()=> { CheckForPatterns(); });
        }
    }
}