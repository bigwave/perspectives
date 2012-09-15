// Guids.cs
// MUST match guids.h
using System;

namespace Company.Test
{
    static class GuidList
    {
        public const string guidTestPkgString = "13ecdaa3-1bec-4e0a-9314-b74d1a09ca7a";
        public const string guidTestCmdSetString = "3283c5e2-38b1-4dce-a46c-6ff1535cf585";

        public static readonly Guid guidTestCmdSet = new Guid(guidTestCmdSetString);
    };
}