namespace GlazeWM.Domain.UserConfigs
{
  public class MonocleIndicatorComponentConfig : BarComponentConfig
  {
    /// <summary>
    /// Text to display when the workspace is in monocle mode.
    /// </summary>
    public string LabelEntered { get; set; } = "Monocle";

    /// <summary>
    /// Text to display when the workspace is not in monocle mode.
    /// </summary>
    public string LabelExited { get; set; } = "Tiled";
  }
}
