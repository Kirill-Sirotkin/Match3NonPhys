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
        [field: SerializeField] public Player _player { get; private set; }
        [field: SerializeField] public Transform _piecesParent { get; private set; }
        [field: SerializeField] public Spawner _spawner { get; private set; }

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

        private void Start()
        {
            //GameStartSpawn();
            _pieces = new GameObject[] { _redPiece, _bluePiece, _yellowPiece, _purplePiece, _greenPiece };
            SetState(new BeginState(this));
        }

        private void Update()
        {
            if (!_takeInput) { return; }
            if (Input.GetMouseButtonDown(0)) { Debug.Log("LMB pressed"); MouseRay(); }
        }

        private void LateUpdate()
        {
            //if (CheckPiecesIdle())
            //{
            //    _player._takeInputs = true;
            //}
            //else
            //{
            //    _player._takeInputs = false;
            //}
        }

        public Piece GetRayPiece(Vector3 origin, Vector3 direction, bool mouseSelection = false)
        {
            RaycastHit hit;
            if (!Physics.Raycast(origin, direction, out hit, Mathf.Infinity)) { return null; }

            if (mouseSelection) { PassSelection(hit); }
            return hit.transform.GetComponent<Piece>();
        }
        private void MouseRay()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            GetRayPiece(ray.origin, ray.direction);
        }
        private void PassSelection(RaycastHit hit)
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
        public int CheckForPatterns()
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
                    for (int j = 1; j < 11; j++)
                    {
                        Piece rayPiece = GetRayPiece(origin + directions[i]*j);
                        if (rayPiece == null) { break; }
                        if (rayPiece._type != p._type) { break; }
                        temporaryPieces.Add(rayPiece);
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
            }

            //Debug.Log(piecesToDespawn.Count);
            DespawnMatchingPieces(piecesToDespawn);
            if (piecesToDespawn.Count > 0) { return piecesToDespawn.Count; }

            if (_swappedPieces[0] != null && _swappedPieces[1] != null)
            {
                _selectedPiece = _swappedPieces[0];
                SwapPieces(_swappedPieces[1], false);
                return piecesToDespawn.Count;
            }

            int possibleMoves = 0;
            foreach (Piece p in _piecesParent.GetComponentsInChildren<Piece>())
            {
                possibleMoves += CheckPossibleMoves(p.transform.position);
            }
            if (possibleMoves == 0) { ShufflePieces(); }

            return piecesToDespawn.Count;
        }
        private Piece GetRayPiece(Vector3 pos)
        {
            Vector3 direction = Vector3.forward;
            RaycastHit hit;

            if (!Physics.Raycast(pos, direction, out hit, Mathf.Infinity)) { return null; }
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
                }
                _spawner.MovePiecesDown();
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
        private void GameStartSpawn()
        {
            Vector3 startVector = new Vector3(-3f, 1f, 0f);
            Sequence seq = DOTween.Sequence();

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    GameObject obj = _spawner.SpawnPiece(startVector + new Vector3(i, j), _piecesParent);
                    seq.Join(obj.GetComponent<Piece>().Move(startVector + new Vector3(i, j - 5)));
                }
            }

            seq.OnComplete(() => { CheckForPatterns(); });
        }
        public void CleanUp()
        {
            foreach (Transform child in _piecesParent)
            {
                if (child.gameObject.activeSelf) { continue; }
                Destroy(child.gameObject);
            }
        }
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
            seq.OnComplete(() => { CheckForPatterns(); });

            return seq;
        }
    }
}