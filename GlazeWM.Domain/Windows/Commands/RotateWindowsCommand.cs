using GlazeWM.Domain.Common.Enums;
using GlazeWM.Infrastructure.Bussing;

namespace GlazeWM.Domain.Windows.Commands
{
  public class RotateWindowsCommand : Command
  {
    public Window CurrentWindow { get; }
    public Sequence Sequence { get; }

    public RotateWindowsCommand(Window currentWindow, Sequence sequence)
    {
      CurrentWindow = currentWindow;
      Sequence = sequence;
    }
  }
}
