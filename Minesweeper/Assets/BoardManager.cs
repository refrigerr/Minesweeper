using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BoardManager : MonoBehaviour
{
    private static int[,] _board = new int[13, 28];
    private static ButtonScript [,] _buttons = new ButtonScript[13,28];
    public static int Difficulty {get;set;}
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _buttonPrefab;
    
    private int _numberOfMines;
    private int _numberOfFlaggedCells = 0;
    // Start is called before the first frame update
    void Start()
    {
        Difficulty = 2;
        GenerateBoard();
        GenerateButtons();
   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void GenerateBoard()
    {
        _numberOfMines = Difficulty * 1;
        int minesAdded = 0;
        while(minesAdded != _numberOfMines){
            for(int i=0; i<13;i++){
                for(int j=0; j<28;j++){
                    if(Random.Range(0f,1f) < ((float)_numberOfMines/(13 * 28)) && _board[i,j] != 1){
                        _board[i,j] = 1;
                        minesAdded ++;
                    }
                    if(minesAdded == _numberOfMines)
                        return;
                }
            }
        }
    }

    public void StartNewGame(){
        int x = _panel.transform.childCount;
        for(int i = 0; i < x ; i++) {
            _panel.transform.GetChild(0);
            
        }
        for(int i=0; i<13;i++){
            for(int j=0; j<28;j++){
                _board[i,j] = 0;
            }
        }
        GenerateBoard();
        GenerateButtons();
        _numberOfFlaggedCells = 0;
    }

    private void GenerateButtons()
    {
        for(int i=0; i<13;i++){
            for(int j=0; j<28;j++)
            {
                GameObject button = Instantiate(_buttonPrefab);
                button.transform.parent = _panel.transform;
                button.GetComponent<RectTransform>().localPosition = new Vector3(-864 + j*64,394 - i*64,0);
                _buttons[i,j] = button.GetComponent<ButtonScript>();
                _buttons[i,j].i =i;
                _buttons[i,j].j =j;
            }
        }
    }



    public static void RevealField(int i, int j)
{
    Queue<Vector2Int> cellsToReveal = new Queue<Vector2Int>();
    cellsToReveal.Enqueue(new Vector2Int(i, j));

    while (cellsToReveal.Count > 0)
    {
        Vector2Int cell = cellsToReveal.Dequeue();
        i = cell.x;
        j = cell.y;

        if (_buttons[i, j].isRevealed) continue;

        if (_board[i, j] == 1)
        {
            _buttons[i, j].Reveal(9);
            return;
        }

        int surroundingBombs = CountSurroundingBombs(i, j);
        _buttons[i, j].Reveal(surroundingBombs);

        if (surroundingBombs == 0)
        {
            int startI = Mathf.Max(i - 1, 0);
            int startJ = Mathf.Max(j - 1, 0);
            int finishI = Mathf.Min(i + 1, 12);
            int finishJ = Mathf.Min(j + 1, 27);

            for (int ii = startI; ii <= finishI; ii++)
            {
                for (int jj = startJ; jj <= finishJ; jj++)
                {
                    if (ii == i && jj == j) continue;
                    if (!_buttons[ii, jj].isRevealed)
                    {
                        cellsToReveal.Enqueue(new Vector2Int(ii, jj));
                    }
                }
            }
        }
    }
}

    private static int CountSurroundingBombs(int i, int j)
    {
        int startI, startJ, finishI, finishJ;
        int bombCount =0;
        if(i == 0){
            startI =0;
            finishI = i+1;
        }else if(i==12){
            startI = i-1;
            finishI = i;
        }
        else{
            startI = i-1;
            finishI = i+1;
        }

        if(j == 0){
            startJ =0;
            finishJ = j+1;
        }else if(j==27){
            startJ = j-1;
            finishJ = j;
        }
        else{
            startJ = j-1;
            finishJ = j+1;
        }

        for (int ii = startI; ii<= finishI ;ii++){
            for (int jj = startJ; jj<= finishJ ;jj++){
                if(ii == i && jj ==j)
                    continue;
                if(_board[ii,jj] == 1)
                    bombCount++;
            }
        }
        return bombCount;
    }
}
