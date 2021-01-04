using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOperation
{
    void Apply();

    void Revert();
}
