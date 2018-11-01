using System;
using NUnit.Framework;
using Microsoft.Extensions.FileProviders;

namespace FileProvider.Test
{
    [TestFixture]
    public class PhysicalFileProviderTest
    {
        [TestCase("")]
        [TestCase("./")]
        public void ContructorInvalidTest(string path) {
            Assert.Throws<ArgumentException>(
                () => new PhysicalFileProvider(path)
            );
        }

        [TestCase(@"c:\")]
        [TestCase(@"c:\bin")]
        public void ContructorValidTest(string path) {
            using (var provider = new PhysicalFileProvider(path)) {
                Assert.That(provider, Is.Not.Null);
            }
        }
    }
}
