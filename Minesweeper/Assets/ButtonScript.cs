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
    private bool _marked = false;
    private bool _revealed = false;
    void Awake()
    {
        _button = GetComponent<Button>();
    }
    void Start()
    {
        _button.onClick.AddListener(() => BoardManager.RevealField(i,j));
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reveal(int i)
    {
        if(_marked || _revealed)
            return;
        GetComponent<Image>().sprite = _sprites[i];
        _revealed = true;

    }

  

    public void OnPointerClick(PointerEventData eventData)
    {

        if(eventData.button == PointerEventData.InputButton.Right && !_marked && !_revealed){
            _marked = true;
            GetComponent<Image>().sprite = _sprites[10]; //flaga
        }
        else if(eventData.button == PointerEventData.InputButton.Right && _marked && !_revealed){
            _marked = false;
            GetComponent<Image>().sprite = _sprites[11]; //nieznanzaczone pole
        }
    }
}
