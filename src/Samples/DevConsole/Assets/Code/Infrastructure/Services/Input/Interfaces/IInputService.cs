// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using UnityEngine;

namespace Code.Infrastructure.Services.Input.Interfaces
{
  public interface IInputService
  {
    bool IsConsoleButtonPressed();
    bool IsConsoleSubmitPressed();
    float GetConsoleHistoryAxis();
  }
}
