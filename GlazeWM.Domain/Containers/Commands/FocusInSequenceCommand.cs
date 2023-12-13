using GlazeWM.Domain.Common.Enums;
using GlazeWM.Infrastructure.Bussing;

namespace GlazeWM.Domain.Containers.Commands
{
  public class FocusInSequenceCommand : Command
  {
    public Sequence Sequence { get; }

    public FocusInSequenceCommand(Sequence sequence)
    {
      Sequence = sequence;
    }
  }
}
