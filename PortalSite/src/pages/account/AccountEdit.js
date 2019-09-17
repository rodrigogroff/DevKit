
import React from 'react';
import {
  Row,
  Col,
  Button,
  FormGroup,
  Label,
  Input,
  UncontrolledButtonDropdown,
  DropdownMenu,
  DropdownItem,
  DropdownToggle,
  Modal,
  ModalHeader,
  ModalBody,
  ModalFooter,
  Tooltip,
} from 'reactstrap';
import MaskedInput from 'react-maskedinput';
import Widget from '../../components/Widget/Widget';
import s from './Account.module.scss';

// vortigo components

import { Api } from '../../shared/Api'
import { Util } from '../../shared/Util'

export default class AccountEditPage extends React.Component {

  state = {
    success: '',
    error: '',
    _email: ' ',
    _name: ' ',
    _phone: ' ',
    selectedOption: '',
    languagesArray: [],
    languageDef: [],
    modalPassword: false,
    invalidForm: false,
    invalidModalSenha: false,
    error_name: false,
    error_email: false,
    error_phone: false,
    error_newPass: false,
    error_currentPass: false,
    error_confPass: false,
  };

  componentDidMount() {
    var api = new Api();

    this.setState({ loading: true });
    api.getTokenPortal('userLoggedDetail', null).then(resp => {
      if (resp.ok === true) {
        this.setState({
          loading: false,
          _cpf: resp.payload.cpf,
          _name: resp.payload.name,
          _email: resp.payload.email,
          _phone: resp.payload.phone,
          translateSelectedIndex: resp.payload.languageOption,
          selectedOption: this.state.languageDef[resp.payload.languageOption]
        });
      }
      else
        this.setState({ loading: false, error: resp.msg })
    }).catch(err => {
      this.setState({ loading: false, error: err.msg })
    });


  }



  handleLanguageChange = e => {
    this.setState({ selectedOption: e.currentTarget.textContent });
  };

  onChange_Name = event => {
    this.setState({ _name: event.target.value }, () => {
      if (this.state.invalidForm === true)
        this.checkInvalidForm();
    })
  }

  onChange_Email = event => {
    this.setState({ _email: event.target.value }, () => {
      if (this.state.invalidForm === true)
        this.checkInvalidForm();
    })
  }

  onChange_Phone = event => {
    this.setState({ _phone: event.target.value }, () => {
      if (this.state.invalidForm === true)
        this.checkInvalidForm();
    })
  }

  checkInvalidForm = () => {
    var util = new Util();
    var errorName = !util.checkName(this.state._name);
    var errorEmail = !util.checkEmail(this.state._email);
    var errorPhone = !util.checkPhone(this.state._phone);
    var invalidForm = false;

    if (errorName || errorEmail || errorPhone)
      invalidForm = true;

    this.setState({
      invalidForm: invalidForm,
      error_name: errorName,
      error_email: errorEmail,
      error_phone: errorPhone
    });

    return invalidForm;
  }

  closeModalWarning = () => {
    this.setState({ modalWarning: false });
  }

  openModalPassword = () => {
    this.setState({ modalPassword: true });
  }

  closeModalPassword = () => {
    this.setState({
      modalPassword: false,
      invalidModalSenha: false,
      error_newPass: false,
      error_currentPass: false,
      error_confirmPass: false,
      currentPassword: ' ',
      newPassword: ' ',
      confirmPassword: ' ',
    });
  }

  onChange_CurrentPassword = event => {
    let check = !new Util().checkPassword(event.target.value);
    if (this.state.invalidModalSenha === false)
      if (check === true)
        check = false;
    this.setState({ currentPassword: event.target.value, error_currentPass: check })
  }

