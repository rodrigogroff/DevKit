
import { connect } from "react-redux";
import { Redirect } from "react-router-dom";
import React from "react";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import {
  NavLink,
  Dropdown,
  DropdownToggle,
  DropdownMenu,
  DropdownItem
} from "reactstrap";

import {
  openSidebar,
  closeSidebar,
  changeSidebarPosition,
  changeSidebarVisibility
} from "../../actions/navigation";

import s from "./Header.module.scss";

// vortigo -----------------------

import { Api } from "../../shared/Api";
import logoImg from "./sidebar_logo.png";
import menuImg from "./menu.png";

class Header extends React.Component {

  static propTypes = {
    dispatch: PropTypes.func.isRequired,
    sidebarPosition: PropTypes.string.isRequired
  };

  state = {
    visible: true,
    messagesOpen: false,
    supportOpen: false,
    settingsOpen: false,
    searchFocused: false,
    searchOpen: false,

    exit: false,
    limites: false,

    _type: '1',

    userName: ""
  };

  constructor(props) {
    super(props);

    this.doLogout = this.doLogout.bind(this);
    this.onDismiss = this.onDismiss.bind(this);
    this.toggleAccountDropdown = this.toggleAccountDropdown.bind(this);
    this.toggleSearchOpen = this.toggleSearchOpen.bind(this);
  }

  componentDidMount() {
    var api = new Api();
    if (!api.isAuthenticated()) this.setState({ exit: true });
    else {
      if (this.props.mainVars.main_userName === "") {
        this.props.updateMainVars({
          name: api.loggedUserName(),
          languageOption: '0'
        });
      }

      api.ping().then(resp => { }).catch(() => {
        this.setState({ exit: true });
      });
    }

    this.setState({ _type: api.loggedUserType() });
  }

  doLogout() {
    var api = new Api();
    api.cleanLogin();

    if (this.state._type === '1') this.setState({ exitAssociado: true });
    if (this.state._type === '2') this.setState({ exitLojista: true });
  }

  onDismiss() {
    this.setState({ visible: false });
  }

  toggleAccountDropdown() {
    this.setState({
      accountOpen: !this.state.accountOpen
    });
  }

  toggleSearchOpen() {
    this.setState({
      searchOpen: !this.state.searchOpen
    });
  }

  toggleSidebar() {
    this.props.isSidebarOpened
      ? this.props.dispatch(closeSidebar())
      : this.props.dispatch(openSidebar());
  }

  moveSidebar(position) {
    this.props.dispatch(changeSidebarPosition(position));
  }

  toggleVisibilitySidebar(visibility) {
    this.props.dispatch(changeSidebarVisibility(visibility));
  }

  redirectLimites = () => {
    this.setState({
      limites: true
    });
  };

  render() {
    if (this.state.exitAssociado === true) return <Redirect to="/loginAssociado" />;
    else if (this.state.exitLojista === true) return <Redirect to="/loginLojista" />;
    else if (this.state.limites === true) return <Redirect to="/app/associado/limites" />;
    else {
      switch (this.state._type) {
        default:
        case '1':
          return (
            <div>
              <table width="100%" className={s.appBarHeader}>
                <tbody>
                  <tr>
                    <td>
                      <table>
                        <tbody>
                          <tr>
                            <td>
                              <img className={s.imgLogo} alt='Limites' src={logoImg} />
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </td>
                    <td>
                      <div align="right">
                        <table>
                          <tbody>
                            <tr>
                              <td>
                                <div className={s.User}>
                                  <Dropdown isOpen={this.state.accountOpen} toggle={this.toggleAccountDropdown}>
                                    <h3>
                                      <DropdownToggle nav className={s.navItem}>
                                        <img className={s.menuLogo} alt='Menu' src={menuImg} />
                                      </DropdownToggle>
                                    </h3>
                                    <DropdownMenu right className={`${s.dropdownMenu} ${s.account}`}>
                                      <DropdownItem>
                                        <NavLink href="#/app/associado/QRCODE">
                                          Cartão Virtual
                                          </NavLink>
                                      </DropdownItem>
                                      <DropdownItem>
                                        <NavLink href="#/app/associado/solics">
                                          Solicitações
                                          </NavLink>
                                      </DropdownItem>
                                      <DropdownItem>
                                        <NavLink href="#/app/associado/limites">
                                          Limites
                                          </NavLink>
                                      </DropdownItem>
                                      <DropdownItem>
                                        <NavLink href="#/app/associado/extratos">
                                          Extrato atual
                                          </NavLink>
                                      </DropdownItem>
                                      <DropdownItem>
                                        <NavLink href="#/app/associado/parcelamentos">
                                          Parcelamentos
                                          </NavLink>
                                      </DropdownItem>
                                      <DropdownItem>
                                        <NavLink href="#/app/associado/faturas">
                                          Histórico Faturas
                                          </NavLink>
                                      </DropdownItem>
                                      <DropdownItem>
                                        <NavLink href="#/app/associado/rede">
                                          Rede Lojas
                                          </NavLink>
                                      </DropdownItem>
                                      <DropdownItem>
                                        <NavLink onClick={this.doLogout}>
                                          Sair
                                          </NavLink>
                                      </DropdownItem>
                                    </DropdownMenu>
                                  </Dropdown>
                                </div>
                              </td>
                            </tr>
                          </tbody>
                        </table>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div >
          );

        case '2':
          return (
            <div>
              <table width="100%" className={s.appBarHeader}>
                <tbody>
                  <tr>
                    <td>
                      <table>
                        <tbody>
                          <tr>
                            <td>
                              <img className={s.imgLogo} alt='Limites' src={logoImg} />
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </td>
                    <td>
                      <div align="right">
                        <table>
                          <tbody>
                            <tr>
                              <td>
                                <div className={s.User}>
                                  <Dropdown isOpen={this.state.accountOpen} toggle={this.toggleAccountDropdown}>
                                    <h3>
                                      <DropdownToggle nav className={s.navItem}>
                                        <img className={s.menuLogo} alt='Menu' src={menuImg} />
                                      </DropdownToggle>
                                    </h3>
                                    <DropdownMenu right className={`${s.dropdownMenu} ${s.account}`}>
                                      <DropdownItem>
                                        <NavLink href="#/app/lojista/venda">
                                          Nova Venda
                                            </NavLink>
                                      </DropdownItem>
                                      <DropdownItem>
                                        <NavLink href="#/app/lojista/autorizacoes">
                                          Autorizações
                                            </NavLink>
                                      </DropdownItem>
                                      <DropdownItem>
                                        <NavLink href="#/app/lojista/cancelamentos">
                                          Cancelamentos
                                            </NavLink>
                                      </DropdownItem>
                                      <DropdownItem>
                                        <NavLink onClick={this.doLogout}>
                                          Sair
                                          </NavLink>
                                      </DropdownItem>
                                    </DropdownMenu>
                                  </Dropdown>
                                </div>
                              </td>
                            </tr>
                          </tbody>
                        </table>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div >
          );
      }
    }

  }
}

function mapStateToProps(store) {
  return {
    isSidebarOpened: store.navigation.sidebarOpened,
    sidebarVisibility: store.navigation.sidebarVisibility,
    sidebarPosition: store.navigation.sidebarPosition
  };
}

export default withRouter(connect(mapStateToProps)(Header));
