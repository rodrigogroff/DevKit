using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevKit.DataAccess
{
    public class TipoSituacao
    {
        public const int Habilitado = 0,
                         Bloqueado = 1;
    }

    public class TipoSitAutorizacao
    {
        public const int Autorizado = 1,
                         Confirmado = 2,
                         Glosado = 3,
                         Rejeitado = 4;
    }

    public class TipoExpedicao
    {
        public const int Requerido = 0,
                         EmProducao = 1,
                         Entregue = 2;
    }

    public class TipoMedico
    {
        public const int Medico = 0,
                         Clinica = 1;
    }

    public class TipoSituacaoConsulta
    {
        public const int Solicitada = 1,
                         Confirmada = 2,
                         CanceladaEmissor = 3,
                         CanceladaMedico = 4,
                         Executada = 5;
    }
}
