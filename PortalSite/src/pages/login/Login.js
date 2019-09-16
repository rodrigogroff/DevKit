
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

// vortigo components

import logoImg from "./logo.png";
import { Api } from "../../shared/Api";
import { Util } from "../../shared/Util";

export default class Login extends React.Component {
  state = {
    loadingLanguages: true,
    loading: false,
    alertIsOpen: false,
    selectActiveClient: true,
    redirectDashboard: false,
    redirectPassword: false,
    invalidForm: false,
    error_cpf: false,
    selectedOption: "PT ",
    languagesArray: [],
    cpf: "",
    userThyssenID: "",
    password: "",
    error: ""
  };

  constructor(props) {
    super(props);
    this.cpfRef = createRef();
  }

  componentDidMount() {
    var api = new Api();

    api.getLanguages("login").then(resp => {
      this.setState(
        {
          languagesArray: resp.payload,
          loadingLanguages: false
        },
        () => {
          this.translate(0);
        }
      );
    });
    setTimeout(() => {
      if (this.cpfRef != null)
        if (this.cpfRef.current != null) this.cpfRef.current.focus();
    }, 500);
  }

  handleLanguageChange = e => {
    this.setState({ selectedOption: e.currentTarget.textContent });
    switch (e.currentTarget.textContent) {
      default:
      case "PT ":
        this.translate(0);
        break;
      case "EN ":
        this.translate(1);
        break;
      case "ES ":
        this.translate(2);
        break;
    }
  };

  translate = option => {
    var uiTranslation = this.state.languagesArray.languages[option];

    if (uiTranslation !== undefined)
      this.setState({
        socialID: uiTranslation.Screens[0].Login.socialID,
        passwordLabel: uiTranslation.Screens[0].Login.passwordLabel,
        tabTitle: uiTranslation.Screens[0].Login.tabTitle,
        title: uiTranslation.Screens[0].Login.title,
        subtitle: uiTranslation.Screens[0].Login.subtitle,
        loginButton: uiTranslation.Screens[0].Login.loginButton,
        forgotPassword: uiTranslation.Screens[0].Login.forgotPassword,
        msg_invalid_cpf: uiTranslation.Screens[0].Login.msg_invalid_cpf,
        msg_user_invalid: uiTranslation.Screens[0].Login.msg_user_invalid,
        msg_invalid_password:
          uiTranslation.Screens[0].Login.msg_invalid_password,
        userThyssen: uiTranslation.Screens[0].Login.userThyssen,
        radioClient: uiTranslation.Screens[0].Login.radioClient,
        radioColaborator: uiTranslation.Screens[0].Login.radioColaborator,
        tooltipCPF: uiTranslation.Screens[0].Login.tooltipCPF,
        tooltipPassword: uiTranslation.Screens[0].Login.tooltipPassword
      });
  };

  redirectToPassword = e => {
    this.setState({ redirectPassword: true });
  };

  changeActive = event => {
    this.setState(
      { selectActiveClient: !this.state.selectActiveClient },
      () => {
        setTimeout(() => {
          if (this.cpfRef != null)
            if (this.cpfRef.current != null) this.cpfRef.current.focus();
        }, 500);
      }
    );
  };

  onChange_CPF = event => {
    var util = new Util();

    if (util.checkCPFMask(event.target.value))
      if (!util.checkCPF(event.target.value))
        this.setState({ error_cpf: true });
      else this.setState({ error_cpf: false });

    this.setState({ cpf: event.target.value }, () => {
      if (this.state.invalidForm === true) this.checkInvalidForm();
    });
  };

  onChange_Password = event => {
    this.setState({ password: event.target.value }, () => {
      if (this.state.invalidForm === true) this.checkInvalidForm();
    });
  };

  checkInvalidForm = () => {
    var util = new Util();
    var errorCPF = !util.checkCPF(this.state.cpf);
    var errorPassword = !util.checkPassword(this.state.password);
    var invalidForm = false;

    if (errorCPF || errorPassword) invalidForm = true;

    this.setState({
      invalidForm: invalidForm,
      error_cpf: errorCPF,
      error_password: errorPassword
    });

    return invalidForm;
  };

