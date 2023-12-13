using System.Linq;
using GlazeWM.Domain.Common.Enums;
using GlazeWM.Domain.Containers.Commands;
using GlazeWM.Domain.Windows;
using GlazeWM.Domain.Workspaces;
using GlazeWM.Infrastructure.Bussing;

namespace GlazeWM.Domain.Containers.CommandHandlers
{
  internal sealed class FocusInSequenceHandler : ICommandHandler<FocusInSequenceCommand>
  {
    private readonly Bus _bus;
    private readonly ContainerService _containerService;

    public FocusInSequenceHandler(
      Bus bus,
      ContainerService containerService)
    {
      _bus = bus;
      _containerService = containerService;
    }

    public CommandResponse Handle(FocusInSequenceCommand command)
    {
      var sequence = command.Sequence;
      var focusedContainer = _containerService.FocusedContainer;

      var focusTarget = GetFocusTarget(focusedContainer, sequence);

      if (focusTarget is null || focusTarget == focusedContainer)
        return CommandResponse.Ok;

      _bus.Invoke(new SetFocusedDescendantCommand(focusTarget));
      _containerService.HasPendingFocusSync = true;

      return CommandResponse.Ok;
    }

    private Container GetFocusTarget(Container focusedContainer, Sequence sequence)
    {
      if (focusedContainer is FloatingWindow)
        return GetFocusTargetFromFloating(focusedContainer, sequence);

      return GetFocusTargetFromTiling(focusedContainer, sequence);
    }

    private static Container GetFocusTargetFromFloating(Container focusedContainer, Sequence sequence)
    {
      var focusTarget = sequence is Sequence.Next
        ? focusedContainer.NextSiblingOfType<FloatingWindow>()
        : focusedContainer.PreviousSiblingOfType<FloatingWindow>();

      if (focusTarget is not null)
        return focusTarget;

      // Wrap if next/previous floating window is not found.
      return sequence is Sequence.Next
        ? focusedContainer.SelfAndSiblingsOfType<FloatingWindow>().FirstOrDefault()
        : focusedContainer.SelfAndSiblingsOfType<FloatingWindow>().LastOrDefault();
    }

    private Container GetFocusTargetFromTiling(Container focusedContainer, Sequence sequence)
    {
      var focusReference = focusedContainer;

      // Traverse upwards from the focused container. Stop searching when a workspace is
      // encountered.
      while (focusReference is not Workspace)
      {
        var parent = focusReference.Parent as SplitContainer;

        if (!focusReference.HasSiblings())
        {
          focusReference = parent;
          continue;
        }

        var focusTarget = sequence is Sequence.Next
          ? focusReference.NextSiblingOfType<IResizable>()
          : focusReference.PreviousSiblingOfType<IResizable>();

        // Wrap if next/previous floating window is not found and there are no other columns.
        if (focusTarget == null && parent is Workspace)
        {
          focusTarget = sequence is Sequence.Next
            ? focusReference.SelfAndSiblingsOfType<IResizable>().FirstOrDefault()
            : focusReference.SelfAndSiblingsOfType<IResizable>().LastOrDefault();
        }

        if (focusTarget == null)
        {
          focusReference = parent;
          continue;
        }

        return _containerService.GetDescendantInSequence(focusTarget, sequence.Inverse());
      }

      return null;
    }
  }
}
