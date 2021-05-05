using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FastEventsTests
{
    public abstract class BaseClassData : IEnumerable<object[]>
    {
        public abstract IEnumerator<object[]> GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}