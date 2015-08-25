// Guids.cs
// MUST match guids.h
using System;

namespace AdamDriscoll.Perspectives
{
    static class GuidList
    {
        public const string guidPerspectivesPkgString = "7d37ba47-aaa0-4b8a-a17d-e7d54172bd0e";
        public const string guidPerspectivesCmdSetString = "7b761763-1470-4789-9162-86aef9666354";
        public const string guidToolWindowPersistanceString = "af0132a0-b958-4962-b4d6-6d90d3362318";

        public static readonly Guid guidPerspectivesCmdSet = new Guid(guidPerspectivesCmdSetString);
    };
}