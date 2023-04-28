using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIItem<T>
{
    T Source { get; set; }
}
