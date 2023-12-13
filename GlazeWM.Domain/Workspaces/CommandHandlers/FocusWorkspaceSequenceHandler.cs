using System;
using System.Linq;
using GlazeWM.Domain.Common.Enums;
using GlazeWM.Domain.Monitors;
using GlazeWM.Domain.UserConfigs;
using GlazeWM.Domain.Workspaces.Commands;
using GlazeWM.Infrastructure.Bussing;

namespace GlazeWM.Domain.Workspaces.CommandHandlers
{
  internal sealed class FocusWorkspaceSequenceHandler : ICommandHandler<FocusWorkspaceSequenceCommand>
  {
    private readonly Bus _bus;
    private readonly UserConfigService _userConfigService;
    private readonly MonitorService _monitorService;
    private readonly WorkspaceService _workspaceService;

    public FocusWorkspaceSequenceHandler(
      Bus bus,
      UserConfigService userConfigService,
      MonitorService monitorService,
      WorkspaceService workspaceService)
    {
      _bus = bus;
      _userConfigService = userConfigService;
      _monitorService = monitorService;
      _workspaceService = workspaceService;
    }

    public CommandResponse Handle(FocusWorkspaceSequenceCommand command)
    {
      var direction = command.Direction;
      var workspacesConfigs = _userConfigService.WorkspaceConfigs;

      // Get active workspaces for the current monitor in order of their config index.
      var focusedMonitor = _monitorService.GetFocusedMonitor();
      var sortedWorkspaces = _workspaceService
        .GetActiveWorkspacesForMonitor(focusedMonitor)
        .OrderBy((workspace) =>
          workspacesConfigs.FindIndex((config) => config.Name == workspace.Name)
        )
        .ToList();

      // Get config index of the currently focused workspace.
      var focusedWorkspace = _workspaceService.GetFocusedWorkspace();
      var configIndex = sortedWorkspaces.IndexOf(focusedWorkspace);

      // Get index in `sortedWorkspaces` of target workspace to focus. Wrap around to start if
      // there is no previous/next workspace.
      var indexToFocus = direction switch
      {
        Sequence.Previous => configIndex == 0 ? sortedWorkspaces.Count - 1 : configIndex - 1,
        Sequence.Next => configIndex == sortedWorkspaces.Count - 1 ? 0 : configIndex + 1,
        _ => throw new ArgumentException(nameof(direction)),
      };

      var workspaceToFocus = sortedWorkspaces.ElementAtOrDefault(indexToFocus);

      // Set focus to the previous/next workspace if found.
      if (workspaceToFocus is not null && workspaceToFocus != focusedWorkspace)
        _bus.Invoke(new FocusWorkspaceCommand(workspaceToFocus.Name));

      return CommandResponse.Ok;
    }
  }
}
