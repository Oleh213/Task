using System;
using Task.Context;

namespace Task.Interfaces
{
    public interface ILoggerBL
    {
        void AddLog(LoggerLevel loggerLevel, string message);
    }
}

