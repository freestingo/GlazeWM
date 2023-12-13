using GlazeWM.Domain.Common.Enums;
using GlazeWM.Infrastructure.Bussing;

namespace GlazeWM.Domain.Workspaces.Commands
{
  public class FocusMonitorCommand : Command
  {
    public Sequence Sequence { get; }

    public FocusMonitorCommand(Sequence sequence)
    {
      Sequence = sequence;
    }
  }
}