  onChange_NewPassword = event => {
    let tooltipText = 'Senha inválida';
    let check = !new Util().checkPassword(event.target.value);
    if (this.state.invalidModalSenha === false)
      if (check === true)
        check = false;
    this.setState({
      newPassword: event.target.value,
      error_newPass: check,
      tooltipPasswordNew: tooltipText
    }, () => {
      this.setState(currentState => {
        var check = currentState.error_newPass;
        var text = currentState.tooltipPasswordNew;
        if (currentState.newPassword === currentState.currentPassword) {
          check = true;
          text = 'Nova senha inválida';
        }
        return { error_newPass: check, tooltipPasswordNew: text }
      })
    })
  }

  onChange_ConfirmPassword = event => {
    let tooltipText = 'Senha inválida';
    let check = !new Util().checkPassword(event.target.value);
    if (this.state.invalidModalSenha === false)
      if (check === true)
        check = false;
    this.setState({
      confirmPassword: event.target.value,
      error_confirmPass: check,
      tooltipPasswordConf: tooltipText
    }, () => {
      this.setState(currentState => {
        var check = currentState.error_confirmPass;
        var text = currentState.tooltipPasswordConf;
        if (currentState.confirmPassword !== currentState.newPassword) {
          check = true;
          text = 'Confirmação de senha inválida'
        }
        return { error_confirmPass: check, tooltipPasswordConf: text }
      })
    })
  }

  submitChangePassword = () => {
    var util = new Util();
    var currentPass = !util.checkPassword(this.state.currentPassword);
    var newPass = !util.checkPassword(this.state.newPassword);
    var confirmPass = !util.checkPassword(this.state.confirmPassword);
    if (currentPass || newPass || confirmPass) {
      this.setState({
        invalidModalSenha: true,
        error_newPass: newPass,
        error_currentPass: currentPass,
        error_confirmPass: confirmPass
      });
    }
    else {
      this.setState({
        invalidModalSenha: false,
        modalPassword: false,
        currentPassword: '',
        newPassword: '',
        confirmPassword: '',
        success: 'Senha atualizada com sucesso'
      })
    }
  }

  submitChanges = e => {

    e.preventDefault();

    if (this.checkInvalidForm())
      return;

    let translateSelectedIndex = 0;

    switch (this.state.selectedOption) {
      case this.state.languageDef[0]: translateSelectedIndex = 0; break;
      case this.state.languageDef[1]: translateSelectedIndex = 1; break;
      case this.state.languageDef[2]: translateSelectedIndex = 2; break;
      default: break;
    }

    this.setState({ error: '', success: '', loading: true });

    var api = new Api();
    var serviceData = JSON.stringify({
      cpf: this.state._cpf,
      name: this.state._name,
      email: this.state._email,
      phone: this.state._phone,
      languageOption: translateSelectedIndex,
    });

    api.postTokenPortal('userUpdateDetail', serviceData).then(resp => {
      if (resp.ok === true) {
        this.setState({ loading: false, success: resp.payload.message })
        localStorage.setItem('user_nameFull', this.state._name)
        localStorage.setItem('languageOption', translateSelectedIndex)

        var novaSigla = this.state._name[0];
        for (let i = 1; i < this.state._name.length; i++) {
          if (this.state._name[i] === ' ') {
            novaSigla += this.state._name[i + 1];
            break;
          }
        }

        this.props.updateMainVars({
          name: novaSigla.toUpperCase(),
          languageOption: translateSelectedIndex
        })
        this.translate(translateSelectedIndex);
      }
      else
        this.setState({ loading: false, error: resp.msg })
    }).catch(err => {
      this.setState({ loading: false, error: err.msg })
    });
  }

