
export class Util {

    checkCPFMask = (valor) => {

        if (valor === undefined || valor === null)
            valor = '';

        valor = valor.toString();
        valor = valor.replace(/[^0-9]/g, '');

        if (valor.length === 11)
            return true;

        return false;
    }

    checkName = (valor) => {
        valor = valor.toString();
        if (valor.length < 5)
            return false;
        if (valor.indexOf(' ') === -1)
            return false;
        if (valor.indexOf(' ') === valor.length - 1)
            return false;
        return true;
    }

    checkPassword = (valor) => {
        if (valor === undefined)
            valor = '';
        valor = valor.toString();
        if (valor.length < 4)
            return false;
        return true;
    }

    checkPhone = (valor) => {
        valor = valor.toString();
        var vlr = ""
        for (let i = 0; i < valor.length; ++i)
            if (valor[i] !== '_' && valor[i] !== '(' && valor[i] !== ')' && valor[i] !== ' ')
                vlr += valor[i]
        valor = vlr;
        if (valor.length < 10)
            return false;
        else
            return true;
    }

    checkEmail = (email) => {
        return email.trim() !== "" && /\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/.test(email);
    }

    checkCPF = (valor) => {
        switch (valor) {
            case "11111111111": case "22222222222": case "33333333333": case "44444444444": case "55555555555":
            case "66666666666": case "77777777777": case "88888888888": case "99999999999": case "00000000000":
                return false;
            default:
                break;
        }
        valor = valor.toString();
        valor = valor.replace(/[^0-9]/g, '');
        var digitos = valor.substr(0, 9);
        var novo_cpf = this.calc_digitos_posicoes(digitos);
        novo_cpf = this.calc_digitos_posicoes(novo_cpf, 11);

        if (novo_cpf === valor) {
            return true;
        } else {
            return false;
        }
    }

    calc_digitos_posicoes(digitos, posicoes, soma_digitos) {
        if (posicoes === null || posicoes === undefined)
            posicoes = 10;
        if (soma_digitos === null || soma_digitos === undefined)
            soma_digitos = 0;
        digitos = digitos.toString();
        for (var i = 0; i < digitos.length; i++) {
            soma_digitos = soma_digitos + (digitos[i] * posicoes);
            posicoes--;
            if (posicoes < 2) {
                posicoes = 9;
            }
        }
        soma_digitos = soma_digitos % 11;
        if (soma_digitos < 2) {
            soma_digitos = 0;
        } else {
            soma_digitos = 11 - soma_digitos;
        }
        var cpf = digitos + soma_digitos;
        return cpf;
    }
}
