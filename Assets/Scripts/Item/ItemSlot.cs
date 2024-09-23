using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image image;

    public event Action<ItemSlot> OnRightClickEvent;
    public event Action<ItemSlot> OnBeingDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<ItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    private Color normalColor = Color.white;
    private Color disabledColor = new Color(1, 1, 1, 0);

    private Item _item;
    public Item Item
    {
        get { return _item; }
        set 
        {
            _item = value;

            if (_item == null)
            {
                image.color = disabledColor;
            }
            else
            {
                image.sprite = _item.Icon;
                image.color = normalColor;
            }
        }
    }

    
    protected virtual void OnValidate()
    {
        if (image == null)
            image = GetComponent<Image>();
    }

    public virtual bool CanReceiveItem(Item item)
    {
        return true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData != null && eventData.button == PointerEventData.InputButton.Right)
        {
            if (Item != null && OnRightClickEvent != null)
                OnRightClickEvent(this);
        }
    }
    Vector2 originalPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = image.transform.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.transform.position = originalPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        image.transform.position = Input.mousePosition;
    }

    public void OnDrop(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }
}
