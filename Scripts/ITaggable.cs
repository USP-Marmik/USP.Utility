using System;

namespace USP.Utility
{
      public interface ITaggable
      {
            public string Tag { get; }
            public sealed bool Compare(string tag) => Tag.Equals(tag, StringComparison.Ordinal);
      }
}