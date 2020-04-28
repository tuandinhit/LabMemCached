using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StackExchange.Redis;

namespace DealApi.Utility
{
    public class RedisConnectionFactory
    {
        private static readonly Lazy<ConnectionMultiplexer> Connection;

        static RedisConnectionFactory()
        {
            var connectionString = "cache1";//System.Configuration.ConfigurationManager.AppSettings["RedisConnection"].ToString();

            var options = ConfigurationOptions.Parse(connectionString);

            Connection = new Lazy<ConnectionMultiplexer>(
                () => ConnectionMultiplexer.Connect(options)
            );
        }

        public static ConnectionMultiplexer GetConnection() { return Connection.Value; }
    }
}