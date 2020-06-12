namespace nats_ui.Data.Scripts
{
    public class ScriptStatement : ICheckable
    {
        public bool Checked { get; set; }
        
        public string Name { get; set; } 
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        
        public string Run => "oi oi-media-play";
        public string Insert => "oi oi-plus";
        public string Up => "oi oi-chevron-top";
        public string Down => "oi oi-chevron-bottom";
        public string Trash => "oi oi-trash";
    }
}