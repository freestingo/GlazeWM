using System;

namespace GlazeWM.Domain.Common.Enums
{
  /// <summary>
  /// Used to represent a given order (eg. previous/next workspace).
  /// </summary>
  public enum Sequence
  {
    Previous,
    Next
  }

  public static class SequenceExtensions
  {
    /// <summary>
    /// Get the inverse of a given sequence.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static Sequence Inverse(this Sequence sequence)
    {
      return sequence switch
      {
        Sequence.Next => Sequence.Previous,
        Sequence.Previous => Sequence.Next,
        _ => throw new ArgumentOutOfRangeException(nameof(sequence)),
      };
    }
  }
}
