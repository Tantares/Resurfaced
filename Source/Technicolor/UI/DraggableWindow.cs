using UnityEngine;
using UnityEngine.EventSystems;

namespace Technicolor
{
  public class DraggableWindow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
  {
    public Transform target;
    public bool shouldReturn;

    private bool _isMouseDown = false;
    private Vector3 _startMousePosition;
    private Vector3 _startPosition;
    public void OnPointerDown(PointerEventData dt)
    {
      _isMouseDown = true;
      _startPosition = target.position;
      _startMousePosition = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData dt)
    {
      _isMouseDown = false;

      if (shouldReturn)
      {
        target.position = _startPosition;
      }
    }

    // Update is called once per frame
    void Update()
    {
      if (_isMouseDown)
      {
        Vector3 currentPosition = Input.mousePosition;

        Vector3 diff = currentPosition - _startMousePosition;

        Vector3 pos = _startPosition + diff;

        target.position = pos;
      }
    }
  }
}