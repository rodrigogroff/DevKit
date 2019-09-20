import React from "react";
import { Switch, Route, Redirect } from "react-router";
import { TransitionGroup, CSSTransition } from "react-transition-group";
import Hammer from "rc-hammerjs";

import Header from "../Header";
import AssociadoLimites from "../../pages/associado/limites/Limites";

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
                      path="/app/associado/limites"
                      exact
                      mainVars={this.props.mainVars}
                      updateMainVars={this.props.updateMainVars}
                      component={AssociadoLimites}
                    />
                  </Switch>
                </CSSTransition>
              </TransitionGroup>
            </main>
          </Hammer>
        </div>
      </div>
    );
  }
}
