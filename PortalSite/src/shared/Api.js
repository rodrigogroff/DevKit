
export const ApiLocation = {
    //api_host: 'http://localhost',
    api_host: 'https://meuconvey.conveynet.com.br',
    api_port: '18524',
    api_portal: '/api/v1/portal/',
}

export class Api {

    versao = () => "v2.1.014";

    isAuthenticated = () => localStorage.getItem('token');

    loggedUserType = () => localStorage.getItem('type');
    loggedUserName = () => localStorage.getItem('user_name');

    loggedUserCartao = () => localStorage.getItem('cartao');

    cleanLogin() {
        localStorage.setItem('token', null)
        localStorage.setItem('user_name', null)
        localStorage.setItem('cartao', null)
        localStorage.setItem('type', null)
        localStorage.setItem('terminal', null)
    }

    loginOk = (token, nome, cartao) => {
        localStorage.setItem('token', token)
        localStorage.setItem('user_name', nome)
        localStorage.setItem('cartao', cartao)
        localStorage.setItem('type', '1')
    }

    loginLojistaOk = (token, terminal, nome) => {
        localStorage.setItem('token', token)
        localStorage.setItem('terminal', terminal)
        localStorage.setItem('user_name', nome)
        localStorage.setItem('type', '2')
    }

    ping = () => {
        return new Promise((resolve, reject) => {
            fetch(ApiLocation.api_host + ':' + ApiLocation.api_port + ApiLocation.api_portal + 'ping',
                {
                    method: 'GET', headers:
                    {
                        'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + localStorage.getItem('token'),
                        'Sessao': localStorage.getItem('sessao'),
                    }
                })
                .then((res) => {
                    if (res.status === 401)
                        reject({ ok: false })
                    else
                        resolve({ ok: true })
                })
                .catch(() => {
                    reject({ ok: false })
                });
        })
    }

    getTokenPortal = (location, parameters) => {
        return new Promise((resolve, reject) => {
            var _params = '';

            if (parameters !== null)
                _params = '?' + parameters;

            fetch(ApiLocation.api_host + ':' + ApiLocation.api_port + ApiLocation.api_portal + location + _params,
                {
                    method: 'GET', headers:
                    {
                        'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + localStorage.getItem('token'),
                    }
                })
                .then((res) => {
                    if (res.status === 401) {
                        reject({
                            ok: false,
                            unauthorized: true
                        })
                    }
                    else if (res.ok === true) {
                        res.json().then((data) => {
                            resolve({
                                ok: true,
                                payload: data,
                            })
                        })
                    }
                    else res.json().then((data) => {
                        var jData = JSON.parse(data.value)

                        resolve({
                            ok: false,
                            msg: jData.message,
                        })
                    });
                })
                .catch((errorMsg) => {
                    resolve({
                        ok: false,
                        msg: errorMsg.toString(),
                    })
                });
        })
    }

    postTokenPortal = (location, data) => {
        return new Promise((resolve, reject) => {
            fetch(ApiLocation.api_host + ':' + ApiLocation.api_port +
                ApiLocation.api_portal + location,
                {
                    method: 'POST', headers:
                    {
                        'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + localStorage.getItem('token'),
                    },
                    body: data
                })
                .then((res) => {
                    if (res.status === 401) {
                        reject({
                            ok: false,
                            unauthorized: true
                        })
                    }
                    else if (res.ok === true) {
                        res.json().then((data) => {
                            resolve({
                                ok: true,
                                payload: data,
                            })
                        })
                    }
                    else res.json().then((data) => {
                        resolve({
                            ok: false,
                            msg: data.message,
                        })
                    });
                })
                .catch((errorMsg) => {
                    resolve({
                        ok: false,
                        msg: errorMsg.toString(),
                    })
                });
        })
    }

    postPublicLoginPortal = (loginInfo) => {
        return new Promise((resolve, reject) => {
            fetch(ApiLocation.api_host + ':' + ApiLocation.api_port + ApiLocation.api_portal + 'authenticate',
                {
                    method: 'POST', headers: { 'Content-Type': 'application/json', }, body: loginInfo
                })
                .then((res) => {
                    if (res.ok === true) {
                        res.json().then((data) => {
                            resolve({
                                ok: true,
                                payload: data,
                            })
                        })
                    }
                    else {
                        res.json().then((data) => {
                            resolve({
                                ok: false,
                                msg: data.message,
                            })
                        })
                    }
                })
                .catch((errorMsg) => {
                    resolve({
                        ok: false,
                        msg: errorMsg.toString(),
                    })
                });
        })
    }

