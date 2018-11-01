using System;
using System.IO;
using NUnit.Framework;
using Microsoft.Extensions.FileProviders;

namespace FileProvider.Test
{
    [TestFixture]
    public class PhysicalFileProviderTest
    {
        [TestCase("")]
        [TestCase("./")]
        [TestCase("./Assets")]
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

        [TestCase("./Assets")]
        [TestCase("./Assets/Folder1")]
        public void ConstructorNormaliseTest(string path) {
            if (!Path.IsPathRooted(path))
                path = Path.GetFullPath(path);
            
            using (var provider=new PhysicalFileProvider(path)) {
                Assert.That(provider, Is.Not.Null);
            }
        }

        [Test]
        public void ConstructorNonExistTest() {
            var path = Path.GetFullPath("./Assets/file2");
            Assert.That(Path.IsPathRooted(path), Is.True);

            Assert.Throws<ArgumentException>(
                () => new PhysicalFileProvider(path)
            );
        }
    }
}
