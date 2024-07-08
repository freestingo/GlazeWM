using GlazeWM.Domain.Common;
using GlazeWM.Infrastructure.Bussing;

namespace GlazeWM.Domain.Workspaces.Events
{
  public record EnterMonocleEvent(Workspace ActivatedWorkspace)
    : Event(DomainEvent.WorkspaceMonocleEntered);
}
