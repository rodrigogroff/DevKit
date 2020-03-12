
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
  ModalFooter,
} from "reactstrap";

import Widget from "../../../components/Widget";
import s from "./Login.module.scss";

import logoImg from "./logo.png";
import { Api } from "../../../shared/Api";

export default class LoginLojista extends React.Component {

  state = {
    loading: false,
    redirectDashboard: false,
    _terminal: "",
    _senha: "",
    error: ""
  };

  componentDidMount() {
    var api = new Api();
    this.setState({ _versao: api.versao() });
  };

  executeLogin = e => {
    e.preventDefault();

    var terminal = this.state._terminal;
    var senha = this.state._senha;

    var serviceData = JSON.stringify({ terminal, senha });

    this.setState({
      loading: true,
      error: ""
    });

    var api = new Api();

    api
      .postPublicLoginLojistaPortal(serviceData)
      .then(resp => {
        if (resp.ok === true) {

          api.loginLojistaOk(
            resp.payload.token,
            resp.payload.user.terminal,
            resp.payload.user.nome,
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
      return <Redirect to="/app/lojista/venda" />;
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
            <form className="mt" onSubmit={this.executeLogin}>
              <Widget className={`${s.widget}`} bodyClass="p-0">
                <div className={s.appBarHeader}>
                  <br></br>
                  <div className="logoClass" align="center">
                    <img className={s.imgLogo} src={logoImg} alt=" " />
                  </div>
                  <table align='center' width='180px'>
                    <tbody>
                      <tr>
                        <td valign="middle">
                          <br></br>
                          <div align='center' style={{ height: '32px' }}>Informe o terminal de lojista</div>
                        </td>
                      </tr>
                      <tr>
                        <td width='100%'>
                          <InputGroup className="input-group-no-border px-4">
                            <InputGroupAddon addonType="prepend">
                              <InputGroupText>
                                <i className="fa fa-user text-white" />
                              </InputGroupText>
                            </InputGroupAddon>
                            <Input className="input-transparent form-control" id="empresa-input" maxLength="6" type="tel" pattern="[0-9]*" inputmode="numeric" autocomplete='off'
                              onChange={event => this.setState({ _terminal: event.target.value })} />
                          </InputGroup>
                        </td>
                      </tr>
                      <tr>
                        <td valign="middle">
                          <br></br>
                          <div align='center' style={{ height: '32px' }}>Senha</div>
                        </td>
                      </tr>
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
                            <Input id="password-input" type="password" className="input-transparent" width='80px' maxLength="6" autoComplete='off'
                              onChange={event => this.setState({ _senha: event.target.value })} />
                          </InputGroup>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                  <br></br>
                  <div className="bg-widget-transparent mt-4">
                    <div className="p-4">
                      <br></br>
                      <h4>
                        <Button color={this.state.invalidForm ? "danger" : "primary"}
                          style={{ width: "100%" }}
                          type="submit"
                          onClick={this.executeLogin}
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
                </div>
              </Widget>
            </form>
          </div>
        </div>
      );
  }
}
