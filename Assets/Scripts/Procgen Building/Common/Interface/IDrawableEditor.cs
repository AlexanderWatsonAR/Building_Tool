using OnlyInvalid.ProcGenBuilding.Building;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public interface IDrawableEditor
{
    void DisplayMessages(DrawState state)
    {
        switch (state)
        {
            case DrawState.Draw:
                {
                    DisplayDrawMessages();
                }
                break;
            case DrawState.Edit:
                {
                    DisplayEditMessages();
                }
                break;
            case DrawState.Hide:
                {
                    DisplayHideMessages();
                }
                break;
        }
    }

    void DisplayDrawMessages();
    void DisplayEditMessages();
    void DisplayHideMessages();
}
