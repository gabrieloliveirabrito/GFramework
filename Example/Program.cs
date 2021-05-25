using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GFramework.Bases;
using GFramework.Enums;
using GFramework.Factories;
using GFramework.Interfaces;
using GFramework.Network.PacketReaders;

namespace Example
{
    class Program
    {
        public uint Interval => 1000;

        public UpdaterMode Mode => UpdaterMode.DelayAfter;

        static BitPacketReader CreatePacket()
        {
            var random = new Random(DateTime.Now.Year + DateTime.Now.Second - DateTime.Now.Millisecond);

            var packet = new BitPacketReader((uint)random.Next(0, int.MaxValue));
            packet.WriteBoolean(false);
            packet.WriteInt(random.Next(-1000, 1000));
            packet.WriteString("Hello World, a small test here");
            packet.WriteUInt(130);
            packet.WriteULong(1000000);
            packet.WriteUShort(10);
            packet.WriteDecimal(decimal.MaxValue);
            packet.WriteFloat(float.MaxValue);
            packet.WriteDouble(random.NextDouble());
            packet.WriteBoolean(false);
            packet.WriteInt(random.Next(-1000, 1000));
            packet.WriteString("Hello World, a small test here");
            packet.WriteUInt(130);
            packet.WriteULong(1000000);
            packet.WriteUShort(10);
            packet.WriteDecimal(decimal.MaxValue);
            packet.WriteFloat(float.MaxValue);
            packet.WriteDouble(random.NextDouble());
            packet.WriteBoolean(false);
            packet.WriteInt(random.Next(-1000, 1000));
            packet.WriteString("Hello World, a small test here");
            packet.WriteUInt(130);
            packet.WriteULong(1000000);
            packet.WriteUShort(10);
            packet.WriteDecimal(decimal.MaxValue);
            packet.WriteFloat(float.MaxValue);
            packet.WriteDouble(random.NextDouble());
            packet.WriteBoolean(false);
            packet.WriteInt(random.Next(-1000, 1000));
            packet.WriteString("Hello World, a small test here");
            packet.WriteUInt(130);
            packet.WriteULong(1000000);
            packet.WriteUShort(10);
            packet.WriteDecimal(decimal.MaxValue);
            packet.WriteFloat(float.MaxValue);
            packet.WriteDouble(random.NextDouble());
            packet.Offset = 0;

            return packet;
        }

        static bool ReadPacket(BitPacketReader packet)
        {
            var b = packet.ReadBoolean();
            var i = packet.ReadInt();
            var s = packet.ReadString();
            var ui = packet.ReadUInt();
            var ul = packet.ReadULong();
            var us = packet.ReadUShort();
            var de = packet.ReadDecimal();
            var f = packet.ReadFloat();
            var d = packet.ReadDouble();
            b = packet.ReadBoolean();
            i = packet.ReadInt();
            s = packet.ReadString();
            ui = packet.ReadUInt();
            ul = packet.ReadULong();
            us = packet.ReadUShort();
            de = packet.ReadDecimal();
            f = packet.ReadFloat();
            d = packet.ReadDouble();
            b = packet.ReadBoolean();
            i = packet.ReadInt();
            s = packet.ReadString();
            ui = packet.ReadUInt();
            ul = packet.ReadULong();
            us = packet.ReadUShort();
            de = packet.ReadDecimal();
            f = packet.ReadFloat();
            d = packet.ReadDouble();
            b = packet.ReadBoolean();
            i = packet.ReadInt();
            s = packet.ReadString();
            ui = packet.ReadUInt();
            ul = packet.ReadULong();
            us = packet.ReadUShort();
            de = packet.ReadDecimal();
            f = packet.ReadFloat();
            d = packet.ReadDouble();

            return b == false && s == "Hello World, a small test here";
        }

        static bool TestPackets(int count, bool threaded, out TimeSpan time)
        {
            bool error = false;
            DateTime start = DateTime.Now;

            if (threaded)
            {
                var run = false;

                var thread = new Thread(() =>
                {
                    while (run == false) Thread.Sleep(10);

                    for (int i = 0; i < count; i++)
                    {
                        using (var packet = CreatePacket())
                            error = ReadPacket(packet);

                        GC.Collect(GC.MaxGeneration);
                        GC.WaitForPendingFinalizers();
                        if (error) break;
                    }
                });

                thread.Start();
                run = true;

                while (thread.ThreadState != ThreadState.Stopped) ;
                time = DateTime.Now - start;

                return error;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    using (var packet = CreatePacket())
                        error = ReadPacket(packet);

                    GC.Collect(GC.MaxGeneration);
                    GC.WaitForPendingFinalizers();
                    if (error) break;
                }

                time = DateTime.Now - start;
                return error;
            }
        }

        static void TestPackets(int count, bool threaded, BaseLogger log)
        {
            if (log == null)
                TestPackets(count, threaded, out _);
            else
            {
                log.LogInfo("Testing {0} packets....", count);
                if (TestPackets(count, threaded, out TimeSpan time))
                    log.LogSuccess("{0} Packets OK! Elapsed = {1}", count, time);
                else
                    log.LogSuccess("{0} Packets OK! Elapsed = {1}", count, time);
            }
        }

        static void Main(string[] args)
        {
            var log = LoggerFactory.GetLogger<Program>();
            log.LogInfo("Info message");
            log.LogSuccess("Success message");
            log.LogWarning("Warning message");
            log.LogError("Error message");
            log.LogDebug("Debug message");
            log.LogFatal(new Exception("Fatal message"));

            DateTime start = DateTime.Now;
            TestPackets(100, true, log);
            TestPackets(1000, true, log);
            TestPackets(10000, true, log);
            TestPackets(100000, true, log);
            TestPackets(1000000, true, log);
            TestPackets(10000000, true, log);
            TestPackets(100000000, true, log);
            TestPackets(1000000000, true, log);
            log.LogSuccess("Packet performance test end! Elapsed = {0}", DateTime.Now - start);

            var threads = new Thread[1000];
            log.LogInfo("Testing packets with {0} threads", threads.Length);

            start = DateTime.Now;
            bool run = false;
            for(int i = 0, n = threads.Length; i < n; i++)
            {
                var thread = new Thread(() =>
                {
                    while (run == false) Thread.Sleep(10);

                    TestPackets(100, false, null);
                    TestPackets(1000, false, null);
                    TestPackets(10000, false, null);
                    TestPackets(100000, false, null);
                    TestPackets(1000000, false, null);
                    TestPackets(10000000, false, null);
                    TestPackets(100000000, false, null);
                    TestPackets(1000000000, false, null);
                });

                thread.Start();
            }

            run = true;
            while (!threads.All(t => t == null)) ;

            log.LogSuccess("Threaded Packet performance test end! Elapsed = {0}", DateTime.Now - start);

            log.LogWarning("Press Enter to exit...");
            Console.ReadLine();
            SingletonFactory.DestroyAll();
        }
    }
}