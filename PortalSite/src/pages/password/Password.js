
import React, { createRef } from 'react';
import { Redirect } from 'react-router-dom';
import {
  Button,
  Input,
  Label,
  Tooltip,
  InputGroup,
  FormGroup,
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
} from 'reactstrap';
import Widget from '../../components/Widget';
import MaskedInput from 'react-maskedinput';
import s from './Password.module.scss';

// vortigo componentes

import { Api } from '../../shared/Api'
import { Util } from '../../shared/Util'

export default class PasswordPage extends React.Component {

  state = {
    loadingLanguages: true,
    loading: false,
    redirectLogin: false,
    invalidForm: false,
    error_cpf: false,
    recoveryOption: 'email',
    selectedOption: 'PT ',
    languagesArray: [],
    cpf: '',
    error: '',
    success: '',
  }

  constructor(props) {
    super(props);
    this.cpfRef = createRef();
  }

  componentDidMount() {
    var api = new Api();
    api.getLanguages().then(resp => {
      this.setState({
        languagesArray: resp.payload,
        loadingLanguages: false
      },
        () => { this.translate(0); })
    });
    setTimeout(() => { this.cpfRef.current.focus(); }, 500);
  }

  translate = option => {
    var uiTranslation = this.state.languagesArray.languages[option];

    if (uiTranslation !== undefined)
      this.setState({
        socialID: uiTranslation.Screens[0].Login.socialID,
        title: uiTranslation.Screens[3].PasswordRecovery.title,
        mainTitle: uiTranslation.Screens[3].PasswordRecovery.mainTitle,
        subtitle: uiTranslation.Screens[3].PasswordRecovery.subtitle,
        button: uiTranslation.Screens[3].PasswordRecovery.button,
        loginLink: uiTranslation.Screens[3].PasswordRecovery.loginLink,
        optionsRecoveryLabel: uiTranslation.Screens[3].PasswordRecovery.optionsRecoveryLabel,
        optionsRecoveryEmailLabel: uiTranslation.Screens[3].PasswordRecovery.optionsRecoveryEmailLabel,
        optionsRecoveryPhoneLabel: uiTranslation.Screens[3].PasswordRecovery.optionsRecoveryPhoneLabel,
        msg_socialID_incomplete: uiTranslation.Screens[3].PasswordRecovery.msg_socialID_incomplete,
        msg_recoveryOption_incomplete: uiTranslation.Screens[3].PasswordRecovery.msg_recoveryOption_incomplete,
        msg_success: uiTranslation.Screens[3].PasswordRecovery.msg_success,
        tooltipCPF: uiTranslation.Screens[0].Login.tooltipCPF,
      });
  }

  handleLanguageChange = e => {
    this.setState({ selectedOption: e.currentTarget.textContent });
    switch (e.currentTarget.textContent) {
      case 'PT ': this.translate(0); break;
      case 'EN ': this.translate(1); break;
      case 'ES ': this.translate(2); break;
      default: break;
    }
  };

  onChange_CPF = event => {

    var util = new Util();

    if (util.checkCPFMask(event.target.value))
      if (!util.checkCPF(event.target.value))
        this.setState({ error_cpf: true });
      else
        this.setState({ error_cpf: false });

    this.setState({ cpf: event.target.value }, () => {
      if (this.state.invalidForm === true)
        this.checkInvalidForm();
    })
  }

  checkInvalidForm = () => {
    var util = new Util();
    var errorCPF = !util.checkCPF(this.state.cpf);
    var invalidForm = false;

    if (errorCPF)
      invalidForm = true;

    this.setState({
      invalidForm: invalidForm,
      error_cpf: errorCPF,
    });

    return invalidForm;
  }

  redirectToLogin = e => { this.setState({ redirectLogin: true }); }

  submitButton = e => {

    e.preventDefault();

    this.setState({ error: '', success: '' });

    if (this.checkInvalidForm())
      return;

    console.log(this.state.cpf)
    console.log(this.state.recoveryOption)

    this.setState({ success: this.state.msg_success });
  }

