using System;

namespace nats_ui.Data.Scripts
{
    public class ScriptCommandException : Exception
    {
        public ScriptCommandException(string message) : base(message)
        {
        }
    }
}