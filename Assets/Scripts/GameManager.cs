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

        [field: SerializeField] private GameObject _redPiece;
        [field: SerializeField] private GameObject _bluePiece;
        [field: SerializeField] private GameObject _yellowPiece;
        [field: SerializeField] private GameObject _purplePiece;
        [field: SerializeField] private GameObject _greenPiece;
        public GameObject[] _pieces { get; private set; }
        public bool _takeInput { get; set; }
        public Piece[] _lastSwappedPieces { get; set; } = new Piece[2];

        #region Start & Update

        private void Start()
        {
            _pieces = new GameObject[] { _redPiece, _bluePiece, _yellowPiece, _purplePiece, _greenPiece };
            SetState(new BeginState(this));
        }
        private void Update()
        {
            if (!_takeInput) { return; }
            if (Input.GetMouseButtonDown(0)) { Debug.Log("LMB pressed"); MouseRay(); }
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
            PassSelection(GetRayPiece(ray.origin, ray.direction).gameObject);
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

            if (distance || samePiece)
            {
                _selectedPiece.ToggleHighlight(false);
                _selectedPiece = null;
                piece.ToggleHighlight(false);

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


        /*
        private int CheckPossibleMoves(Vector3 pos)
        {
            int p1 = CheckPattern1(pos);
            int p2 = CheckPattern2(pos);
            int p3 = CheckPattern3(pos);

            return p1 + p2 + p3;
        }
        private int CheckPattern1(Vector3 pos)
        {
            int res = 0;
            Vector3 modifierVector = new Vector3(1f, 1f, -1.05f);
            PieceType type = GetRayPiece(pos + new Vector3(0f, 0f, -1.05f))._type;

            for (int i = 0; i < 4; i++)
            {
                Vector3 checkVector = pos + modifierVector;
                //Debug.Log(checkVector);
                Piece p = GetRayPiece(checkVector);

                if (p != null && p._type == type)
                {
                    Vector3 v1 = pos + new Vector3(modifierVector.x * 2, modifierVector.y * 0, modifierVector.z);
                    Vector3 v2 = pos + new Vector3(modifierVector.x * 0, modifierVector.y * 2, modifierVector.z);

                    Piece p2 = GetRayPiece(v1);
                    if (p2 != null && p2._type == type) { res++; }
                    
                    p2 = GetRayPiece(v2);
                    if (p2 != null && p2._type == type) { res++; }
                }

                modifierVector = new Vector3(modifierVector.y, modifierVector.x * -1, modifierVector.z);
            }

            return res;
        }
        private int CheckPattern2(Vector3 pos)
        {
            int res = 0;
            Vector3 modifierVector = new Vector3(1f, 0f, -1.05f);
            PieceType type = GetRayPiece(pos + new Vector3(0f, 0f, -1.05f))._type;

            for (int i = 0; i < 4; i++)
            {
                Vector3 checkVector = pos + modifierVector;
                //Debug.Log(checkVector);
                Piece p = GetRayPiece(checkVector);

                if (p != null && p._type == type)
                {
                    Vector3 v1 = pos + modifierVector * 2 + new Vector3(modifierVector.y, modifierVector.x);
                    Vector3 v2 = pos + modifierVector * 2 + new Vector3(modifierVector.y, modifierVector.x) * -1;

                    Piece p2 = GetRayPiece(v1);
                    if (p2 != null && p2._type == type) { res++; }

                    p2 = GetRayPiece(v2);
                    if (p2 != null && p2._type == type) { res++; }
                }

                modifierVector = new Vector3(modifierVector.y, modifierVector.x * -1, modifierVector.z);
            }

            return res;
        }
        private int CheckPattern3(Vector3 pos)
        {
            int res = 0;
            Vector3 modifierVector = new Vector3(1f, 0f, -1.05f);
            PieceType type = GetRayPiece(pos + new Vector3(0f, 0f, -1.05f))._type;

            for (int i = 0; i < 4; i++)
            {
                Vector3 checkVector = pos + modifierVector;
                //Debug.Log(checkVector);
                Piece p = GetRayPiece(checkVector);

                if (p != null && p._type == type)
                {
                    Vector3 v1 = pos + modifierVector * 3;

                    Piece p2 = GetRayPiece(v1);
                    if (p2 != null && p2._type == type) { res++; }
                }

                modifierVector = new Vector3(modifierVector.y, modifierVector.x * -1, modifierVector.z);
            }

            return res;
        }
        */

        private Sequence ShufflePieces()
        {
            Sequence seq = DOTween.Sequence();
            List<Piece> pieces = new List<Piece>();
            foreach(Piece p in _piecesParent.GetComponentsInChildren<Piece>())
            {
                pieces.Add(p);
            }

            List<Piece> shuffledPieces = new List<Piece>();
            List<Piece> copiedPieces = new List<Piece>(pieces);
            for (int i = 0; i < pieces.Count; i++)
            {
                int index = UnityEngine.Random.Range(0, copiedPieces.Count);

                shuffledPieces.Add(copiedPieces[index]);
                copiedPieces.RemoveAt(index);
            }

            for (int i = 0; i < pieces.Count; i++)
            {
                seq.Join(pieces[i].Move(shuffledPieces[i].transform.position));
            }
            //seq.OnComplete(() => { CheckForPatterns(); });

            return seq;
        }
    }
}