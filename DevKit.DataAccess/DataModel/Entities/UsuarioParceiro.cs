using LinqToDB;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModel
{
    public partial class UsuarioParceiro
    {
        [NotMapped]
        public string _stParceiro { get; set; }
        [NotMapped]
        public string _sbAtivo { get; set; }
        [NotMapped]
        public string _dtCadastro { get; set; }
        [NotMapped]
        public string _dtLastLogin { get; set; }
        [NotMapped]
        public string _tipo { get; set; }

        [NotMapped]
        public string updateCommand { get; set; }

        [NotMapped]
        public string _novaSenha { get; set; }

        public bool Create(AutorizadorCNDB db, ref string apiError)
        {
            try
            {
                this.dtCadastro = DateTime.Now;

                db.Insert(this);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(AutorizadorCNDB db, ref string apiError)
        {
            try
            {
                db.Update(this);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
