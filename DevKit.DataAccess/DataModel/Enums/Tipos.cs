using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevKit.DataAccess
{
    public class ComboItem
    {
        public long id { get; set; }
        public string stName { get; set; }
    }

    public class TipoSituacaoCartao
    {
        public const int Habilitado = 0,
                         Bloqueado = 1;
    }

    public class TipoExpedicaoCartao
    {
        public const int Requerido = 0,
                         EmProducao = 1,
                         Entregue = 2;
    }

    public class TipoSitAutorizacao
    {

        /*
         *  'Em aberto', 'Em revisão', 'Aprovado emissor', 'Cancelada emissor', 'Cancelada plano' */

        public const int EmAberto = 1,
                         EmRevisao = 2,
                         AprovadoEmissor = 3,
                         CanceladaEmissor = 4,
                         GlosadoPlano = 5,
                         AprovadoPlano = 6,
                         RejeitadoPlano = 7,
                         ErroAutorizador = 8;
    }
    
    public class TipoCredenciado
    {
        public const int Medico = 1,
                         Clinica = 2,
                         Laboratorio = 3;
    }

    public class TipoSituacaoConsultaCredenciado
    {
        public const int Solicitada = 1,
                         Confirmada = 2,
                         CanceladaEmissor = 3,
                         CanceladaCredenciado = 4,
                         Executada = 5;
    }
}
