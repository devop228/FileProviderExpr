using System;
using System.IO;
using System.Collections.Generic;

using NUnit.Framework;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

using com.zhusmelb.Util.Logging;

namespace FileProvider.Test
{
    [TestFixture]
    public class PhysicalFileProviderTest
    {
        private IFileProvider _fileProvider;
        private const string Test_Assets_Path = "./Assets/";
        private static readonly ILogger _log 
            = LogHelper.GetLogger(typeof(PhysicalFileProviderTest).FullName);

        [SetUp]
        public void Init() {
            var path = Path.GetFullPath(Test_Assets_Path);
            Assert.That(Path.IsPathRooted(path), Is.True);
            Assert.That(Directory.Exists(path), Is.True);
            _fileProvider = new PhysicalFileProvider(Test_Assets_Path, ExclusionFilters.Sensitive);
            Assert.That(_fileProvider, Is.Not.Null);
        }

        [TearDown]
        public void Cleanup() {
            (_fileProvider as IDisposable)?.Dispose();
        }

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
        [Ignore("Testing a bug in .NET Core 2.1.1")]
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

        [Test]
        public void GetDirectoryContentsTest() {
            var provider = _fileProvider;
            _log.Info($"provider.Root: {provider?.Root}");
            var contents = provider.GetDirectoryContents("\temp");
            Assert.That(Object.ReferenceEquals(contents, NotFoundDirectoryContents.Singleton), Is.True);

            contents = provider.GetDirectoryContents("");
            Assert.That(contents.Exists, Is.True);
            logFileInfo(contents);

            contents = provider.GetDirectoryContents("Folder1");
            Assert.That(contents.Exists, Is.True);
            logFileInfo(contents);
        }

        private void logFileInfo(IEnumerable<IFileInfo> files) {
            foreach (var fileInfo in files) {
                if (!fileInfo.Exists)
                    _log.Info($"{fileInfo.Name} does not exist.");
                else 
                    _log.Info($"{fileInfo.Name}, {(fileInfo.IsDirectory ? "Directory" : fileInfo.Length.ToString())}, {fileInfo.LastModified}, {fileInfo.PhysicalPath}");
            }
        }
    }
}
