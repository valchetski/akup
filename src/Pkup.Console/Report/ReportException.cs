using System.Runtime.Serialization;

namespace Pkup.Console.Report
{
    [Serializable]
    public class ReportException : Exception
    {
        public ReportException()
        {
        }

        public ReportException(string message)
            : base(message)
        {
        }

        protected ReportException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
