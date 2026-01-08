
using System;
using UnityEngine;

namespace UI.UIScenes
{
    public interface IUISceneBinder
    {
        Vector3 WorldPos { get; }

        event Action PosChangeEvent;
    }
}
