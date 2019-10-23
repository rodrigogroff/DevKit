
export const ApiLocation = {
    api_host: 'http://localhost',
    //api_host: 'https://177.85.160.41',
    api_port: '18524',
    api_portal: '/api/v1/portal/',
}

export class Api {

    versao = () => "v2.1.0003";

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
                    else if (res.status === 400) {
                        reject({
                            ok: false,
                            msg: 'Ops. Houve um erro e não pudemos concluir sua transação'
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
                    else if (res.status === 400) {
                        reject({
                            ok: false,
                            msg: 'Ops. Houve um erro e não pudemos concluir sua transação'
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
    }
}
