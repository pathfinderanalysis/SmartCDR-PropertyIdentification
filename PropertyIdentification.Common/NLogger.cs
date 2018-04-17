using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace PropertyIdentification.Common
{
    public class NLogger : Interfaces.ILogger
    {
        /// <summary>
        /// The logger field
        /// </summary>
        private Logger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogger" /> class.
        /// </summary>
        public NLogger()
        {
            this.logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// The WriteLog method
        /// </summary>
        /// <param name="ex">The ex parameter</param>
        /// <param name="message">The message parameter</param>
        /// <param name="logLevel">The logLevel parameter</param>
        public void WriteLog(Exception ex, string message, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Fatal:
                    this.logger.Fatal(ex, message);
                    break;

                case LogLevel.Error:
                    this.logger.Error(ex, message);
                    break;

                case LogLevel.Warn:
                    this.logger.Warn(ex, message);
                    break;

                case LogLevel.Info:
                    this.logger.Info(ex, message);
                    break;

                case LogLevel.Debug:
                    this.logger.Debug(ex, message);
                    break;

                case LogLevel.Trace:
                    this.logger.Trace(ex, message);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// The WriteLog method
        /// </summary>
        /// <param name="message">The message parameter</param>
        /// <param name="logLevel">The logLevel parameter</param>
        public void WriteLog(string message, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Fatal:
                    this.logger.Fatal(message);
                    break;

                case LogLevel.Error:
                    this.logger.Error(message);
                    break;

                case LogLevel.Warn:
                    this.logger.Warn(message);
                    break;

                case LogLevel.Info:
                    this.logger.Info(message);
                    break;

                case LogLevel.Debug:
                    this.logger.Debug(message);
                    break;

                case LogLevel.Trace:
                    this.logger.Trace(message);
                    break;

                default:
                    break;
            }
        }
    }
}