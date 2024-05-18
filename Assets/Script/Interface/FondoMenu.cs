using System;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Interface
{
  public class FondoMenu : MonoBehaviour
  {
    [SerializeField] private RawImage fondo;
    [SerializeField] private float horizontal;
    [SerializeField] private float vertical;

    private void Update()
    {
      fondo.uvRect = new Rect(fondo.uvRect.position + new Vector2(horizontal, vertical), fondo.uvRect.size);
    }
  }
}
