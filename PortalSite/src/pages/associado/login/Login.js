
import React from "react";
import { Redirect } from "react-router-dom";
import {
  Button,
  Input,
  Tooltip,
  InputGroup,
  InputGroupAddon,
  InputGroupText,
  Modal,
  ModalHeader,
  ModalBody,
  ModalFooter
} from "reactstrap";
import Widget from "../../../components/Widget";
import s from "./Login.module.scss";

import logoImg from "./logo.png";
import { Api } from "../../../shared/Api";

export default class Login extends React.Component {

  state = {
    loading: false,
    redirectDashboard: false,
    _empresa: "",
    _matricula: "",
    _codAcesso: "",
    _venc: "",
    _senha: "",
    error: ""
  };

  componentDidMount() {
    var api = new Api();
    this.setState({ _versao: api.versao() });
  };

  executeLogin = e => {
    e.preventDefault();

    var empresa = this.state._empresa;
    var matricula = this.state._matricula;
    var codAcesso = this.state._codAcesso;
    var venc = this.state._venc;
    var senha = this.state._senha;

    var serviceData = JSON.stringify({ empresa, matricula, codAcesso, venc, senha });

    this.setState({
      loading: true,
      error: ""
    });

    var api = new Api();

    api
      .postPublicLoginPortal(serviceData)
      .then(resp => {
        if (resp.ok === true) {

          let cartao = this.state._empresa + '.' + this.state._matricula + '.' + this.state._codAcesso + '.' + this.state._venc;

          api.loginOk(
            resp.payload.token,
            resp.payload.user.nome,
            cartao
          );

          this.props.updateMainVars({
            name: resp.payload.user.nome,
            languageOption: '0'
          });

          this.setState({ loading: false, redirectDashboard: true });
        } else {
          api.cleanLogin();
          this.setState({
            loading: false,
            error: resp.msg
          });
        }
      })
      .catch(err => {
        this.setState({
          loading: false,
          error: "Nao foi possivel verificar os dados de sua requisição"
        });
      });
  };

  render() {
    if (this.state.redirectDashboard === true)
      return <Redirect to="/app/associado/limites" />;
    else
      return (
        <div className={s.root}>
          <Modal isOpen={this.state.error.length > 0} toggle={() => this.setState({ error: "" })}>
            <ModalHeader toggle={() => this.setState({ error: "" })}>
              Aviso do Sistema
            </ModalHeader>
            <ModalBody className="bg-danger-system">
              <div className="modalBodyMain">
                <br />
                {this.state.error}
                <br />
                <br />
              </div>
            </ModalBody>
            <ModalFooter className="bg-white">
              <Button color="primary" onClick={() => this.setState({ error: "" })}> Fechar </Button>
            </ModalFooter>
          </Modal>
          <div align='center' style={{ width: '330px' }}>
            <Widget className={`${s.widget}`} bodyClass="p-0">
              <div className={s.appBarHeader}>
                <br></br>
                <div className="logoClass" align="center">
                  <img className={s.imgLogo} src={logoImg} alt=" " />
                </div>
                <br></br>
                <br></br>
                <form className="mt" onSubmit={this.executeLogin}>
                  <label htmlFor="email-input">
                    Informe o número do seu Cartão Benefícios
                </label>
                  <InputGroup className="input-group-no-border px-4">
                    <table>
                      <tbody>
                        <tr>
                          <td width='100px'>
                            <Input className="input-transparent form-control" id="empresa-input" maxLength="6" type="tel" pattern="[0-9]*" inputmode="numeric" autoComplete='off'
                              onChange={event => this.setState({ _empresa: event.target.value })} />
                          </td>
                          <td width='10px'></td>
                          <td width='100px'>
                            <Input className="input-transparent form-control" id="matricula-input" maxLength="6" type="tel" pattern="[0-9]*" inputmode="numeric" autoComplete='off'
                              onChange={event => this.setState({ _matricula: event.target.value })} />
                          </td>
                          <td width='10px'></td>
                          <td width='80px'>
                            <Input className="input-transparent form-control" id="codAcesso-input" maxLength="4" type="tel" pattern="[0-9]*" inputmode="numeric" autoComplete='off'
                              onChange={event => this.setState({ _codAcesso: event.target.value })} />
                          </td>
                          <td width='10px'></td>
                          <td width='80px'>
                            <Input className="input-transparent form-control" id="vencimento-input" maxLength="4" type="tel" pattern="[0-9]*" inputmode="numeric" autoComplete='off'
                              onChange={event => this.setState({ _venc: event.target.value })} />
                          </td>
                        </tr>
                      </tbody>
                    </table>
                  </InputGroup>
                  <br></br>
                  <br></br>
                  <label htmlFor="password-input">
                    Senha numérica de 4 dígitos
                </label>
                  <table align='center' width='180px'>
                    <tbody>
                      <tr>
                        <td width='100%'>
                          <InputGroup className="input-group-no-border px-4">
                            <InputGroupAddon addonType="prepend">
                              <InputGroupText>
                                <i className="fa fa-lock text-white" />
                              </InputGroupText>
                            </InputGroupAddon>
                            <Tooltip placement="top" isOpen={this.state.error_password} target="password-input">
                              Informe a senha corretamente
                          </Tooltip>
                            <Input id="password-input" type="password" className="input-transparent" width='80px' maxLength="4" pattern="[0-9]*" inputmode="numeric" autoComplete='off'
                              onChange={event => this.setState({ _senha: event.target.value })} />
                          </InputGroup>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                  <br></br><br></br>
                  <div className="bg-widget-transparent mt-4">
                    <div className="p-4">
                      <br></br>
                      <h4>
                        <Button color={this.state.invalidForm ? "danger" : "primary"}
                          style={{ width: "100%" }}
                          type="submit"
                          disabled={this.state.loading} >
                          {this.state.loading === true ? (
                            <span className="spinner">
                              <i className="fa fa-spinner fa-spin" />
                              &nbsp;&nbsp;&nbsp;
                            </span>
                          ) : (
                              <div />
                            )}
                          Efetuar Login
                      </Button>
                      </h4>
                      <br></br>
                      <br></br>
                      <p className={s.widgetLoginInfo}>Sistema Convênios | {this.state._versao}</p>
                      <br></br>
                    </div>
                  </div>
                </form>
              </div>
            </Widget>
          </div>
        </div>
      );
  }
}
