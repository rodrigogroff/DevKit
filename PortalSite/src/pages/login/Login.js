
import React, { createRef } from "react";
import { Redirect } from "react-router-dom";
import {
  Button,
  Input,
  Tooltip,
  InputGroup,
  InputGroupAddon,
  InputGroupText,
  UncontrolledButtonDropdown,
  DropdownToggle,
  DropdownMenu,
  Modal,
  ModalHeader,
  ModalBody,
  ModalFooter,
  DropdownItem
} from "reactstrap";
import Widget from "../../components/Widget";
import s from "./Login.module.scss";
import MaskedInput from "react-maskedinput";

import logoImg from "./logo.png";
import { Api } from "../../shared/Api";

export default class Login extends React.Component {

  state = {
    loading: false,
    alertIsOpen: false,

    redirectDashboard: false,

    _empresa: "",
    _matricula: "",
    _codAcesso: "",
    _venc: "",
    _senha: "",
    error: ""
  };

  //constructor(props) {
  //    super(props);
  //  this.cpfRef = createRef();
  //}

  componentDidMount() {
    //    setTimeout(() => {
    //    if (this.cpfRef != null)
    //    if (this.cpfRef.current != null) this.cpfRef.current.focus();
    //}, 500);
  }

  checkInvalidForm = () => {
    return false;
  };

  executeLogin = e => {
    e.preventDefault();

    if (this.checkInvalidForm()) return;

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

          api.loginOk(
            resp.payload.token,
            resp.payload.user.nome
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
            alertIsOpen: true,
            error: resp.msg
          });
        }
      })
      .catch(err => {
        this.setState({
          loading: false,
          alertIsOpen: true,
          error: "Nao foi possivel verificar os dados de sua requisição"
        });
      });
  };

  render() {
    if (this.state.redirectDashboard === true)
      return <Redirect to="/app/main/dashboard" />;
    else if (this.state.loadingLanguages === true) return <div />;
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
            <Widget
              className={`${s.widget}`}
              bodyClass="p-0"
              title={<h3 className="mt-0"> Login </h3>} >
              <div className="logoClass" align="center">
                <img className={s.imgLogo} src={logoImg} alt=" " />
              </div>
              <p className={s.widgetLoginInfo}>Entre com os digitos de seu cartao</p>
              <form className="mt" onSubmit={this.executeLogin}>

                <label htmlFor="email-input" className="ml-4">
                  Empresa
                </label>
                <InputGroup className="input-group-no-border px-4">
                  <InputGroupAddon addonType="prepend">
                    <InputGroupText>
                      <i className="fa fa-user text-white" />
                    </InputGroupText>
                  </InputGroupAddon>
                  <Tooltip placement="top" isOpen={this.state.error_cpf} target="email-input">
                    Informe a empresa corretamente
                  </Tooltip>
                  <Input className="input-transparent form-control" id="email-input" maxLength="6"
                    onChange={event => this.setState({ _empresa: event.target.value })} />
                </InputGroup>

                <label htmlFor="password-input" className="mt ml-4">
                  Senha
                </label>
                <InputGroup className="input-group-no-border px-4">
                  <InputGroupAddon addonType="prepend">
                    <InputGroupText>
                      <i className="fa fa-lock text-white" />
                    </InputGroupText>
                  </InputGroupAddon>
                  <Tooltip placement="top" isOpen={this.state.error_password} target="password-input">
                    Informe a senha corretamente
                  </Tooltip>
                  <Input id="password-input" type="password" className="input-transparent" maxLength="6"
                    onChange={event => this.setState({ _senha: event.target.value })} />
                </InputGroup>

                <div className="bg-widget-transparent mt-4">
                  <div className="p-4">
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
                        Entrar
                      </Button>
                    </h4>
                    <br></br>
                  </div>
                </div>

              </form>
            </Widget>
          </div>

        </div>
      );
  }
}
