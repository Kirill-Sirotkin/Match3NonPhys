using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Additional
using System.Linq;
using DG.Tweening;
using TMPro;

namespace Match3NonPhys
{
    public class GameManager : StateManager
    {
        [field: SerializeField] public Transform _piecesParent { get; private set; }
        [field: SerializeField] public TMP_Text _scoreNumUI { get; private set; }
        private int _scoreNum = 0;

        private delegate void ActionOnSelectionDelegate(GameObject g);
        private ActionOnSelectionDelegate ActionOnSelection;

        public Piece _selectedPiece { get; private set; } = null;
        public Piece[] _swappedPieces { get; private set; } = new Piece[2];

        [Header ("Regular Pieces")]
        [field: SerializeField] private GameObject _redPiece;
        [field: SerializeField] private GameObject _bluePiece;
        [field: SerializeField] private GameObject _yellowPiece;
        [field: SerializeField] private GameObject _purplePiece;
        [field: SerializeField] private GameObject _greenPiece;
        [Header("Bomb Pieces")]
        [field: SerializeField] private GameObject _redBombPiece;
        [field: SerializeField] private GameObject _blueBombPiece;
        [field: SerializeField] private GameObject _yellowBombPiece;
        [field: SerializeField] private GameObject _purpleBombPiece;
        [field: SerializeField] private GameObject _greenBombPiece;
        [Header("Lightning Pieces")]
        [field: SerializeField] private GameObject _redLightningPiece;
        [field: SerializeField] private GameObject _blueLightningPiece;
        [field: SerializeField] private GameObject _yellowLightningPiece;
        [field: SerializeField] private GameObject _purpleLightningPiece;
        [field: SerializeField] private GameObject _greenLightningPiece;
        [Header("Seed")]
        [field: SerializeField] private string _spawnerSeed;
        public GameObject[] _pieces { get; private set; }
        public GameObject[,] _specialPieces { get; private set; }
        public bool _takeInput { get; set; }
        public Piece[] _lastSwappedPieces { get; set; } = new Piece[2];
        public List<Pattern> _patterns { get; set; } = new List<Pattern>();

        #region Start & Update

        private void Start()
        {
            _pieces = new GameObject[] { _redPiece, _bluePiece, _yellowPiece, _purplePiece, _greenPiece };
            _specialPieces = new GameObject[,] { 
                { _redBombPiece, _blueBombPiece, _yellowBombPiece, _purpleBombPiece, _greenBombPiece }, 
                { _redLightningPiece, _blueLightningPiece, _yellowLightningPiece, _purpleLightningPiece, _greenLightningPiece } 
            };
            SetState(new BeginState(this, _spawnerSeed));
        }
        private void Update()
        {
            if (!_takeInput) { return; }
            if (Input.GetMouseButtonDown(0)) { MouseRay(); }
        }

        #endregion

        #region Public methods

        public void AddScore(int score)
        {
            _scoreNum += score;
            _scoreNumUI.text = _scoreNum.ToString();
        }
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
            Piece selection = GetRayPiece(ray.origin, ray.direction);

            if (selection == null) { return; }
            PassSelection(selection.gameObject);
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
                ISpecialPiece special = piece.GetComponent<ISpecialPiece>();
                if (special != null)
                {
                    List<Piece> pieces = special.SpecialMovePieces();
                    foreach(Piece p in pieces)
                    {
                        _patterns.Add(new Pattern(p));
                    }
                    _patterns.Add(new Pattern(piece));

                    _state.SwitchState(1);
                }

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
            seq.OnComplete(() => { _state.SwitchState(0); });
            _selectedPiece = null;
        }

        #endregion
    }
}