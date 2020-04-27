using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;

namespace DemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
         //   log4net.Config.XmlConfigurator.Configure();


            var mbcc = new MemcachedClientConfiguration();

            mbcc.SocketPool.ReceiveTimeout = new TimeSpan(0, 0, 2);
            mbcc.SocketPool.DeadTimeout = new TimeSpan(0, 0, 10);

            mbcc.AddServer("localhost:11211");

            var client = new MemcachedClient(mbcc);
            var item1= client.Get("item10");


            

            if (item1 == null)
            {
                byte[] bytes = File.ReadAllBytes(@"d:\VPS_CV_DINH VAN TUAN.doc");

                var item10 = client.Cas(StoreMode.Set, "item10", bytes);
                Console.WriteLine("tao lai cache:" + item10);
            }
            else
            {
                //File.WriteAllBytes("Foo1.doc", (byte[])item1); // Requires System.IO
                Console.WriteLine("Gia tri tu cache: " + item1);


            }


            //var item2 = client.Cas(StoreMode.Set, "item2", 2);

            //var add1 = client.Cas(StoreMode.Add, "item1", 4);

            //Console.WriteLine(add1.Result);
            //Console.WriteLine(add1.Cas);
            //Console.WriteLine(add1.StatusCode);

            Console.WriteLine("item1 = " + item1);
            

            //mre.Set();

            //client.Sync("item1", item1.Cas, SyncMode.Mutation);
            Console.WriteLine("Changed");

            Console.ReadLine();
        }

        private static void MultigetSpeedTest()
        {
            //Enyim.Caching.LogManager.AssignFactory(new ConsoleLogFactory());
            var tmc = new MemcachedClientConfiguration();

            tmc.AddServer("172.16.203.2:11211");
            tmc.AddServer("172.16.203.2:11212");
            //tmc.AddServer("172.16.203.2:11213");
            //tmc.AddServer("172.16.203.2:11214");

            tmc.Protocol = MemcachedProtocol.Binary;
            //tmc.SocketPool.MinPoolSize = 1;
            //tmc.SocketPool.MaxPoolSize = 1;

            tmc.SocketPool.ReceiveTimeout = new TimeSpan(0, 0, 4);
            tmc.SocketPool.ConnectionTimeout = new TimeSpan(0, 0, 4);

            tmc.KeyTransformer = new DefaultKeyTransformer();

            var tc = new MemcachedClient(tmc);
            const string KeyPrefix = "asdfghjkl";

            var val = new byte[10 * 1024];
            val[val.Length - 1] = 1;

            for (var i = 0; i < 100; i++)
                if (!tc.Store(StoreMode.Set, KeyPrefix + i, val))
                    Console.WriteLine("Fail " + KeyPrefix + i);

            var keys = Enumerable.Range(0, 500).Select(k => KeyPrefix + k).ToArray();

            Console.WriteLine("+");

            var sw = Stopwatch.StartNew();
            //tc.Get(KeyPrefix + "4");
            //sw.Stop();
            //Console.WriteLine(sw.ElapsedMilliseconds);

            //sw = Stopwatch.StartNew();

            //var p = tc.Get2(keys);

            //sw.Stop();
            //Console.WriteLine(sw.ElapsedMilliseconds);


            //sw = Stopwatch.StartNew();

            //var t = tc.Get(keys);
            //Console.WriteLine(" --" + t.Count);

            //sw.Stop();
            //Console.WriteLine(sw.ElapsedMilliseconds);

            //Console.WriteLine("Waiting");
            //Console.ReadLine();

            //return;

            for (var i = 0; i < 100; i++)
            {
                const int MAX = 300;

                sw = Stopwatch.StartNew();
                for (var j = 0; j < MAX; j++) tc.Get(keys);
                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds);

                //sw = Stopwatch.StartNew();
                //for (var j = 0; j < MAX; j++) tc.GetOld(keys);
                //sw.Stop();
                //Console.WriteLine(sw.ElapsedMilliseconds);

                //sw = Stopwatch.StartNew();
                //for (var j = 0; j < MAX; j++)
                //    foreach (var k in keys) tc.Get(k);
                //sw.Stop();
                //Console.WriteLine(sw.ElapsedMilliseconds);

                Console.WriteLine("----");
            }

            Console.ReadLine();
            return;
        }

        private static void StressTest(MemcachedClient client, string keyPrefix)
        {
            var i = 0;
            var last = true;

            var progress = @"-\|/".ToCharArray();
            Console.CursorVisible = false;
            Dictionary<bool, int> counters = new Dictionary<bool, int>() { { true, 0 }, { false, 0 } };

            while (true)
            {
                var key = keyPrefix + i;
                var state = client.Store(StoreMode.Set, key, i) & client.Get<int>(key) == i;

                Action updateTitle = () => Console.Title = "Success: " + counters[true] + " Fail: " + counters[false];

                if (state != last)
                {
                    Console.ForegroundColor = state ? ConsoleColor.White : ConsoleColor.Red;
                    Console.Write(".");

                    counters[state] = 0;
                    last = state;

                    updateTitle();
                }
                else if (i % 200 == 0)
                {
                    //Console.ForegroundColor = state ? ConsoleColor.White : ConsoleColor.Red;

                    //Console.Write(progress[(i / 200) % 4]);
                    //if (Console.CursorLeft == 0)
                    //{
                    //    Console.CursorLeft = Console.WindowWidth - 1;
                    //    Console.CursorTop -= 1;
                    //}
                    //else
                    //{
                    //    Console.CursorLeft -= 1;
                    //}

                    updateTitle();
                }

                i++;
                counters[state] = counters[state] + 1;
            }
        }
    }
}