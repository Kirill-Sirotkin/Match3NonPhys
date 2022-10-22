using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Match3NonPhys
{
    public class GameManager : StateManager
    {
        [field: SerializeField] public Transform _piecesParent { get; private set; }

        private delegate void ActionOnSelectionDelegate(GameObject g);
        private ActionOnSelectionDelegate ActionOnSelection;

        public Piece _selectedPiece { get; private set; } = null;
        public Piece[] _swappedPieces { get; private set; } = new Piece[2];

        [field: SerializeField] private string _spawnerSeed;
        [field: SerializeField] public GameObject[] _regularPieces { get; private set; }

        [field: SerializeField] private GameObject _redPieceBomb;
        [field: SerializeField] private GameObject _bluePieceBomb;
        [field: SerializeField] private GameObject _yellowPieceBomb;
        [field: SerializeField] private GameObject _purplePieceBomb;
        [field: SerializeField] private GameObject _greenPieceBomb;
        [field: SerializeField] private GameObject _redPieceLightning;
        [field: SerializeField] private GameObject _bluePieceLightning;
        [field: SerializeField] private GameObject _yellowPieceLightning;
        [field: SerializeField] private GameObject _purplePieceLightning;
        [field: SerializeField] private GameObject _greenPieceLightning;
        public bool _takeInput { get; set; }
        public Piece[] _lastSwappedPieces { get; set; } = new Piece[2];

        #region Start & Update

        private void Start()
        {
            SetState(new BeginState(this, _spawnerSeed));
        }
        private void Update()
        {
            if (!_takeInput) { return; }
            if (Input.GetMouseButtonDown(0)) { MouseRay(); }
        }

        #endregion

        #region Public methods

        public Sequence SwapPieces(Piece piece1, Piece piece2)
        {
            Sequence seq = DOTween.Sequence();
            Vector3 swapPos = piece1.transform.position;

            seq.Join(piece1.Move(piece2.transform.position));
            seq.Join(piece2.Move(swapPos));
            
            _swappedPieces[0] = piece1;
            _swappedPieces[1] = piece2;

            piece1.ToggleHighlight(false);
            piece2.ToggleHighlight(false);

            return seq;
        }
        public Piece GetRayPiece(Vector3 origin, Vector3 direction)
        {
            RaycastHit hit;
            if (!Physics.Raycast(origin, direction, out hit, Mathf.Infinity)) { return null; }
            return hit.transform.GetComponent<Piece>();
        }

        #endregion

        #region Own methods

        private void MouseRay()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Piece p = GetRayPiece(ray.origin, ray.direction);
            if (p == null) { return; }
            PassSelection(p.gameObject);
        }
        private void PassSelection(GameObject g)
        {
            IClickable clickable = g.GetComponent<IClickable>();
            Piece piece = g.GetComponent<Piece>();

            if (clickable == null) { return; }
            clickable.ClickAction();

            if (piece != null) { ActionOnSelection = PieceAction; }

            ActionOnSelection(piece.gameObject);
        }
        private void PieceAction(GameObject g)
        {
            Sequence seq = DOTween.Sequence();
            Piece piece = g.GetComponent<Piece>();

            if (_selectedPiece == null)
            {
                _selectedPiece = piece;
                return;
            }

            bool distance = Vector3.Distance(_selectedPiece.transform.position, piece.transform.position) > 1.1f;
            bool samePiece = _selectedPiece.gameObject.GetInstanceID() == piece.gameObject.GetInstanceID();

            if (samePiece)
            {
                _selectedPiece.ToggleHighlight(false);
                _selectedPiece = null;
                piece.ToggleHighlight(false);

                return;
            }

            if (distance)
            {
                _selectedPiece.ToggleHighlight(false);
                _selectedPiece = piece;
                piece.ToggleHighlight(true);

                return;
            }

            _takeInput = false;
            _lastSwappedPieces[0] = piece;
            _lastSwappedPieces[1] = _selectedPiece;
            seq = SwapPieces(piece, _selectedPiece);
            seq.OnComplete(() => { _state.SwitchState(); });
            _selectedPiece = null;
        }

        #endregion
    }
}