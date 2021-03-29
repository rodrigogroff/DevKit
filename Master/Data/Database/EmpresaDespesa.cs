
namespace Master.Data.Database
{
    public class EmpresaDespesa
    {
        public long id { get; set; }
        public long? fkEmpresa { get; set; }
        public string stCodigo { get; set; }
        public string stDescricao { get; set; }
    }
}
