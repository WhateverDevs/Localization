using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Packages.Localization.Test.Runtime
{
    public class TestExampleRuntime
    {
        // A Test behaves as an ordinary method
        [Test]
        public void EmptyTest()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator passes()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}