namespace nats_ui.Data.Scripts
{
    public class ScriptStatement : ICheckable
    {
        public bool Checked { get; set; }
        
        public string Name { get; set; } 
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        
        public string Up => "https://img.icons8.com/flat_round/64/000000/collapse-arrow--v1.png";
        public string Down => "https://img.icons8.com/flat_round/64/000000/expand-arrow--v1.png";
        public string Trash => "img/trash.svg";
    }
}