using System.Text.RegularExpressions;
using NLog;

namespace nats_ui.Data.Scripts
{
    public class CheckRegexCommand : AbstractScriptCommand
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    
        public override string ParamName1 => "Pattern";
        public override string ParamName2 => "Expected";
        
        public override string Execute(NatsService natsService, ExecutorService executorService)
        {
            string data = executorService.Message.Data;
            Regex regex = new Regex(Param1);
            var match = regex.Match(data);
            if (match.Success && match.Groups.Count > 1)
            {
                var capture = match.Groups[1];
                if (capture.Value == Param2)
                {
                    return "Ok.";
                }

                Logger.Error($"Pattern: {Param1}");
                Logger.Error($"Data: {data}");
                Logger.Error($"Capture: {capture.Value}");
                Logger.Error($"Expected: {Param2}");
                throw new ScriptCommandException("Result and Expected values are different !");
            }
            Logger.Error($"Pattern: {Param2}");
            Logger.Error($"Data: {data}");
            throw new ScriptCommandException("No match for regex !");
        }
    }
}