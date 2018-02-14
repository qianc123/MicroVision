﻿using System;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using NUnit.Framework;
using Services;

namespace SerialServiceTest
{
    [TestFixture]
    public class SerialServiceBasicUnitTest
    {
        private string Uri = "localhost";
        private int Port = 39945;

        private Channel channel;
        private CameraController.CameraControllerClient client;
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        private string _comPort = "COM23";
        private TextWriter _writer;


        [SetUp]
        public void Init()
        {
            _writer = TestContext.Out;
            channel = new Channel(Uri, Port, ChannelCredentials.Insecure);
            client = new CameraController.CameraControllerClient(channel);
        }

        [Category("Basic")]
        [Test]
        public void TestGetInfo ()
        {
            var info = client.GetInfo(new Empty());
            TestContext.WriteLine(
                $"Info package: \n \tFirmware = {info.FirmwareVersion} \n \tHardware = {info.HardwareVersion} \n \tService = {info.ServiceVersion}");
        }

        [Category("Basic")]
        [Test]
        public async Task TestGetInfoAsync()
        {
            var info = await client.GetInfoAsync(new Empty());
            TestContext.WriteLine($"Info package: \n \tFirmware = {info.FirmwareVersion} \n \tHardware = {info.HardwareVersion} \n \tService = {info.ServiceVersion}");
        }

        [Category("Basic")]
        [Test]
        public void TestIsConnected()
        {
            var isConnected = client.IsConnected(new Empty());
            TestContext.WriteLine($"IsConnected={isConnected.IsConnected}");
        }

        [Category("Basic")]
        [Test]
        public void TestGetComList()
        {
            var comList = client.RequestComList(new ComListRequest());
            TestContext.WriteLine($"COM Detected: {comList.ComPort}");
        }

        [Category("Basic")]
        [Test]
        public void TestConnectAndClose()
        {
            var connect = client.RequestConnectToPort(new ConnectionRequest() {ComPort = _comPort, Connect = true});
            Assert.IsNull(connect.Error);
            var disconnect = client.RequestConnectToPort(new ConnectionRequest() { Connect = false });
            Assert.IsNull(disconnect.Error);
        }

        [Category("Nauty")]
        [Description("Should it fail when the connection argument is invalid")]
        [Test]
        public void TestConnectWithoutName()
        {
            var connect = client.RequestConnectToPort(new ConnectionRequest() {Connect = true });
            Assert.NotNull(connect.Error);
            TestContext.WriteLine($"When no COM name is given, you will get error {connect.Error}");
        }

        [Category("Nauty")]
        [Description("Should it fail when the connection is already opened")]
        [Test]
        public void TestRepeatedOpen()
        {
            var connect = client.RequestConnectToPort(new ConnectionRequest() { ComPort = _comPort, Connect = true });
            Assert.IsNull(connect.Error, "The first connection should not fail");
            connect = client.RequestConnectToPort(new ConnectionRequest() { ComPort = _comPort, Connect = true });
            Assert.NotNull(connect.Error, "The second connection should fail");
            TestContext.WriteLine($"When you try to reopen the COM port, it should give {connect.Error}");
        }

        [Category("Basic")]
        [Description("Read power code, write, and read again")]
        [Test]
        public async Task TestPowerConfiguration()
        {
            const int delay = 2000;
            
            int powerCode = 9;

            var connect = client.RequestConnectToPort(new ConnectionRequest() { ComPort = _comPort, Connect = true });
            Assert.IsNull(connect.Error, "The connection should not fail");

            var powerStatusResponse = client.RequestPowerStatus(new PowerStatusRequest() {Write = false});
            Assert.IsNull(powerStatusResponse.Error, "Error when read the power status");
            _writer.WriteLine($"Initial power code is {powerStatusResponse.PowerCode}");

            await Task.Delay(delay);
            powerStatusResponse = client.RequestPowerStatus(new PowerStatusRequest() { Write = true, PowerCode = powerCode });
            Assert.IsNull(powerStatusResponse.Error, "Error when read the power status");
            Assert.AreEqual(powerStatusResponse.PowerCode, powerCode);
            _writer.WriteLine($"After write, power code is {powerStatusResponse.PowerCode}");

            await Task.Delay(delay);
            powerStatusResponse = client.RequestPowerStatus(new PowerStatusRequest() { Write = false });
            Assert.IsNull(powerStatusResponse.Error, "Error when read the power status");
            Assert.AreEqual(powerStatusResponse.PowerCode, powerCode);
            _writer.WriteLine($"Read again, power code is {powerStatusResponse.PowerCode}");

            await Task.Delay(delay);
            powerStatusResponse = client.RequestPowerStatus(new PowerStatusRequest() { Write = true, PowerCode = 0});
            Assert.IsNull(powerStatusResponse.Error, "Error when read the power status");
            Assert.AreEqual(powerStatusResponse.PowerCode, 0);
            _writer.WriteLine($"Finally, reset the power code to 0, power code is {powerStatusResponse.PowerCode}");
        }

        [Category("Basic")]
        [Test]
        public void TestReadCurrent()
        {
            client.RequestConnectToPort(new ConnectionRequest() {Connect = true, ComPort = _comPort});
            var currentStatusResponse = client.RequestCurrentStatus(new CurrentStatusRequest());
            Assert.IsNull(currentStatusResponse.Error, "Error occured in the current read process");
            _writer.WriteLine($"The current reading is {currentStatusResponse.Current}");
        }
        [TearDown]
        public void Cleanup()
        {
            // shutdown the COM connection
            client.RequestConnectToPort(new ConnectionRequest() {Connect = false});
            channel.ShutdownAsync().Wait();
        }
    }
}
