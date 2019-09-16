import React from "react";
import { Switch, Route, Redirect } from "react-router";
import { TransitionGroup, CSSTransition } from "react-transition-group";
import Hammer from "rc-hammerjs";

import Header from "../Header";
import AccountEditPage from "../../pages/account/AccountEdit";
import ContractDetailPage from "../../pages/contract/Contract";
import DashboardContracts from "../../pages/dashboardContracts/DashboardContracts";

import s from "./Layout.module.scss";

export default class LayoutComponent extends React.Component {

  render() {
    return (
      <div
        className={[
          s.root,
          "sidebar-" + this.props.sidebarPosition,
          "sidebar-" + this.props.sidebarVisibility
        ].join(" ")}
      >
        <div className={s.wrap}>
          <Header
            mainVars={this.props.mainVars}
            updateMainVars={this.props.updateMainVars}
          />
          <Hammer onSwipe={this.handleSwipe}>
            <main className={s.content}>
              <TransitionGroup>
                <CSSTransition
                  key={this.props.location.pathname}
                  classNames="fade"
                  timeout={200}
                >
                  <Switch>
                    <Route
                      path="/app/main"
                      exact
                      render={() => <Redirect to="/app/main/dashboard" />}
                    />
                    <Route
                      path="/app/main/dashboard"
                      exact
                      mainVars={this.props.mainVars}
                      updateMainVars={this.props.updateMainVars}
                      component={DashboardContracts}
                    />
                    <Route
                      path="/app/main/contract"
                      exact
                      mainVars={this.props.mainVars}
                      updateMainVars={this.props.updateMainVars}
                      component={ContractDetailPage}
                    />
                    <Route
                      path="/app/accountEdit"
                      exact
                      mainVars={this.props.mainVars}
                      updateMainVars={this.props.updateMainVars}
                      component={AccountEditPage}
                    />
                  </Switch>
                </CSSTransition>
              </TransitionGroup>
              <footer className={s.contentFooter}>Vortigo Prototype 1.0</footer>
            </main>
          </Hammer>
        </div>
      </div>
    );
  }
}