  executeLogin = e => {
    e.preventDefault();

    if (this.checkInvalidForm()) return;

    let login = this.state.cpf;
    let passwd = this.state.password;
    let typeLogin = "1";
    let browserIP = "123465";

    if (this.state.selectActiveClient === false) {
      login = this.state.userThyssenID;
      typeLogin = "2";
    }

    var serviceData = JSON.stringify({ login, passwd, typeLogin, browserIP });

    this.setState({
      redirectHome: null,
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
            resp.payload.sigla,
            resp.payload.name,
            login,
            resp.payload.languageOption
          );

          this.props.updateMainVars({
            name: resp.payload.sigla,
            languageOption: resp.payload.languageOption
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
    else if (this.state.redirectPassword === true)
      return <Redirect to="/password" />;
    else if (this.state.loadingLanguages === true) return <div />;
    else
      return (
        <div className={s.root}>
          <Modal
            isOpen={this.state.error.length > 0}
            toggle={() => this.setState({ error: "" })}
          >
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
              <Button
                color="primary"
                onClick={() => this.setState({ error: "" })}
              >
                Fechar
              </Button>
            </ModalFooter>
          </Modal>
          <div align='center' style={{ width: '330px' }}>
            <Widget
              className={`${s.widget}`}
              bodyClass="p-0"
              title={
                <h3 className="mt-0">
                  {this.state.title}
                  &nbsp;&nbsp;
                  <UncontrolledButtonDropdown>
                    <DropdownToggle
                      caret
                      color="grey"
                      className="dropdown-toggle-split"
                    >
                      {this.state.selectedOption}
                    </DropdownToggle>
                    <DropdownMenu>
                      <DropdownItem onClick={this.handleLanguageChange}>
                        PT{" "}
                      </DropdownItem>
                      <DropdownItem onClick={this.handleLanguageChange}>
                        EN{" "}
                      </DropdownItem>
                      <DropdownItem onClick={this.handleLanguageChange}>
                        ES{" "}
                      </DropdownItem>
                    </DropdownMenu>
                  </UncontrolledButtonDropdown>
                </h3>
              }
            >
              <div className="logoClass" align="center">
                <img className={s.imgLogo} src={logoImg} alt=" " />
              </div>
              <p className={s.widgetLoginInfo}>{this.state.subtitle}</p>
              <p className="text-center text-white w-100 d-block mt-2">
                <span
                  className={
                    this.state.selectActiveClient === true
                      ? s.lblActive
                      : s.lblInactive
                  }
                  onClick={this.changeActive}
                >
                  {this.state.radioClient}{" "}
                </span>{" "}
                &nbsp;&nbsp;|&nbsp;&nbsp;
                <span
                  className={
                    this.state.selectActiveClient === true
                      ? s.lblInactive
                      : s.lblActive
                  }
                  onClick={this.changeActive}
                >
                  {this.state.radioColaborator}
                </span>
              </p>
              <form className="mt" onSubmit={this.executeLogin}>
                <label htmlFor="email-input" className="ml-4">
                  {this.state.selectActiveClient === true
                    ? this.state.socialID
                    : this.state.userThyssen}
                </label>
                <InputGroup className="input-group-no-border px-4">
                  <InputGroupAddon addonType="prepend">
                    <InputGroupText>
                      <i className="fa fa-user text-white" />
                    </InputGroupText>
                  </InputGroupAddon>
                  <Tooltip
                    placement="top"
                    isOpen={this.state.error_cpf}
                    target="email-input"
                  >
                    {this.state.tooltipCPF}
                  </Tooltip>
                  {this.state.selectActiveClient === true ? (
                    <MaskedInput
                      className="input-transparent form-control"
                      id="email-input"
                      ref={this.cpfRef}
                      mask="111.111.111-11"
                      onChange={this.onChange_CPF}
                    />
                  ) : (
                      <Input
                        className="input-transparent form-control"
                        id="email-input"
                        maxLength="20"
                        onChange={event =>
                          this.setState({ userThyssenID: event.target.value })
                        }
                      />
                    )}
                </InputGroup>
                <label htmlFor="password-input" className="mt ml-4">
                  {this.state.passwordLabel}
                </label>
                <InputGroup className="input-group-no-border px-4">
                  <InputGroupAddon addonType="prepend">
                    <InputGroupText>
                      <i className="fa fa-lock text-white" />
                    </InputGroupText>
                  </InputGroupAddon>
                  <Tooltip
                    placement="top"
                    isOpen={this.state.error_password}
                    target="password-input"
                  >
                    {this.state.tooltipPassword}
                  </Tooltip>
                  <Input
                    id="password-input"
                    type="password"
                    className="input-transparent"
                    maxLength="9"
                    onChange={this.onChange_Password}
                  />
                </InputGroup>
                <div className="bg-widget-transparent mt-4">
                  <div className="p-4">
                    <h4>
                      <Button
                        color={this.state.invalidForm ? "danger" : "primary"}
                        style={{ width: "100%" }}
                        type="submit"
                        disabled={this.state.loading}
                      >
                        {this.state.loading === true ? (
                          <span className="spinner">
                            <i className="fa fa-spinner fa-spin" />
                            &nbsp;&nbsp;&nbsp;
                          </span>
                        ) : (
                            <div />
                          )}
                        {this.state.loginButton}
                      </Button>
                    </h4>
                    <br></br>
                    <p onClick={this.redirectToPassword} className={s.mousePointer} align="center">
                      {this.state.forgotPassword}
                    </p>
                  </div>
                </div>
              </form>
            </Widget>
          </div>

        </div>
      );
  }
}
