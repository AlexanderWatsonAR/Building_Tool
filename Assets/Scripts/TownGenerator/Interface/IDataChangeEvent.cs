using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataChangeEvent
{
    event Action<IData> OnDataChange;
    void OnDataChange_Invoke();
}
