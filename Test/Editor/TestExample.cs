using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Packages.Localization.Test.Editor
{
    public class TestExample
    {
        // A Test behaves as an ordinary method
        [Test]
        public void EmptyTest()
        {
            bool isActive = false;

            Assert.AreEqual(false, isActive);
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