  render() {
    return (
      <div className={s.root}>
        <br></br>
        <ol className="breadcrumb">
          <li className="breadcrumb-item">Portal</li>
          <li className="active breadcrumb-item">
            {this.state.title}
          </li>
        </ol>

        {this.state.loading ? <div className="loader"><p className="loaderText"><i className='fa fa-spinner fa-spin'></i></p></div> : <div ></div>}

        <Row>
          <Col md={12}>

            <Modal isOpen={this.state.success.length > 0} toggle={() => this.setState({ success: '' })}>
              <ModalHeader toggle={() => this.setState({ success: '' })}>Aviso do Sistema</ModalHeader>
              <ModalBody className="bg-success-system"><div className="modalBodyMain"><br></br>{this.state.success}<br></br><br></br></div></ModalBody>
              <ModalFooter className="bg-white"><Button color="primary" onClick={() => this.setState({ success: '' })}>Fechar</Button></ModalFooter>
            </Modal>
            <Modal isOpen={this.state.error.length > 0} toggle={() => this.setState({ error: '' })}>
              <ModalHeader toggle={() => this.setState({ error: '' })}>Aviso do Sistema</ModalHeader>
              <ModalBody className="bg-danger-system"><div className="modalBodyMain"><br></br>{this.state.error}<br></br><br></br></div></ModalBody>
              <ModalFooter className="bg-white"><Button color="primary" onClick={() => this.setState({ error: '' })}>Fechar</Button></ModalFooter>
            </Modal>

            <Widget title={<h4>Meus Dados</h4>} className={s.MainWidget}>
              <form onSubmit={this.submitButton}>
                <FormGroup row>
                  <Col xs={12} sm={10} lg={6} xl={6}>
                    <br></br>
                    <legend>{this.state.subtitle}</legend>
                    <br></br>
                    <FormGroup row>
                      <Label for="normal-field" md={4} className="text-md-left">{this.state.fieldID}</Label>
                      <Col md={7}>
                        <h4>{this.state._cpf}</h4>
                      </Col>
                    </FormGroup>
                    <FormGroup row>
                      <Label for="normal-field" md={4} className="text-md-left">{this.state.fieldName}</Label>
                      <Col md={7}>
                        <Tooltip placement="top" isOpen={this.state.error_name} target="fieldName">{this.state.tooltipName}</Tooltip>
                        <Input className={this.state.error_name ? "input-transparent-red" : "input-transparent"}
                          type="text"
                          id="fieldName"
                          maxLength="50"
                          value={this.state._name}
                          onChange={this.onChange_Name} />
                      </Col>
                    </FormGroup>
                    <FormGroup row>
                      <Label for="normal-field" md={4} className="text-md-left">{this.state.fieldEmail}</Label>
                      <Col md={7}>
                        <Tooltip placement="top" isOpen={this.state.error_email} target="fieldEmail">{this.state.tooltipEmail}</Tooltip>
                        <Input className={this.state.error_email ? "input-transparent-red" : "input-transparent"}
                          type="text"
                          id="fieldEmail"
                          value={this.state._email}
                          maxLength="50" placeholder=""
                          onChange={this.onChange_Email} />
                      </Col>
                    </FormGroup>
                    <FormGroup row>
                      <Label for="normal-field" md={4} className="text-md-left">{this.state.fieldPhone}</Label>
                      <Col md={7}>
                        <Tooltip placement="top" isOpen={this.state.error_phone} target="fieldPhone">{this.state.tooltipPhone}</Tooltip>
                        <MaskedInput className={this.state.error_phone ? "input-transparent-red form-control" : "input-transparent form-control"}
                          id="fieldPhone"
                          mask="(11) 111111111"
                          value={this.state._phone}
                          onChange={this.onChange_Phone} />
                      </Col>
                    </FormGroup>
                  </Col>
                  <Col xs={12} sm={10} lg={6} xl={6}>
                    <br></br>
                    <legend>Ações sobre sua conta</legend>
                    <br></br>
                    <FormGroup row>
                      <Label for="normal-field" md={4} className="text-md-left">{this.state.fieldLanguage}</Label>
                      <Col md={7}>
                        <UncontrolledButtonDropdown>
                          <DropdownToggle caret color="dark" className="dropdown-toggle-split">{this.state.selectedOption}</DropdownToggle>
                          <DropdownMenu>
                            <DropdownItem onClick={this.handleLanguageChange}>{this.state.languageDef[0]}</DropdownItem>
                            <DropdownItem onClick={this.handleLanguageChange}>{this.state.languageDef[1]}</DropdownItem>
                            <DropdownItem onClick={this.handleLanguageChange}>{this.state.languageDef[2]}</DropdownItem>
                          </DropdownMenu>
                        </UncontrolledButtonDropdown>
                      </Col>
                    </FormGroup>
                    <FormGroup row>
                      <Label for="normal-field" md={4} className="text-md-left">Segurança</Label>
                      <Col md={7}>
                        <Button color="primary" onClick={this.openModalPassword} className="mr-xs">Trocar Senha</Button>

                        <Modal isOpen={this.state.modalPassword} toggle={this.closeModalPassword}>
                          <ModalHeader toggle={this.closeModalPassword}>Troca de senha</ModalHeader>
                          <ModalBody className="bg-gray-dark">
                            <div className={s.modalBodyMain}>
                              <br></br>
                              <br></br>
                              <FormGroup row>
                                <Label for="normal-field" md={4} className="text-md-left">Senha Atual</Label>
                                <Col >
                                  <Tooltip placement="top" isOpen={this.state.error_currentPass} target="fieldCurrentPassword">{this.state.tooltipPassword}</Tooltip>
                                  <Input className={this.state.error_currentPass ? "input-transparent-red" : "input-transparent"}
                                    type="password"
                                    id="fieldCurrentPassword"
                                    maxLength="20"
                                    onChange={this.onChange_CurrentPassword} />
                                </Col>
                              </FormGroup>
                              <br></br>
                              <FormGroup row>
                                <Label for="normal-field" md={4} className="text-md-left">Nova Senha</Label>
                                <Col >
                                  <Tooltip placement="top" isOpen={this.state.error_newPass} target="fieldNewPassword">{this.state.tooltipPasswordNew}</Tooltip>
                                  <Input className={this.state.error_newPass ? "input-transparent-red" : "input-transparent"}
                                    type="password"
                                    id="fieldNewPassword"
                                    maxLength="20"
                                    onChange={this.onChange_NewPassword} />
                                </Col>
                              </FormGroup>
                              <br></br>
                              <FormGroup row>
                                <Label for="normal-field" md={4} className="text-md-left">Confirmar Senha</Label>
                                <Col >
                                  <Tooltip placement="top" isOpen={this.state.error_confirmPass} target="fieldConfirmPassword">{this.state.tooltipPasswordConf}</Tooltip>
                                  <Input className={this.state.error_confirmPass ? "input-transparent-red" : "input-transparent"}
                                    type="password"
                                    id="fieldConfirmPassword"
                                    maxLength="20"
                                    onChange={this.onChange_ConfirmPassword} />
                                </Col>
                              </FormGroup>
                              <br></br>
                            </div>
                          </ModalBody>
                          <ModalFooter className="bg-white">
                            <Button color="dark" onClick={this.closeModalPassword}>Close</Button>
                            <Button color="primary" onClick={this.submitChangePassword}>Save changes</Button>
                          </ModalFooter>
                        </Modal>

                      </Col>
                    </FormGroup>
                  </Col>
                  <Col xs={12} sm={12} lg={12} xl={12}>
                    <br></br>
                    <br></br>
                    <br></br>
                    <Widget className={s.MainWidget}>
                      <div className="floatRight">
                        <Button color={this.state.invalidForm ? "danger" : "primary"}
                          type="submit"
                          className="mr-xs"
                          disabled={this.state.loading}
                          onClick={this.submitChanges}>
                          {this.state.submitButton}
                        </Button>
                      </div>
                    </Widget>
                  </Col>
                </FormGroup>
              </form>
            </Widget>
          </Col>
        </Row>

      </div >
    );
  }
}
