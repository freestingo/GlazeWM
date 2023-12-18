using GlazeWM.Domain.Containers;
using GlazeWM.Domain.Workspaces.Events;
using GlazeWM.Domain.Workspaces.Commands;
using GlazeWM.Infrastructure.Bussing;

namespace GlazeWM.Domain.Workspaces.CommandHandlers
{
  internal sealed class ToggleMonocleHandler : ICommandHandler<ToggleMonocleCommand>
  {
    private readonly Bus _bus;
    private readonly ContainerService _containerService;
    private readonly WorkspaceService _workspaceService;

    public ToggleMonocleHandler(
      Bus bus,
      ContainerService ContainerService,
      WorkspaceService workspaceService)
    {
      _bus = bus;
      _containerService = ContainerService;
      _workspaceService = workspaceService;
    }

    public CommandResponse Handle(ToggleMonocleCommand command)
    {
      var currentWorkspace = _workspaceService.GetFocusedWorkspace();

      currentWorkspace.IsMonocle = !currentWorkspace.IsMonocle;

      if (currentWorkspace.IsMonocle)
        _bus.Emit(new EnterMonocleEvent(currentWorkspace));
      else
        _bus.Emit(new ExitMonocleEvent(currentWorkspace));

      _containerService.ContainersToRedraw.Add(currentWorkspace);

      return CommandResponse.Fail;
    }
  }
}
