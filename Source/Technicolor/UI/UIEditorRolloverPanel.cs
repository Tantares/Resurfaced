using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Technicolor
{
  public class UIEditorRolloverPanel: MonoBehaviour
  {
    [SerializeField]
    protected Text _title;

    [SerializeField]
    protected Text _text;

    public void AssignReferences()
    {
      _title = UIUtils.FindChildOfType<Text>("TitleText", transform);
      _text = UIUtils.FindChildOfType<Text>("ZoneText", transform);
    }

    public void SetText(string txt)
    {
      _text.text = txt;
    }
  }
}
