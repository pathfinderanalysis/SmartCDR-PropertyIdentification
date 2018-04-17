using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyIdentification.Common.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The ILogger interface
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// The WriteLog method
        /// </summary>
        /// <param name="ex">The ex parameter</param>
        /// <param name="message">The message parameter</param>
        /// <param name="logLevel">The logLevel parameter</param>
        void WriteLog(Exception ex, string message, LogLevel logLevel);

        /// <summary>
        /// The WriteLog method
        /// </summary>
        /// <param name="message">The message parameter</param>
        /// <param name="logLevel">The logLevel parameter</param>
        void WriteLog(string message, LogLevel logLevel);
    }
}