    postPublicLoginLojistaPortal = (loginInfo) => {
        return new Promise((resolve, reject) => {
            fetch(ApiLocation.api_host + ':' + ApiLocation.api_port + ApiLocation.api_portal + 'authenticateLojista',
                {
                    method: 'POST', headers: { 'Content-Type': 'application/json', }, body: loginInfo
                })
                .then((res) => {
                    if (res.ok === true) {
                        res.json().then((data) => {
                            resolve({
                                ok: true,
                                payload: data,
                            })
                        })
                    }
                    else {
                        res.json().then((data) => {
                            resolve({
                                ok: false,
                                msg: data.message,
                            })
                        })
                    }
                })
                .catch((errorMsg) => {
                    resolve({
                        ok: false,
                        msg: errorMsg.toString(),
                    })
                });
        })
    };

    trimStart = (character, string) => {
        var startIndex = 0;
        while (string[startIndex] === character)
            startIndex++;
        return string.substr(startIndex);
    }

    cleanup = (strMoney) => {
        var ret = '';
        var i = 0;
        for (; i < strMoney.length; i++)
            if (strMoney[i] === '0') { } else {
                break;
            }
        for (; i < strMoney.length; i++)
            ret += strMoney[i];
        return ret;
    };

    extractNumbers = (strMoney) => {
        var ret = '';
        for (var i = 0; i < strMoney.length; i++) {
            var va = strMoney[i];
            if (va === '0' || va === '1' || va === '2' || va === '3' || va === '4' || va === '5' || va === '6' || va === '7' || va === '8' || va === '9')
                ret += va;
        }
        return ret;
    };

    round = (value, decimals) => {
        return Number(Math.round(value + 'e' + decimals) + 'e-' + decimals);
    }

    ValorNum = (v) => {
        if (v === undefined || v === null)
            return "";
        v = v.replace(/\D/g, "")
        return v;
    }

    ValorNum12 = (v) => {
        if (v === undefined || v === null)
            v = "0";
        v = v.replace(/\D/g, "")

        var prefix = '';

        for (var i = 0; i < 12 - v.length; i++)
            prefix += '0';

        return prefix + v;
    }

    ValorMoney = (v) => {
        var extract = this.extractNumbers(v);
        var clean = this.cleanup(extract);
        var resp = '';
        switch (clean.length) {
            case 1: resp = '0,0' + clean; break;
            case 2: resp = '0,' + clean; break;
            case 3:
                {
                    let p1 = clean.substr(0, 1);
                    let p2 = clean.substr(1, 2);
                    resp = p1 + ',' + p2;
                    break;
                }

            case 4:
                {
                    let p1 = clean.substr(0, 2);
                    let p2 = clean.substr(2, 2);
                    resp = p1 + ',' + p2;
                    break;
                }

            case 5:
                {
                    let p1 = clean.substr(0, 3);
                    let p2 = clean.substr(3, 2);
                    resp = p1 + ',' + p2;
                    break;
                }

            case 6:
                {
                    let p1 = clean.substr(0, 1);
                    let p2 = clean.substr(1, 3);
                    let p3 = clean.substr(4, 2);
                    resp = p1 + '.' + p2 + ',' + p3;
                    break;
                }

            case 7:
                {
                    let p1 = clean.substr(0, 2);
                    let p2 = clean.substr(2, 3);
                    let p3 = clean.substr(5, 2);
                    resp = p1 + '.' + p2 + ',' + p3;
                    break;
                }

            case 8:
                {
                    let p1 = clean.substr(0, 3);
                    let p2 = clean.substr(3, 3);
                    let p3 = clean.substr(6, 2);
                    resp = p1 + '.' + p2 + ',' + p3;
                    break;
                }

            case 9:
                {
                    let p1 = clean.substr(0, 1);
                    let p2 = clean.substr(1, 3);
                    let p3 = clean.substr(4, 3);
                    let p4 = clean.substr(7, 2);
                    resp = p1 + '.' + p2 + '.' + p3 + ',' + p4;
                    break;
                }

            default: break;
        }
        return resp;
    }
}
