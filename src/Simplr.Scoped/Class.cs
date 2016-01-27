using System;
using System.Collections.Generic;

namespace Scopes
{
    public static class StaticContainer
    {
        private static int count = 0;
        public static int Count => ++count;
        public static int? LastCount;
    }
    public interface ILazyCount
    {
        Lazy<int> LazyCount { get; }
    }
    public class ScopedClass : ILazyCount
    {
        private Lazy<int> lazyCount = new Lazy<int>(() =>
        {
            var value = StaticContainer.Count;
            StaticContainer.LastCount = value;
            return value;
        });
        public Lazy<int> LazyCount => lazyCount;
    }

    public class ScopedClass2 : ILazyCount
    {
        private Lazy<int> lazyCount = new Lazy<int>(() =>
        {
            var value = StaticContainer.Count;
            StaticContainer.LastCount = value;
            return value + 10;
        });
        public Lazy<int> LazyCount => lazyCount;
    }
}
