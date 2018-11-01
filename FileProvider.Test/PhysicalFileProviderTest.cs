using System;
using System.IO;
using NUnit.Framework;
using Microsoft.Extensions.FileProviders;
using com.zhusmelb.Util.Logging;

namespace FileProvider.Test
{
    [TestFixture]
    public class PhysicalFileProviderTest
    {
        private static readonly ILogger _log 
            = LogHelper.GetLogger(typeof(PhysicalFileProviderTest).FullName);

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
            var path = Path.GetFullPath("./Assets/Folder2");
            _log.Info($"Full path of target folder {path}");
            Assert.That(Path.IsPathRooted(path), Is.True);
            
            // This test failed with .NET Core 2.1.1 (tags/7923b2c6) and below.
            // The ctor of PhysicalFileProvider call CreateFileWatcher to create
            // a FileSystemWatcher with the 'path'. This will cause an 
            // ArugmentException when the 'path' doesn't exist.
            Assert.Throws<DirectoryNotFoundException>(
                () => new PhysicalFileProvider(path)
            );
        }
    }
}
