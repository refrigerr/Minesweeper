using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BoardManager : MonoBehaviour
{

    //efekty dzwiekowe
    static SoudEffects soundEffects;
    //pola tekstowe licznika bomb oraz czasu
    [SerializeField] TMP_Text _bombCounterTextToAssign;
    private static TMP_Text _bombCounterText;
    [SerializeField] private TMP_Text _timeCounterText;
    //

    //dropdown do wyboru poziomu trudnosci
    [SerializeField] private TMP_Dropdown _dropDown;

    //obrazki wygranej i przegranej
    [SerializeField] private GameObject _victoryToAssign;
    [SerializeField] private GameObject _defeatToAssign;
    private static GameObject _victory;
    private static GameObject _defeat;
    //

    //plansza
    private static int[,] _board = new int[13, 28];
    private static ButtonScript [,] _buttons = new ButtonScript[13,28];
    //

    //poziom trudnosci
    public static int Difficulty {get;set;}

    [SerializeField] private GameObject _panel;
    //prefab pola do klikniecia
    [SerializeField] private GameObject _buttonPrefab;

    //czy gracz wygral czy przegral (da sie uproscic do jednej zmiennej)
    public static bool PlayerLost {get; private set;}
    public static bool PlayerWon {get; private set;}

    //liczba min oraz oznaczonych flaga pol
    private static int _numberOfMines;
    private static int _numberOfFlaggedCells = 0;
    //

    //licznik czasu
    private float _timePassed = 0;
    private static bool _countTime = false;
    //

    //czy to pierwsza rozgrywka i czy wystapilo pierwsze klikniecie
    private static bool _firstClick = true;
    //

    // Start is called before the first frame update
    void Start()
    {
        soundEffects = GetComponent<SoudEffects>();
        Difficulty = 1;
        _victory = _victoryToAssign;
        _defeat = _defeatToAssign;
        _bombCounterText = _bombCounterTextToAssign;
        GenerateButtons();
        StartNewGame();
    }

    // Update is called once per frame
    void Update()
    {
        if(_countTime)
        {
            _timePassed+=Time.deltaTime;
            _timeCounterText.text = ((int)_timePassed).ToString();
        }
    }
    
    private static void GenerateBoard(int a, int b)
    {
        
        int minesAdded = 0;
        while(minesAdded != _numberOfMines){
            for(int i=0; i<13;i++){
                for(int j=0; j<28;j++)
                {
                    if(Random.Range(0f,1f) < ((float)_numberOfMines/(13 * 28)) 
                    && _board[i,j] != 1
                    && (i != a || j != b))
                    {
                        _board[i,j] = 1;
                        minesAdded ++;
                    }
                    if(minesAdded == _numberOfMines)
                        return;
                }
            }
        }
    }

    public void StartNewGame()
    {
        Difficulty = _dropDown.value + 1;
        _numberOfMines = Difficulty * 30;
        PlayerLost = false;
        PlayerWon = false;
        _countTime = false;
        _firstClick = true;
        _victory.SetActive(false);
        _defeat.SetActive(false);
        _timePassed = 0;
    
        for(int i=0; i<_board.GetLength(0);i++){
            for(int j=0; j<_board.GetLength(1);j++){
                _board[i,j] = 0;
            }
        }

        //resetowanie pol na planszy
        for(int i=0; i<_buttons.GetLength(0);i++){
            for(int j=0; j<_buttons.GetLength(1);j++){
                _buttons[i,j].ResetButton();
            }
        }
        
        
        _bombCounterText.text = _numberOfMines.ToString();
        _timeCounterText.text = 0.ToString();
        _numberOfFlaggedCells = 0;
    }

    private void GenerateButtons()
    {
        for(int i=0; i<13;i++){
            for(int j=0; j<28;j++)
            {
                GameObject button = Instantiate(_buttonPrefab);
                button.transform.SetParent(_panel.transform,false);
                button.GetComponent<RectTransform>().localPosition = new Vector3(-864 + j*64,394 - i*64,0);
                _buttons[i,j] = button.GetComponent<ButtonScript>();
                _buttons[i,j].i =i;
                _buttons[i,j].j =j;
            }
        }
    }

    private static void RevealAllBombs()
    {
        for(int i=0;i<_buttons.GetLength(0);i++)
        {
            for(int j=0;j<_buttons.GetLength(1);j++)
            {
                if(!_buttons[i,j].isRevealed && _board[i,j] == 1)
                {
                    _buttons[i, j].Reveal(12);
                }
            }
        }
    }

    public static void RevealField(int i, int j)
    {
        if(PlayerLost || PlayerWon)
            return;

        if(_firstClick){
            GenerateBoard(i,j);
            _firstClick = false;
        }


        if(!_countTime)
            _countTime = true;

        Queue<Vector2Int> cellsToReveal = new Queue<Vector2Int>();
        cellsToReveal.Enqueue(new Vector2Int(i, j));

        while (cellsToReveal.Count > 0)
        {
            Vector2Int cell = cellsToReveal.Dequeue();
            i = cell.x;
            j = cell.y;

            if (_buttons[i, j].isRevealed || _buttons[i, j].isMarked) continue;

            if (_board[i, j] == 1)
            {
                _buttons[i, j].Reveal(9);
                RevealAllBombs();
                soundEffects.playDefeat();
                PlayerLost = true;
                _countTime = false;
                _defeat.SetActive(true);
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
                        if (!_buttons[ii, jj].isRevealed || !_buttons[ii, jj].isMarked)
                        {
                            cellsToReveal.Enqueue(new Vector2Int(ii, jj));
                        }
                    }
                }
            }
        }
        if(CheckForWinCondition())
        {
            soundEffects.playVictory();
            _victory.SetActive(true);
            _countTime = false;
            PlayerWon = true;
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
                if(ii == i && jj == j)
                    continue;
                if(_board[ii,jj] == 1)
                    bombCount++;
            }
        }
        return bombCount;
    }

    public static void UpdateBombCounterTextAfterFlag(bool incrementFlaggedCells)
    {
        if(incrementFlaggedCells)
            _numberOfFlaggedCells++;
        else
            _numberOfFlaggedCells--;
        _bombCounterText.text = (_numberOfMines - _numberOfFlaggedCells).ToString();
    }

    private static bool CheckForWinCondition()
    {
        
        int allCells = _buttons.GetLength(0)*_buttons.GetLength(1);

        for(int i=0;i<_buttons.GetLength(0);i++)
        {
            for(int j=0;j<_buttons.GetLength(1);j++)
            {
                if(!_buttons[i,j].isRevealed && _board[i,j] == 0)
                {
                    return false;
                }
            
            }
        }
       
        return true;
    }
}
