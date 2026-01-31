using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace Assets
{
    public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
		private RectTransform _rectTransform;
        private RectTransform _parentRectTransform;
        private Transform _originalParent;

        private Vector2 _pointerOffset;

        private CanvasGroup _canvasGroup;
        

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            _canvasGroup = GetComponent<CanvasGroup>();
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0.7f;

            _originalParent = transform.parent;
            _parentRectTransform = transform.parent as RectTransform;

            _rectTransform.SetAsLastSibling();

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localCursorPoint))
            {
                _pointerOffset = _rectTransform.anchoredPosition - localCursorPoint;
            }
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (_parentRectTransform == null)
            {
                return;
            }

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localCursorPoint))
            {
                _rectTransform.anchoredPosition = localCursorPoint + _pointerOffset;
            }
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 1f;

            var raycastResult = eventData.pointerCurrentRaycast;
            if (raycastResult.gameObject != null)
            {
                if (raycastResult.gameObject.TryGetComponent<Slot>(out var slot))
                {
                    Debug.Log("Dropped on a Slot");
                    transform.SetParent(slot.transform, false);
                    _rectTransform.anchoredPosition = Vector2.zero;
                }
            }
            else
            {
                transform.SetParent(_originalParent, false);
                _rectTransform.anchoredPosition = Vector2.zero;
            }
        }
    }
}