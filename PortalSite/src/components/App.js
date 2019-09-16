import React from 'react';
import { connect } from 'react-redux';
import { Switch, Route, Redirect } from 'react-router';
import { HashRouter } from 'react-router-dom';

import PasswordPage from '../pages/password/Password';

import LayoutComponent from '../components/Layout';
import LoginComponent from '../pages/login';
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
                pathname: '/login',
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
          <Route path="/" exact render={() => <Redirect to="/app/main" />} />
          <Route path="/app" exact render={() => <Redirect to="/app/main" />} />
          <PrivateRoute path="/app" component={LayoutComponent} mainVars={this.state} updateMainVars={this.updateMainVars} />
          <Route path="/login" exact >
            <LoginComponent mainVars={this.state} updateMainVars={this.updateMainVars} />
          </Route>
          <Route path="/password" exact component={PasswordPage} />
          <Redirect from="*" to="/app/main/analytics" />
        </Switch>
      </HashRouter>
    );
  }
}

const mapStateToProps = state => ({
  isAuthenticated: state.auth.isAuthenticated,
})

export default connect(mapStateToProps)(App);
