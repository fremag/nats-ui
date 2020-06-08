using System.Collections.Generic;
using System.Text.Json;
using JsonPathway;
using NLog;

namespace nats_ui.Data.Scripts
{
    public class CheckJsonCommand : AbstractScriptCommand
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    
        public override string ParamName1 => "Path";
        public override string ParamName2 => "Expected";
        
        public override string Execute(NatsService natsService, ExecutorService executorService)
        {
            string data = executorService.Message.Data;
            IReadOnlyList<JsonElement> resultJson = JsonPath.ExecutePath(Param1, data);
            var result = JsonSerializer.Serialize(resultJson, new JsonSerializerOptions {WriteIndented = true});

                if (result == Param2)
                {
                    return "Ok.";
                }

                Logger.Error($"Pattern: {Param1}");
                Logger.Error($"Data: {data}");
                Logger.Error($"Result: {result}");
                Logger.Error($"Expected: {Param2}");
                throw new ScriptCommandException("Result and Expected values are different !");
        }
    }
}