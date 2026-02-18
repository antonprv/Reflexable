// Created by Anton Piruev in 2026. 
// Any direct commercial use of derivative work is strictly prohibited.

using Code.Gameplay;
using Code.Infrastructure.Services.DevConsole.Commands;
using Code.Infrastructure.Services.DevConsole.Commands.Logs;
using Code.Infrastructure.Services.DevConsole.Commands.Performance;
using Code.Infrastructure.Services.StaticDataService.Interfaces.Subservice;
using Code.Zenjex.Extensions.Attribute;
using UnityEngine;

// Here we implicitly inject everything when GameInstaller runs, so
// by the time this object's Awake() shoots,
// _buildConfig and _console are already injected.
namespace Code.Infrastructure.Services.DevConsole
{
  public class ConsoleComponent : MonoBehaviour
  {
    public FramerateManager framerateManager;

    [Zenjex] private IBuildConfigSubservice _buildConfig;
    [Zenjex] private IDevConsole _console;

    private void Awake()
    {
      _console?.Initialize();
      InitializeConsoleCommands();
      Debug.Log($"{nameof(ConsoleComponent)} initialized!");
    }

    private void InitializeConsoleCommands()
    {
      if (!_buildConfig.IsDevelopment()) return;

      if (_console == null)
        return;

      // LOGS
      _console.RegisterCommand(new ToggleUnityLogsCommand(_console));
      _console.RegisterCommand(new ExportLogsCommand(_console));
      _console.RegisterCommand(new FilterLogsCommand(_console));
      _console.RegisterCommand(new LogStatsCommand(_console));

      // PERFORMANCE
      _console.RegisterCommand(new SetFPSCommand(_console));
      if (framerateManager != null)
        _console.RegisterCommand(new ToggleFPSCounterCommand(_console, framerateManager));

      // CONTROLFLOW
      _console.RegisterCommand(new ClearCommand(_console));
    }
  }
}
