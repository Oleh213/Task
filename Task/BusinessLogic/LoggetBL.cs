using System;
using Task.Context;
using Task.DBContext;
using Task.Interfaces;

namespace Task.BusinessLogic
{
    public class LoggerBL : ILoggerBL
    {
        private DogsContext _context;

        public LoggerBL(DogsContext context)
        {
            _context = context;
        }
        public void AddLog(LoggerLevel loggerLevel, string message)
        {
            TimeZoneInfo ukraineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev");

            DateTime utcTime = DateTime.UtcNow;

            DateTime newDate = TimeZoneInfo.ConvertTimeFromUtc(utcTime, ukraineTimeZone);

            _context.Loggers.Add(new Logger { LoggerId = Guid.NewGuid(), Message = message, LoggerLevel = loggerLevel, LogTime = newDate });

            _context.SaveChanges();
        }
    }
}

