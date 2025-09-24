using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
public enum CardInputType
{
    MouseEnter,
    MouseExit,
    MouseDown,
    MouseUp,
    MouseDrag
}
public delegate void CardInputAction(CardComEventArgs args);
public class CardComEventArgs
{
    public CardInputType inputType;
    public CardCom target;
}
public class CardCom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text mana;
    [SerializeField] private TMP_Text description;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private GameObject wrapper;
    [SerializeField] private LayerMask dropAreaLayer;
    public int cardId = 0;
    public event CardInputAction inputAction = (args) => { };
    public CardModel cardData => GM.LevelData.GetCardModel(cardId);
    public void Init(int cardId)
    {
        this.cardId = cardId;
        CardModel data = cardData;
        if (data == null)
            return;
        title.text = data.Cfg.title;
        mana.text = data.Cfg.manaCost.ToString();
        description.text = data.RichTextDesc ?? data.Desc;
        imageSR.sprite = data.Cfg.image;
    }
    public void ShowWrapper()
    {
        wrapper.SetActive(true);
    }
    public void HideWrapper()
    {
        wrapper.SetActive(false);
    }
    public void UpdateDesc()
    {
        CardModel data = cardData;
        description.text = data.RichTextDesc ?? data.Desc;
    }
    private CardComEventArgs GetInputArgs(CardInputType type)
    {
        return new CardComEventArgs
        {
            inputType = type,
            target = this
        };
    }
    public void OnDrag(PointerEventData eventData)
    {
        inputAction.Invoke(GetInputArgs(CardInputType.MouseDrag));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        inputAction.Invoke(GetInputArgs(CardInputType.MouseDown));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inputAction.Invoke(GetInputArgs(CardInputType.MouseEnter));

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inputAction.Invoke(GetInputArgs(CardInputType.MouseExit));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputAction.Invoke(GetInputArgs(CardInputType.MouseUp));
    }
}