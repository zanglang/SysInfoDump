//-----------------------------------------------------------------------
// <copyright file="UnitTest.cs" company="muvee Technologies Pte Ltd">
//   Copyright (c) muvee Technologies Pte Ltd. All rights reserved.
// </copyright>
// <author>Jerry Chong</author>
//-----------------------------------------------------------------------

namespace SysInfoDump
{
    using System.Diagnostics;
    using System.Windows.Forms;

    using NUnit.Framework;
    using SysInfoSharp;

    /// <summary>
    /// NUnit test fixtures
    /// </summary>
    [TestFixture, RequiresSTA]
    public class UnitTest
    {
        /// <summary>
        /// DirectX test stub
        /// </summary>
        private DxDiagLib directXInfo;

        /// <summary>
        /// Driver Information stub
        /// </summary>
        private DriverInfoLib driverInfo;

        /// <summary>
        /// System Information stub
        /// </summary>
        private SysInfoLib sysInfo;

        /// <summary>
        /// Initialize test fixtures.
        /// </summary>
        [SetUp]
        public void Init()
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new ConsoleTraceListener());
            Trace.AutoFlush = true;

            this.directXInfo = new DxDiagLib();
            this.driverInfo = new DriverInfoLib("{5B45201D-F2F2-4F3B-85BB-30FF1F953599}");
            this.sysInfo = new SysInfoLib();
        }

        /// <summary>
        /// The driver info test.
        /// </summary>
        [Test]
        public void DriverInfoTest()
        {
            Assert.IsNotNull(this.driverInfo);

            var count = this.driverInfo.GetNumDevices();
            Trace.WriteLine("Devices: " + count);
            Assert.GreaterOrEqual(count, 0);

            const uint SpdrpInstallState = 0x00000022;
            const uint SpdrpPhysicalDeviceObjectName = 0x0000000E;

            for (int i = 0; i < count; i++)
            {
                MessageBox.Show("test", "test");

                var locationInfo = this.driverInfo.GetPropertyString(SpdrpPhysicalDeviceObjectName, i);
                var installState = this.driverInfo.GetPropertyValue(SpdrpInstallState, i);

                Assert.IsNotNullOrEmpty(locationInfo);
                Trace.WriteLine("Location: " + locationInfo);
                Trace.WriteLine("State: " + installState);
            }
        }

        /// <summary>
        /// Test DirectX querying
        /// </summary>
        [Test]
        public void DxDiagTest()
        {
            Assert.IsNotNull(this.directXInfo);

            var major = this.directXInfo.GetInfoByContainer("DxDiag_SystemInfo", 1, "dwDirectXVersionMajor");
            var minor = this.directXInfo.GetInfoByContainer("DxDiag_SystemInfo", 1, "dwDirectXVersionMinor");
            var value = this.directXInfo.GetInfoByContainer("DxDiag_SystemInfo", 1, "szDirectXVersionLetter");
            
            Assert.IsNotNullOrEmpty(major);
            Assert.Greater(int.Parse(major), 0);

            Assert.IsNotNullOrEmpty(minor);
            Assert.GreaterOrEqual(int.Parse(major), 0);

            var version = major + "." + minor;
            if (!string.IsNullOrEmpty(value))
            {
                version += "." + value;
            }

            Trace.WriteLine("Version: " + version);

            var vram = this.directXInfo.GetInfoByContainer("DxDiag_DisplayDevices", 0, "szDisplayMemoryEnglish");
            Assert.IsNotNullOrEmpty(vram);
            Trace.WriteLine("VRAM: " + vram);
        }

        /// <summary>
        /// Test enumerating DirectX display devices
        /// </summary>
        [Test]
        public void DxDiagEnumTest()
        {
            Assert.IsNotNull(this.directXInfo);

            int count = this.directXInfo.GetContainerChildrenCount("DxDiag_DisplayDevices");
            Assert.GreaterOrEqual(count, 0);

            var keys = this.directXInfo.EnumProperties("DxDiag_DisplayDevices", count - 1);
            CollectionAssert.IsNotEmpty(keys);

            foreach (var key in keys)
            {
                Assert.IsNotNull(key);
                var value = this.directXInfo.GetInfoByContainer("DxDiag_DisplayDevices", 0, key);

                Assert.IsNotNull(value);
                Trace.WriteLine(key + ": " + value);
            }
        }

        /// <summary>
        /// Test system information querying
        /// </summary>
        [Test]
        public void SysInfoEnumTest()
        {
            Assert.IsNotNull(this.sysInfo);
            this.sysInfo.Init();

            var categories = this.sysInfo.GetCategories();
            CollectionAssert.IsNotEmpty(categories);

            foreach (var category in categories)
            {
                Assert.IsNotNull(category);
                
                var map = this.sysInfo[category];
                Assert.IsNotNull(map);

                foreach (var pair in map)
                {
                    Assert.IsNotNull(pair.Key);
                    Assert.IsNotNull(pair.Value);
                    Trace.WriteLine(pair.Key + ": " + pair.Value);
                }
            }
        }
    }
}
