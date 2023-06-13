using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ButtonScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Sprite[] _sprites;
    private Button _button;
    public int i {get;set;}
    public int j {get;set;}
    public bool isMarked {get;set;}
    public bool isRevealed {get;set;}
    void Awake()
    {
        _button = GetComponent<Button>();
    }
    void Start()
    {
        _button.onClick.AddListener(() => BoardManager.RevealField(i,j));
        isRevealed = false;
        isMarked = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reveal(int i)
    {
        //if(isMarked || BoardManager.PlayerLost || BoardManager.PlayerWon)
         //   return;
        GetComponent<Image>().sprite = _sprites[i];
        isRevealed = true;

    }

  

    public void OnPointerClick(PointerEventData eventData)
    {

        if(eventData.button == PointerEventData.InputButton.Right && !isMarked && !isRevealed && !BoardManager.PlayerLost && !BoardManager.PlayerWon){
            isMarked = true;
            GetComponent<Image>().sprite = _sprites[10]; //flaga
            BoardManager.UpdateBombCounterTextAfterFlag(true);
        }
        else if(eventData.button == PointerEventData.InputButton.Right && isMarked && !isRevealed && !BoardManager.PlayerLost && !BoardManager.PlayerWon){
            isMarked = false;
            GetComponent<Image>().sprite = _sprites[11]; //nieznanzaczone pole
            BoardManager.UpdateBombCounterTextAfterFlag(false);
        }
    }
}
