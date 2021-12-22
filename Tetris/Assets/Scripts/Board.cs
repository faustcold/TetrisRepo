using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public Piece nextPiece { get; private set; }
    public Piece holdPiece { get; private set; }
    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    Vector3Int previewPosition = new Vector3Int(12, 4, 0);
    Vector3Int holdPosition = new Vector3Int(12, -3, 0);
    [SerializeField] private UIManager uIManager;
    private bool HoldWindowNull = true;
    private int ComboCount = 0;
    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }
    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();
        nextPiece = gameObject.AddComponent<Piece>();
        holdPiece = gameObject.AddComponent<Piece>();
        holdPiece.enabled = false;
        nextPiece.enabled = false;

        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SetNextPiece();
        SpawnPiece();
    }

    private void SetHoldPiece()
    {
        if (HoldWindowNull)
        {
            if (holdPiece.cells != null)
            {
                Clear(holdPiece);
            }

            TetrominoData data = activePiece.data;
            holdPiece.Initialize(this, holdPosition, data);

            for (int i = 0; i < activePiece.cells.Length; i++)
            {
                Vector3Int tilePosition = activePiece.cells[i] + activePiece.position;
                tilemap.SetTile(tilePosition, null);
            }

            SpawnPiece();
            Set(holdPiece);
            HoldWindowNull = false;
        }
        else if (!HoldWindowNull)
        {
            SpawnHoldPiece();
        }
    }

    private void SpawnHoldPiece()
    {
        TetrominoData holdData = activePiece.data;
        TetrominoData activeData = holdPiece.data;
        for (int i = 0; i < activePiece.cells.Length; i++)
        {
            Vector3Int tilePosition = activePiece.cells[i] + activePiece.position;
            tilemap.SetTile(tilePosition, null);
        }
        for (int i = 0; i < holdPiece.cells.Length; i++)
        {
            Vector3Int tilePosition = holdPiece.cells[i] + holdPiece.position;
            tilemap.SetTile(tilePosition, null);
        }

        holdPiece.Initialize(this, spawnPosition, holdData);
        activePiece.Initialize(this, holdPosition,activeData);

        Set(holdPiece);
        Set(activePiece);
    }

    private void SetNextPiece()
    {
        if (nextPiece.cells != null)
        {
            Clear(nextPiece);
        }
        int random = Random.Range(0, tetrominoes.Length);
        TetrominoData data = tetrominoes[random];

        nextPiece.Initialize(this, previewPosition, data);
        Set(nextPiece);
    }

    public void SpawnPiece()
    {
        activePiece.Initialize(this, spawnPosition, nextPiece.data);

        if (!IsValidPosition(activePiece, spawnPosition))
        {
            GameOver();
        }
        else
        {
            Set(activePiece);
        }
        SetNextPiece();
    }

    public void GameOver()
    {
        uIManager.GameOverUI(true);
        Time.timeScale = 0;
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }
            if (tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }
        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;
        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                ComboCount++;
            }
            else
            {
                uIManager.ScoreSystem(ComboCount);
                ComboCount = 0;
                row++;
            }
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }
        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }
            row++;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            tilemap.ClearAllTiles();
            uIManager.GameOverUI(false);
            Time.timeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SetHoldPiece();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            SpawnHoldPiece();
        }
    }
}
