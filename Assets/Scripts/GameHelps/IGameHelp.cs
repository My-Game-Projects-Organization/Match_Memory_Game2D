using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameHelp 
{
    void ExecuteHelp();

    bool CanUseHelp();

    void ShowHelpInfo();
}
