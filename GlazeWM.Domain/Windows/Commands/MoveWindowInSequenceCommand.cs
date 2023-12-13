using GlazeWM.Domain.Common.Enums;
using GlazeWM.Infrastructure.Bussing;

namespace GlazeWM.Domain.Windows.Commands
{
  public class MoveWindowInSequenceCommand : Command
  {
    public Window WindowToMove { get; }
    public Sequence Sequence { get; }

    public MoveWindowInSequenceCommand(Window windowToMove, Sequence sequence)
    {
      WindowToMove = windowToMove;
      Sequence = sequence;
    }
  }
}
