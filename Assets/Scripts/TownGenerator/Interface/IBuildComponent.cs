
using System;

public interface IBuildable
{
    IBuildable Initialize(IData data);
    void Build();

    //event Action<IData> OnDataChange;
}
