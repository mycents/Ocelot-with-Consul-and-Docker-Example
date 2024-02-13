using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Infra.Shared.StaticLogger
{
    public static class AppLogger
    {
        private static ILoggerFactory _loggerFactory;

        public static IServiceCollection AddStaticLogger(this IServiceCollection services)
        {
            _loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();

            return services;
        }

        public static ILogger CreateLogger<T>()
        {
            if (_loggerFactory != null)
            {
                return _loggerFactory.CreateLogger<T>();
            }
            else
            {
                return new NoOpLogger();
            }
        }

        public static ILogger CreateLogger(string categoryName)
        {
            if (_loggerFactory != null)
            {
                return _loggerFactory.CreateLogger(categoryName);
            }
            else
            {
                return new NoOpLogger();
            }
        }
    }

    public class NoOpLogger : ILogger, IDisposable
    {
        public IDisposable BeginScope<TState>(TState state) => this;
        public void Dispose() { }
        public bool IsEnabled(LogLevel logLevel) => false;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) { }
    }
}