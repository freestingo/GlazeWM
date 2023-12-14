using System;
using System.Linq;
using GlazeWM.Domain.Common.Enums;
using GlazeWM.Domain.Containers;
using GlazeWM.Domain.Windows.Commands;
using GlazeWM.Infrastructure.Bussing;
using GlazeWM.Infrastructure.Utils;

namespace GlazeWM.Domain.Windows.CommandHandlers
{
  internal sealed class RotateWindowsHandler : ICommandHandler<RotateWindowsCommand>
  {
    private readonly ContainerService _containerService;

    public RotateWindowsHandler(
      ContainerService containerService)
    {
      _containerService = containerService;
    }

    public CommandResponse Handle(RotateWindowsCommand command)
    {
      var windowToMove = command.CurrentWindow;
      var sequence = command.Sequence;
      var workspaceWindows = windowToMove.SelfAndSiblings
        .OfType<TilingWindow>()
        .Select(workspaceWindow => new
        {
          WorkspaceWindow = workspaceWindow,
          OriginalSiblingIndex = GetOriginalSibling(workspaceWindow, sequence).Index,
        })
        .ToList();

      workspaceWindows.ToList().ForEach(x => x.WorkspaceWindow
        .Parent.Children.ShiftToIndex(x.OriginalSiblingIndex, x.WorkspaceWindow));

      _containerService.ContainersToRedraw.AddRange(workspaceWindows.Select(x => x.WorkspaceWindow));

      return CommandResponse.Ok;
    }

    private static Container GetOriginalSibling(TilingWindow window, Sequence sequence)
    {
      return sequence switch
      {
        Sequence.Next => window.NextSiblingOfType<TilingWindow>()
            ?? window.SelfAndSiblingsOfType<TilingWindow>().FirstOrDefault(),
        Sequence.Previous => window.PreviousSiblingOfType<TilingWindow>()
            ?? window.SelfAndSiblingsOfType<TilingWindow>().LastOrDefault(),
        _ => throw new ArgumentException(null, nameof(sequence)),
      };
    }
  }
}