  render() {
    if (this.state.redirectLogin === true)
      return <Redirect to='/login' />
    else if (this.state.loadingLanguages === true)
      return <div></div>
    else
      return (
        <div className={s.root}>

          <Modal isOpen={this.state.success} toggle={() => this.setState({ success: '' })}>
            <ModalHeader toggle={() => this.setState({ success: '' })}>Aviso do Sistema</ModalHeader>
            <ModalBody className="bg-success-system"><div className="modalBodyMain"><br></br>{this.state.success}<br></br><br></br></div></ModalBody>
            <ModalFooter className="bg-white"><Button color="primary" onClick={() => this.setState({ success: '' })}>Fechar</Button></ModalFooter>
          </Modal>
          <Modal isOpen={this.state.error} toggle={() => this.setState({ error: '' })}>
            <ModalHeader toggle={() => this.setState({ error: '' })}>Aviso do Sistema</ModalHeader>
            <ModalBody className="bg-danger-system"><div className="modalBodyMain"><br></br>{this.state.error}<br></br><br></br></div></ModalBody>
            <ModalFooter className="bg-white"><Button color="primary" onClick={() => this.setState({ error: '' })}>Fechar</Button></ModalFooter>
          </Modal>

          {this.state.loading ? <div className="loader"><p className="loaderText"><i className='fa fa-spinner fa-spin'></i></p></div> : <div ></div>}

          <div align='center' style={{ width: '330px' }}>
            <Widget className={`${s.widget} mx-auto`}
              bodyClass="p-0"
              title={<h3 className="mt-0">{this.state.mainTitle}
                &nbsp;&nbsp;
                    <UncontrolledButtonDropdown>
                  <DropdownToggle caret color="grey" className="dropdown-toggle-split">{this.state.selectedOption}</DropdownToggle>
                  <DropdownMenu>
                    <DropdownItem onClick={this.handleLanguageChange}>PT </DropdownItem>
                    <DropdownItem onClick={this.handleLanguageChange}>EN </DropdownItem>
                    <DropdownItem onClick={this.handleLanguageChange}>ES </DropdownItem>
                  </DropdownMenu>
                </UncontrolledButtonDropdown>
              </h3>}>
              <br></br>
              <h4>{this.state.title} </h4>
              <p className={s.widgetLoginInfo}>
                {this.state.subtitle}
              </p>
              <br></br>
              <form className="mt" onSubmit={this.submitButton} href="/app">
                <label htmlFor="email-input" className="ml-4">
                  {this.state.socialID}
                </label>
                <InputGroup className="input-group-no-border px-4">
                  <InputGroupAddon addonType="prepend">
                    <InputGroupText><i className="fa fa-user text-white" /></InputGroupText>
                  </InputGroupAddon>
                  <Tooltip placement="top" isOpen={this.state.error_cpf} target="email-input">{this.state.tooltipCPF}</Tooltip>
                  <MaskedInput className="input-transparent form-control"
                    id="email-input"
                    ref={this.cpfRef}
                    mask="111.111.111-11"
                    onChange={this.onChange_CPF} />
                </InputGroup>
                <br></br>
                <label htmlFor="email-input" className="ml-4" id="radio_label">
                  {this.state.optionsRecoveryLabel}
                </label>
                <InputGroup className="input-group-no-border px-4">
                  <FormGroup className="radio abc-radio">
                    <Widget>
                      <table >
                        <tbody>
                          <tr>
                            <td>
                              <Input type="radio"
                                name="radio1"
                                checked={this.state.recoveryOption === 'email'}
                                id="email"
                                onChange={event => this.setState({ recoveryOption: 'email' })} />
                              <Label for="email">{this.state.optionsRecoveryEmailLabel} </Label>
                            </td>
                            <td width='20px'></td>
                            <td>
                              <Input type="radio"
                                name="radio1"
                                id="phone"
                                checked={this.state.recoveryOption === 'phone'}
                                value='phone'
                                onChange={event => this.setState({ recoveryOption: 'phone' })} />
                              <Label for="phone">{this.state.optionsRecoveryPhoneLabel} </Label>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </Widget>
                  </FormGroup>
                </InputGroup>
                <div className="bg-widget-transparent">
                  <div className="p-4">
                    <div>
                      <h4>
                        <Button id="btnSubmit"
                          color={this.state.invalidForm ? "danger" : "primary"}
                          style={{ width: '100%' }}
                          type="submit"
                          disabled={this.state.loading || this.state.success.length > 0}>
                          {
                            this.state.loading === true ?
                              <span className="spinner"><i className="fa fa-spinner fa-spin"></i>&nbsp;&nbsp;</span>
                              :
                              <div></div>
                          }
                          {this.state.button}
                        </Button>
                      </h4>
                    </div>
                    <br></br>
                    <p onClick={this.redirectToLogin} className={s.mousePointer} align="center"> {this.state.loginLink}</p>
                  </div>
                </div>
              </form>
            </Widget>
          </div>
        </div>
      );
  }
}
