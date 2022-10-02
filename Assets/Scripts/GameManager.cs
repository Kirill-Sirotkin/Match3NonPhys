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
        [field: SerializeField] private Spawner _spawner;

        private delegate void ActionOnSelectionDelegate(GameObject g);
        private ActionOnSelectionDelegate ActionOnSelection;

        private Piece _selectedPiece = null;
        private Piece[] _swappedPieces = new Piece[2];

        private void LateUpdate()
        {
            if (CheckPiecesIdle())
            {
                _player._takeInputs = true;
            }
            else
            {
                _player._takeInputs = false;
            }
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

            bool distance = Vector3.Distance(_selectedPiece.transform.position, piece.transform.position) > 1.1f;
            bool samePiece = _selectedPiece.gameObject.GetInstanceID() == piece.gameObject.GetInstanceID();

            if (distance || samePiece)
            {
                _selectedPiece.ToggleHighlight(false);
                _selectedPiece = null;
                piece.ToggleHighlight(false);

                return;
            }

            SwapPieces(piece);
        }
        private void SwapPieces(Piece piece, bool checkForPatters = true)
        {
            Vector3 swapPos = piece.transform.position;
            Sequence seq = DOTween.Sequence();

            seq.Append(piece.Move(_selectedPiece.transform.position));
            seq.Join(_selectedPiece.Move(swapPos));
            if (checkForPatters) { seq.OnComplete(() => { CheckForPatterns(); }); }
            
            _swappedPieces[0] = _selectedPiece;
            _swappedPieces[1] = piece;

            _selectedPiece.ToggleHighlight(false);
            _selectedPiece = null;
            piece.ToggleHighlight(false);
        }
        private int CheckForPatterns()
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

            //Debug.Log(piecesToDespawn.Count);
            DespawnMatchingPieces(piecesToDespawn);
            if (piecesToDespawn.Count > 0) { return piecesToDespawn.Count; }
            if (_swappedPieces[0] == null || _swappedPieces[1] == null) { return piecesToDespawn.Count; }

            _selectedPiece = _swappedPieces[0];
            SwapPieces(_swappedPieces[1], false);

            return piecesToDespawn.Count;
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

            seq.OnComplete(()=> 
            { 
                foreach (Piece p in dic.Values)
                {
                    _spawner.SpawnPiece(p.transform.position + new Vector3(0f, 5f, 0f), _piecesParent);
                    _spawner.MovePiecesDown();
                }
            });

            _swappedPieces[0] = null;
            _swappedPieces[1] = null;
        }
        private bool CheckPiecesIdle()
        {
            bool b = true;
            foreach(Piece p in _piecesParent.GetComponentsInChildren<Piece>())
            {
                if (p.gameObject.activeSelf == false) { continue; }
                if (!p._isIdle) { b = false; }
            }
            return b;
        }
    }
}