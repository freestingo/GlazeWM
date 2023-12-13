using GlazeWM.Domain.Common.Enums;
using GlazeWM.Domain.Containers.Commands;
using GlazeWM.Domain.Monitors;
using GlazeWM.Domain.Windows.Commands;
using GlazeWM.Infrastructure.Bussing;

namespace GlazeWM.Domain.Windows.CommandHandlers
{
  internal sealed class MoveWindowInSequenceHandler : ICommandHandler<MoveWindowInSequenceCommand>
  {
    private readonly Bus _bus;

    public MoveWindowInSequenceHandler(
      Bus bus)
    {
      _bus = bus;
    }

    public CommandResponse Handle(MoveWindowInSequenceCommand command)
    {
      var windowToMove = command.WindowToMove;
      var sequence = command.Sequence;

      if (windowToMove is FloatingWindow)
      {
        // TODO: add floating window support
        //MoveFloatingWindow(windowToMove as FloatingWindow, direction);
        return CommandResponse.Ok;
      }

      if (windowToMove is TilingWindow)
      {
        MoveToWorkspaceInSequence(windowToMove as TilingWindow, sequence);
        return CommandResponse.Ok;
      }

      return CommandResponse.Fail;
    }

    private void MoveToWorkspaceInSequence(Window windowToMove, Sequence sequence)
    {
      var monitor = MonitorService.GetMonitorFromChildContainer(windowToMove);
      var monitorInSequence = MonitorService.GetMonitorInSequence(sequence, monitor);
      var workspaceInSequence = monitorInSequence?.DisplayedWorkspace;

      if (workspaceInSequence == null)
        return;

      // Since window is crossing monitors, adjustments might need to be made because of DPI.
      windowToMove.HasPendingDpiAdjustment = true;

      // Update floating placement since the window has to cross monitors.
      windowToMove.FloatingPlacement =
        windowToMove.FloatingPlacement.TranslateToCenter(workspaceInSequence.ToRect());

      _bus.Invoke(new MoveContainerWithinTreeCommand(windowToMove, workspaceInSequence, 0));
    }
  }
}
