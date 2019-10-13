import React from 'react';
import { connect } from 'react-redux';
import { Switch, Route, Redirect } from 'react-router';
import { HashRouter } from 'react-router-dom';
import LayoutComponent from '../components/Layout';

import LoginAssociadoComponent from '../pages/associado/login';
import LoginLojistaComponent from '../pages/lojista/login';

import '../styles/theme.scss';

const PrivateRoute = ({ component, ...rest }) => {
  var x = { ...rest }
  return (
    <Route
      {...rest} render={props => (
        localStorage.getItem('token') ? (
          React.createElement(component, x)
        ) : (
            <Redirect
              to={{
                pathname: '/loginAssociado',
                state: { from: props.location },
              }}
            />
          )
      )}
    />
  );
}

class App extends React.PureComponent {

  constructor() {
    super();
    this.state = {
      main_userName: "",
      main_languageOption: -1
    };
  }

  updateMainVars = detail => {
    this.setState({
      main_userName: detail.name,
      main_languageOption: detail.languageOption,
    });
  };

  render() {
    return (
      <HashRouter>
        <Switch>
          <Route path="/" exact render={() => <Redirect to="/loginAssociado" />} />
          <PrivateRoute path="/app" component={LayoutComponent} mainVars={this.state} updateMainVars={this.updateMainVars} />
          <Route path="/loginAssociado" exact >
            <LoginAssociadoComponent mainVars={this.state} updateMainVars={this.updateMainVars} />
          </Route>
          <Route path="/loginLojista" exact >
            <LoginLojistaComponent mainVars={this.state} updateMainVars={this.updateMainVars} />
          </Route>
        </Switch>
      </HashRouter>
    );
  }
}

const mapStateToProps = state => ({
  isAuthenticated: state.auth.isAuthenticated,
})

export default connect(mapStateToProps)(App);

