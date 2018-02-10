﻿using System;
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
        public const int Autorizado = 1,
                         Confirmado = 2,
                         Glosado = 3,
                         Rejeitado = 4;
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
