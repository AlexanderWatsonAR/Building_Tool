using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Note on data change event.
// When a building is first constructed it passes a reference to the storey data to the storey class.
// As such, when editing the storey data in the building inspector or the storey inspector the data displayed remains uniform bewtween both classes.
// However on serialization, storey & building have their own unique copy of the serialized storey data.
// As a result of the limitation I want to use data change events to keep data syncronised.

public interface IDataChangeEvent
{
    event Action<IData> OnDataChange;
    void OnDataChange_Invoke();
}
